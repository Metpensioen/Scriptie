using SharpDX;

using System;
using System.Diagnostics; // voor het starten van programma's
using System.IO;
using System.Windows;
using System.Windows.Input;

using static CtrlCPU;
using static EditText;
using static WindowGrid;
using static GridView;
using static RoomCamera;
using static RoomLamp;
using static RoomMods;
using static ModsPrts;
using static ModsRigs;
using static PartMats;
using static PartLego;
using static PartBins;
using static ViewData;
using static ViewDrawing;
using static ShowGame;
using static ViewHTML;
using static ViewImage;
using static ViewPlayer;
using static ViewRoom;
using static ViewText;
using static TabsCalc;
using static TabsCtrl;
using static TabsFile;
using static TabsMidi;
using static TabsParts;
using static TextFunctions;
using static TextVoice;

class TextParser
{
    public static string[] W = new string[24]; // woord
    public static double[] V = new double[24]; // waarde
    public static int FI; // File Index
    public static double[] TX = new double[24]; // X transformatie
    public static double[] TY = new double[24]; // Y
    public static double[] TZ = new double[24]; // Z
    public static double[] RX = new double[24]; // X rotatie
    public static double[] RY = new double[24]; // Y
    public static double[] RZ = new double[24]; // Z
    public static double[] SX = new double[24]; // X schaal
    public static double[] SY = new double[24]; // Y
    public static double[] SZ = new double[24]; // Z
    public static Vector3[] PA = new Vector3[200000]; // punt array
    public static Vector2[] TA = new Vector2[200000]; // textuur array
    public static Vector3[] NA = new Vector3[200000]; // normaal array
    public static int PN = 0; // punt index // PI kan niet
    public static Key KeyNumb; // onthoudt het nummer van de vorige ingedrukte toets
    public static long parserTime = 0;
    public static SharpDX.Matrix[] MM = new SharpDX.Matrix[200];
    public static bool parserRuns = false;
    public static bool parserReset = false;
    public static bool Navigate = false;
    public static double StepHeight;
    public static double[] KeepHeight = new double[48];
    public static string JumpWord = "";
    public static string DrawSave = "";
    public static string AddWord = "";
    public static string[] parserFile = new string[24];

    public void ParserInit()
    {
        partLego.LegoInit();

        for (int I = 0; I < 24; I++)
        {
            W[I] = "";
            V[I] = 0;
            TX[I] = 0;
            TY[I] = 0;
            TZ[I] = 0;
            RX[I] = 0;
            RY[I] = 0;
            RZ[I] = 0;
            SX[I] = 1;
            SY[I] = 1;
            SZ[I] = 1;
        }

        GS = 1;
        RI = 0;
        parserRuns = false;
        AddWord = "";
        FI = 0;
    }

    public void ParserOpen(string S, int B, int E) // start het uitvoeren van opdrachten
    {
        if (File.Exists(S)) // als het bestand met opdrachten gevonden is
        {
            parserRuns = true;

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            parserFile[FI] = S; // bestand naam onthouden

            char C; // scheidingskarakter

            if (FileType(S) == ".txt") C = ','; else C = ' '; // in .txt bestanden zijn de woorden gescheiden door een komma, in .dat en .asm bestanden door een spatie

            string[] T = File.ReadAllLines(S); // regels van het bestand lezen

            if (E == -1) E = T.Length; // aantal regels van het bestand bepalen

            for (int I = B; I < E; I++) // voer de opdrachten van regel B tot E uit
            {
                if (JumpWord == "") // geen sprong opdracht
                {
                    if (T[I] != "") // als de regel niet leeg is
                    {
                        ParserLine(AddWord + T[I], C); // bepaal de woorden in de regel
                        if (W[1] == "parse") AddWord = ""; else ParserWord();
                    }
                    if (parserReset) // als een sprong opdracht gevonden is moet het uitvoeren gereset worden om het sprong woord te zoeken
                    {
                        I = -1;
                        parserReset = false;
                    }
                }
                else // wel een sprong opdracht
                {
                    if (T[I] == JumpWord) JumpWord = ""; // als de regel het sprong opdracht woord bevat is de sprong gevonden
                }

                if (!parserRuns) break; // als het uitvoeren van opdrachten beeindigd moet worden
            }

            AddWord = "";
            JumpWord = "";

            if (FI == 0) ParserDone();
        }
        else // als het bestand niet gevonden is
        {
            MessageBox.Show("Bestand " + S + " niet gevonden");
            parserRuns = false;
        }
    }

    public void ParserDone() // stopt het uitvoeren van opdrachten
    {
        for (int i = 0; i < 24; i++) W[i] = "";

        if (gridView.Children.Contains(viewData)) viewData.DataView(viewData.TI);

        Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

        parserRuns = false;
    }

    public void ParserFiles() // bestand toevoegen
    {
        // W(0) = "file"
        // W(1) = bestandnaam
        // V(2) = tx verplaatsing
        // V(3) = ty
        // V(4) = tz
        // V(5) = rx rotatie
        // V(6) = ry
        // V(7) = rz
        // V[8] = sx schaal
        // V[9] = sy
        // V[10] = sz
        // V[11] = [kleur]

        string file = W[1];
        
        file = FileFind(file);

        if (file != "")
        {
            FI += 1;
            parserFile[FI] = file;

            TX[FI] = V[2] * SX[0] * GS; // verplaatsing langs X-as

            if (W[2] != "" && W[3] == "") TY[FI] = StepHeight * SY[0] * GS; else TY[FI] = V[3] * SY[0] * GS; // verplaatsing langs Y-as

            TZ[FI] = V[4] * SZ[0] * GS; // verplaatsing langs Z-as

            RX[FI] = V[5]; // rotatie om X-as
            RY[FI] = V[6];
            RZ[FI] = V[7];

            if (W[8] == "") SX[FI] = 1; else SX[FI] = V[8]; // schaal van X-as
            if (W[9] == "") SY[FI] = 1; else SY[FI] = V[9];
            if (W[10] == "") SZ[FI] = 1; else SZ[FI] = V[10];

            if (W[11] != "") // als een kleur bekent is
            {
                PartMat.DiffuseMap = null;
                PartMat.DiffuseColor = partMats.MatsColor(W[11]);
                PartMat.Name = W[11];
            }

            string type = FileType(parserFile[FI]);

            switch (type)
            {
                case ".bin":
                    partBins.BinOpen(parserFile[FI]); break;
                case ".bvh":
                case ".dae":
                case ".fbx":
                case ".obj":
                    if (!gridView.Children.Contains(viewRoom)) viewRoom.RoomOpen();
                    modsRigs.RigsOpen(parserFile[FI]); break;
                case ".dat":
                    ParserOpen(parserFile[FI], 0, -1);
                    partLego.LegoDone(); break;
                case ".txt":
                    ParserOpen(parserFile[FI], 0, -1); break;
                default:
                    FileOpen(parserFile[FI]); break;
            }

            if (FI > 0) FI -= 1;
        }
    }

    public void ParserJump()
    {
        // W[0] = "jump"
        // W[1] = jumpword

        JumpWord = W[1] + ":";
        parserReset = true;
    }

    public void ParserLine(string S, char C) // woorden en waarden uit een regel bepalen
    {
        int J;
        char K;
        if (S == null) S = "";

        for (int I = 0; I < 24; I++) // de woorden W[0..23] en waarden V[0..23]
        {
            S = S.Trim(); // verwijder spaties en new line tekens
            K = C;

            if (S.StartsWith("\"")) // als de regel begint met een "
            {
                S = S.Substring(1); // verwijder de "
                K = '"'; // verander het zoek karakter in "
            }
            else if (S.StartsWith("<")) // als de regel begint met een <
            {
                S = S.Substring(1); // verwijder de <
                K = '>'; // verander het zoek karakter in >
            }

            J = S.IndexOf(K);

            if (J > -1)
            {
                W[I] = S.Substring(0, J).Trim();
                S = S.Substring(J + 1);
                if (K != C && S.Length > 0) S = S.Substring(1); // verwijder de , na een " of >
            }
            else
            {
                W[I] = S.Trim();
                S = "";
            }

            if (W[I].StartsWith("=")) V[I] = tabsCalc.CalcCalc(W[I].Substring(1)); else double.TryParse(W[I], out V[I]);
        }
    }

    public void ParserWord() // opdracht bepalen
    {
        switch (W[0].ToLower())
        {
            case "0": partLego.LegoComment(); break;
            case "1": partLego.LegoFile(); break;
            case "3": partLego.LegoTriangle(); break;
            case "4": partLego.LegoRectangle(); break;

            case "add": ctrlCPU.CPU_ADD(); break;
            case "angle": viewDrawing.DrawingAngle(); break;
            case "animate": modsRigs.RigsAnimate(); break;

            case "bend": modsPrts.PartsBend(); break;
            case "bone": modsRigs.RigsBone(); break;
            case "bones": modsRigs.RigsBones(); break;
            case "box": modsPrts.PartsBox(); break;
            case "button": break; //ParsTextButton()

            case "calc": viewDrawing.DrawingCalc(); break; // berekent variabelen
            case "camera": roomCamera.CameraOpen(); break;
            case "choice": showGame.GameChoice(); break;
            case "circle": viewDrawing.DrawingCircle(); break;
            case "clear": viewDrawing.DrawingClear(); break;
            case "color": viewDrawing.DrawingColor(); break;
            case "column": viewData.DataColumn(); break;
            case "control": tabsCtrl.CtrlOpen(); break;
            case "copy": editText.TextCopy(); break;
            case ".cpu": ctrlCPU.CPU_Open(); break;
            case "cylinder": modsPrts.PartsCylinder(); break;

            case "data": viewData.DataOpen(); break;
            case "download": viewHTML.HTMLSave(); break;
            case "dirs": viewData.DataDirs(); break;
            case "disk": modsPrts.PartsDisk(); break;
            case "draw": viewDrawing.DrawingOpen(); break;
            case "drawpaint": viewDrawing.DrawingPaint(); break;

            case "export": modsRigs.RigsExport(); break;

            case "f":
            case "face": modsPrts.PartsFace(); break;
            case "file": ParserFiles(); break;
            case "files": viewData.DataFile(); break;
            case "find": viewData.DataFind(); break;
            case "floor": viewRoom.RoomFloor(); break;
            case "focus": roomCamera.CameraFocus(); break;

            case "gide": partLego.LegoGide = true; break;
            case "graph": viewDrawing.DrawingGraph(); break;
            case "grid": viewDrawing.DrawingGrid(); break;

            case "halt": ctrlCPU.CPU_HALT(); break;

            case "idle": IdleFile = W[1]; break;
            case "if": viewData.DataIf(); break;
            case "info": viewData.DataInfo(); break;
            case "input": tabsCtrl.CtrlInput(); break;
            case "iso": viewDrawing.DrawingIso(); break;

            case "jump": ParserJump(); break;

            case "ka": PartMat.AmbientColor = new Color4((float)V[1], (float)V[2], (float)V[3], 1); break;
            case "kd": PartMat.DiffuseColor = new Color4((float)V[1], (float)V[2], (float)V[3], 1); break;
            case "ke": PartMat.EmissiveColor = new Color4((float)V[1], (float)V[2], (float)V[3], 1); break;
            case "kr": PartMat.ReflectiveColor = new Color4((float)V[1], (float)V[2], (float)V[3], 1); break;
            case "ks": PartMat.SpecularColor = new Color4((float)V[1], (float)V[2], (float)V[3], 1); break;
            case "keyframe": modsRigs.RigsKeyFrame(); break;
            case "keyframes": modsRigs.RigsKeyFrames(); break;
            case "keytimes": modsRigs.RigsTime(); break;

            case "lamp": roomLamp.LampAdd(); break;
            case "ld": ctrlCPU.CPU_LD(); break;
             case "lego": partLego.LegoPart(); break;
            case "line": viewDrawing.DrawingLine(); break;
            case "list": partLego.LegoList(); break;
            case "listen": textVoice.TextHear(); break;

            case "makedirs": tabsFile.FileDirs(); break;
            case "map_bump": partMats.MatsBump(); break;
            case "map_kd": partMats.MatsKd(); break;
            case "map_kn": partMats.MatsKn(); break;
            case "map_ks": partMats.MatsKs(); break;
            case "material": partMats.MatsImport(); break;
            case "materials": partMats.MatsExport(); break;
            case "mem": tabsCtrl.CtrlMem(); break;
            case "merge": modsRigs.RigsMerg(); break;
            case "midi": tabsMidi.MidiOpen(); break;
            case "mode": windowGrid.GridModes((int)V[1]); break; 
            case "modelfile": ModelFile = W[1]; break; // onthoud een modelnaam voor exporteren
            case "move": modsPrts.PartsMove(); break; // verplaatst een onderdeel

            case "navigate": modsRigs.RigsNavi(); break;
            case "normal":
            case "vn": modsPrts.PartsNormal(); break;
            case "ns": PartMat.SpecularShininess = (float)V[1]; break;

            case "offset": viewDrawing.DrawingOffset(); break;
            case "output": tabsCtrl.CtrlOutput(); break;

            case "paint": viewDrawing.DrawingPaint(); break;
            case "parse": AddWord = W[1] + ", "; break;
            case "part": tabsParts.PartsPart(); break;
            case "parts": tabsParts.PartsLists(); break; // voegt de onderdelen van een model aan de onderdelen lijst toe
            case "photo": gridView.ShowPhoto(); break; // maak een foto van het media veld
            case "pict": viewDrawing.DrawingPict(); break; // plaats een foto in een tekening
            case "pictcopy": viewImage.ImageCopy(); break; // copyeer een foto
            case "pillar": modsPrts.PartsPillar(); break; // teken een pilaar
            case "pixel": viewDrawing.DrawingPixel(); break;
            case "plane": modsPrts.PartsPlane(); break; // teken een vlak
            case "play": viewPlayer.PlayerOpen(GetFileAddress(W[1])); break; // speel een film of muziek bestand
            case "point":
            case "v": modsPrts.PartsPoint(); break;
            case "pose": modsRigs.RigsPose(); break;
            case "poses": modsRigs.RigsPoses(); break;
            case "pyramid": modsPrts.PartsPyramid(); break;

            case "quad": modsPrts.PartsQuad(); break;

            case "read": viewData.DataRead(); break; // voegt gegevens aan een tabel toe
            case "rectangle": viewDrawing.DrawingRectangle(); break; // tekent een rechthoek
            case "reform": roomMods.ModsRfrm(); break;
            case "room": viewRoom.RoomOpen(); break; // start een omgeving
            case "roompaint": viewRoom.RoomPaint(); break; // tekent een model met de muis
            case "row": viewData.DataRows(); break; // voegt een rij aan een tabel toe
            case "rung": tabsCtrl.CtrlRung(); break;

            case "save": viewData.DataSave(); break;
            case "say": textVoice.SynthSay(); break;
            case "scale": viewDrawing.DrawingScale(); break;
            case "segment": modsPrts.PartsSegment(); break;
            case "show": viewImage.ImageShow(); break;
            case "skip": parserRuns = false; break;
            case "slope": modsPrts.PartsSlope(); break;
            case "sort": viewData.DataSort(); break;
            case "sphere": modsPrts.PartsSphere(); break;
            case "spin": viewRoom.RoomSpin(); break;
            case "spiro": viewDrawing.DrawingSpiro(); break;
            case "start": ParserStart(); break;
            case "step": ParserStep(); break;
            case "sum": viewData.DataSum(); break;

            case "table": viewData.DataTables(); break;
            case "tell": textVoice.SynthTell(false); break;
            case "test": ParserTest(); break;
            case "text": viewDrawing.DrawingText(); break;
            case "texture":
            case "vt": modsPrts.PartsTexture(); break;
            case "triangle": viewDrawing.DrawingTriangle(); break;
            case "trng": modsPrts.PartsTrng(); break;
            case "turn": modsPrts.PartsTurn(); break;
            case "type": viewText.TextType(W[0]); break;

            case "var": viewDrawing.DrawingVar(); break;
            case "view": viewRoom.RoomView(); break; // "look" is misschien beter
            case "voice": textVoice.SynthVoice(); break;

            case "wait": ParserWait((int)V[1]); break;
            case "walk": WalkFile = W[1]; break;
            case "wires": roomMods.ModsWires(); break;

            case "":
            case "#":
            case "2":
            case "5":
            case "l":
            case "mtllib":
            case "naam":
            case "o":
            case "s":
            case "usemtl": break;
            default:
                if (W[0].Substring(0, 1) != "'" && !W[0].Contains(":"))
                {
                    MessageBox.Show(W[0] + ", in " + parserFile[FI] + " is geen opdracht");
                    parserRuns = false;
                }
                break;
        }
    }

    public void ParserStart() // start een extern programma
    {
        // W[0] = "start"
        // W[1] = programma

        try
        {
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = W[1],
                Arguments = W[2],
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal
            };

            Process.Start(psi);
        }
        catch { }
    }

    public void ParserStep() // stap
    {
        // W[0] = "step"
        // V[1] = hoogte
        // W[2] = [foto nummer]

        string S = W[1]; // hoogte

        if (S != "") // als hoogte niet leeg is
        {
            switch (S.Substring(0, 1)) // controleer eerste karakter van de hoogte
            {
                case "+":
                case "-":
                    StepHeight += V[1]; break; // relatieve hoogte
                case "=":
                    StepHeight = KeepHeight[int.Parse(W[1].Substring(1))]; break; // gebruik een vorige hoogte opnieuw
                default:
                    StepHeight = V[1]; break; // absolute hoogte
            }

            KeepHeight[(int)V[2]] = StepHeight; // onthoud hoogte
        }

        if (partLego.LegoGide) // als handleiding foto's gemaakt moeten worden
        {
            if (W[2] != "") // als een foto nummer opgegeven is wordt een handleiding foto gemaakt
            {
                W[1] = @"#\" + FSL((float)V[2], 3, '0') + ".jpg"; // foto naam
                gridView.ShowPhoto(); // maak foto
            }
        }
    }

    public void ParserTest() // kan waardes tonen tijdens het uitvoeren van opdrachten
    {
        // W[0] = "test"

        MessageBox.Show(V[1] + ", " + V[2] + ", " + V[3] + ", " + V[4] + ", " + V[5]);
    }

    public void ParserWait(int T) // wacht t milliseconden
    {
        // W[0] = "wait"
        // V[1] = aantal milliseconden = T

        long S = DateTime.Now.Ticks; // onthoudt huidige tijd

        while ((DateTime.Now.Ticks - S) / 10000 < T) ParserDoEvents(); // zolang de wachttijd niet verstreken is en het uitvoeren van opdrachten niet onderbroken is
    }

    public void ParserDoEvents() // maakt het mogelijk om gebeurtenissen te verwerken tijdens het uitvoeren van opdrachten
    {
        try
        {
            System.Windows.Forms.Application.DoEvents();
        }
        catch
        { }

        /*

        var Frame = new DispatcherFrame();

        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(
            delegate (object f)
            {
                ((DispatcherFrame)f).Continue = false;
                return null;
            }), Frame);
        try
        {
            Dispatcher.PushFrame(Frame); // dit loopt vaak vast
        }
        catch { }
        */
    }

    public static TextParser textParser = new TextParser();
}