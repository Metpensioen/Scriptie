using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using static TabsFile;
using static TextParser;
using static TextVoice;
using static ViewPlayer;

class TabsPlay : TabItem
{
    public DispatcherTimer PlayTime = new DispatcherTimer();
    public TextBox PlayText = new TextBox();
    public Label PositionLabel = new Label();
    public Slider PlayPosition = new Slider();
    public Label VolumeLabel = new Label();
    public Slider PlayVolume = new Slider();
    public Button CopyButton = new Button();
    public Button NameButton = new Button();
    public Button PlayButton = new Button();
    public Button SaveButton = new Button();
    public TextBox PlayMusician = new TextBox();
    public TextBox PlayYear = new TextBox();
    public TextBox PlayAlbum = new TextBox();
    public TextBox PlayNumber = new TextBox();
    public TextBox PlayTrackn = new TextBox();
    public TextBox PlayTrack = new TextBox();
    public TextBox PlayCoworker = new TextBox();
    //public ListBox TagsList = new ListBox();

    public TabItem PlayInit()
    {
        StackPanel PlayStack = new StackPanel();

        PlayText.Background = Brushes.Transparent;
        PlayText.Foreground = Brushes.White;
        PlayText.BorderThickness = new Thickness(0);
        PlayText.Height = 30;
        PlayText.FontFamily = new FontFamily("Consolas");
        PlayText.FontSize = 16;

        PlayStack.Children.Add(PlayText);

        PositionLabel.Background = Brushes.DarkSlateGray;
        PositionLabel.Foreground = Brushes.White;
        PositionLabel.Height = 30;
        PlayStack.Children.Add(PositionLabel);

        PlayPosition.Height = 30;
        PlayStack.Children.Add(PlayPosition);

        VolumeLabel.Background = Brushes.DarkSlateGray;
        VolumeLabel.Foreground = Brushes.White;
        VolumeLabel.Height = 30;
        VolumeLabel.Content = "Volume  50";
        PlayStack.Children.Add(VolumeLabel);

        PlayVolume.Value = 50;
        PlayVolume.Maximum = 100;
        PlayVolume.Height = 30;
        PlayStack.Children.Add(PlayVolume);

        PlayButton.Content = "Pause";
        PlayStack.Children.Add(PlayButton);

        PlayStack.Children.Add(PlayMusician);
        PlayStack.Children.Add(PlayYear);
        PlayStack.Children.Add(PlayAlbum);
        PlayStack.Children.Add(PlayNumber);
        PlayStack.Children.Add(PlayTrack);
        PlayStack.Children.Add(PlayCoworker);

        CopyButton.Content = "Copy";
        PlayStack.Children.Add(CopyButton);

        NameButton.Content = "Name";
        PlayStack.Children.Add(NameButton);

        SaveButton.Content = "Save";
        PlayStack.Children.Add(SaveButton);

        //TagsList.MaxHeight = 600;

        //PlayStack.Children.Add(TagsList);

        Header = "Play";
        Background = Brushes.Black;
        Foreground = Brushes.White;
        
        Content = PlayStack;

        PlayPosition.PreviewMouseDown += PlayPosition_PreviewMouseDown;
        PlayPosition.PreviewMouseUp += PlayPosition_PreviewMouseUp;
        PlayPosition.ValueChanged += PlayPosition_ValueChanged;
        PlayVolume.ValueChanged += PlayVolume_ValueChanged;

        PlayButton.Click += PlayButton_Click;
        CopyButton.Click += CopyButton_Click;
        NameButton.Click += NameButton_Click;
        SaveButton.Click += SaveButton_Click;

        //TagsList.PreviewMouseUp += TagsList_PreviewMouseUp;

        return this;
    }

    public void MP3TagsCopy(string s) // nummer en naam van mp3 bestand naar tags copieren
    {
        s = FileName(s);

        PlayNumber.Text = "Number  : " + s.Substring(0, 3);
        PlayTrack.Text = "Track  : " + s.Substring(6);
    }

    public void MP3TagsName(string s) // nummer en naam van mp3 bestand van tags hernoemen
    {
        string p = GetFilePath(s);

        string n = PlayNumber.Text.Substring(PlayNumber.Text.IndexOf(":") + 1).Trim();
        string t = PlayTrack.Text.Substring(PlayTrack.Text.IndexOf(":") + 1).Trim();

        p = FileRoot() + p + "\\" + n + " - " + t + ".mp3";

        File.Move(s, p);
    }

    public void MP3TagsGet(String S)
    {
        //        TagLib.File F;

        TagLib.File F = TagLib.File.Create(S);

        string a = FilePrev(S); // album
        string m = FilePrev(a); // muzikant
        string r = FilePrev(m); // bron
        int l = r.Length + 1;
        string y = a.Substring(m.Length + 1).Substring(2, 4);

        PlayMusician.Text = "Musician: " + F.Tag.FirstAlbumArtist;
        
        if (m.Substring(l) != F.Tag.FirstAlbumArtist) PlayMusician.Background = Brushes.Yellow; else PlayMusician.Background = Brushes.White;

        PlayYear.Text = "Year    : " + F.Tag.Year;

        if (y != F.Tag.Year.ToString()) PlayYear.Background = Brushes.Yellow; else PlayYear.Background = Brushes.White;

        PlayAlbum.Text = "Album   : " + F.Tag.Album;

        if (a.Substring(m.Length + 1).Substring(12) != F.Tag.Album) PlayAlbum.Background = Brushes.Yellow; else PlayAlbum.Background = Brushes.White;

        PlayNumber.Text = "Number  : " + F.Tag.Track;

        if (S.Substring(a.Length + 1).Substring(0, 3) != F.Tag.Track.ToString()) PlayNumber.Background = Brushes.Yellow; else PlayNumber.Background = Brushes.White;

        PlayTrack.Text = "Track   : " + F.Tag.Title;

        if (S.Substring(a.Length + 1).Substring(6) != F.Tag.Title + ".mp3") PlayTrack.Background = Brushes.Yellow; else PlayTrack.Background = Brushes.White;

        PlayCoworker.Text = "Coworker: " + F.Tag.JoinedPerformers;
    }

    public void MP3TagsSet(string s)
    {
        string t;

        viewPlayer.Stop();
        viewPlayer.Source = null;
        textParser.ParserDoEvents();

        using (TagLib.File f = TagLib.File.Create(s))
        {
            t = PlayMusician.Text.Substring(PlayMusician.Text.IndexOf(":") + 1).Trim();
            f.Tag.AlbumArtists = new[] { t };

            t = PlayYear.Text.Substring(PlayYear.Text.IndexOf(":") + 1).Trim();
            f.Tag.Year = Convert.ToUInt16(t);

            t = PlayAlbum.Text.Substring(PlayAlbum.Text.IndexOf(":") + 1).Trim();
            f.Tag.Album = t;

            t = PlayNumber.Text.Substring(PlayNumber.Text.IndexOf(":") + 1).Trim();
            f.Tag.Track = Convert.ToUInt16(t);

            t = PlayTrack.Text.Substring(PlayTrack.Text.IndexOf(":") + 1).Trim();
            f.Tag.Title = t;

            t = PlayCoworker.Text.Substring(PlayCoworker.Text.IndexOf(":") + 1).Trim();
            f.Tag.Performers = new[] { t };

            f.Save();
        }
    }

    public void PlayPosition_PreviewMouseDown(object sender, EventArgs e)
    {
        tabsPlay.PlayTime.Stop();
    }

    public void PlayPosition_PreviewMouseUp(object sender, EventArgs e)
    {
        viewPlayer.Position = TimeSpan.FromSeconds(PlayPosition.Value);
        tabsPlay.PlayTime.Start();
    }

    public void PlayPosition_ValueChanged(Object sender, EventArgs e)
    {
        TimeSpan P = TimeSpan.FromSeconds(PlayPosition.Value);
        PositionLabel.Content = "Position " +
            P.ToString().Substring(0, 8) + " " +
            viewPlayer.NaturalDuration.ToString().Substring(0, 8);
        viewPlayer.PlayerInfo();

    }

    public void PlayVolume_ValueChanged(Object sender, EventArgs e)
    {
        viewPlayer.Volume = PlayVolume.Value / 100; // View volume 0..1
        textSynths.Volume = (int)PlayVolume.Value; // stem volume 0..100

        string S = PlayVolume.Value.ToString();
        int I = S.IndexOf(".");
        if (I > 0) S = S.Substring(0, I);
        VolumeLabel.Content = "Volume " + S;
    }

    public void PlayButton_Click(object sender, EventArgs e)
    {
        viewPlayer.PlayerStart();
    }

    public void CopyButton_Click(object sender, EventArgs e)
    {
        MP3TagsCopy(viewPlayer.PlayerFile);
    }

    public void NameButton_Click(object sender, EventArgs e)
    {
        MP3TagsName(viewPlayer.PlayerFile);
    }

    public void SaveButton_Click(object sender, EventArgs e)
    {
        MP3TagsSet(viewPlayer.PlayerFile);
    }

    /*
    public void TagsList_PreviewMouseUp(object sender, EventArgs e)
    {
        if (TagsList.SelectedIndex > -1)
        {
            string T = TagsList.SelectedItem.ToString();
            string S = GetFileRoot() + GetFirstChar(T) + T + @"\";
            Directory.CreateDirectory(S);
            using (StreamWriter SW = File.AppendText(S + "list.txt"))
            {
                SW.WriteLine(showPict.pictFile.Substring(GetFileRoot().Length + 2));
            }
            if (tabsPlay.PlayMusician.Text != "") T = ';' + T;
            tabsPlay.PlayMusician.Text += T;
        }
        editText.Focus();
    }
    */

    public static TabsPlay tabsPlay = new TabsPlay();
}