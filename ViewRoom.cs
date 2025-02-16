using HelixToolkit.Wpf.SharpDX; // intalleer met Reference | Manage NuGet Packages
using HelixToolkit.Wpf.SharpDX.Animations;
using HelixToolkit.Wpf.SharpDX.Controls;

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

using static GridView;
using static ModsRigs;
using static PartMats;
using static RoomCamera;
using static RoomLamp;
using static RoomMods;
using static TabsFile;
using static TextParser;
using static ViewImage;

class ViewRoom : Viewport3DX // omgeving class
{
    public static int DA; // richting hoek
    public static double MA; // model hoek met y-as
    public static Vector3D MP = new Vector3D(); // model positie
    public static double MT; // model tijd voor verplaatsing
    public static double MY; // model delta verplaatsing langs y-as
    public static double MZ;  // model delta verplaatsing langs z-as
    public static bool Navigates = false;
    public static TPosed Posed;
    public static bool AE = false; // animatie enabled
    public static bool SE = false; // sliders enabled
    public static NodeAnimationUpdater[] RigsUpdt = new NodeAnimationUpdater[4];
    public static CompositionTargetEx RigsPoser = new CompositionTargetEx();
    public static Transform3DGroup RigsTransformer = new Transform3DGroup();
    public static string IdleFile;
    public static string WalkFile;
    public static int[] QC = new int[4]; // quartenion w correctie
    public static bool roomReady = false;

    public Viewport3DX RoomInit() // Start omgeving
    {
        EffectsManager = new DefaultEffectsManager();
        //MSAA = MSAALevel.Two;
        FXAALevel = FXAALevel.Medium;
        EnableDesignModeRendering = false;
        EnableSwapChainRendering = false;
        ShowCoordinateSystem = false;
        ShowViewCube = false;
        ShowFrameRate = false;

        InputBindings.Add(new InputBinding(ViewportCommands.Rotate, new MouseGesture(MouseAction.LeftClick)));
        InputBindings.Add(new InputBinding(ViewportCommands.Pan, new MouseGesture(MouseAction.RightClick)));
        InputBindings.Add(new InputBinding(ViewportCommands.FrontView, new MouseGesture(MouseAction.LeftDoubleClick)));
        InputBindings.Add(new InputBinding(ViewportCommands.TopView, new KeyGesture(Key.NumPad8)));
        InputBindings.Add(new InputBinding(ViewportCommands.LeftView, new KeyGesture(Key.NumPad6)));
        InputBindings.Add(new InputBinding(ViewportCommands.FrontView, new KeyGesture(Key.NumPad5)));
        InputBindings.Add(new InputBinding(ViewportCommands.RightView, new KeyGesture(Key.NumPad4)));
        InputBindings.Add(new InputBinding(ViewportCommands.BottomView, new KeyGesture(Key.NumPad2)));
        InputBindings.Add(new InputBinding(ViewportCommands.BackView, new KeyGesture(Key.NumPad0)));

        PreviewKeyDown += This_PreviewKeyDown;
        PreviewKeyUp += This_PreviewKeyUp;
        PreviewMouseDown += This_PreviewMouseDown;
        PreviewMouseMove += This_PreviewMouseMove;

        this.OnRendered += ViewRoom_OnRendered;

        return this;
    }

    private void ViewRoom_OnRendered(object sender, EventArgs e)
    {
        roomReady = true;
    }

    public void RoomDone()
    {
        Dispose();
    }

    public void RoomOpen() // Start omgeving
    {
        // W[0] = "room"
        // W[1] = [achtergrondkleur]
        // V[2] = [camera afstand]
        // V[3] = [omgevinglamp kleur]
        // V[4] = [richtinglamp 1 kleur]
        // V[5] = [richtinglamp 2 kleur]

        roomReady = false;

        Items.Clear();
        RoomMod.Clear();
        Items.Add(roomMods.ModsInit());

        Camera = roomCamera.CameraInit(V[2]);

        if (W[1] == "") BackgroundColor = partMats.MatsColor("grijs-x").ToColor(); else BackgroundColor = partMats.MatsColor(W[1]).ToColor();
        roomLamp.LampInit(V[3], V[4], V[5]);

        viewImage.imageFile = "";

        gridView.Children.Clear();
        gridView.Children.Add(this);

        Navigates = false;

        roomCamera.CameraView();
    }

    public void RoomPaint()
    {
        // W[0] = "roompaint"
        // W[1] = afbeelding bestand

        string F = GetFileAddress(W[1]);

        viewImage.imageFile = F;
        //showPict.PictMap = null;
        viewImage.ImageShow();

        //gridView.Children.Clear();
        //gridView.Children.Add(DrawView);

        //gridEdit.Children.Clear();
        //gridEdit.Children.Add(viewRoom);
    }

    public void RoomFloor() // maakt een vloer
    {
        // W(0) = "floor"
        // V(1) = [maas] in meter
        // W(2) = [kleur]
        // W(3) = [offset]

        AxisPlaneGridModel3D F = new AxisPlaneGridModel3D()
        {
            GridPattern = GridPattern.Tile,
            AutoSpacing = false
        };

        if (W[1] != "") F.GridSpacing = V[1];
        
        viewRoom.Items.Add(F);
    }

    public void RoomSpin() // laat een omgeving draaien
    {
        // W[0] = "spin"
        // W[1] = [iest]

        if (W[1] == "")
        {
            viewRoom.InfiniteSpin = true;
            viewRoom.StartSpin(new Vector(50, 0), new System.Windows.Point(0, 0), new Point3D(0, 0, 0));
        }
        else
        {
            viewRoom.StopSpin();
        }
    }

    public void RoomView() // kijk richting
    {
        // W[0] = "view"
        // W[1] = richting

        switch (W[1])
        {
            case "front": ViewportCommands.FrontView.Execute(null, this); break;
            case "top": ViewportCommands.TopView.Execute(null, this); break;
        }
    }

    public void This_Changed(object sender, EventArgs e) // als camera veranderd
    {
        roomLamp.LampTurn();
        roomCamera.CameraView();
    }

    public void This_PreviewKeyDown(object sender, KeyEventArgs e) // als op een toets gedrukt wordt
    {
        if (Navigates)
        {
            int H = 5;
            if (KeyNumb == 0)
            {
                modsRigs.RigsLoad(WalkFile);
            }
            KeyNumb = e.Key;
            e.Handled = true;
            textParser.ParserDoEvents();
            switch (KeyNumb)
            {
                case Key.Up:
                    MT = 1.2;
                    DA = 180;
                    MZ = -1.2 * CS;
                    MA = CA + DA; //  model hoek = camera hoek + richting hoek
                    break;
                case Key.Down:
                    MT = 1.2;
                    DA = 0;
                    MZ = -1.2 * CS;
                    MA = CA + DA;
                    break;
                case Key.Left:
                    MT = 0.12;
                    if (CA < 360) CA += H; else CA = 0;
                    MZ = 0 * CS;
                    MA = CA + DA;
                    break;
                case Key.Right:
                    MT = 0.12;
                    if (CA > -360) CA -= H; else CA = 0;
                    MZ = 0 * SX[0];
                    MA = CA + DA;
                    break;
            }
            if (DateTime.Now.Ticks > parserTime + (MT * 100000))
            {
                Vector3D O; // model positie
                Point3D P = new Point3D(0, MY / SY[0], -MZ / SZ[0]);
                RotateTransform3D R = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), MA));
                P = R.Transform(P);
                O = MP / 2 / SX[0]; // ???
                TranslateTransform3D TTX = new TranslateTransform3D(O);
                DoubleAnimation DAX = new DoubleAnimation(0, P.X, new Duration(TimeSpan.FromSeconds(MT)));
                TranslateTransform3D TTY = new TranslateTransform3D(O);
                DoubleAnimation DAY = new DoubleAnimation(0, P.Y, new Duration(TimeSpan.FromSeconds(MT)));
                TranslateTransform3D TTZ = new TranslateTransform3D(O);
                DoubleAnimation DAZ = new DoubleAnimation(0, P.Z, new Duration(TimeSpan.FromSeconds(MT)));
                TTX.BeginAnimation(TranslateTransform3D.OffsetXProperty, DAX);
                TTY.BeginAnimation(TranslateTransform3D.OffsetYProperty, DAY);
                TTZ.BeginAnimation(TranslateTransform3D.OffsetZProperty, DAZ);
                ScaleTransform3D S = new ScaleTransform3D(new Vector3D(SX[0], SY[0], SZ[0]));
                RigsTransformer.Children[0] = R;
                RigsTransformer.Children[1] = TTX;
                RigsTransformer.Children[2] = TTY;
                RigsTransformer.Children[3] = TTZ;
                RigsTransformer.Children[4] = S;

                parserTime = DateTime.Now.Ticks;
            }
            textParser.ParserDoEvents();
        }
    }

    public void This_PreviewKeyUp(object sender, KeyEventArgs e) // als op een toets gedrukt wordt
    {
        if (Navigates)
        {
            modsRigs.RigsLoad(IdleFile);
            KeyNumb = 0;
        }
    }

    public void This_PreviewMouseDown(object sender, MouseEventArgs e) // als op een muis toets gedrukt wordt
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.LeftShift)) roomMods.ModsSlct(e.GetPosition(this).ToVector2());
    }

    public void This_PreviewMouseMove(object sender, MouseEventArgs e) // als op een muis toets gedrukt wordt
    {
        if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.LeftShift)) && e.LeftButton == MouseButtonState.Pressed) roomMods.ModsSlct(e.GetPosition(this).ToVector2());
    }

    public static ViewRoom viewRoom = new ViewRoom(); // het omgeving object
}