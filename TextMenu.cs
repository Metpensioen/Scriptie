using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using static EditText;
using static TabsFile;

public class TextMenu : ContextMenu
{
    public class MenuMove : MenuItem
    {
        public MenuItem MoveInit()
        {
            Header = "Move text.txt";

            Click += MoveClick;

            return this;
        }

        public void MoveClick(object sender, RoutedEventArgs e)
        {
            string P = FilePath(textFile);
            string B = GetFileAddress("text.txt");
            string D = FileRoot() + P + @"\boek";
            Directory.CreateDirectory(D);
            D += @"\00.txt";
            File.Move(B, D);
            editText.TextOpen(D);
        }
    }

    public MenuMove menuMove = new MenuMove();

    public class MenuInfo : MenuItem
    {
        public MenuItem InfoInit()
        {
            Header = "Maak info.txt";

            Click += InfoClick;

            return this;
        }

        public void InfoClick(object sender, RoutedEventArgs e)
        {
            string B = FilePath(textFile) + "\\info.txt";

            editText.TextOpen(B);
        }
    }

    public MenuInfo menuInfo = new MenuInfo();

    public class MenuSource : MenuItem
    {
        public MenuItem SourceInit()
        {
            Header = "Maak bron";

            Click += SourceClick;

            return this;
        }

        public void SourceClick(object sender, RoutedEventArgs e)
        {
            string star = FilePrev(FilePath(textFile));

            star = star.Substring(star.LastIndexOf("\\") + 1);

            star = star.Replace(" ", "-");
            star = star.Replace("'", "-");

            string name = FileName(textFile).Substring(4) + "-" + star;

            editText.AppendText("https://sexhd.pics/gallery/" + name);
        }
    }

    public MenuSource menuSource = new MenuSource();

    public class MenuLego : MenuItem
    {
        public MenuItem LegoInit()
        {
            Header = "Lego onderdeel";

            Click += LegoClick;

            return this;
        }

        public void LegoClick(object sender, RoutedEventArgs e)
        {
            string file = editText.TextWord();

            file = FileRoot() + FileFirst(file) + file + ".txt";
            File.WriteAllText(file, "");

            editText.TextOpen(file);

            string name = FileName(textFile);

            editText.AppendText("material, " + name + "\n");
            editText.AppendText("file, part.bin");
        }
    }

    public MenuLego menuLego = new MenuLego();

    public class MenuFile : MenuItem // bestand uit bestanden lijst verwijderen
    {
        public MenuItem FileInit()
        {
            Header = "Delete bestand";

            Click += FileClick;

            return this;
        }

        void FileClick(object sender, EventArgs e)
        {
            if (tabsFile.fileFiles.SelectedIndex != -1)
            {
                string S = FileRoot() + tabsFile.fileFiles.SelectedItem.ToString();

                editText.TextSave();
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(S, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                S = FileRoot() + tabsFile.filePaths.Items[1].ToString();
                tabsFile.fileFiles.FilesUpdate(S);
                FileOpen(S + @"\index.txt");
                editText.TextIndex();
            }
        }
    }

    public MenuFile menuFile = new MenuFile();

    public class MenuFolder : MenuItem
    {
        public MenuItem FolderInit()
        {
            Header = "Delete folder";

            Click += FolderClick;

            return this;
        }

        void FolderClick(object sender, EventArgs e)
        {
            string folder = tabsFile.filePaths.SelectedItem.ToString();
            int i = folder.LastIndexOf("\\");
            string name = folder.Substring(i);
            string path = folder.Substring(0, i);
            string root = FileRoot();
            string source = root + folder;
            string destination = root + "\\#" + path;

            Directory.CreateDirectory(destination);

            destination += name;

            try
            {
                Directory.Move(source, destination);
            }
            catch { }
        }
    }

    public MenuFolder menuFolder = new MenuFolder();

    public ContextMenu MenuInit()
    {
        //Items.Add(TextCM1.CM1Init());
        Items.Add(menuInfo.InfoInit());
        Items.Add(menuSource.SourceInit());
        Items.Add(menuLego.LegoInit());
        Items.Add(menuFile.FileInit());
        Items.Add(menuFolder.FolderInit());

        return this;
    }

    public static TextMenu textMenu = new TextMenu(); // contextmenu
}
