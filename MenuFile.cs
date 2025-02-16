using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using static EditText;
using static GridMenu;
using static MainWindow;
using static TabsFile;

class MenuFile : MenuItem
{
    public MenuItem FileInit()
    {
        Background = backColor;
        Foreground = Brushes.White;
        Header = "_File";
        Items.Add(fileNew.NewInit());
        Items.Add(fileOpen.OpenInit());
        Items.Add(fileSave.SaveInit());
        Items.Add(fileLast.LastInit());
        Items.Add(fileMove.MoveInit());
        Items.Add(fileNavi.NaviInit());
        Items.Add(fileFavorite.FavoriteInit());
        Items.Add(fileExplore.ExploreInit());
        Items.Add(fileExport.ExportInit());
        Items.Add(fileReset.ResetInit());
        Items.Add(fileWork.WorkInit());
        Items.Add(fileDelete.DeleteInit());
        Items.Add(fileDelete2.Delete2Init());
        Items.Add(fileRename.RenameInit());

        return this;
    }

    class FileDelete : MenuItem
    {
        public MenuItem DeleteInit()
        {
            Background = backColor;
            Header = "_Delete";
            InputGestureText = "Alt+Delete";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.Delete, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            //tabsFile.FileDelete();
        }
    }

    readonly FileDelete fileDelete = new FileDelete(); // verwijder een bestand met alt+delete

    class FileDelete2 : MenuItem
    {
        public MenuItem Delete2Init()
        {
            Background = backColor;
            Header = "_Delete";
            InputGestureText = "alt+d";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.D, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            tabsFile.FileDelete2();
        }
    }

    readonly FileDelete2 fileDelete2 = new FileDelete2(); // verwijder een bestand met alt+delete

    class FileRename : MenuItem
    {
        public MenuItem RenameInit()
        {
            Background = backColor;
            Header = "_Rename";
            InputGestureText = "F2";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F2, ModifierKeys.None, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            tabsFile.FileRename();
        }
    }

    readonly FileRename fileRename = new FileRename(); // verwijder een bestand met alt+delete

    class FileExplore : MenuItem
    {
        public MenuItem ExploreInit()
        {
            Background = backColor;
            Header = "_Explore";
            InputGestureText = "Ctrl+E";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.E, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            tabsFile.FileExplore();
        }
    }

    readonly FileExplore fileExplore = new FileExplore(); // opent een geselecteerde bestand in de verkenner met ctrl+e

    class FileExport : MenuItem // exporteert een model met F4
    {
        public MenuItem ExportInit()
        {
            Background = backColor;
            Header = "_Export";
            InputGestureText = "F4";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F4, ModifierKeys.None, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            //modsRigs.RigsExport();
        }
    }

    readonly FileExport fileExport = new FileExport();

    class FileFavorite : MenuItem // open favoriet bestand
    {
        public MenuItem FavoriteInit()
        {
            Background = backColor;
            Header = "_Favorite";
            InputGestureText = "Ctrl+Q";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.Q, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            FileOpen(File.ReadAllText(tabsFile.fileRoots.Items[0].ToString() + "\\saved.txt"));
        }
    }

    readonly FileFavorite fileFavorite = new FileFavorite(); // opent een favoriet bestand met ctrl+q

    class FileLast : MenuItem
    {
        public MenuItem LastInit() // laatste bestand openen met Alt+l
        {
            Background = backColor;
            Header = "_Last";
            InputGestureText = "Alt+L";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.L, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            FileOpen(File.ReadAllText(tabsFile.fileRoots.Items[0] + "\\last.txt"));
        }
    }

    readonly FileLast fileLast = new FileLast();

    class FileMove : MenuItem // ctrl+o bestand verplaatsen
    {
        public MenuItem MoveInit()
        {
            Background = backColor;
            Header = "_Move";
            InputGestureText = "Ctrl+O";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.O, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            tabsFile.FileMove();
        }
    }

    readonly FileMove fileMove = new FileMove();

    class FileNavi : MenuItem
    {
        public MenuItem NaviInit()
        {
            Background = backColor;
            Header = "_Navigate";
            InputGestureText = "Ctrl+P";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.P, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            tabsFile.FileNavi();
        }
    }

    readonly FileNavi fileNavi = new FileNavi(); // ctrl+p, maak bestand boom

    class FileNew : MenuItem // ctrl+n
    {
        public MenuItem NewInit()
        {
            Background = backColor;
            BorderThickness = new Thickness(0);
            Header = "_New";
            InputGestureText = "Alt+N";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.N, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            tabsFile.FileNew();
        }
    }

    readonly FileNew fileNew = new FileNew(); // maakt een niew bestand met ctrl+n

    class FileOpen : MenuItem // geselecteerde bestand met F12 openen
    {
        public MenuItem OpenInit()
        {
            Background = backColor;
            BorderThickness = new Thickness(0);
            Header = "_Open";
            InputGestureText = "F12";
            Click += This_Click;
            gridMenu.MenuShort(Key.F12, ModifierKeys.None, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            tabsFile.FileGoTo();
        }
    }

    readonly FileOpen fileOpen = new FileOpen();

    class FileReset : MenuItem // bron pad resetten
    {
        public MenuItem ResetInit()
        {
            Background = backColor;
            Header = "_Reset";
            InputGestureText = "Ctrl+R";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.R, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            tabsFile.fileRoots.RootsReset();
        }
    }

    readonly FileReset fileReset = new FileReset();

    class FileSave : MenuItem // bestand bewaren en onthouden
    {
        public MenuItem SaveInit()
        {
            Background = backColor;
            Header = "_Save";
            InputGestureText = "Ctrl+S";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.S, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            editText.TextSave();
            File.WriteAllText(FileRoot() + "\\saved.txt", textFile);

        }
    }

    readonly FileSave fileSave = new FileSave();

    public class FileWork : MenuItem // werk bestand openen
    {
        public MenuItem WorkInit()
        {
            Background = backColor;
            Header = "_Work";
            InputGestureText = "Ctrl+W";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.W, ModifierKeys.Control, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            FileOpen(GetFileAddress("werk.txt"));
        }
    }

    readonly FileWork fileWork = new FileWork();

    public static MenuFile menuFile = new MenuFile();
}