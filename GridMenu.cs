using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using static WindowGrid;
using static MainWindow;
using static MenuFile;
using static MenuEdit;
using static MenuView;
using static MenuStart;
using static MenuHelp;
using static ViewImage;

class GridMenu : Menu
{
    public static TextBox menuText = new TextBox(); // menu text
    public static TextBox windowText = new TextBox(); // formulier titel

    public Menu MenuInit() // menu initialiseren
    {
        BorderThickness = new Thickness(0);
        Background = Brushes.Transparent;
        Foreground = Brushes.White;
        FontFamily = new FontFamily("Consolas");
        FontSize = 16;

        Rectangle MenuIcon = new Rectangle()
        {
            Width = 30,
            Height = 30,
            Fill = new ImageBrush(viewImage.ImageLoad("d:\\data\\png\\sc\\c#.png"))
        };

        Items.Add(MenuIcon);

        Items.Add(menuFile.FileInit());
        Items.Add(menuEdit.EditInit());
        Items.Add(menuView.ViewInit());
        Items.Add(menuStart.StartInit());
        Items.Add(menuHelp.HelpInit());

        menuText.BorderThickness = new Thickness(0);
        menuText.Background = Brushes.Transparent;
        menuText.Foreground = Brushes.White;
        menuText.FontFamily = new FontFamily("Consolas");
        menuText.FontSize = 16;

        Items.Add(menuText);
        menuText.HorizontalAlignment = HorizontalAlignment.Right;

        windowGrid.GridMove(gridMenu, 0, 0, 3, 1);

        windowText.BorderThickness = new Thickness(0);
        windowText.Background = Brushes.Transparent;
        windowText.Foreground = Brushes.White;
        windowText.FontFamily = new FontFamily("Consolas");
        windowText.FontSize = 16;

        Menu formDock = new Menu()
        {
            Background = Brushes.Transparent
        }; 

        formDock.Items.Add(windowText);

        windowGrid.Children.Add(formDock);
        windowGrid.GridMove(formDock, 2, 0, 2, 1);

        Rectangle menuButton = new Rectangle()
        {
            Width = 130,
            Height = 30,
            Fill = new ImageBrush(viewImage.ImageLoad("d:\\data\\png\\sc\\buttons.png")),
            HorizontalAlignment = HorizontalAlignment.Right,
        };

        DockPanel menuDock = new DockPanel();
        
        menuDock.Children.Add(menuButton);

        windowGrid.Children.Add(menuDock);
        windowGrid.GridMove(menuDock, 4, 0, 1, 1);

        return this;
    }

    public void MenuShort(Key K, ModifierKeys M, ExecutedRoutedEventHandler E) // menu shortcut
    {
        // voor Ctrl samen met Alt; gebruik ModifierKeys.Control | ModifierKeys.Alt

        RoutedCommand C = new RoutedCommand();

        C.InputGestures.Add(new KeyGesture(K, M));
        mainWindow.CommandBindings.Add(new CommandBinding(C, E));
    }

    public static GridMenu gridMenu = new GridMenu();
}