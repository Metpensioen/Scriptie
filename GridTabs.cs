using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using static TabsCalc;
using static TabsCtrl;
using static TabsData;
using static TabsDraw;
using static TabsFile;
using static TabsList;
using static TabsMidi;
using static TabsParts;
using static TabsPlay;
using static TabsRigs;
using static TabsRoom;
using static TabsTree;

class GridTabs : Grid
{
    public TabControl tabsControl = new TabControl();

    public UIElement TabsInit() // Start het tabs veld
    {
        tabsControl.BorderThickness = new Thickness(0);
        tabsControl.Background = Brushes.Transparent;

        tabsControl.Items.Add(tabsPlay.PlayInit());
        tabsControl.Items.Add(tabsRoom.RoomInit());
        tabsControl.Items.Add(tabsParts.PartsInit());
        tabsControl.Items.Add(tabsRigs.RigsInit());
        tabsControl.Items.Add(tabsData.DataInit());
        tabsControl.Items.Add(tabsMidi.MidiInit());
        tabsControl.Items.Add(tabsCtrl.CtrlInit());
        tabsControl.Items.Add(tabsCalc.CalcInit());
        tabsControl.Items.Add(tabsDraw.DrawInit());
        tabsControl.Items.Add(tabsFile.FileInit());
        tabsControl.Items.Add(tabsList.ListInit());
        tabsControl.Items.Add(tabsTree.TreeInit());

        Children.Add(tabsControl); // TabsControl aan GridTabs toevoegen

        tabsControl.SelectionChanged += TabSet_SelectionChanged;

        tabsFile.IsSelected = true;

        return this;
    }

    void TabSet_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        foreach (TabItem item in tabsControl.Items)
        {
            item.Background = new SolidColorBrush(item.IsSelected ? Colors.White : Colors.Transparent);
            item.Foreground = new SolidColorBrush(item.IsSelected ? Colors.Black : Colors.White);
        }
    }

    public static GridTabs gridTabs = new GridTabs();
}