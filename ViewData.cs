using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using static EditText;
using static GridMenu;
using static GridView;
using static TabsData;
using static TabsFile;
using static TextParser;

class ViewData : DataGrid
{
    public int TI; // tabel index
    public int CI; // column index
    public DataTable[] dataTable = new DataTable[8];
    public DataView[] gridTable = new DataView[8];
    public int[,] CW = new int[8, 16];
    public string[] CV = new string[16];

    public UIElement DataInit() // Start database
    {
        FontFamily = new FontFamily("Consolas");
        FontSize = 16;

        Margin = new Thickness(0);
        BorderThickness = new Thickness(0);

        Foreground = Brushes.White;
        Background = Brushes.Transparent;
        RowBackground = Brushes.Transparent;
        HorizontalGridLinesBrush = Brushes.DarkSlateGray;
        VerticalGridLinesBrush = Brushes.DarkSlateGray;

        Style SC = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
        SC.Setters.Add(new Setter(BackgroundProperty, Brushes.Transparent));
        ColumnHeaderStyle = SC;

        RowHeaderWidth = 0;

        PreviewMouseRightButtonUp += This_PreviewMouseRightButtonUp;
        PreviewMouseLeftButtonUp += This_PreviewMouseLeftButtonUp;

        return this;
    }

    public void DataColumn() // voeg een kolom aan een tabel toe
    {
        // W[0] = "column"
        // W[1] = kolomnaam
        // W[2] = [type]
        // W[3] = [uniek]
        // W[4] = [breedte]
        // W[5] = [voorwaarde]

        CI = dataTable[TI].Columns.Count;

        dataTable[TI].Columns.Add(new DataColumn(W[1]));

        switch (W[2].ToLower())
        {
            case "float": dataTable[TI].Columns[CI].DataType = typeof(Single); break;
            case "int": dataTable[TI].Columns[CI].DataType = typeof(int); break;
            default: dataTable[TI].Columns[CI].DataType = typeof(string); break;
        }
        if (W[3] != "") dataTable[TI].Columns[CI].Unique = true;
        if (W[4] != "") CW[TI, CI] = (int)V[4]; else CW[TI, CI] = 50;
        CV[CI] = W[5];
    }

    public void DataDirs() // voeg paden aan een tabel toe
    {
        // W[0] = "dirs"
        // W[1] = [bron pad]
        // W[2] = [bron tabel]
        // W[3] = [bron kolom]

        string FR = FileRoot(); // bron pad
        int RL = FR.Length; // lengte van het bron pad
        string BP = W[1];
        if (BP == "") BP = GetPathAddress(""); else BP = FileRoot() + FileFirst(BP) + BP; // zoek pad
        int BI = DataNumb(W[2]); // bron tabel index
        string BC = W[3]; // kolom met paden in de bron tabel
        DataRow NR; // nieuwe data rij
        string T;
        int cn = dataTable[TI].Columns.Count;

        if (W[2] == "") // als er geen bron tabel is opgegeven
        {
            foreach (string D in Directory.GetDirectories(BP)) // zoek naar mappen in het zoek pad
            {
                if (BP == FR) T = D.Substring(RL); else T = D.Substring(RL + 2); // verwijder het bron pad en de eerste letter
                T = T.Substring(1);
                W[1] = T;
                DataRows(); // sla de gevonden map op in de nieuwe tabel
            }
        }
        else // wel een bron tabel
        {
            int RI = dataTable[TI].Rows.Count + 1; // rij index

            foreach (DataRow R in dataTable[BI].Rows) // voor alle rijen in de bron tabel
            {
                T = @"\" + R[BC].ToString();
                if (T.Length == 2) BP = FileRoot() + T; else BP = FileRoot() + FileFirst(T) + T; // volledige zoek pad

                if (Directory.Exists(BP))
                {
                    foreach (string D in Directory.GetDirectories(BP)) // voor alle mappen in de bron map
                    {
                        NR = dataTable[TI].NewRow(); // nieuwe rij voor doel tabel
                        NR[0] = RI; // rij index in kolom 1
                        NR[1] = D.Substring(RL + 3);  // zet het pad zonder bron pad in kolom 1

                        if (cn == 3) NR[2] = D.Substring(D.LastIndexOf("\\") + 1);

                        dataTable[TI].Rows.Add(NR); // zet de nieuwe rij in de doel tabel
                        RI += 1; // verhoog de rij index
                    }
                }
                else
                {
                    MessageBox.Show("pad " + BP + " niet gevonden"); // zoek pad niet gevonden
                    return;
                }
            }
        }
    }

    public void DataFile() // bestanden aan een tabel toevoegen
    {
        // W[0] = "files"
        // W[1] = [bron pad]
        // W[2] = [bron tabel]
        // W[3] = [bron kolom]
        // W[4] = [bestandtype]

        string FR = FileRoot(); // bronpad
        int RL = FR.Length; // bronpad lengte
        string BP = W[1]; // subbronpad naam
        if (BP == "") BP = GetPathAddress(""); else BP = FileRoot() + FileFirst(BP) + BP; // zoek pad
        int BI = DataNumb(W[2]); // bron tabel index
        string BC = W[3]; // kolom met paden in de bron tabel
        DataRow NR; // nieuwe data rij
        string T;

        if (W[2] == "") // geen bron tabel opgegeven
        {
            foreach (string D in Directory.GetFiles(BP)) // zoek naar mappen in het zoek pad
            {
                if (BP == FR) T = D.Substring(RL); else T = D.Substring(RL + 2); // verwijder het bron pad en de eerste letter
                T = T.Substring(1);
                W[1] = T;
                DataRows(); // sla de gevonden map op in de nieuwe tabel
            }
        }
        else // wel een bron tabel
        {
            int RI = dataTable[TI].Rows.Count + 1; // rij index

            foreach (DataRow R in dataTable[BI].Rows) // voor alle rijen in de bron tabel
            {
                T = @"\" + R[BC].ToString();

                if (T.Length == 2) BP = FileRoot() + T; else BP = FileRoot() + FileFirst(T) + T; // volledige zoek pad

                if (Directory.Exists(BP))
                {
                    foreach (string F in Directory.GetFiles(BP)) // voor alle mappen in de bron map
                    {
                        if (F.EndsWith(W[4]))
                        {
                            NR = dataTable[TI].NewRow(); // nieuwe rij voor doel tabel
                            NR[0] = RI; // rij index in kolom 1
                            NR[1] = F.Substring(RL + 3);  // zet het pad zonder bron pad in kolom 1
                            dataTable[TI].Rows.Add(NR); // zet de nieuwe rij in de doel tabel
                            RI += 1; // verhoog de rij index
                        }
                    }
                }
                else
                {
                    MessageBox.Show("pad " + BP + " niet gevonden"); // zoek pad niet gevonden
                    return;
                }
            }
        }
    }

    public void DataFind() // bestand of map zoeken
    {
        // W[0] = "find"
        // W[1] = tabel naam
        // W[2] = pad kolom
        // W[3] = kolom naam
        // W[4] = bestand naam
        // W[5] = [maak]

        int N;
        TI = DataNumb(W[1]);
        string C = W[2];
        string K = W[3];
        string B = W[4];
        string F;
        string T;

        foreach (DataRow R in dataTable[TI].Rows)
        {
            if (B.Contains(".")) // als een bestand gezocht wordt
            {
                T = R[C].ToString();
                F = FileRoot() + FileFirst(T) + T + @"\" + B;
                if (File.Exists(F)) R[K] = "V";
                else if (W[5] != "")
                {
                    File.Create(F).Close();
                    R[K] += "V";
                }
            }
            else // als een map gezocht wordt
            {
                T = R[C].ToString();
                F = FileRoot() + FileFirst(T) + T + @"\" + B;

                if (Directory.Exists(F)) // als de map gevonden is
                {
                    N = Directory.GetFiles(F, "*" + B).Length;

                    if (W[5] == "+")
                    {
                        foreach (string y in Directory.GetDirectories(F))
                        {
                            N += Directory.GetFiles(y, "*" + B).Length;

                            foreach (string q in Directory.GetDirectories(y))
                            {
                                N += Directory.GetFiles(q, "*" + B).Length;
                            }
                        }
                    }

                    if (N > 0) R[K] = N;
                }
            }
        }
    }

    public void DataIf() // verwijder rijen uit een tabel die niet de gezochte inhoud hebben  
    {
        // W[0] = "if"
        // W[1] = zoek kolom
        // W[2] = zoek waarde

        DataTable T = dataTable[TI];
        string S = W[2];
        string Q;
        bool N = false; // de cel waarde niet gelijk moet zijn aan de zoek waarde
        bool E = false; // de cel waarde exact gelijk moet zijn aan de zoek waarde
        DataRow R;
        int I;

        if (S != "") // als de zoekwaarde niet leeg is
        {
            if (S.Substring(0, 1) == "!") // als de zoek waarde met een ! begint
            {
                S = S.Substring(1);
                N = true;
            }

            if (S.Substring(0, 1) == "=") // als de zoekwaarde met = begint
            {
                S = S.Substring(1);
                E = true;
            }
        }

        for (I = 0; I < T.Rows.Count; I++) // voor alle rijen
        {
            windowText.Text = I.ToString();
            textParser.ParserDoEvents();

            R = T.Rows[I];
            Q = R[W[1]].ToString();

            if (N)
            {
                if (E && Q == S) // verwijder cell
                {
                    DataDelete();
                }
                else if (S != "" && Q.Contains(S))
                {
                    DataDelete();
                }
            }
            else
            {
                if (E && Q != S) // verwijder cell
                {
                    DataDelete();
                }
                else if (S != "" && !Q.Contains(S))
                {
                    DataDelete();
                }
            }
        }

        void DataDelete()
        {
            R.Delete();
            I--;
            T.AcceptChanges();
        }
    }

    public void DataInfo() // gegevens uit een info bestand toevoegen
    {
        // W[0] = "info"
        // W[1] = tabel
        // W[2] = pad kolom
        // W[n] = kolom naam

        TI = DataNumb(W[1]);
        string C = W[2];
        string F;
        string[] S;
        int N;
        int M = 0;
        int K;
        string T;
        while (W[M] != "") M++;

        foreach (DataRow R in dataTable[TI].Rows)
        {
            T = R[C].ToString();
            F = FileRoot() + FileFirst(T) + T + @"\info.txt";
            if (File.Exists(F))
            {
                S = File.ReadAllLines(F);
                N = S.Length;
                for (int I = 0; I < N; I++)
                {
                    if (S[I] == "") break; // procedure afbreken als een regel leeg is

                    for (int J = 3; J < M; J++)
                    {
                        K = S[I].IndexOf(W[J] + ",");
                        try
                        {
                            if (K > -1) R[W[J]] = S[I].Substring(K + W[J].Length + 1).Trim();
                        }
                        catch { }
                    }
                }
            }
        }
    }

    public int DataNumb(string S) // tabel index
    {
        int I = -1;
        int J = 0;

        if (S == "") return 0;

        while (I < 0 && J <= TI)
        {
            if (dataTable[J].TableName == S) I = J;
            J++;
        }

        return I;
    }

    public void DataOpen() // open een database
    {
        // W[0] = "data"
        // W[1] = [0 = niet tonen] 

        TI = -1;

        tabsData.TableList.Items.Clear();

        if (W[1] != "0")
        {
            gridView.Children.Clear();
            gridView.Children.Add(viewData);
        }
    }

    public void DataRead() // lees data uit tekst bestand
    {
        // W[0] = "read"
        // W[1] = bestandnaam
        // W[2] = bron tabel
        // W[3] = bron kolom
        // W[4] = doel tabel

        string F = W[1]; // data bestand naam
        int BI;
        string BC;
        if (W[2] != "") BI = DataNumb(W[2]); else BI = 0;// bron tabel nummer
        if (W[3] != "") BC = W[3]; else BC = ""; // brontabel kolom met paden
        int DI;
        if (W[4] != "") DI = DataNumb(W[4]); else DI = TI; // als het doel tabel niet gegeven is wordt de laatste tebel gebruikt
        string B; // ???
        string[] S; // ???
        int N;
        DataRow DR;
        string T;
        bool skip;

        CI = dataTable[DI].Columns.Count - 1;

        T = FileType(F);

        if (T == ".csv") // lees een csv bestand in
        {
            S = File.ReadAllLines(F); // lees alle regels van het bestand in array S
            N = S.Length; // het aantal regels

            for (int I = 1; I < N; I++) // voor alle regels; sla regel 0 over
            {
                textParser.ParserLine(S[I], ','); // bepaal de woorden van de regel
                DR = dataTable[DI].NewRow(); // nieuwe rij voor de doel tabel
                DR[0] = I; // rij index in kolom 0

                for (int J = 0; J < CI; J++) // voor alle woorden van de regel
                {
                    if (dataTable[DI].Columns[J + 1].DataType == typeof(string)) DR[J + 1] = W[J]; else DR[J + 1] = V[J]; // voeg het woord of de waarde toe aan de rij
                }
                try
                {
                    dataTable[DI].Rows.Add(DR); // voeg de rij toe aan de tabel
                }
                catch { }
            }
        }
        else
        {
            if (W[2] != "") // als de bron tabel gegeven is
            {
                foreach (DataRow BR in dataTable[BI].Rows) // voor elke rij in de bron tabel
                {
                    B = BR[BC].ToString();
                    if (!(F == ""))
                    {
                        B = FileRoot() + "\\" + FileFirst(B) + B + "\\" + F + ".txt";
                    }
                    else
                    {
                        B = FileRoot() + "\\" + FileFirst(B) + B;
                    }

                    ReadTxt();

                    if (!parserRuns) return;
                }
            }
            else
            {
                B = GetFileAddress(F + ".txt");

                ReadTxt();
            }

        }

        void ReadTxt() // bestand lezen
        {
            if (File.Exists(B)) // als het bestand bestaat
            {
                windowText.Text = B;
                textParser.ParserDoEvents();
                S = File.ReadAllLines(B); // alle regels lezen
                N = S.Length; // aantal regels

                for (int I = 1; I < N; I++) //  voor alle regels behalve regel 0 die wordt overgeslagen omdat daar kolomnamen kunnen instaan
                {
                    if (S[I] != "") // als de regel niet leeg is
                    {
                        skip = false;
                        textParser.ParserLine(S[I], ','); // woorden van de regel bepalen
                        DR = dataTable[DI].NewRow(); // nieuwe rij voor de doel tabel
                        DR[0] = I; // rij index in kolom 0

                        for (int J = 1; J <= CI; J++) // voor alle kolummen
                        {
                            if (dataTable[DI].Columns[J].DataType == typeof(string))
                                DR[J] = W[J - 1]; // voeg het woord toe aan de rij
                            else
                                DR[J] = V[J - 1]; // voeg de waarde toe aan de rij

                            if (CV[J] != "")
                                if (CV[J] != W[J - 1]) skip = true;
                        }

                        if (dataTable[DI].Columns[CI].ColumnName == "file") DR[CI] = F;

                        if (!skip)
                        {
                            try
                            {
                                dataTable[DI].Rows.Add(DR); // probeer de rij toe aan de tabel toe te voegen
                            }
                            catch
                            {
                                /* geen idee wat dit doet als het toevoegen niet lukt
                                K = 0;
                                X = dataTable[DI].Rows[K][1].ToString();
                                Y = DR[1].ToString();
                                while (X != Y)
                                {
                                    K++;
                                    X = dataTable[DI].Rows[K][1].ToString();
                                }
                                Z = int.Parse(dataTable[DI].Rows[K][2].ToString());
                                Z += int.Parse(DR[2].ToString());
                                dataTable[DI].Rows[K][2] = Z;
                                */
                            }
                        }
                    }
                }
            }
        }
    }

    public void DataRows() // voeg een rij aan een tabel toe
    {
        // W[0] = "row"
        // W[n] = data

        int CI = dataTable[TI].Columns.Count;
        int RI = dataTable[TI].Rows.Count + 1;

        DataRow R = dataTable[TI].NewRow();
        R[0] = RI;

        for (int I = 1; I < CI; I++) if (dataTable[TI].Columns[I].DataType == typeof(string)) R[I] = W[I]; else R[I] = V[I];

        dataTable[TI].Rows.Add(R);
    }

    public void DataSave() // data in tekstbestand opslaan
    {
        // W[0] = "save"
        // W[1] = bestandnaam
        // W[2] = test kolom
        // W[3] = waarde test kolom
        // W[n] = [kolom]
        // W[n + 1] = "+" toevoegen

        string tc = W[2];
        string tw = W[3];
        int N = gridTable[TI].Count; // aantal rijen in huidige tabel
        DataRowView R; // rij
        string S = "\n";
        string T;
        int J;
        string c; // kolom
        string d; // data

        for (int I = 0; I < N; I++) // voor alle rijen van de tabel
        {
            R = gridTable[TI][I];

            T = "";

            J = 4; // W[4] bevat de eerste kolom naam

            windowText.Text = I.ToString(); // voortgang tonen
            textParser.ParserDoEvents();

            while (W[J] != "") // voor alle cellen in de rij
            {
                c = W[J];

                if (c.StartsWith("+")) // als iets moet worden toegevoegd
                {
                    T = T + @"\" + c.Substring(1);

                    if (T.Contains(",")) T = '<' + T + '>'; // als de waarde een komma bevat
                }
                else
                {
                    if (R[tc].ToString() == tw)
                    {
                        d = R[c].ToString();

                        if (d.Contains(",")) d = "<" + d + ">";

                        if (T == "") T = d; else T += ", " + d;
                    }
                }

                J++; // volgende cel
            }

            if (T != "") S += T + "\n"; // regel toevoegen
        }

        File.WriteAllText(GetFileAddress(W[1]), S); // bestand opslaan
    }

    public void DataSort() // sorteer een tabel
    {
        // W[0] = "sort"
        // W[n] = kolommen

        string S = "";
        int I = 1;

        while (W[I] != "")
        {
            if (S != "") S += ", ";
            S += W[I];
            I++;
        }

        gridTable[TI].Sort = S;
    }

    public void DataSum() // tel de waardes van de cellen in een kolom bij elkaar
    {
        // W[0] = "sum"
        // W[1]..W[n] = kolom

        int N = dataTable[TI].Rows.Count; // aantal rijen in de laatste tabel
        int S;
        string T;
        int J = 1;

        DataRow R = dataTable[TI].NewRow(); // niewue rij
        R[0] = N + 1; // index
        R[1] = "#total";

        while (W[J] != "") // zolang W[J] een kolom naam bevat
        {
            S = 0;
            for (int I = 0; I < N; I++) // voor alle rijen
            {
                T = dataTable[TI].Rows[I][W[J]].ToString(); // inhoud van een cell
                if (T != "") S += int.Parse(T); // als de inhoud niet leeg is sommeren
            }
            R[W[J]] = S; // zet het totaal in de nieuwe rij
            J++; // volgende kolom
        }
        dataTable[TI].Rows.Add(R);
    }

    public void DataTables() // maak een tabel
    {
        // W[0] = "table"
        // W[1] = tabelnaam

        if (W[1] != "") // als de tabel een naam heeft
        {
            TI += 1; // verhoog de tabel index

            dataTable[TI] = new DataTable(W[1]); // nieuwe tabel
            dataTable[TI].Columns.Add(new DataColumn("#", typeof(int))); // kolom 0 toevoegen voor index
            CW[TI, 0] = 50; // breedte van kolom 0

            gridTable[TI] = new DataView() // nieuwe tabel presentator
            {
                Table = dataTable[TI] // voeg de tabel toe aan een data view
            };

            tabsData.TableList.Items.Add(dataTable[TI].TableName); // voeg de tabel toe aan de lijst met tabellen in de data tab
            tabsData.TableList.SelectedIndex = tabsData.TableList.Items.Count - 1; // selecteer de toegevoegde tabel in de tabellen lijst
        }
        else
        {
            MessageBox.Show("tabel heeft geen naam");
            parserRuns = true;
        }
    }

    public void DataView(int I) // toon data tabel
    {
        if (I == -1) return;

        gridView.Children.Clear();
        gridView.Children.Add(viewData);

        ItemsSource = gridTable[I]; // koppel de laatste tabel aan het datagrid

        if (dataTable[TI] != null)
        {
            int N = dataTable[I].Columns.Count;

            for (int J = 0; J < N; J++)
            {
                if (dataTable[I].Columns[J].DataType != typeof(string)) // kolommen met getallen rechts uitlijnen
                {
                    Style S = new Style(typeof(DataGridCell));

                    S.Setters.Add(new Setter(DataGridCell.HorizontalAlignmentProperty, HorizontalAlignment.Right));

                    try
                    {
                        this.Columns[J].CellStyle = S;
                    }
                    catch { }
                }

                try
                {
                    this.Columns[J].Width = CW[I, J];
                }
                catch { }
            }

            DataShow();
        }
    }

    public void DataShow()
    {
        int i = tabsData.TableList.SelectedIndex;

        windowText.Text = "Rows: " + dataTable[i].Rows.Count.ToString() + ", " + SelectedItems.Count.ToString() + ", " + SelectedIndex;
    }

    public void This_PreviewMouseRightButtonUp(object sender, MouseEventArgs e) // open de map van de geselecteerde data rij
    {
        DataRowView R = (DataRowView)SelectedItem;

        string P = "";
        string f;

        if (R != null)
        {
            string T = R[1].ToString();

            if (!T.Contains(":")) f = FileRoot() + FileFirst(T) + T + P; else f = T;

            if (!Directory.Exists(f)) f = f.Substring(0, f.Length - 5);

            FileOpen(f + @"\index.txt");

            e.Handled = true;
        }
    }

    public void This_PreviewMouseLeftButtonUp(object sender, MouseEventArgs e) // selecteer een rij als er rijen geselecteerd zijn, wordt het aantal geselecteerde rijen getoond
    {
        DataShow();
    }

    public static ViewData viewData = new ViewData();
}