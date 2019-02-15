
<h1>So what is NiceClip anyway?</h1>
<p>
    NiceClip is a clipboard monitor. Essentially, NiceClip keeps in memory the things you copy to your clipboard.
    So whenever you hit Ctrl + C, Ctrl + X or you do RightClick -> Copy, NiceClip registers what it is you've just copied so you can acces it later on.
	Note, however, that <b>none of your clipboard data ever leaves your computer</b>.
</p>
<hr />
<h1>Usage</h1>
<h2>Basic Usage</h2>
<ul>
	<li>
		Press `Alt + Shift + C` at any moment to open the NiceClip window.
	</li>
    <li>
		When shown, the NiceClip window is `Topmost` meaning that it will display above any other window on your desktop. To dismiss it, either click away from it,
		 press `Escape`, click on the 'X' symbol located at the top right corner of the window or press `Alt + Ctrl + C`.
    </li>
    <li>
        To completely exit the application, right click the NiceClip icon in the task tray and then click "Quit". All your clipboard history will be lost
    </li>
    <li>
        To add a clipboard history entry, simply copy anything in your clipboard. "Cutting" something does the same.
    </li>
    <li>
        You can select any number of elements from the clipboard history list and press `Enter` or press the 'Copy' button to copy them to your system clipboard.
		If multiple elements are selected, they will all be concatenated in one single string inside your system clipboard.
    </li>
    <li>
        To clear the contents of your clipboard, click the "Clear" button in the graphical interface.
    </li>
    <li>
        To clear all of the clipboard history list, click the "Clear History" button in the graphical interface or press the "Delete" key of your keyboard.
    </li>
</ul>
<h2>Copying Formatted Text</h2>
<p>
	Whenever you select at least one elemen from the clipboard history list, you can usethe following shortcuts to copy the text with a special format
</p>
<ul>
	<li>Press `Enter` to copy the text 'as is'.</li>
	<li>Press `C` to copy the text formatted to lower case.</li>
	<li>Press `Shift + C` to copy the text formatted to UPPER CASE.</li>
	<li>Press `Shift + T` to copy the text formatted to Title Case.</li>
	<li>Press `W` to copy the text while removing empty spaces on the left.</li>
	<li>Press `Shift + W` to copy the text while removing empty spaces on the right.</li>
	<li>Press `Ctrl + Shift + W` to copy the text while removing any whit spaces on the left and right.</li>
</ul>
<hr />
<h1>How is NiceClip useful?</h1>
<p>
    When I'm programming, I often find myself in a situation in which I, for instance, cut a function with the intention of pasting it back
    somewhere else but I forget to paste it and copy something else in my clipboard. But wait! Now my whole function is gone! Now I have to
    Ctrl + Z to retrieve my precious code! To avoid this kind of situation is essentially why I decided to build NiceClip.
    I found such an application is also useful in the context of a job that requires a lot of data entry.
</p>
<hr />
<h1>Compatibility</h1>
<p>
    Since this application was built using WPF, it is only compatible with Windows operating systems (as far as I know). (I know, I'm sorry *nix users but I just felt like coding this in .NET).
    So far the application has been tested on
    <ul>
        <li>Windows 10</li>
    </ul>
</p>
<hr />
<h1>Limitations</h1>
<p>
    Literally anyone is free to use this application however they please. It is also free to use in an enterprise context. This software is under an MIT License, see the LICENSE file for
    further legal information.
</p>
<hr />
<h1>Contributing</h1>
<p>Any contributions of any kind is very welcomed!</p>
