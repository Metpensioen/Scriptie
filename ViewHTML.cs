using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

using System;
using System.Drawing;
using System.Windows.Input;
using System.IO;
using System.Net;
using System.Windows;

using static EditText;
using static GridMenu;
using static GridView;
using static ViewImage;
using static TabsFile;
using static TextParser;

class ViewHTML
{
    public static WebView2 HTMLView = new WebView2();

    public static string HTMLFile;

    public UIElement HTMLInit()
    {
        HTMLView.DefaultBackgroundColor = Color.White;
        HTMLView.ZoomFactor = 1;

        HTMLView.SourceChanged += This_SourceChanged;

        return HTMLView;
    }

    public void HTMLOpen(string S)
    {
        viewImage.imageFile = "";

        HTMLView.Source = new System.Uri(FileRoot() + "leeg.html"); // anders wordt hetzelfde bestand niet opnieuw geladen
        HTMLView.Source = new System.Uri(S);

        gridView.Children.Clear();
        gridView.Children.Add(HTMLView);
    }

    public void HTMLDone()
    {
        HTMLView.Dispose();
    }

    public void HTMLDownload()
    {
        // W[0] = "download"
        // W[1] = bestand
        // V[2] = aantal

        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

        string s = W[1];
        int n = (int)V[2];
        string p = GetFilePath(textFile);
        int j = p.LastIndexOf("\\");
        if (j > 3) p = p.Substring(0, j);
        string t;
        string f;
        DateTime d; // = new DateTime();

        for (int i = 1; i <= n; i++)
        {
            t = ".jpg";

            f = FileRoot() + p + "\\new";
            Directory.CreateDirectory(f);
            d = DateTime.Now;

            HTMLFile = tabsFile.FileDate(d);
            f += "\\" + HTMLFile + t;

            WebClient client = new WebClient();
            client.DownloadFile(new Uri(s + i + t), f);

            textParser.ParserWait(10);
        }

        Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
    }

    public void HTMLSave() // alle foto's opslaan wordt gestart met F3
    {
        string bron = HTMLView.Source.ToString(); // geselecteerde afbeelding 
        string type = FileType(bron); // afbeelding type

        if (type != ".jpg")
        {
            //client.DownloadFile(new Uri(bron), "d:\\backup\\test.html");
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

        string studio = scene.Substring(0, 3);

        scene = scene.Substring(3);
        
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

        //string studio = name.Substring(0, name.IndexOf("/"));
        
        name = name.Substring(0, name.LastIndexOf("/"));
        name = name.Substring(name.IndexOf("/") + 1);

        string pad = FilePath(textFile);

        pad += "\\" + scene;

        if (!Directory.Exists(pad))
        {
            Directory.CreateDirectory(pad);
        }

        File.WriteAllText(pad + "\\' " + name + ".txt", "");

        DateTime datum; // = new DateTime();

        string file;
        string doel;
        string load;

        for (int i = nummer; i > 0; i--)
        {
            datum = DateTime.Now;

            file = tabsFile.FileDate(datum);
            doel = pad + "\\" + file + type;
            load = bron + i + type;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", "Your User-Agent");
                webClient.DownloadFile(new Uri(load), doel);
                textParser.ParserWait(10);
                viewImage.ImageSize(doel);
                //textParser.ParserWait(10);
                //viewImage.ImageTagsSet(doel, studio + scene + ", " + name); // tags in foto schrijven
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

    public void This_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e) // als de bron veranderd
    {
        windowText.Text = HTMLView.Source.ToString(); // bron tonen
        textParser.ParserWait(10);
    }

    public static ViewHTML viewHTML = new ViewHTML();
}