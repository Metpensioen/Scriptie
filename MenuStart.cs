using Microsoft.VisualBasic;

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Linq;

using static EditText;
using static GridMenu;
using static RoomMods;
using static ModsRigs;
using static PartLego;
using static TabsFile;
using static TextParser;
using static TextVoice;
using static ViewHTML;
using static ViewPlayer;
using static ViewRoom;

class MenuStart : MenuItem
{
    public MenuItem StartInit()
    {
        Header = "_Start";

        Items.Add(StartPars.Init()); // F5
        Items.Add(StartTill.Init()); // F6
        Items.Add(StartLine.Init()); // F7
        Items.Add(StartTell.Init()); // F8
        Items.Add(StartTell2.Init()); // F8
        Items.Add(StartStop.Init()); // F9
        Items.Add(StartStart.Init()); // F10
        Items.Add(StartStep.Init()); // F11

        return this;
    }

    public class ClassStartPars : MenuItem // als op F5 wordt gedrukt, start het uitvoeren van een script
    {
        public MenuItem Init()
        {

            Header = "_Pars";

            InputGestureText = "F5";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F5, ModifierKeys.None, This_Click);

            return this;
        }

        public static void This_Click(object sender, RoutedEventArgs e)
        {
            editText.TextSave();
            string F = textFile;
            string T = FileType(F);

            switch (T)
            {
                case ".html":
                case ".xml": viewHTML.HTMLOpen(textFile); break;
                case ".obj":
                    viewRoom.RoomOpen();
                    RoomMod.Clear();
                    modsRigs.RigsOpen(F); break;
                case ".py": Interaction.Shell(@"cmd.exe /k " + "python " + textFile, AppWinStyle.NormalFocus); break;
                default:
                    textParser.ParserInit();
                    textParser.ParserOpen(textFile, 0, -1);
                    if (partLego.LegoData) partLego.LegoList(); break;
            }
        }
    }

    public static ClassStartPars StartPars = new ClassStartPars();

    public class ClassStartTill : MenuItem // als op F6 wordt gedrukt, Start het uitvoeren van een script tot de geselecteerde regel
    {
        public MenuItem Init()
        {
            Header = "_Pars";
            InputGestureText = "F6";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F6, ModifierKeys.None, This_Click);

            return this;
        }

        public static void This_Click(object sender, RoutedEventArgs e)
        {
            editText.TextSave();
            textParser.ParserInit();
            textParser.ParserOpen(textFile, 0, editText.GetLineIndexFromCharacterIndex(editText.CaretIndex) + 1);
        }
    }

    public static ClassStartTill StartTill = new ClassStartTill();

    public class ClassStartLine : MenuItem // als op F7 wordt gedrukt, start het uitvoeren de geselecteerde regel van een script
    {
        public MenuItem Init()
        {
            Header = "_Pars";
            InputGestureText = "F7";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F7, ModifierKeys.None, This_Click);

            return this;
        }

        public static void This_Click(object sender, RoutedEventArgs e)
        {
            editText.TextSave();
            textParser.ParserInit();
            int I = editText.GetLineIndexFromCharacterIndex(editText.CaretIndex);
            textParser.ParserOpen(textFile, I, I + 1);
        }
    }

    public static ClassStartLine StartLine = new ClassStartLine();

    public class ClassMenuStartTell : MenuItem // als op F8 is gedrukt wordt een tekst voorgelezen
    {
        public MenuItem Init()
        {
            Header = "_Tell";
            InputGestureText = "F8";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F8, ModifierKeys.None, This_Click);

            return this;
        }

        public static void This_Click(object sender, RoutedEventArgs e)
        {
            textVoice.SynthTell(false);
        }
    }

    public static ClassMenuStartTell StartTell = new ClassMenuStartTell();

    public class ClassMenuStartTell2 : MenuItem // als op F8 is gedrukt wordt een tekst voorgelezen
    {
        public MenuItem Init()
        {
            Header = "_Tell";
            InputGestureText = "Alt+F8";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F8, ModifierKeys.Alt, This_Click);

            return this;
        }

        public static void This_Click(object sender, RoutedEventArgs e)
        {
            textVoice.SynthTell(true);
        }
    }

    public static ClassMenuStartTell2 StartTell2 = new ClassMenuStartTell2();

    public class ClassMenuStartStop : MenuItem // als op F9 is gedrukt wordt het uitvoeren of voorlezen gestopt
    {
        public MenuItem Init()
        {
            Header = "_Stop";
            InputGestureText = "F9";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F9, ModifierKeys.None, This_Click);

            return this;
        }

        public static void This_Click(object sender, RoutedEventArgs e) // parser en player stoppen
        {
            parserRuns = false;
            viewPlayer.PlayerStop = true;
            viewPlayer.Stop();
            //gridEdit.Children.Clear();
            //gridEdit.Children.Add(editText);
        }
    }

    public static ClassMenuStartStop StartStop = new ClassMenuStartStop();

    public class ClassMenuStartStart : MenuItem
    {
        public MenuItem Init()
        {
            Header = "_Start";
            InputGestureText = "F10";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F10, ModifierKeys.None, This_Click);

            return this;
        }

        public static void This_Click(object sender, RoutedEventArgs e) // Start Pars
        {
            viewPlayer.PlayerStart();
        }
    }

    public static ClassMenuStartStart StartStart = new ClassMenuStartStart();

    public class ClassMenuStartStep : MenuItem
    {
        public MenuItem Init()
        {
            Header = "_Step";
            InputGestureText = "F11";
            Click += new RoutedEventHandler(This_Click);
            gridMenu.MenuShort(Key.F11, ModifierKeys.None, This_Click);

            return this;
        }

        public static void This_Click(object sender, RoutedEventArgs e) // Start Pars
        {
            int N = editText.Text.Count();

            if (editText.CaretIndex < N)
            {
                int I = editText.GetLineIndexFromCharacterIndex(editText.CaretIndex) + 1;
                textParser.ParserOpen(textFile, I, I + 1);
                editText.SelectionStart = editText.GetCharacterIndexFromLineIndex(I);
            }
        }
    }

    public static ClassMenuStartStep StartStep = new ClassMenuStartStep();

    public static MenuStart menuStart = new MenuStart();
}