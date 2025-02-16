using Microsoft.VisualBasic;

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

using static EditText;
using static WindowGrid;
using static GridMenu;
using static GridView;
using static TabsFile;
using static TextTrash;
using static ViewHTML;
using static ViewText;

class MenuView : MenuItem
{
    public MenuItem ViewInit()
    {
        Header = "_View";

        Items.Add(ViewMode.Init());
        Items.Add(ViewMode2.Init());
        Items.Add(ViewPrev.Init());
        Items.Add(ViewGoogle.Init());
        Items.Add(ViewTools.Init());
        Items.Add(viewWeb.Init());
        Items.Add(viewTexts.TextsInit());
        Items.Add(viewAlbum.AlbumInit());
        Items.Add(viewLink2.LinkInit());

        return this;
    }

    public class ClassViewMode : MenuItem // verander veld modus
    {
        public MenuItem Init()
        {
            Header = "_Mode";
            InputGestureText = "Ctrl+M";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.M, ModifierKeys.Control, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            if (windowGrid.gridMode < 3) windowGrid.gridMode += 1; else windowGrid.gridMode = 1;
            windowGrid.GridModes(windowGrid.gridMode);
        }
    }

    public ClassViewMode ViewMode = new ClassViewMode();

    public class ClassViewMode2 : MenuItem // verander veld modus
    {
        public MenuItem Init()
        {
            Header = "_Mode";
            InputGestureText = "Alt+M";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.M, ModifierKeys.Alt, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            if (windowGrid.gridMode > 1) windowGrid.gridMode -= 1; else windowGrid.gridMode = 1;
            windowGrid.GridModes(windowGrid.gridMode);
        }
    }

    public ClassViewMode2 ViewMode2 = new ClassViewMode2();

    public class ClassViewPrev : MenuItem // open vorige bestand
    {
        public MenuItem Init()
        {
            Header = "_Previous";
            InputGestureText = "Ctrl+-";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.OemMinus, ModifierKeys.Control, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            fileUseds.UsedsPrev();
        }
    }

    public ClassViewPrev ViewPrev = new ClassViewPrev();

    public class ClassViewGoogle : MenuItem // Start google
    {
        public MenuItem Init()
        {
            Header = "_Google";
            InputGestureText = "Ctrl+G";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.G, ModifierKeys.Control, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            string P = editText.TextWord();
            if (P.EndsWith(".html"))
            {
                P = GetFileAddress(P);
            }
            else if (!P.StartsWith("http"))
            {
                P = "https://www.bing.com/images/search?q=" + P;
            }
            Interaction.Shell(@"C:\Program Files (x86)\Internet Explorer\iexplore.exe " + '"' + P + '"', AppWinStyle.NormalFocus);
        }
    }

    public static ClassViewGoogle ViewGoogle = new ClassViewGoogle();

    public class ClassViewBinair : MenuItem // toon binaire inhoud
    {
        public MenuItem Init()
        {
            Header = "_Tools";
            InputGestureText = "Ctrl+U";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.U, ModifierKeys.Control, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            editText.TextTools();
        }
    }

    public static ClassViewBinair ViewTools = new ClassViewBinair();

    public class ViewWeb : MenuItem // toon HTML opnieuw
    {
        public MenuItem Init()
        {
            Header = "_Web";
            InputGestureText = "Alt+V";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.V, ModifierKeys.Alt, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            gridView.Children.Clear();
            gridView.Children.Add(HTMLView);
        }
    }

    public static ViewWeb viewWeb = new ViewWeb();

    public class ViewTexts : MenuItem
    {
        public MenuItem TextsInit()
        {
            Header = "_text";
            InputGestureText = "Alt+K";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.K, ModifierKeys.Alt, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            viewText.TextLoad();
        }
    }

    public static ViewTexts viewTexts = new ViewTexts();

    public class ViewAlbum : MenuItem // album met foto's maken
    {
        public MenuItem AlbumInit()
        {
            Header = "_album";
            InputGestureText = "Alt+a";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.A, ModifierKeys.Alt, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            textTrash.TrashAlbum();
        }
    }

    public static ViewAlbum viewAlbum = new ViewAlbum();

    public class ViewLink2 : MenuItem // link bestand openen
    {
        public MenuItem LinkInit()
        {
            //Background = BackColor;

            Header = "_Tags";
            InputGestureText = "Alt+Shift+T";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.T, ModifierKeys.Alt | ModifierKeys.Shift, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            textTrash.TrashLink2();
        }
    }

    public static ViewLink2 viewLink2 = new ViewLink2();

    public static MenuView menuView = new MenuView();
}