using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using static EditText;

public class TabsTree : TabItem
{
    public TreeView treeView = new TreeView()
    {
        FontFamily = new FontFamily("Consolas"),
        FontSize = 16
    };

    public TabItem TreeInit()
    {
        Header = "Tree";

        string[] treeDrives = Directory.GetLogicalDrives();

        foreach (string drive in treeDrives)
        {
            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Header = drive;

            treeView.Items.Add(treeViewItem);

            TreeDirs(treeViewItem);
        }

        Content = treeView;

        treeView.SelectedItemChanged += This_SelectedItemChanged;

        return this;
    }

    public void TreeDirs(TreeViewItem item)
    {
        string path = item.Header.ToString();

        try
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                TreeViewItem newItem = new TreeViewItem();
                newItem.Header = dir;
                item.Items.Add(newItem);
            }
        }
        catch { }
    }

    public void This_SelectedItemChanged(object sender, RoutedEventArgs e)
    {
        string path;

        TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
        path = item.Header.ToString();
        TreeDirs((TreeViewItem)treeView.SelectedItem);

        editText.Clear();

        foreach (string file in Directory.GetFiles(path))
        {
            editText.AppendText(file + "\n");
        }
    }

    public static TabsTree tabsTree = new TabsTree();
}