using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NiceCLip2
{
    /// <summary>
    /// Provides an implementaton of the basic window chrome functionnalities for a custom window chrome
    /// </summary>
    public partial class MainWindow : Window
    {
        protected void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
