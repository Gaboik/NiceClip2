using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NiceCLip2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IntPtr NextClipboardViewer { get; private set; }

        private delegate string TextFormatter(string input);
        private bool isCopying;
        private WindowInteropHelper InteropHelper;
        private delegate void ClipboardEvent();
        private event ClipboardEvent OnClipboardMessage;
        private ObservableCollection<ClipboardHistoryItem> ClipboardEntriesCol;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private string LastCopiedText;

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern bool RegisterHotKey(IntPtr hwnd, int id, uint fsModifiers, uint vk);

        public MainWindow()
        {
            InitializeComponent();

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("./assets/NiceClip2Icon_light.ico");
            notifyIcon.Text = "NiceClip 2";
            notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.None;
            notifyIcon.BalloonTipTitle = "Copied Text";
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += Open_MenuItem_Click;

            System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem openMenuItem = new System.Windows.Forms.MenuItem("Open NiceClip");
            openMenuItem.Click += Open_MenuItem_Click;
            System.Windows.Forms.MenuItem quitMenuItem = new System.Windows.Forms.MenuItem("Quit");
            openMenuItem.Click += Quit_MenuItem_Click;

            menu.MenuItems.Add(openMenuItem);
            menu.MenuItems.Add(quitMenuItem);

            notifyIcon.ContextMenu = menu;

            OnClipboardMessage += PollClipboard;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            LastCopiedText = String.Empty;

            StatusBar.Text = "Welcome to NiceClip2!";

            ClipboardEntriesCol = new ObservableCollection<ClipboardHistoryItem>();
            ClipboardEntries.ItemsSource = ClipboardEntriesCol;

            // Show the Window now to ensure it has a valid handle to pass to User32.dll functions
            Show();

            try
            {
                InteropHelper = new WindowInteropHelper(this);
                HwndSource source = HwndSource.FromHwnd(InteropHelper.Handle);
                source.AddHook(new HwndSourceHook(WndProc));
                NextClipboardViewer = (IntPtr) SetClipboardViewer((int) InteropHelper.Handle);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not set up Clipboard Viewer on startup. \n" + ex.Message, "Error setting up Clipboard Viewer", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            uint MOD_ALT      = 0x0001;
            uint MOD_SHIFT    = 0x0004;
            uint MOD_NOREPEAT = 0x4000;
            // Register ALT + SHIFT + C
            bool hotKeyRegisteerd = RegisterHotKey(InteropHelper.Handle, 0x1, MOD_ALT | MOD_SHIFT | MOD_NOREPEAT, 0x43);
            if (!hotKeyRegisteerd)
            {
                MessageBox.Show("Could not register system-wide hotkey Alt + C", "Coult not register hot key", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PollClipboard()
        {
            if (!isCopying)
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        string data = Clipboard.GetText();
                        ClipboardHistoryItem c = new ClipboardHistoryItem(data);
                        ClipboardEntriesCol.Insert(0, c);
                    }
                }
                catch (Exception e)
                {
                    notifyIcon.ShowBalloonTip(2000, "Exception while attempting to access the system clipboard", e.Message, System.Windows.Forms.ToolTipIcon.Error);
                }
            }
        }

        /// <summary>
        /// Takes care of the external DLL calls to user32 to receive notification when
        /// the clipboard is modified. Passes along notifications to any other process that
        /// is subscribed to the event notification chain.
        /// </summary>
        protected IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_DRAWCLIPBOARD = 0x308;     // 776
            const int WM_CHANGECBCHAIN = 0x30D;     // 781
            const int VM_HOTKEY        = 0x312;     // 786

            switch (msg)
            {
                case WM_DRAWCLIPBOARD:
                    OnClipboardMessage.Invoke();
                    handled = true;
                    break;
                case VM_HOTKEY:
                    if (IsActive)
                    {
                        Hide();
                    }
                    else
                    {
                        ActivateWindow();
                    }

                    break;
                case WM_CHANGECBCHAIN:
                    if (wParam == NextClipboardViewer)
                        NextClipboardViewer = lParam;
                    else
                        SendMessage(NextClipboardViewer, msg, wParam,
                                    lParam);
                    handled = true;
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private void Open_MenuItem_Click(object Sender, EventArgs e)
        {
            ActivateWindow();
        }

        private void Quit_MenuItem_Click(object Sender, EventArgs e)
        {
            Shutdown();
        }

        private void ActivateWindow()
        {
            Show();
            Activate();
        }

        private string GetSelectedText()
        {
            // Most copied items will likely be around 10 characters long, initialize string builder to this
            // capacity to avoid some useless memory allocations
            if (ClipboardEntries.SelectedItems.Count > 0)
            {
                StringBuilder s = new StringBuilder(ClipboardEntries.SelectedItems.Count * 10);
                foreach (object o in ClipboardEntries.SelectedItems)
                {
                    ClipboardHistoryItem c = (ClipboardHistoryItem) o;
                    s.Append(c);
                }

                return s.ToString();
            }

            return String.Empty;
        }

        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            CopyProcedure();
        }

        private Task<string> FormatText(string s)
        {
            return Task.FromResult(s);
        }

        private Task<string> FormatText(string s, TextFormatter formatter)
        {
            return Task.FromResult(formatter(s));
        }

        private async void CopyProcedure()
        {
            isCopying = true;

            string selected = GetSelectedText();
            string finalText = await FormatText(selected);
            CopyTextToUserClipboard(finalText);
            StatusBar.Text = "Copied text to System Clipboard";

            isCopying = false;
        }

        private async void CopyProcedure(TextFormatter formatter)
        {
            isCopying = true;

            string selected = GetSelectedText();
            string finalText = await FormatText(selected, formatter);
            CopyTextToUserClipboard(finalText);
            LastCopiedText = finalText;
            StatusBar.Text = "Copied text to System Clipboard with format";

            isCopying = false;
        }

        private void CopyTextToUserClipboard(string text)
        {
            DateTime now = DateTime.Now;
            Clipboard.SetText(text);
            notifyIcon.BalloonTipText = text;
            notifyIcon.ShowBalloonTip(2000);
        }

        private void NiceClip2Window_KeyDown(object sender, KeyEventArgs e)
        {
            TextFormatter formatter = null;
            switch (e.Key)
            {
                case Key.Escape:
                    Hide();
                    break;
                case Key.Down:
                    
                    break;
                case Key.S: // Save last copied text to list
                    ClipboardHistoryItem c = new ClipboardHistoryItem(LastCopiedText);
                    ClipboardEntriesCol.Insert(0, c);
                    ClipboardEntries.SelectedIndex = 0;
                    break;
                case Key.T: // Title Case
                    formatter = TextUtilities.ToTitleCase;
                    break;
                case Key.W: // Trim White Spaces
                    formatter = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftShift)
                                ? Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)
                                    ? (TextFormatter) TextUtilities.TrimSpaces
                                    : TextUtilities.TrimSpacesEnd
                                : TextUtilities.TrimSpacesStart;
                    break;
                case Key.C: // Upper / Lower case
                    formatter = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)
                                ? (TextFormatter) TextUtilities.ToUpper : TextUtilities.ToLower;
                    break;
                case Key.Enter:
                    CopyProcedure();
                    break;
            }

            if (formatter != null) CopyProcedure(formatter);
        }

        private void NiceClip2Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void NiceClip2Window_Activated(object sender, EventArgs e)
        {
            DefaultFocus();
        }

        private void DefaultFocus()
        {
            ClipboardEntries.Focus();
            if (ClipboardEntries.SelectedIndex == -1 && ClipboardEntries.Items.Count > 0)
                ClipboardEntries.SelectedIndex = 0;
        }

        private void NiceClip2Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Shutdown();
        }

        private void Shutdown()
        {
            notifyIcon.Visible = false;
            notifyIcon.Icon = null;
            notifyIcon.Dispose();
            Application.Current.Shutdown();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClipboardEntries.SelectedItems.Count > 0)
            {
                int[] selectedIndices = new int[ClipboardEntries.SelectedItems.Count];
                List<ClipboardHistoryItem> selected = new List<ClipboardHistoryItem>(ClipboardEntries.SelectedItems.Cast<ClipboardHistoryItem>().ToList());
                int i = 0;
                foreach (ClipboardHistoryItem reference in selected)
                {
                    int selectedIndex = ClipboardEntriesCol.IndexOf(reference);
                    selectedIndices[i] = selectedIndex;
                    ClipboardEntriesCol.Remove(reference);

                    i++;
                }

                int firstIndex = selectedIndices.Min();

                if (ClipboardEntries.Items.Count > 0)
                {
                    if (ClipboardEntries.Items.Count >= firstIndex - 1)
                        ClipboardEntries.SelectedIndex = firstIndex;
                    else
                        ClipboardEntries.SelectedIndex = ClipboardEntries.Items.Count - 1;
                }

                StatusBar.Text = String.Format("Removed {0} {1}", selected.Count, selected.Count > 1 ? "items" : "item");
                ClipboardEntries.Focus();
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            StatusBar.Text = "System Clipboard content was cleared";
        }
    }
}
