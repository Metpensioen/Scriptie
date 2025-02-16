using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using static EditText;
using static GridMenu;
using static GridView;
using static TabsFile;
using static TabsPlay;
using static TextParser;
using static ViewImage;

class ViewPlayer : MediaElement
{
    public struct TSubs
    {
        public int I; // index
        public TimeSpan B; // begin
        public TimeSpan E; // eind
        public string T; // tekst
    }

    public bool PlayerRuns;
    public int PlayerState;
    public long PlayerSize;
    public string PlayerFile;
    public double PlayerFrom = 0;
    public double PlayerTill;
    public bool PlayerStop = false;

    public bool SubsPlay = false; // ???
    public string[] SubsText;
    public TextBox SubsBox = new TextBox();
    public TSubs SubsStep;

    public UIElement PlayerInit() // Start viewPlayer
    {
        LoadedBehavior = MediaState.Manual;
        Volume = 50;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Top;
        Stretch = Stretch.Uniform;
        Name = "Player";
        PlayerState = 0;

        SubsBox.Background = Brushes.Transparent;
        SubsBox.Foreground = Brushes.White;
        SubsBox.FontSize = 20;
        SubsBox.BorderBrush = Brushes.Transparent;

        tabsPlay.PlayTime.Interval = TimeSpan.FromSeconds(1);

        tabsPlay.PlayTime.Tick += PlayTime_Tick;

        MediaOpened += This_ViewOpened;
        MediaEnded += This_ViewEnded;

        return this;
    }

    public void PlayerDone()
    {
        viewPlayer.Stop();
    }

    public void PlayerCover(string single) // hoes van de single tonen
    {
        string album = single.Substring(0, single.LastIndexOf("\\")); // singlenaam van het album verwijder 

        string name = "*" + FileName(single).Substring(6) + ".jpg"; // "101 - " van de singlenaam verwijderen

        string cover;

        try
        {
            cover = Directory.GetFiles(album, name)[0]; // zoek de hoes 
        }
        catch
        {
            cover = album + "\\folder.jpg";
        }

        viewImage.ImageOpen(cover);
        gridView.Children.Add(viewPlayer);
    }

    public void PlayerOpen(string S)
    {
        if (File.Exists(S))
        {
            editText.TextSave();

            gridView.Children.Clear();
            gridView.Children.Add(viewPlayer);

            PlayerRuns = false;
            PlayerFrom = V[2];
            PlayerTill = V[3];
            PlayerFile = S;
            PlayerStop = false;
            tabsPlay.PlayText.Text = PlayerFile;

            try
            {
                using (FileStream F = new FileStream(S, FileMode.Open))
                {
                    PlayerSize = F.Length;
                }
            }
            catch { }

            Source = new Uri(S);
            Play();
            Position = TimeSpan.FromSeconds(PlayerFrom);
            PlayerState = 2;
            //TabsPlay.IsSelected = true;
            PlayerInfo();

            string T = FileType(PlayerFile);
            if (T == ".mp3")
            {
                PlayerCover(PlayerFile);
                tabsPlay.MP3TagsGet(PlayerFile);
            }
            if (T == ".mp4") PlayerSubs(PlayerFile);
        }
    }

    public void PlayerSetTags(string file) // tags naar video bestand schrijven
    {
        Stop(); // afspelen stoppen

        TagLib.File video = TagLib.File.Create(file); // video bestand openen
        video.Tag.Title = "CORRECT"; // titel invullen
        video.Tag.Comment = "CORRECT"; // commentaar invullen
        video.Save(); // bestand opslaan
    }

    public void PlayerInfo()
    {
        try
        {
            windowText.Text = PlayerFile + ", " + PlayerSize + ", " + Position.ToString().Substring(0, 8) + ", " + NaturalDuration.ToString().Substring(0, 8);
        }
        catch { }
    }

    public void PlayerStart()
    {
        if (tabsPlay.PlayButton.Content.ToString() == "Start")
        {
            tabsPlay.PlayButton.Content = "pause";
            Play();
        }
        else
        {
            PlayerSubs(PlayerFile);
            tabsPlay.PlayButton.Content = "Start";
            Pause();
        }
    }

    public void PlayerSubs(string S)
    {
        S = S.Substring(0, S.Length - 4) + ".srt";

        if (File.Exists(S))
        {
            SubsPlay = true;
            SubsText = File.ReadAllLines(S);

            if (!gridView.Children.Contains(SubsBox)) gridView.Children.Add(SubsBox);

            SubsStep.I = 1;
            PlayerTime();
        }
        else SubsPlay = false;
    }

    public void PlayerTime()
    {
        if (SubsPlay)
        {
            if (SubsStep.I > 0)
            {
                try
                {
                    string S = SubsText[SubsStep.I];

                    SubsStep.B = new TimeSpan(int.Parse(S.Substring(0, 2)), int.Parse(S.Substring(3, 2)), int.Parse(S.Substring(6, 2)));
                    SubsStep.E = new TimeSpan(int.Parse(S.Substring(17, 2)), int.Parse(S.Substring(20, 2)), int.Parse(S.Substring(23, 2)));
                    SubsStep.T = SubsText[SubsStep.I + 1];
                    SubsStep.I += 4;
                }
                catch { }
            }
        }
    }

    public void PlayTime_Tick(object sender, EventArgs e)
    {
        tabsPlay.PlayPosition.Value = viewPlayer.Position.TotalSeconds;

        if (PlayerTill > 0 && Position.TotalMilliseconds >= PlayerTill * 1000f) Stop();

        if (SubsPlay)
        {
            TimeSpan D = viewPlayer.Position;

            if (D > SubsStep.B)
            {
                SubsBox.Text = SubsStep.T;
                int L = (1000 - SubsStep.T.Length * 10) / 2;
                try
                {
                    SubsBox.BorderThickness = new Thickness(L, viewPlayer.ActualHeight - 40, 0, 0);
                }
                catch { }
            }
            if (D > SubsStep.E)
            {
                SubsBox.Text = "";
                PlayerTime();
            }
        }
    }

    public void This_ViewOpened(object sender, RoutedEventArgs e)
    {
        PlayerRuns = true;
        
        try
        {
            tabsPlay.PlayPosition.Maximum = NaturalDuration.TimeSpan.TotalSeconds;
            Position = TimeSpan.FromMilliseconds(PlayerFrom * 1000f);
            tabsPlay.PlayTime.Start();
        }
        catch { }
        
        if (FileType(PlayerFile) == ".mp3") tabsPlay.MP3TagsGet(PlayerFile);
    }

    public void This_ViewEnded(object sender, EventArgs e)
    {
        Position = TimeSpan.FromMilliseconds(1);
        Play();

        //editText.TextNext();
    }

    public static ViewPlayer viewPlayer = new ViewPlayer();
}