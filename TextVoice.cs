using System;
using System.Diagnostics.Eventing.Reader;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using static EditText;
using static TabsFile;
using static TextParser;
using static ViewPlayer;
using static ViewText;

class TextVoice
{
    public static SpeechSynthesizer textSynths = new SpeechSynthesizer();

    public void SynthInit()
    {
        SynthVoice();
    }

    public void SynthDone()
    {
        textSynths.Dispose();
    }

    public void SynthSay()
    {
        // W[0] = "say"
        // W[1] = tekst of bestand

        int i = 2;

        while (W[i] != "")
        {
            W[1] += ", " + W[i];
            i++;
        }

        textSynths.Speak(W[1]);
        textParser.ParserDoEvents();
    }

    public void SynthTell(bool R) // tekst voorlezen
    {
        // W[0] = "tell"
        // W[1] = tekst of bestand

        editText.TextSave();

        viewPlayer.PlayerRuns = true;

        int J;
        int N;
        string S;
        int T = 0;

        N = editText.LineCount; // aantal regels

        parserRuns = true;

        if (R) // toon random afbeeldingen
        {
            while (parserRuns)
            {
                J = (new Random().Next(0, N));
                editText.SelectionStart = editText.GetCharacterIndexFromLineIndex(J); // verplaats cursor hierdoor worden eventuele .jpg of .gif bestanden getoond
                editText.CaretIndex = editText.SelectionStart;
                textParser.ParserWait(2000); // nodig om de cursor te verplaatsen en de afbeelding te tonen
            }
        }
        else
        {
            J = editText.GetLineIndexFromCharacterIndex(editText.CaretIndex); // geselecteerde regel nummer als start regel
            for (int I = J; I < N; I++) // voor alle regels
            {
                editText.SelectionStart = editText.GetCharacterIndexFromLineIndex(I); // verplaats cursor hierdoor worden eventuele .jpg of .gif bestanden getoond
                editText.CaretIndex = editText.SelectionStart;
                textParser.ParserWait(100); // nodig om de cursor te verplaatsen
                S = editText.GetLineText(I).ToLower().Trim(); // huidige regel

                if (S.Contains(".jpg")) // als in de regel geen afbeelding bestand staat
                {
                    T = 0;
                }
                else if (S.Contains(".gif"))
                {
                    T = -1000;
                    int X = DateTime.Now.Millisecond;
                    while (!viewPlayer.PlayerRuns)
                    {
                        textParser.ParserDoEvents();
                        if (DateTime.Now.Ticks > X + 2000)
                        {
                            break;
                        }
                    }
                }
                else if (S.StartsWith("type"))
                {
                    try
                    {
                        viewText.TextType(S.Substring(6));
                    }
                    catch { }
                }
                else if(!S.StartsWith("'"))
                {
                    T = DateTime.Now.Millisecond; // tijd dat voorlezen start
                    try
                    {
                        textSynths.Speak(S); // lees de tekst voor
                    }
                    catch { }

                    T = DateTime.Now.Millisecond - T; // tijd dat voorgelezen is
                }

                if (I + 1 < N)
                {
                    S = editText.GetLineText(I + 1).ToLower().Trim();

                    if (S.Contains(".jpg") || S.Contains(".gif") || S.StartsWith("type, ")) // als de volgende regel een afbeelding is
                    {
                        textParser.ParserWait(2000 - T);
                    }
                }

                textParser.ParserDoEvents();

                if (!parserRuns) return; // stop het voorlezen als op F9 gedrukt is 
            }
        }
    }

    public void SynthVoice() // stelt de voorlees stem in
    {
        // W[0] = "voice"
        // W[1] = [stem]
        // W[2] = [volume 0..100]
        // W[3] = [tempo -10..10]

        if (W[1] == null)
        {
            W[1] = "";
            W[2] = "";
            W[3] = "";
        }

        if (W[1] == "kies")
        {
            ListBox L = new ListBox();
            Window F = new Window();
            string S = "";

            foreach (var Voice in textSynths.GetInstalledVoices())
            {
                L.Items.Add(Voice.VoiceInfo.Name);
            }
            F.Content = L;
            F.Show();

            while (S == "")
            {
                textParser.ParserDoEvents();
                if (L.SelectedIndex > -1) S = L.SelectedItem.ToString();
            }
            F.Close();
            textSynths.SelectVoice(S);
        }

        switch (W[1].ToLower())
        {
            case "david": textSynths.SelectVoice("Microsoft David Desktop"); break;
            case "zira": textSynths.SelectVoice("Microsoft Zira Desktop"); break;
            default: textSynths.SelectVoice("Microsoft Server Speech Text to Speech Voice (nl-NL, Hanna)"); break;
        }
        if (W[2] == "") V[2] = 50;
        textSynths.Volume = (int)V[2];
        if (W[3] == "") V[3] = 0;
        textSynths.Rate = (int)V[3];
    }

    public void TextHear()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(); // new System.Globalization.CultureInfo("en-US")
        Grammar grammer = new DictationGrammar();
        recognizer.LoadGrammar(grammer);

        if (W[1] != "")
        {
            recognizer.SetInputToWaveFile(GetFileAddress(W[1]));
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }
        else
        {
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(TextHearing);
        recognizer.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(TextComplete);

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void TextHearing(object sender, SpeechRecognizedEventArgs e)
    {
        editText.Text += e.Result.Text + "\n";
    }

    public void TextComplete(object sender, RecognizeCompletedEventArgs e)
    {
        editText.Text += "@klaar" + "\n";
    }

    public static TextVoice textVoice = new TextVoice();
}