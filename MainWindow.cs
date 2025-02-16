using System;
using System.ComponentModel; // voor closing
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;

using static WindowGrid;

public class MainWindow : Window
{
    public WindowChrome windowChrome = new WindowChrome();
    public static Brush backColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x10, 0x28, 0x3B));

    public void WindowInit() // formulier instellen
    {
        windowChrome.GlassFrameThickness = new Thickness(0, 30, 0, 0);

        WindowChrome.SetWindowChrome(this, windowChrome);

        Background = backColor;
        BorderThickness = new Thickness(0);

        Width = 2250;
        Height = 930;

        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        Content = windowGrid.GridInit(); // raster toevoegen

        Closing += WindowDone;
        StateChanged += WindowSize;
    }

    public void WindowRuns() // start
    {
        windowGrid.GridRuns();
        ShowDialog();
    }

    public void WindowDone(object sender, CancelEventArgs e) // stop
    {
        windowGrid.GridDone();
    }

    public void WindowSize(object sender, EventArgs e) // afmeting veranderen
    {
        BorderThickness = (WindowState == WindowState.Maximized) ? new Thickness(8) : new Thickness(0);
    }

    public static MainWindow mainWindow = new MainWindow();

    [STAThread]

    static void Main()
    {
        mainWindow.WindowInit();
        mainWindow.WindowRuns();
        mainWindow.Close(); 
    }
}