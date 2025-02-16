using Microsoft.VisualBasic; // voor input box
using Microsoft.VisualBasic.FileIO; // voor prullenbak

using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

using static EditText;
using static WindowGrid;
using static GridView;
using static ModsRigs;
using static RoomCamera;
using static RoomMods;
using static TextParser;
using static ViewHTML;
using static ViewImage;
using static ViewPlayer;
using static ViewRoom;
using static TextFunctions;

class TabsFile : TabItem
{
    public static Label rootsLabel = new Label();
    public static Label pathsLabel = new Label();
    public static Label filesLabel = new Label();
    public static Label usedsLabel = new Label();
    public static string moveWord = "";

    public class FileRoots : ListBox // bronnen lijst
    {
        public UIElement RootsInit() // Start bronnen
        {
            rootsLabel.Background = Brushes.DarkSlateGray;
            rootsLabel.Foreground = Brushes.White;
            rootsLabel.Content = "Roots";

            BorderThickness = new Thickness(0);
            Background = Brushes.Transparent;
            Foreground = Brushes.White;
            MaxHeight = 200;

            Items.Clear();

            string[] S;
            S = File.ReadAllLines("d:\\onedrive\\index\\root.txt");
            int N = S.Length;

            for (int I = 0; I < N; I++) Items.Add(S[I]);


            MouseUp += new MouseButtonEventHandler(This_MouseUp);

            return this;
        }

        public void RootsUpdate() // vernieuw bronnen lijst
        {
            fileUseds.Items.Clear();

            FileOpen(FileRoot() + @"\index.txt");
        }

        public void RootsReset()
        {
            editText.TextSave(); // sla het huidige tekst bestand eventueel op
            viewImage.imageFile = "";
            SelectedIndex = 0;
            RootsInit();
        }

        public void This_MouseUp(object sender, System.EventArgs e) // selecteer bron
        {
            RootsUpdate();
        }
    }

    public FileRoots fileRoots = new FileRoots(); // bronnen lijst object

    public class FilePaths : ListBox // folder lijst
    {
        public UIElement PathsInit()
        {
            pathsLabel.Background = Brushes.DarkSlateGray;
            pathsLabel.Foreground = Brushes.White;
            pathsLabel.Content = "Paths";

            Background = Brushes.Transparent;
            Foreground = Brushes.White;

            BorderThickness = new Thickness(0);
            MaxHeight = 200;

            MouseUp += This_MouseUp;

            return this;
        }

        public void PathsUpdate(string S) // vernieuw mappen lijst
        {
            string R = FileRoot();
            int L = R.Length;
            string P;
            string T;

            Items.Clear();
            try
            {
                if (S != R && S.Length > L) P = S.Substring(L); else P = "";
                Items.Add(FilePrev(P));
                Items.Add(P);

                foreach (string D in Directory.GetDirectories(S))
                {
                    T = D.Substring(L);
                    Items.Add(T);
                }
                SelectedIndex = 1;
                tabsFile.fileFiles.FilesUpdate(S);
            }
            catch { }
        }

        public static void This_MouseUp(object sender, System.EventArgs e) // selecteert pad
        {
            string S = FileRoot() + tabsFile.filePaths.SelectedItem.ToString();

            //tabsFile.FilePath.PathUpdt(S);
            FileOpen(S + @"\index.txt");
        }
    }

    public FilePaths filePaths = new FilePaths(); // padenlijst obect

    public class FileFiles : ListBox // bestandenlijst
    {
        public UIElement FilesInit() // Start bestandenlijst
        {
            filesLabel.Background = Brushes.DarkSlateGray;
            filesLabel.Foreground = Brushes.White;
            filesLabel.Content = "Files";

            Background = Brushes.Transparent;
            Foreground = Brushes.White;

            BorderThickness = new Thickness(0);
            MaxHeight = 200;

            MouseUp += new MouseButtonEventHandler(This_MouseUp);
            KeyUp += new KeyEventHandler(This_KeyUp);

            return this;
        }

        public void FilesUpdate(string S) // vernieuw bestandenlijst
        {
            if (!Directory.Exists(S)) return;
            string R = FileRoot();
            int L = R.Length;

            Items.Clear(); // wis lijst
            foreach (string F in Directory.GetFiles(S))
            {
                Items.Add(F.Substring(L));
            }
        }

        public void FilesSelect() // selecteer een bestand in de bestandenlijst
        {
            int I = tabsFile.fileFiles.SelectedIndex;

            if (I > -1)
            {
                FileOpen(FileRoot() + tabsFile.fileFiles.SelectedItem.ToString());
                tabsFile.fileFiles.SelectedIndex = I;
            }
        }

        public void This_MouseUp(object sender, System.EventArgs e) // muis toets omhoog
        {
            FilesSelect();
        }

        public void This_KeyUp(object sender, KeyEventArgs e) // toetsenbord toets omhoog
        {
            FilesSelect();
        }
    }

    public FileFiles fileFiles = new FileFiles(); // bestanden lijst object

    public class FileUseds : ListBox // gebruikte bestanden
    {
        public UIElement UsedsInit()
        {
            usedsLabel.Background = Brushes.DarkSlateGray;
            usedsLabel.Foreground = Brushes.White;
            usedsLabel.Content = "Useds";

            Background = Brushes.Transparent;
            Foreground = Brushes.White;

            BorderThickness = new Thickness(0);
            MaxHeight = 400;

            MouseUp += new MouseButtonEventHandler(This_MouseUp);

            return this;
        }

        public void UsedsUpdate(string s) // gebruikte bestanden lijst vernieuwen
        {
            int i = SelectedIndex; // geselecteerde positie onthouden
            int n = Items.Count; // aantal posities
            string r = FileRoot(); // bestanden bronpad
            int l = r.Length; // bestanden bron lengte

            if (s.Contains(r)) // als het bronpad in de bestandnaam voorkomt
            {
                s = s.Substring(l); // gebruikte bestand zonder bronpad

                if (n == 0) // als de gebruikte bestanden lijst nog leeg is
                {
                    i++;
                    Items.Insert(i, s); // bestand toevoegen
                    SelectedIndex = i;
                }
                else // als de gebruikte bestanden lijst niet leeg is
                {
                    string t = Items[i].ToString(); // huidige bestand

                    textParser.ParserLine(t, ','); // pars huidige bestand

                    if (W[0] != s) // als het huidige bestand anders is dan het nieuwe bestand
                    {
                        Items[i] = W[0] + ", " + editText.CaretIndex; // cursor positie onthouden
                        i++;
                        Items.Insert(i, s); // nieuw bestand toevoegen
                        SelectedIndex = i;
                    }
                }
            }
        }

        public void UsedsPrev() // vorige bestand openen
        {
            int i = SelectedIndex;

            if (i > 0)
            {
                Items.RemoveAt(i);
                i--;
                SelectedIndex = i;
                string s = Items[i].ToString();
                textParser.ParserLine(s, ',');
                i = (int)V[1];
                FileOpen(FileRoot() + W[0]);
                editText.CaretIndex = i;
            }
        }

        public void This_MouseUp(object sender, System.EventArgs e) // als een bestand geselecteerd wordt
        {
            string s = SelectedItem.ToString();
            textParser.ParserLine(s, ',');
            int i = (int)V[1];
            FileOpen(FileRoot() + W[0]);
            editText.CaretIndex = i;

            i = SelectedIndex + 1;
            int n = Items.Count;

            while (i < n)
            {
                Items.RemoveAt(i);
                n--;
            }
        }
    }

    public static FileUseds fileUseds = new FileUseds(); // gebruikte bestanden lijst object

    public TabItem FileInit() // initialiseer het bestanden tab blad
    {
        StackPanel FileStack = new StackPanel()
        {
            Background = Brushes.Transparent
        };

        FileStack.Children.Add(rootsLabel);
        FileStack.Children.Add(fileRoots.RootsInit());
        FileStack.Children.Add(pathsLabel);
        FileStack.Children.Add(filePaths.PathsInit());
        FileStack.Children.Add(filesLabel);
        FileStack.Children.Add(fileFiles.FilesInit());
        FileStack.Children.Add(usedsLabel);
        FileStack.Children.Add(fileUseds.UsedsInit());

        BorderThickness = new Thickness(0);

        Content = FileStack;

        Header = "File";

        return this;
    }

    public void FileRuns() // start
    {
        fileRoots.SelectedIndex = 0;

        fileRoots.RootsUpdate();
    }

    public string FileDate(DateTime D) // maakt een bestandnaam van de datum en tijd
    {
        string S = D.Year.ToString() + "-" + SL(D.Month.ToString(), '0', 2) + "-" + SL(D.Day.ToString(), '0', 2) + "-" +
                    SL(D.Hour.ToString(), '0', 2) + "-" + SL(D.Minute.ToString(), '0', 2) + "-" + SL(D.Second.ToString(), '0', 2) + "-" + SL(D.Millisecond.ToString(), '0', 3);

        return S;
    }

    public void FileDirs() // map maken
    {
        // W[0] = "makedirs"
        // W[1] = mapnaam

        string s = W[1];

        if (!s.StartsWith("\\"))
        {
            s = FileRoot() + GetFilePath(textFile) + "\\" + s;
        }
        else
        {
            s = FileRoot() + FileFirst(s) + s;
        }
        Directory.CreateDirectory(s);
    }

    public void FileDelete2() // verwijdert een bestand vanuit een bestand met alt+d
    {
        editText.TextSave();

        int caret = editText.CaretIndex;
        string line = editText.GetLineText(editText.GetLineIndexFromCharacterIndex(caret));

        textParser.ParserLine(line, ',');

        string file = W[0];
        file = FileFind(file);

        Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);

        if (textFile.Contains("index"))
        {
            editText.TextOpen(textFile);
        }
        else
        {
            int J = editText.GetLineIndexFromCharacterIndex(caret);

            int N = editText.LineCount;

            string[] T = new string[N];

            for (int I = 0; I < N; I++) T[I] = editText.GetLineText(I);

            editText.Clear();

            for (int I = 0; I < N - 1; I++)
            {
                if (I < J) editText.AppendText(T[I]); else editText.AppendText(T[I + 1]);
            }
        }

        editText.CaretIndex = caret;
    }

    public void FileExplore() // windows explorer openen
    {
        editText.TextSave(); // huidig tekst bestand opslaan

        string line = editText.GetLineText(editText.GetLineIndexFromCharacterIndex(editText.CaretIndex)); // geselecteerde regel

        textParser.ParserLine(line, ',');

        string file = W[0];

        file = FileFind(file);

        if (file == "" || !File.Exists(file)) file = FilePath("");

        W[1] = "C:\\Windows\\explorer.exe";
        W[2] = "/select," + '"' + file + '"';

        textParser.ParserStart();
    }

    public static string FilePath(string file) // pad van het huidige bestand zoeken
    {
        if (file == "") file = parserFile[FI];

        if (file != "") file = file.Substring(0, file.LastIndexOf("\\"));

        return file;
    }

    public static string FileType(string file)
    {
        int i = file.LastIndexOf('.');

        if (i == -1) return ""; else return file.Substring(i);
    }

    public static string FileFind(string word) // bestand zoeken
    {
        if (word.StartsWith("\\"))
        {
            MessageBox.Show(word + " begint met een \\");

            editText.TextOpen(parserFile[FI]);
            return "";

            //word = word.Substring(1);
        }

        string file = word;

        int i = file.IndexOf(":");

        if (i == 1) return file;

        string path = FilePath(""); // pad van het huidige bestand zoeken

        string type = FileType(word); // type van het nieuwe bestand zoeken

        if (type == "") // als er geen bestandtype is
        {
            if (word != "")
            {
                file = path + "\\" + word + ".txt";

                if (!File.Exists(file))
                {
                    file = FileRoot() + FileFirst(word) + word + ".txt"; // bestand vanaf bron pad

                    if (!File.Exists(file))
                    {
                        file = path + "\\" + word;

                        if (Directory.Exists(file))
                        {
                            file += "\\index.txt";
                        }
                        else
                        {
                            file = FileRoot() + FileFirst(word) + word;

                            if (Directory.Exists(file))
                            {
                                file += "\\index.txt";
                            }
                            else file = "";
                        }
                    }
                }
            }
            else
            {
                file = path + "\\index.txt";
            }
        }
        else // als bestandtype bekent is
        {
            file = path + "\\" + word; // bestand vanaf huidige map

            if (!File.Exists(file))
            {
                file = FileRoot() + FileFirst(word) + word; // bestand vanaf bron pad

                if (!File.Exists(file)) file = "";
            }
        }

        if (file == "")
        {
            if (parserRuns)
            {

                if (MessageBox.Show("bestand " + word + " niet gevonden, zoeken?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    editText.TextOpen(parserFile[FI]);

                    try
                    {
                        editText.Select(editText.Text.IndexOf(word), 1);
                    }
                    catch { }
                }
                else
                {
                    file = "";
                }

                parserRuns = false;

                return "";
            }
        }

        return file;
    }

    public void FileGoTo() // geselecteerde bestand openen
    {
        string file = editText.TextWord(); // geselecteerde woord bepalen

        if (file != "") // als het woord niet leeg is
        {
            editText.TextSave(); // sla het huidige bestand op

            if (file.Length > 4) // als het woord tenminste 4 karakters heeft
            {
                //if (S.Substring(0, 2) == "//") S = "https:" + S; // als het woord met 2 schuine strepen begint

                if (file.Substring(0, 4) == "http") // als het woord met http begint
                {
                    viewHTML.HTMLOpen(file); // open de html pagina

                    return; // procedure beeindigen
                }
            }

            file = FileFind(file);

            if (file != "")
            {
                if (FileType(file) == ".jpg")
                {
                    file = FilePath(file) + "\\index.txt";
                    FileOpen(FileFind(file));
                }
                else
                {
                    FileOpen(file);
                }
            }
        }
    }

    public bool FileMake(string S) // maak een nieuw bestand
    {
        string P = FilePath(S);

        if (!Directory.Exists(P))
        {
            if (MessageBox.Show("map " + P + " niet gevonden, maken?", "", MessageBoxButton.YesNo) == MessageBoxResult.No) return false;

            Directory.CreateDirectory(P);
        }

        if (!File.Exists(S) && !S.Contains("index.txt")) File.WriteAllText(S, ""); // als het bestand niet bestaat, maak dan een nieuw leeg bestand

        return true;
    }

    public void FileMove() // bestand of map verplaatsen
    {
        string line = editText.GetLineText(editText.GetLineIndexFromCharacterIndex(editText.CaretIndex));
        string path;
        string file;

        textParser.ParserLine(line, ','); // bepaal de woorden in de geselecteerde regel

        if (W[1] != "") moveWord = W[1];

        if (moveWord == "") return;

        file = W[0];

        if (FileType(file) == "") file += ".txt";

        if (moveWord.StartsWith("@")) // map verplaatsen
        {
            path = FilePath(textFile);

            string folder = path + "\\" + file;

            string dest = path + "\\" + moveWord.Substring(1);

            Directory.CreateDirectory(dest);

            dest = dest + "\\" + file;

            try
            {
                Directory.Move(folder, dest);

                editText.TextNext(); // ga naar volgende regel
            }
            catch
            {
                MessageBox.Show("verplaatsen is niet gelukt");
            }
        }
        else // bestand verplaatsen
        {
            int i = file.LastIndexOf("\\") + 1;

            string source;

            if (i > 0)
            {
                source = FileRoot() + FileFirst(file) + file;
                file = file.Substring(i);
            }
            else
            {
                source = FilePath(textFile) + "\\" + file; // bepaal het adres van het bron bestand
            }

            path = moveWord; // zoek een geselecteerd woord als doel pad

            int number = 0;

            try
            {
                number = Convert.ToInt32(moveWord);
            }
            catch { }

            if (number > 0)
            {
                if (moveWord.Length > 3)
                {
                    path = FilePrev(FilePrev(FilePath(textFile))) + "\\" + moveWord.Substring(0, 3) + "\\" + moveWord.Substring(3);
                }
                else
                {
                    moveWord = number.ToString("000");

                    path = FilePath(textFile);

                    while (!path.EndsWith("jpg")) path = FilePrev(path);

                    path = path + "\\" + moveWord + "\\000";
                }
            }
            else
            {
                path = FileRoot() + FileFirst(path) + path + "\\jpg\\000\\000"; // doel bestand
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                
                string name = Interaction.InputBox("studio naam", "");

                if (name != "") File.WriteAllText(FilePrev(path) + "\\" +  name + ".txt", "");
            }

            path += "\\" + file; // doel bestand

            try
            {
                File.Move(source, path); // verplaats bestand
                editText.TextNext(); // ga naar volgende regel
            }
            catch
            {
                MessageBox.Show("verplaatsen is niet gelukt");
            }
        }
    }

    public void FileNavi() // maakt een mappen boom
    {
        TreeView T = new TreeView()
        {
            FontFamily = new FontFamily("Consolas"),
            FontSize = 16
        };

        foreach (var D1 in Directory.GetDirectories(tabsFile.fileRoots.SelectedItem.ToString())) // zoek mappen
        {
            TreeViewItem T1 = new TreeViewItem()
            {
                Header = D1
            };

            try
            {
                foreach (var D2 in Directory.GetDirectories(D1))
                {
                    TreeViewItem T2 = new TreeViewItem()
                    {
                        Header = D2
                    };

                    try
                    {
                        foreach (var D3 in Directory.GetDirectories(D2))
                        {
                            TreeViewItem T3 = new TreeViewItem()
                            {
                                Header = D3
                            };

                            try
                            {
                                foreach (var D4 in Directory.GetDirectories(D3))
                                {
                                    TreeViewItem T4 = new TreeViewItem()
                                    {
                                        Header = D4
                                    };

                                    T3.Items.Add(T4);
                                }
                            }
                            catch { }

                            T2.Items.Add(T3);

                        }
                    }
                    catch { }
                    T1.Items.Add(T2);
                }
            }
            catch { }

            T.Items.Add(T1);
        }
        gridView.Children.Clear();
        gridView.Children.Add(T);
        windowGrid.GridMove(T, 0, 0, 2, 2);
    }

    public void FileNew() // maakt nieuw onderwerp
    {
        ListBox L = new ListBox();
        Window F = new Window();
        string S = editText.TextWord();

        L.Items.Clear();
        L.Items.Add(GetFileAddress("\\" + S)); // woord.txt in huidige folder
        L.Items.Add(GetFileAddress("\\" + S + "\\" + "index.txt")); // woord\index.txt in huidige folder
        L.Items.Add(GetFileAddress(S)); // woord.txt vanaf bron folder
        L.Items.Add(GetFileAddress(S + "\\" + "index.txt")); // woord\index.txt vanaf bron folder

        F.Height = 150;
        F.Width = 400;
        F.Left = 1500;
        F.Top = 500;
        F.Title = "maak een keuze";
        F.Content = L;
        F.Show();

        L.PreviewMouseLeftButtonUp += ListClick;
        F.KeyDown += ListKey;

        void ListClick(object sender, RoutedEventArgs e)
        {
            if (L.SelectedIndex > -1)
            {
                string T = L.SelectedItem.ToString();

                FileMake(T);
                FileOpen(T);
            }
            F.Close();
        }

        void ListKey(object sender, KeyEventArgs e)
        {
            int K = (int)e.Key - 75;
            if (K < 0) K = (int)e.Key - 35;

            if (K > -1 && K < 4)
            {
                string T = L.Items[K].ToString();

                FileMake(T);
                FileOpen(T);
            }
            F.Close();
        }
    }

    public static void FileOpen(string file) // bestand openen
    {
        string type = FileType(file);

        switch (type)
        {
            case "":
            case ".asm":
            case ".cs":  // c#
            case ".css": // html style
            case ".csv": // comma separated values
            case ".dat": // lego onderdeel
            case ".hex":
            case ".ino": // arduino code
            case ".js":  // java script
            case ".lst": // track list
            case ".mtl": // obj materiaal
            case ".obj": // 3d object
            case ".py":  // python
            case ".srt": // onder titeling
            case ".txt": // tekst bestand
            case ".vb":  // visual basic
                editText.TextOpen(file);
                break;
            case ".bvh":
            case ".dae":
            case ".fbx":
                RI = 0;
                if (!gridView.Children.Contains(viewRoom)) viewRoom.RoomOpen();
                RoomMod.Clear();
                modsRigs.RigsOpen(file);
                modsRigs.RigsAnimate();
                roomCamera.CameraInit(2);
                break;
            case ".avi":
            case ".gif":
            case ".mid":
            case ".mp3":
            case ".mp4":
            case ".mpg":
            case ".wav":
            case ".wmv":
                viewPlayer.PlayerOpen(file);
                break;
            case ".html":
            case ".xml":
                editText.TextOpen(file);
                viewHTML.HTMLOpen(file);
                break;
            case ".pdf":
            case ".webp":
            case ".avif":
                viewHTML.HTMLOpen(file);
                break;
            case ".bmp":
            case ".dds":
            case ".jpg":
            case ".png":
            //case ".tga":
            case ".tif":
                viewImage.ImageOpen(file);
                break;
        }
    }

    public void FileRename() // bestand hernoemen
    {
        editText.TextSave();

        string F;

        try
        {
            if (fileFiles.SelectedValue != null)
            {
                F = fileFiles.SelectedItem.ToString();
                string B = FileRoot() + F;
                string P = FileRoot() + GetFilePath(B) + "\\";
                string D = Interaction.InputBox("", "nieuwe naam", FileName(F)); // nieuwe naam invoeren

                if (D != "")
                {
                    if (D.IndexOf(".") == -1) D += FileType(B); // als geen nieuw bestandtype bekent is, type van oude bestand gebruiken
                    D = P + D;
                    File.Move(B, D); // bestand hernoemen
                    FileOpen(D); // bestand met nieuwe naam openen
                }
            }
        }
        catch { }
    }

    public string FileSave(string file) // slaat een bestand op
    {
        if (file.StartsWith("\\"))
        {
            file = FileRoot() + FileFirst(file) + file;
        }
        else if (file.Contains(":"))
        {

        }
        else
        {
            file = FilePath("") + "\\" + file;
        }

        return file;
    }

    public static TabsFile tabsFile = new TabsFile();

    // functies

    public static string GetFileAddress(string file) // volledige address naar een bestand bepalen
    {
        if (file == null || file == "") return ""; // eindig de procedure als er geen bestand naam bekent is

        if (file.Contains(":")) return file; // adres bevat een schijf letter bevat is het pad al volledig

        if (file.StartsWith("\\")) // als S met een \ begint, dan is het adres vanaf het huidige pad
        {
            file = parserFile[FI].Substring(0, parserFile[FI].LastIndexOf("\\")) + file;
        }
        else // als S niet met een \ begint, dan is het adres vanaf het bron pad
        {
            file = FileRoot() + FileFirst(file) + file;
        }

        string type = FileType(file);

        if (type == "") file += ".txt";

        return file;
    }

    public static string FileName(string S) // bepaalt de bestandnaam
    {
        if (S != null)
        {
            int I = S.LastIndexOf(@"\"); // zoek laatste \

            if (I > -1) S = S.Substring(I + 1); // verwijder links t.m. \
            I = S.LastIndexOf("."); // zoek .
            if (I > -1) S = S.Substring(0, I); // verwijder rechts vanaf .
        }

        return S;
    }

    public static string GetFilePath(string s) // bepaalt het pad van een bestand
    {
        if (s != null)
        {
            int i = s.LastIndexOf("\\"); // laatste backslash zoeken

            if (i > -1) // als laatste backslah gevonden is
            {
                s = s.Substring(0, i); ; // bestandnaam verwijderen

                string r = FileRoot().ToLower();

                if (s.ToLower().Contains(r))
                {
                    s = s.Substring(r.Length); // bronpad verwijderen
                }
            }
            else
            {
                s = "";
            }
        }
        else
        {
            s = "";
        }

        return s;
    }

    public static string FileRoot() // bepaalt het bron pad
    {
        string S = tabsFile.fileRoots.SelectedItem.ToString();

        return S;
    }

    public static string FileFirst(string path) // eerste karakter van een pad bepalen
    {
        if (path == "")
        {
            return path;
        }
        else
        {
            path = path.Substring(0, 1).ToLower();
            if (path[0] < 58) path = "#";
            path = "\\" + path + "\\";
        }

        return path;
    }

    public static string GetPathAddress(string path) // bepaalt het adres van een pad
    {
        if (path == "")
        {
            path = FileRoot() + GetFilePath(parserFile[FI]);
        }
        else if (!path.StartsWith("\\")) // pad vanaf het bronpad bepalen
        {
            path = FileRoot() + FileFirst(path) + GetFilePath(path);
        }
        else // pad vanaf huidige pad bepalen
        {
            path = FileRoot() + GetFilePath(parserFile[FI]) + "\\" + GetFilePath(path);
        }

        return path;
    }

    public static string FilePrev(string path) // vorige pad bepalen
    {
        if (path != "")
        {
            int I = path.LastIndexOf("\\");
            if (I > -1) path = path.Substring(0, I); else path = "";
        }

        return path;
    }
}