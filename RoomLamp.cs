using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows.Media.Media3D;

using static RoomCamera;
using static TabsRoom;
using static TextParser;
using static ViewRoom;

class RoomLamp
{
    public AmbientLight3D LampA1 = new AmbientLight3D();
    public DirectionalLight3D LampD1 = new DirectionalLight3D(); // lamp object
    public DirectionalLight3D LampD2 = new DirectionalLight3D(); // lamp object

    public void LampInit(double V1, double V2, double V3) // Start lampen
    {
        if (V1 == 0) V1 = 0.20f;
        if (V2 == 0) V2 = 0.50f;
        if (V3 == 0) V3 = 0.90f;

        tabsRoom.lampA1CSlider.Value = V1;

        tabsRoom.lampD1CSlider.Value = V2;
        tabsRoom.LampD1XSlider.Value = 0.8;
        tabsRoom.LampD1YSlider.Value = 0.8;
        tabsRoom.LampD1ZSlider.Value = 0.0;

        tabsRoom.lampD2CSlider.Value = V3;
        tabsRoom.LampD2XSlider.Value = 0;
        tabsRoom.LampD2YSlider.Value = 0;
        tabsRoom.LampD2ZSlider.Value = 0;

        LampTurn();

        if (!viewRoom.Items.Contains(LampA1)) viewRoom.Items.Add(LampA1);
        if (!viewRoom.Items.Contains(LampD1)) viewRoom.Items.Add(LampD1);
        if (!viewRoom.Items.Contains(LampD2)) viewRoom.Items.Add(LampD2);
    }

    public void LampAdd()
    {
        // W[0] = "lamp"
        // W[1] = type a of d
        // V[2] = rood
        // V[3] = groen
        // V[4] = blauw

        if (W[1] == "a")
        {
            LampA1.Color = new Color4((float)V[2], (float)V[3], (float)V[4], 1).ToColor();
        }
        else
        {
            LampD1.Color = new Color4((float)V[2], (float)V[3], (float)V[4], 1).ToColor();
        }
    }

    public void LampTurn()
    {
        Vector3D D1 = roomCamera.LookDirection;
        Vector3D D2 = roomCamera.LookDirection;

        double D = roomCamera.Position.Z;

        Vector3D S1 = new Vector3D(tabsRoom.LampD1XSlider.Value * D, tabsRoom.LampD1YSlider.Value * D, tabsRoom.LampD1ZSlider.Value * D);
        //Vector3D S2 = new Vector3D(tabsRoom.LampD2XSlider.Value * D, tabsRoom.LampD2YSlider.Value * D, tabsRoom.LampD2ZSlider.Value * D);

        LampD1.Direction = D1 + S1;
        LampD2.Direction = D2 - S1;

        tabsRoom.LampD1Label.Content = S1.ToString();
        //TabsRoom.LampD2Label.Content = LampD2.Direction.ToString() + "  " + S2.ToString();
    }

    public static RoomLamp roomLamp = new RoomLamp(); // lampen object
}