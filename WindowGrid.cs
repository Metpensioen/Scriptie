using System;
using System.Windows;
using System.Windows.Controls;

using static EditText;
using static GridEdit;
using static GridMenu;
using static GridTabs;
using static GridView;
using static TabsFile;
using static ViewHTML;
using static ViewImage;
using static ViewRoom;
using static ViewPlayer;

public class WindowGrid : Grid
{
    public int gridMode; // raster indeling

    public Grid GridInit() // initialiseer het formulier grid
    {
        for (int i = 1; i <= 5; i++) // maak 5 kolommen
        {
            ColumnDefinitions.Add(new ColumnDefinition());
        }

        for (int i = 1; i <= 2; i++) // maak 2 rijen
        {
            RowDefinitions.Add(new RowDefinition());
        }

        RowDefinitions[0].Height = new GridLength(30);
        RowDefinitions[1].Height = new GridLength(900);

        Children.Add(gridMenu.MenuInit());
        Children.Add(gridView.ShowInit());
        Children.Add(gridEdit.EditInit());
        Children.Add(gridTabs.TabsInit());

        GridModes(1);

        return this;
    }

    public void GridRuns() // start het formulier grid
    {
        tabsFile.FileRuns(); // start het bestanden tabblad
        editText.Focus();
    }

    public void GridDone()
    {
        viewHTML.HTMLDone();
        viewRoom.RoomDone();
        editText.TextDone();
        viewPlayer.PlayerDone();
    }

    public void GridMove(UIElement E, Int16 X, Int16 Y, Int16 W, Int16 H)
    {
        SetColumn(E, X);
        SetColumnSpan(E, W);
        SetRow(E, Y);
        SetRowSpan(E, H);
    }

    public void GridModes(int I)
    {
        if (I == 1)
        {
            GridMove(gridView, 0, 1, 2, 1);
            GridMove(gridEdit, 2, 1, 2, 1);
            GridMove(gridTabs, 4, 1, 1, 1);

            gridEdit.Visibility = Visibility.Visible;
            gridTabs.Visibility = Visibility.Visible;

            viewImage.HorizontalAlignment = HorizontalAlignment.Left;
        }
        else if (I == 2)
        {
            GridMove(gridView, 0, 1, 3, 1);
            GridMove(gridEdit, 3, 1, 2, 1);

            gridEdit.Visibility = Visibility.Visible;
            gridTabs.Visibility = Visibility.Hidden;
        }
        else if (I == 3)
        {
            GridMove(gridView, 0, 1, 5, 1);

            gridEdit.Visibility = Visibility.Hidden;
            gridTabs.Visibility = Visibility.Hidden;

            viewImage.HorizontalAlignment = HorizontalAlignment.Center;
        }

        gridMode = I;
    }

    public static WindowGrid windowGrid = new WindowGrid();
}