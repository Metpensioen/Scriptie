using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;

using static EditText;
using static TabsFile;
using static TextParser;
using static ViewHTML;
using static ViewImage;

class TextTrash
{
    string linkWord;

    public void TrashAlbum() // met alt+a album van foto's maken
    {
        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

        editText.TextSave(); // huidige bestand opslaan
        editText.Clear();

        string text = "";
        string path = FilePath(textFile);
        string type = "*." + path.Substring(path.LastIndexOf("\\") + 1);
        int i = FileRoot().Length + 3;
        int n = 0;
        int album = 1;

        foreach (string foto in Directory.GetFiles(path, type))
        {
            MessageBox.Show(path + " bevat foto");

            return;
        }

        foreach (string studio in Directory.GetDirectories(path))
        {
            foreach (string foto in Directory.GetFiles(studio, type))
            {
                MessageBox.Show(studio + " bevat foto");
                
                return;
            }

            foreach (string scene in Directory.GetDirectories(studio))
            {
                foreach (string foto in Directory.GetFiles(scene, type))
                {
                    n++;

                    if (n % 8000 == 0)
                    {
                        textFile = path + "\\album" + album + ".txt";

                        File.WriteAllText(textFile, text);

                        text = "";

                        album++;

                    }

                    text += foto.Substring(i) + "\n";

                }

                foreach (string trash in Directory.GetDirectories(scene))
                {
                    MessageBox.Show(scene + " bevat foto");
                    
                    return;
                }
            }

        }

        textFile = path + "\\album" + album + ".txt";

        File.WriteAllText(textFile, text);

        editText.Text = text;

        System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
    }

    public void TrashBook() // boek maken met alt+b
    {
        string path = Path.GetDirectoryName(textFile); // pad waarin de foto's staan waarvan een boek gemaakt moet worden
        string file;
        string text = "";
        int l = FileRoot().Length + 3;
        int i = path.LastIndexOf("\\");
        string scene = path.Substring(i + 1);

        textFile = path.Substring(0, i);
        i = textFile.LastIndexOf("\\");
        scene = textFile.Substring(i + 1) + scene;
        textFile = textFile.Substring(0, i);
        i = textFile.LastIndexOf("\\");
        textFile = textFile.Substring(0, i) + "\\txt\\scene\\";
        Directory.CreateDirectory(textFile);
        textFile += scene + ".txt";

        if (File.Exists(textFile))
        {
            editText.TextOpen(textFile);

            text = editText.Text;
        }
        else
        {
            editText.Clear();
        }

        foreach (string foto in Directory.GetFiles(path, "*.jpg"))
        {
            file = foto.Substring(l);

            if (!text.Contains(file)) editText.AppendText(file + ", 0\n");
        }

        editText.TextSave();
    }

    public void TrashLink() // voeg foto toe aan link bestand met alt+t
    {
        string line = editText.GetLineText(editText.GetLineIndexFromCharacterIndex(editText.CaretIndex));

        textParser.ParserLine(line, ',');

        if (W[1] != "") linkWord = W[1];

        string path = FilePath(textFile);
        string file;
        int l = FileRoot().Length + 3;

        line = path.Substring(l) + "\\" + W[0];

        if (path.EndsWith("gif"))
        {
            file = FilePrev(path) + "\\txt\\link\\" + linkWord + ".txt";
        }
        else
        {
            file = FilePrev(FilePrev(FilePrev(path))) + "\\txt\\link\\" + linkWord + ".txt";
        }

        string text;

        if (File.Exists(file))
        {
            text = File.ReadAllText(file);

            if (!text.Contains(line))
            {
                text += "\n" + line;
                File.WriteAllText(file, text);
            }
        }
        else
        {
            Directory.CreateDirectory(FilePath(file));
            File.WriteAllText(file, line);
        }

        editText.TextNext(); // ga naar volgende regel
    }

    public void TrashLink2() // link bestand openen met alt+shift+t
    {
        string file = editText.GetLineText(editText.GetLineIndexFromCharacterIndex(editText.CaretIndex));

        textParser.ParserLine(file, ',');

        file = W[1];

        string path = FilePath(textFile).Substring(FileRoot().Length + 3);

        path = path.Substring(0, path.IndexOf("\\"));

        file = FileRoot() + FileFirst(path) + path + "\\txt\\link\\" + file + ".txt";

        editText.TextOpen(file);
    }

    public void TrashScene() // scenelijst maken met alt+o
    {
        string path = FilePath(textFile);

        if (path.EndsWith("jpg"))
        {
            editText.TextSave();

            string text = "";
            string file;
            int n;
            int l = FileRoot().Length + 3;

            textFile = path + "\\scene.txt";
            editText.Clear();

            foreach (string studio in Directory.GetDirectories(path))
            {
                foreach (string scene in Directory.GetDirectories(studio))
                {
                    n = Directory.GetFiles(scene, "*.jpg").Length; // aantal foto's in de scene

                    if (n > 0)
                    {
                        file = Directory.GetFiles(scene, "*.jpg")[0]; // eerste foto in de scene
                        text += file.Substring(l) + ", " + n + "\n";
                    }
                }
            }

            editText.Text = text;
        }
        else
        {
            MessageBox.Show("jpg map is niet geselecteerd");
        }
    }

    public void TrashScene2() // alt+p
    {
        string path = FilePath(textFile);
        string file;

        foreach (string scene in Directory.GetDirectories(path))
        {
            try
            {
                file = Directory.GetFiles(scene, "*.txt")[0].Substring(scene.Length);
            }
            catch { file = ""; }

            editText.AppendText(scene.Substring(path.Length + 1) + file + "\n");
        }
    }

    public void TrashSave() // alle foto's opslaan met F3
    {
        string bron = HTMLView.Source.ToString(); // geselecteerde afbeelding 
        string type = FileType(bron); // afbeelding type

        if (type != ".jpg")
        {
            return; // geen .jpg dan stoppen
        }

        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // wacht cursor tonen

        int j = bron.LastIndexOf("-") + 1; // waar het nummer van de afbeelding begint
        int l = bron.LastIndexOf("."); // waar .jpg begint
        int nummer;

        if (l - j == 2) nummer = Convert.ToInt32(bron.Substring(j, 2)); else nummer = Convert.ToInt32(bron.Substring(j, 1)); // afbeelding nummer bepalen

        bron = bron.Substring(0, j); // zonder nummer en zonder type

        string scene = editText.GetLineText(editText.LineCount - 1); // scene nummer uit bronbestand lezen

        int scenes;

        try
        {
            scenes = Convert.ToInt32((string)scene);
        }
        catch
        {
            MessageBox.Show("scene \"" + scene + "\" is niet geldig");
            return;
        }

        //string studio = scene.Substring(0, 3);

        //scene = scene.Substring(3);

        string hulp = bron.Replace("x.uuu.cam/pics", "sexhd.pics/gallery");

        hulp = hulp.Substring(0, hulp.LastIndexOf("/"));

        int z = editText.Text.IndexOf(hulp);

        if (z > 0)
        {
            editText.ScrollToLine(editText.GetLineIndexFromCharacterIndex(z));
            HTMLView.GoBack();
            HTMLView.GoBack();
            editText.SelectionStart = z;
            editText.SelectionLength = 10;

            return;
        }

        string name = hulp.Substring(27);

        name = name.Substring(0, name.LastIndexOf("/"));
        name = name.Replace("/", "_");

        string path = FilePath(textFile);

        int k = path.Length;

        if (!path.Contains("jpg"))
        {
            path += "\\000\\" + path.Substring(k - 7, 3) + path.Substring(k - 3, 3) + scene;
        }
        else
        {
            path += "\\000\\" + scene;
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        if (name.Length > 80) name = name.Substring(0, 80);

        File.WriteAllText(path + "\\' " + name + ".txt", "");

        DateTime datum; // = new DateTime();

        string file;
        string doel;
        string load;

        for (int i = nummer; i > 0; i--)
        {
            datum = DateTime.Now;

            file = tabsFile.FileDate(datum);
            doel = path + "\\" + file + type;
            load = bron + i + type;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", "Your User-Agent");
                webClient.DownloadFile(new Uri(load), doel);
                textParser.ParserWait(10);
                viewImage.ImageSize(doel);
            }
        }

        editText.AppendText(", " + hulp + "\n");
        scenes++;
        editText.AppendText(scenes.ToString("000000"));
        editText.ScrollToEnd();

        editText.TextSave();

        HTMLView.GoBack();
        HTMLView.GoBack();
    }


    public static TextTrash textTrash = new TextTrash();
}