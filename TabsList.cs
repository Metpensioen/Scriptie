using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using static EditText;
using static TabsFile;

public class TabsList : TabItem
{
    readonly ListBox listBox = new ListBox();

    public TabItem ListInit()
    {
        Button button = new Button
        {
            Content = "load studios",
        };

        button.Click += Button_Click;

        listBox.MaxHeight = 800;

        StackPanel listPanel = new StackPanel();

        listPanel.Children.Add(button);
        listPanel.Children.Add(listBox);

        Content = listPanel;
        Header = "List";

        return this;
    }

    public void Button_Click(object sender, EventArgs e) // lijst met studio namen vullen
    {
        List<string> list1 = new List<string>();
        List<string> list2 = new List<string>();

        string number;
        string file;
        string name;
        int count;
        int n;
        string high = "000";
        string found = "";

        string path = FilePath(textFile);
        
        while (!path.EndsWith("jpg"))
        {
            path = FilePrev(path);
        }

        listBox.Items.Clear();

        foreach (string studio in Directory.GetDirectories(path))
        {
            number = studio.Substring(studio.LastIndexOf("\\") + 1);

            count = Convert.ToInt16(number);

            if (Convert.ToInt16(high) < count && count < 800) high = number;

            count = Directory.GetFiles(studio, "*.txt").Length;

            if (count == 1)
            {
                file = Directory.GetFiles(studio)[0];

                name = file.Substring(file.LastIndexOf("\\") + 1);

                if (name.StartsWith("'"))
                {
                    name = name.Substring(1).Trim();

                    File.Move(file, studio + "\\" + name);
                }

                name = name.Substring(0, name.LastIndexOf("."));
            }
            else
            {
                MessageBox.Show(studio + " heeft " + count + " namen");
                
                return;
            }

            if (!list1.Contains(name))
            {
                list1.Add(name);
            }
            else
            {
                foreach (string zoek in Directory.GetDirectories(path))
                {
                    if (File.Exists(zoek + "\\" + name + ".txt"))
                    {
                        found = zoek.Substring(zoek.Length - 3);
                        break;
                    }
                }

                MessageBox.Show(number + " " + name + " is dubbel met " + found );

                return;
            }

            list2.Add(number + "\\" + name);
        }

        n = list1.Count;

        list1.Sort();

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (list2[j].Substring(4) == list1[i])
                {
                    listBox.Items.Add(list2[j]);
                    break;
                }
            }
        }

        listBox.Items.Add(high);

        editText.Focus();
    }

    public static TabsList tabsList = new TabsList();
}