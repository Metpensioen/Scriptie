using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using static EditText;
using static ViewData;

class TabsData : TabItem
{
    public ListBox TableList = new ListBox();
    public ListBox StudioList = new ListBox();

    public TabItem DataInit()
    {
        StackPanel DataStack = new StackPanel();

        DataStack.Children.Add(TableList);
        DataStack.Children.Add(StudioList);
        StudioList.Height = 40;

        Header = "Data";
        Background = Brushes.Black;
        Foreground = Brushes.White;
        Content = DataStack;

        TableList.PreviewMouseUp += TableList_MouseUp;
        StudioList.PreviewMouseUp += StudioList_MouseUp;

        return this;
    }

    public void TableList_MouseUp(object sender, EventArgs e)
    {
        viewData.DataView(TableList.SelectedIndex); // geselecteerde tabel aan het datagrid koppelen
    }

    public void StudioList_MouseUp(object sender, EventArgs e)
    {
        if (StudioList.Items.Count == 0)
        {
            string[] s = File.ReadAllLines(@"d:\backup\s\studio\data.txt");

            int n = s.Length;

            for (int i = 0; i < n; i++) StudioList.Items.Add(s[i]);

            StudioList.Height = 200;

            return;
        }

        Clipboard.SetText(StudioList.SelectedItem.ToString()); // geselecteerde studio naar het clipboard copieeren
        editText.Focus();
    }

    public static TabsData tabsData = new TabsData();
}