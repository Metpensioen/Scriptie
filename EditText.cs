using Microsoft.VisualBasic;

using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using static GridMenu;
using static GridView;
using static ShowGame;
using static ViewImage;
using static ViewText;
using static TabsFile;
using static TextFunctions;
using static TextMenu;
using static TextParser;
using static TextVoice;
using static ViewData;

using Microsoft.VisualBasic.Devices;

public class EditText : TextBox
{
    public static string textFile = ""; // tekstbestandnaam
    public static bool textEdit = false; // is waar als het tekst bestand veranderd is
    public string textFind = ""; // zoek tekst
    public string textReplace = ""; // vervang tekst
    public bool TextLoad = false;
    public bool TextRuns = false;
    public int lastNumber; // laatste regelnummer onthouden

    public TextBox TextInit() // tekstveld starten
    {
        viewText.TextStyle(this);

        System.Windows.Input.Mouse.SetCursor(Cursors.Arrow);

        ContextMenu = textMenu.MenuInit(); // contextmenu starten

        textParser.ParserInit(); // parser starten
        textVoice.SynthInit(); // voorleesstem starten

        TextChanged += This_TextChanged;
        MouseDoubleClick += This_MouseDoubleClick;
        PreviewKeyDown += This_PreviewKeyDown;
        PreviewKeyUp += This_PreviewKeyUp;
        SelectionChanged += This_SelectionChanged;
        Loaded += This_Loaded;

        return this;
    }

    public void TextDone() // tekstveld stoppen
    {
        TextSave(); // tekstbestand opslaan
        File.WriteAllText("d:\\onedrive\\index\\last.txt", textFile); // bestandnaam in last.txt opslaan
        parserRuns = false; // parser stoppen
        textVoice.SynthDone(); // voorlezen stoppen
    }

    public void TextOpen(string file) // open een tekst bestand
    {
        TextSave(); // vorige bestand opslaan

        if (!File.Exists(file) && !file.Contains("index.txt")) // als het bestand bestaat of het is een index bestand
        {
            if (MessageBox.Show("bestand " + file + " niet gevonden, maken?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                File.WriteAllText(file, "");
            }
        }

        TextRuns = false; // blokkeer selectie
        parserRuns = false; // uitvoeren van een script stoppen

        parserFile[FI] = file; // 
        TextLoad = false;
        textFile = file;
        fileUseds.UsedsUpdate(textFile);

        try
        {
            Text = File.ReadAllText(textFile);
            //textLines = File.ReadAllLines(textFile);
        }
        catch { }

        lastNumber = -1;
        TextLoad = true;
        textEdit = false;
        string R = FileRoot();

        if (!file.ToLower().Contains(R.ToLower())) // als het bestand een ander bronpad heeft
        {
            int N = tabsFile.fileRoots.Items.Count; // aantal bronnen

            for (int I = 0; I < N; I++) // bronnen doorzoeken
            {
                R = tabsFile.fileRoots.Items[I].ToString(); // bron
                if (file.ToLower().Contains(R.ToLower()))
                {
                    tabsFile.fileRoots.SelectedIndex = I; // als het bestand bij deze bron hoort, deze bron selecteren
                    tabsFile.fileRoots.Focus();
                    break;
                }
            }
        }

        if (file.Contains("index.txt")) TextIndex();

        string t = GetFilePath(textFile);

        t = FileRoot() + t;
        tabsFile.filePaths.PathsUpdate(t);
        tabsFile.fileFiles.FilesUpdate(t);

        if (textFile.Contains("data"))
        {
            viewData.DataView(viewData.TI);
        }
        else // folder afbeelding openen 
        {
            try
            {
                FileOpen(Directory.GetFiles(t, "folder.*")[0]);
            }
            catch { }
        }

        TextRuns = true;

        Focus();

        TextInfo(); // bestandnaam tonen
    }

    public void TextCopy() // copieer de tekst uit een bestand in het huidige bestand
    {
        // W[0] = "copy"
        // W[1] = bestand naam

        string file = FileFind(W[1]);

        if (File.Exists(file)) AppendText(File.ReadAllText(file));
    }

    public void TextFind()
    {
        textFind = Interaction.InputBox("", "", textFind);

        int L = textFind.Length;
        int J = CaretIndex;
        int I = Text.IndexOf(textFind, J + L);
        if (I > -1)
        {
            Select(I, L);
        }
        else
        {
            Select(J, 0);
        }
    }

    public void TextIndex() // lijst met map en bestand namen maken
    {
        if (textFile.Contains("index.txt")) // alleen voor een index.txt bestand
        {
            string path = FilePath(textFile);
            bool trash = path.Contains("Trash");
            int length = path.Length + 1;
            string type;
            string S = "";
            string O = "";
            string I;
            string X = "";
            string name;

            Clear();

            if (Directory.Exists(path))
            {
                foreach (string folder in Directory.GetDirectories(path)) // mappen zoeken
                {
                    S = folder.Substring(length); // bron map verwijderen
                    X += S + "\n"; // map naar index schrijven
                }

                if (X != "") X += "\n"; // als er mappen zijn gevonden, een lege regel toevoegen

                foreach (string file in Directory.GetFiles(path)) // bestanden zoeken
                {
                    S = file;
                    name = FileName(S);
                    type = FileType(S);

                    if (trash && name != "folder" && type != ".txt" && type != ".srt") // hernoem bestanden in de backup bronmap; .txt en folder.jpg bestand overslaan
                    {
                        if (!name.StartsWith("202")) // als de bestandnaam geen datum en tijd is dan bestand hernoemen
                        {
                            var E = new FileInfo(S);
                            I = tabsFile.FileDate(E.CreationTime);
                            try
                            {
                                name = path + "\\" + I + type; // nieuwe naam
                                File.Move(S, name);
                                name = I;
                            }
                            catch { }
                        }
                    }

                    if (name != "folder") // folder.jpg niet toevoegen
                    {
                        if (O != type && O != "") X += "\n"; // als het bestand type veranderd een regel overslaan

                        O = type; // nieuwe bestandtype onthouden

                        if (type == ".txt") S = name; else S = name + type; // als bestandtype .txt is dan niet toevoegen

                        X += S + "\n"; // bestand naar index schrijven
                    }
                }

                Text = X;
            }
        }
        else
        {
            MessageBox.Show("geen index bestand");
        }
    }

    public void TextInfo() // bestandnaam tonen
    {
        int N = LineCount - 1; // aantal regels
        int I = GetLineIndexFromCharacterIndex(CaretIndex); // geselecteerde regel

        int S = 0;

        foreach (char c in SelectedText) if ((int)c == 10) S++; // aantal geselecteerde regels

        if (textEdit) windowText.Text = textFile + " * " + N + ", " + I + ", " + S; else windowText.Text = textFile + ",  " + N + ", " + I + ", " + S;

        textParser.ParserDoEvents(); // toon dit 
    }

    public void TextNext() // cursor in de volgende regel zetten
    {
        try
        {
            Select(GetCharacterIndexFromLineIndex(GetLineIndexFromCharacterIndex(CaretIndex) + 1), 1); // volgende foto tonen
        }
        catch { }
    }

    public void TextSave() // tekst bestand opslaan
    {
        if (!textFile.Contains("index.txt")) // als het bestand geen index bestand is
        {
            if (textEdit) // als het bestand veranderd is
            {
                try
                {
                    File.WriteAllText(textFile, Text);

                    textEdit = false;
                    
                    TextInfo(); // info tonen
                }
                catch { }
            }
        }
        else // als het wel een index bestand is
        {
            if (File.Exists(textFile)) File.Delete(textFile); // als het index bestand bestaat, dit verwijderen
        }
    }

    public void TextSort() // tekst bestand sorteren
    {
        DataSet D = new DataSet("D1"); // database maken // hoeft eigenlijk niet

        //DataTable dataTable = new DataTable();

        D.Tables.Add("T1"); // tabel maken

        for (int I = 0; I < 15; I++) // 15 kolommen toevoegen
        {
            //dataTable.Columns.Add(new DataColumn("C" + I.ToString(), typeof(string)));

            D.Tables["T1"].Columns.Add(new DataColumn("C" + I.ToString(), typeof(string)));
        }

        DataRow R;

        string T;
        int N = LineCount; // aantal tekst regels

        for (int I = 0; I < N; I++) // voor alle regels
        {
            T = GetLineText(I).Trim();

            if (T != "")
            {
                textParser.ParserLine(GetLineText(I), ','); // woorden bepalen
                R = D.Tables["T1"].NewRow(); // rij maken

                R[0] = W[0]; // kolom 0 bestandnaam
                R[1] = FileType(W[0]); // kolom 2 bestand type

                for (int J = 1; J < 14; J++) if (W[J] != "") R[J + 1] = W[J]; // alle andere woorden

                D.Tables["T1"].Rows.Add(R); // rij toevoegen
            }
        }

        DataView dataView = new DataView(D.Tables["T1"]); // dataview maken alleen een dataview kan gesorteerd worden

        // dataView.AddNew // er kunnen rijen aan een dataview toegevoegd worden

        N = D.Tables["T1"].Columns.Count; // aantal kolommen, dat was toch 15 ???

        string S = "";

        for (int I = 2; I < N; I++) // voor kolom 2 tot 15
        {
            if (S != "") S += ", ";

            S += "C" + I.ToString() + " ASC"; // sorteer regel maken
        }

        S += ", C1 ASC, C0 ASC"; // laatste sorteer voorwaarden

        dataView.Sort = S; // sorteer dataview

        N = dataView.Count; // voor alle rijen
        S = "";
        DataRowView L; // 
        T = "";

        for (int I = 0; I < N; I++)
        {
            L = dataView[I];

            if (I > 0 && T != L[1].ToString()) S += "\n";

            T = L[1].ToString();
            S += L[0].ToString();

            for (int J = 2; J < 14; J++)
            {
                if (L[J].ToString().Trim() != "") S += ", " + L[J].ToString().Trim();
            }

            S += "\n";
        }

        Text = S;

        TextSave();
    }

    public void TextTags() // labels in een foto bestand opslaan
    {
        viewImage.ImageTagsSet("", "");

        int I = GetLineIndexFromCharacterIndex(CaretIndex); // huidige regel nummer
        I++; // verhoog regel nummer

        try
        {
            Select(GetCharacterIndexFromLineIndex(I), 1); // ga naar volgende regel
            string S = GetLineText(I);
            I = S.IndexOf(".jpg") + 4;
            CaretIndex += I;
        }
        catch { }
    }

    public void TextTools() // binaire inhoud tonen met ctrl+u
    {
        if (tabsFile.fileFiles.SelectedIndex == -1) return;

        TextSave();

        byte[] B = File.ReadAllBytes(FileRoot() + tabsFile.fileFiles.SelectedItem.ToString());
        
        int L = B.Length / 16 - 1;
        string S;
        byte T;

        if (L > 64) L = 64;

        textFile = "";

        Clear();

        for (int I = 0; I <= L; I++)
        {
            S = HexW(I * 16) + " ";

            for (int J = 0; J < 16; J++)
            {
                S += HexB(B[I * 16 + J]) + " ";
            }

            S += " | ";

            for (int J = 0; J < 16; J++)
            {
                T = B[I * 16 + J];
                if (T > 19 && T < 127)
                {
                    S += Convert.ToChar(T);
                }
                else
                {
                    S += ".";
                }
            }
            
            AppendText(S + "\n");
        }

        textEdit = false;
    }

    public string TextWord() // het geselecteerde woord bepalen
    {
        int i = SelectionStart;
        int j = i;
        int n = Text.Length;
        string s = "";

        if (i == n) i--; // als cursor aan het eind van de tekst staat cursor 1 plaats verlagen

        while (i > 0 && Text[i] != ',' && Text[i] > (char)31) i--; // begin van het woord zoeken

        if (Text[i] == ',') i++; // als het woord met een komma begint cursor 1 plaats verhogen

        while (j < n && Text[j] != ',' && Text[j] > (char)31) j++; // eind van het woord zoeken

        if (j > i) s = Text.Substring(i, j - i).Trim(); // als het eind groter is dan het begin, woord bepalen en eventuele spaties verwijderen

        return s; 
    }

    // events

    public void This_Loaded(object sender, EventArgs e) // als een bestand geladen is
    {
        TextInfo();
    }

    public void This_MouseDoubleClick(object sender, EventArgs e) // als dubbel geklikt is
    {
        tabsFile.FileGoTo(); // open geselecteerde bestand
    }

    public void This_PreviewKeyDown(object sender, KeyEventArgs e) // als op een toets wordt gedrukt
    {
        if (e.Key == Key.Escape) // als op de escape toets is gedrukt
        {
            parserRuns = false; // script stoppen
            gridView.Children.Clear(); // verwijder alle media
            e.Handled = true;
        }

        if (e.Key == Key.Tab) // als op de tab toets is gedrukt, voegt 4 spaties tussen de tekst, laat tekst verspring
        {
            int I = CaretIndex;
            Text = Text.Insert(I, "    ");
            CaretIndex = I + 4;
            e.Handled = true;
        }
    }

    public void This_PreviewKeyUp(object sender, KeyEventArgs e) // als een toets losgelaten wordt
    {
        if (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightCtrl))
        {
            int I = (int)e.Key;

            if (I > 34 && I < 44) showGame.GameGoto(I); // als een nummer toets is ingedrukt
        }
    }

    public void This_SelectionChanged(object sender, RoutedEventArgs e) // als de cursor verplaatst is
    {
        if (TextRuns) // als het tekst bestand klaar is met laden
        {
            TextInfo(); // toon bestand naam en regel nummers

            string line = null;
            int number;
            int count = LineCount;

            try
            {
                number = GetLineIndexFromCharacterIndex(CaretIndex);

                if (lastNumber == number) return; else lastNumber = number;

                //if (number >= 8992) number--;
                line = GetLineText(number); // onthoud de geselecteerde regel
                //line = textLines[number];
            }
            catch { }

            if (line != null) // als er een regel geselecteerd is
            {
                textParser.ParserLine(line, ','); // bepaal de woorden in de regel

                for (int i = 0; i < 1; i++) // voor de eerste twee woorden
                {
                    string file = W[i];

                    if (file.Length > 2) // als het woord meer dan 5 karakters heeft
                    {
                        string type = FileType(file); // bepaal het bestand type

                        switch (type) // als het bestand type voldoet open het bestand
                        {
                            case "":
                            case ".txt":
                                {
                                    string path = FilePath(textFile) + "\\" + file;

                                    try
                                    {
                                        int n = Directory.GetFiles(path, "*.jpg").Length;

                                        file = Directory.GetFiles(path, "*.jpg")[0];

                                        if (n > 0) FileOpen(file);
                                    }
                                    catch { }

                                    break;
                                }
                            case ".avif":
                            case ".bmp":
                            case ".dds":
                            case ".gif":
                            case ".jpg":
                            case ".lst":
                            case ".mp3":
                            //case ".mp4":
                            case ".png":
                            case ".tif":
                            //case ".tga": 
                            case ".webp":
                                file = FileFind(file);

                                if (file != "") // als het bestand bestaat
                                {
                                    FileOpen(file); // open het bestand
                                    if (System.Windows.Input.Keyboard.IsKeyDown(Key.Down)) textParser.ParserWait(200); // wacht 200 miliseconden om te kunnen scrollen
                                }

                                break;
                        }
                    }
                }
            }
        }
    }

    public void This_TextChanged(object sender, System.EventArgs e) // als een tekst bestand verandert is
    {
        textEdit = true;
        TextInfo();
    }

    public static EditText editText = new EditText(); // edittekst object
}