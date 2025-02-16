using HelixToolkit.Wpf.SharpDX;

using System;
using System.Windows.Media.Media3D;

using static RoomLamp;
using static RoomMods;
using static TabsCalc;
using static TabsRoom;
using static TextFunctions;
using static TextParser;
using static ViewRoom;

class RoomCamera : HelixToolkit.Wpf.SharpDX.PerspectiveCamera
{
    public static double CA; // camera hoek met y-as
    public static bool CF = false; // camera volgt
    public static double CS = 1; // camera schaal

    public HelixToolkit.Wpf.SharpDX.PerspectiveCamera CameraInit(double L) // Start camera
    {
        if (L == 0.0) L = 2.5;
        CameraSpot(L);
        UpDirection = new Vector3D(0, 1, 0);
        NearPlaneDistance = 0.01f;
        FarPlaneDistance = 5000;
        FieldOfView = Deg(Math.Atan(50.9 / (50.0 * 2.0)) * 2.0); // 50 mm kleinbeeld lens met 36 mm film = (36^2 + 36^2)^0.5 = 50.9

        Changed += This_Changed;

        return this;
    }

    public void CameraOpen()
    {
        // W[0] = "camera"
        // W[1] = type, o of p
        // W[2] = brandpunt
        // W[3] = positie X
        // W[4] = positie Y
        // W[5] = positie Z
        // W[6] = richting X
        // W[7] = richting Y
        // W[8] = richting Z

        Point3D P = new Point3D(V[3], V[4], V[5]);
        Vector3D D = new Vector3D(V[6], V[7], V[8]);

        if (W[1] == "o")
        {
            HelixToolkit.Wpf.SharpDX.OrthographicCamera C = new HelixToolkit.Wpf.SharpDX.OrthographicCamera()
            {
                Position = P,
                LookDirection = D
            };

            viewRoom.Camera = C;
            CameraView();
        }
        else
        {
            HelixToolkit.Wpf.SharpDX.PerspectiveCamera C = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera()
            {
                Position = P,
                LookDirection = D,
                FieldOfView = Deg(Math.Atan(50.9 / (V[2] * 2.0)) * 2.0)
            };

            viewRoom.Camera = C;
            
            C.Changed += This_Changed;
        }
    }

    public void CameraFocus()
    {
        RoomMod.SceneNode.UpdateAllTransformMatrix();
        RoomMod.SceneNode.TryGetBound(out var B);
        var maxWidth = Math.Max(Math.Max(B.Width, B.Height), B.Depth) + B.Depth / 2.0f;
        var pos = B.Center + new SharpDX.Vector3(-maxWidth, 0, maxWidth);
        pos.Y *= 2;
        roomCamera.Position = pos.ToPoint3D();
        roomCamera.LookDirection = (-pos).ToVector3D();
        CameraView();
    }

    public void CameraMove()
    {
        Point3D P;
        RotateTransform3D R = new RotateTransform3D();

        if (!CF)
        {
            R.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), CA); // draai camera om y-as
            P = R.Transform(new Point3D(0, 1.2 * CS, 3 * CS)); // positie camera t.o.v. model

            Position = MP + P; // camera positie
            LookDirection = new Vector3D(-P.X, -P.Y / 6, -P.Z);
            CameraView();
        }
    }

    public void CameraView()
    {
        Point3D P = viewRoom.Camera.Position;
        Vector3D V = viewRoom.Camera.LookDirection;

        tabsRoom.CameraInfo.Text = FL(P.X) + FL(P.Y) + FL(P.Z) + FL(V.X) + FL(V.Y) + FL(V.Z);
    }

    public void CameraSpot(double L)
    {
        Position = new Point3D(-L, L / 2, L);
        LookDirection = new Vector3D(L, -L / 5, -L);
    }

    public void This_Changed(object sender, EventArgs e) // als camera veranderd
    {
        roomLamp.LampTurn();
        CameraView();
    }

    public static RoomCamera roomCamera = new RoomCamera(); // camera object
}