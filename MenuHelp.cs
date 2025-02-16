using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

using static GridMenu;
using static TabsFile;

class MenuHelp : MenuItem
{
    public MenuItem HelpInit()
    {
        Header = "_Help";
        InputGestureText = "F1";
        Click += new RoutedEventHandler(This_Click);
        gridMenu.MenuShort(Key.F1, ModifierKeys.None, This_Click);

        HorizontalAlignment = HorizontalAlignment.Right;

        return this;
    }

    public static void This_Click(object sender, RoutedEventArgs e)
    {
        FileOpen(GetFileAddress("hulp.txt"));
    }

    public static MenuHelp menuHelp = new MenuHelp();
}