using Microsoft.VisualBasic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using static EditText;
using static GridMenu;
using static GridView;
using static MainWindow;
using static TabsFile;
using static TextParser;
using static TextTrash;
using static ViewHTML;
using static ViewImage;

public class MenuEdit : MenuItem
{
    public MenuItem EditInit()
    {
        Foreground = Brushes.White;

        Header = "_Edit";

        Items.Add(editNotepath.NotepathInit());
        Items.Add(editPhoto.PhotoInit());
        Items.Add(editPhoto2.Photo2Init());
        Items.Add(editPhoto3.Photo3Init());
        Items.Add(editPhoto4.Photo4Init());
        Items.Add(editIndex.IndexInit());
        Items.Add(editIndex2.Index2Init());
        Items.Add(editLink.LinkInit());
        Items.Add(editScene.SceneInit());
        Items.Add(editScene2.Scene2Init());
        Items.Add(editTags.TagsInit());
        Items.Add(editTags3.Tags3Init());
        Items.Add(editFind.FindInit());
        Items.Add(editRepl.ReplInit());
        Items.Add(editSort.SortInit());
        Items.Add(editBook.BookInit());

        return this;
    }

    public class EditNotepath : MenuItem // tekst bestand buiten Scriptie openen
    {
        public MenuItem NotepathInit()
        {
            Background = backColor;

            Header = "_Notepath";
            InputGestureText = "Ctrl+K";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.K, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            Interaction.Shell("D:\\Code\\C#\\EditText\\bin\\debug\\EditText.exe " + '"' + textFile + '"', AppWinStyle.NormalFocus);
        }
    }

    public EditNotepath editNotepath = new EditNotepath();

    class EditPhoto : MenuItem
    {
        public MenuItem PhotoInit()
        {
            Background = backColor;

            Header = "_Photo";
            InputGestureText = "F3";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F3, ModifierKeys.None, This_Click);

            return this;
        }

        public void This_Click(object sender, RoutedEventArgs e)
        {
            if (gridView.Children.Contains(HTMLView))
            {
                textTrash.TrashSave();
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

            }
            else
            {
                W[0] = "";
                photoFile = viewImage.imageFile;
                gridView.ShowPhoto();
            }
        }
    }

    readonly EditPhoto editPhoto = new EditPhoto(); // foto van GridView maken

    class EditPhoto2 : MenuItem // slaat alle foto's opnieuw op om de afmetingen te normaliseren
    {
        public MenuItem Photo2Init()
        {
            Background = backColor;

            Header = "_Photo2";
            InputGestureText = "Ctrl+F3";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F3, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            parserRuns = true;

            int N = editText.LineCount; // aantal foto's
            int J = editText.GetLineIndexFromCharacterIndex(editText.CaretIndex); // huidige regel

            for (int i = J; i < N; i++)
            {
                if (FileType(viewImage.imageFile) == ".jpg")
                {
                    photoFile = viewImage.imageFile;
                    gridView.ShowPhoto();
                }

                if (!textFile.Contains("index")) editText.TextNext();

                textParser.ParserDoEvents();

                if (!parserRuns) return;
            }
        }
    }

    readonly EditPhoto2 editPhoto2 = new EditPhoto2();

    class EditPhoto3 : MenuItem // afbeeldingen van een web pagina opslaan
    {
        public MenuItem Photo3Init()
        {
            Background = backColor;

            Header = "_Photo3";
            InputGestureText = "Alt+F3";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F3, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            viewHTML.HTMLSave();
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }
    }

    readonly EditPhoto3 editPhoto3 = new EditPhoto3();

    class EditPhoto4 : MenuItem
    {
        public MenuItem Photo4Init()
        {
            Background = backColor;

            Header = "_Photo4";
            InputGestureText = "Alt+R";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.R, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            //showPict.PictCrop();
        }
    }

    readonly EditPhoto4 editPhoto4 = new EditPhoto4(); // fotorand verwijderen

    class EditIndex : MenuItem
    {
        public MenuItem IndexInit()
        {
            Background = backColor;

            Header = "_Index";
            InputGestureText = "Ctrl+I";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.I, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            editText.TextIndex();
        }
    }

    readonly EditIndex editIndex = new EditIndex(); // ctrl+i, maak index

    class EditIndex2 : MenuItem // maak continu index 
    {
        public MenuItem Index2Init()
        {
            Background = backColor;

            Header = "_Index";
            InputGestureText = "Alt+I";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.I, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            parserRuns = true;

            while (parserRuns) // herhaal indexeren tot op F9 wordt gedrukt
            {
                editText.TextIndex();
                textParser.ParserWait(2000);
            }
        }
    }

    readonly EditIndex2 editIndex2 = new EditIndex2(); // ctrl+i, maak continue een index stoppen met F9

    class EditTags : MenuItem // labels in een foto bestand opslaan
    {
        public MenuItem TagsInit()
        {
            Background = backColor;

            Header = "_Tags";
            InputGestureText = "Ctrl+T";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.T, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            editText.TextTags();
        }
    }

    readonly EditTags editTags = new EditTags();

    class EditLink : MenuItem // link bestand maken
    {
        public MenuItem LinkInit()
        {
            Background = backColor;

            Header = "_Link";
            InputGestureText = "Alt+T";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.T, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            textTrash.TrashLink();
        }
    }

    readonly EditLink editLink = new EditLink();

    class EditScene : MenuItem // scene lijst maken
    {
        public MenuItem SceneInit()
        {
            Background = backColor;

            Header = "_Scene";
            InputGestureText = "Alt+O";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.O, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            textTrash.TrashScene();
        }
    }

    readonly EditScene editScene = new EditScene();

    class EditScene2 : MenuItem // scene lijst maken
    {
        public MenuItem Scene2Init()
        {
            Background = backColor;

            Header = "_Scene2";
            InputGestureText = "Alt+P";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.P, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            textTrash.TrashScene2();
        }
    }

    readonly EditScene2 editScene2 = new EditScene2();

    class EditTags3 : MenuItem // labels in een foto bestand opslaan
    {
        public MenuItem Tags3Init()
        {
            Background = backColor;

            Header = "_Tags";
            InputGestureText = "Ctrl+Shift+T";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.T, ModifierKeys.Control | ModifierKeys.Shift, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            int N = editText.LineCount; // aantal regels

            int J = editText.GetLineIndexFromCharacterIndex(editText.CaretIndex); // geselecteerde regel nummer als start regel
            editText.TextTags();
            parserRuns = true;

            for (int I = J; I < N; I++) // voor alle regels
            {
                editText.SelectionStart = editText.GetCharacterIndexFromLineIndex(I); // verplaats cursor hierdoor worden eventuele .jpg of .gif bestanden getoond
                textParser.ParserDoEvents();
                if (!parserRuns) return;
                editText.TextTags();
            }
        }
    }

    readonly EditTags3 editTags3 = new EditTags3(); // ctrl+t, labels opslaan in foto bestand

    class EditFind : MenuItem // vind tekst
    {
        public MenuItem FindInit()
        {
            Background = backColor;

            Header = "_Find";
            InputGestureText = "Ctrl+F";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            editText.TextFind();
        }
    }

    readonly EditFind editFind = new EditFind();

    class EditRepl : MenuItem
    {
        public MenuItem ReplInit()
        {
            Background = backColor;

            Header = "_Replace";
            InputGestureText = "Ctrl+H";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.H, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            editText.textFind = Interaction.InputBox("", "zoek", editText.textFind);
            editText.textReplace = Interaction.InputBox("", "vervang", editText.textReplace);
            int I = 0;
            int L = 0;
            int N;
            string S;
        volgende:
            try
            {
                I = editText.Text.IndexOf(editText.textFind, L);
                L = I + editText.textFind.Length;
            }
            catch { }
            S = editText.Text;
            N = S.Length;
            if (I > -1 && L < N)
            {
                editText.Text = S.Substring(0, I) + editText.textReplace + S.Substring(L);
                textParser.ParserDoEvents();
                goto volgende;
            }
        }
    }

    readonly EditRepl editRepl = new EditRepl();

    class EditSort : MenuItem // tekst sorteren
    {
        public MenuItem SortInit()
        {
            Background = backColor;

            Header = "_Sort";
            InputGestureText = "Ctrl+D";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.D, ModifierKeys.Control, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            editText.TextSort();
        }
    }

    readonly EditSort editSort = new EditSort();

    public class EditBook : MenuItem // boek van scene maken
    {
        public MenuItem BookInit()
        {
            Background = backColor;

            Header = "_Book";
            InputGestureText = "Alt+B";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.B, ModifierKeys.Alt, This_Click);

            return this;
        }

        void This_Click(object sender, RoutedEventArgs e)
        {
            textTrash.TrashBook();
        }
    }

    public EditBook editBook = new EditBook();

    public static MenuEdit menuEdit = new MenuEdit();
}