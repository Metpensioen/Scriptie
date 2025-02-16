using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;

using SharpDX;

using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using static EditText;
using static ModsPrts;
using static ModsRigs;
using static PartMats;
using static TextParser;
using static ViewDrawing;
using static ViewRoom;

class RoomMods
{
    public static SceneNodeGroupModel3D RoomMod = new SceneNodeGroupModel3D(); // hierin komen de modellen en onderdelen; kan niet overerven
    public static MeshGeometry3D PartGeom; // kan de geometrie van een model en onderdeel onthouden
    public static string ModelFile; // onthoud de bestandnaam van een model

    public SceneNodeGroupModel3D ModsInit() // start modellen
    {
        modsPrts.PartsInit(); // start onderdelen
        modsRigs.RigsInit(); // start bewegelijke modelen

        return RoomMod; 
    }

    public void ModsRfrm() // hervormt de onderdelen van een model
    {
        // W[1] = "reform"

        System.Windows.Media.Media3D.Matrix3D M;
        HelixToolkit.Wpf.SharpDX.Geometry3D G;
        int J;
        int K = 0;
        System.Windows.Media.Media3D.Point3D O;
        System.Windows.Media.Media3D.Point3D P;

        foreach (var Node in RoomMod.SceneNode.Traverse(true))
        {
            if (Node is MeshNode N)
            {
                M = N.ModelMatrix.ToMatrix3D();
                G = N.Geometry;
                J = G.Positions.Count;

                for (int I = 0; I < J; I++)
                {
                    O = G.Positions[I].ToPoint3D();
                    P = O * M;
                    if (O != P)
                    {
                        K++;
                        G.Positions[I] = P.ToVector3();
                    }
                }
            }
        }
    }
    public void ModsSlct(Vector2 C) // selecteerd een driehoek van een onderdeel
    {
        Vector2 G = new Vector2(-500, 500);
        Vector2 H = new Vector2(1000, -1000);

        Vector2[] U = new Vector2[4];
        Vector3[] P = new Vector3[4];
        Vector3[] N = new Vector3[4];

        MeshNode Q;
        PhongMaterialCore M;

        System.Tuple<int, int, int> T;

        try
        {
            T = viewRoom.FindHits(C)[0].TriangleIndices;                 // geeft de 3 indexen van de geselecteerde driehoek!
            PartGeom = (MeshGeometry3D)viewRoom.FindHits(C)[0].Geometry; // copieerd de gehele geometry van het model
            PartGeom.UpdateVertices();
            PartGeom.UpdateTextureCoordinates();

            P[1] = PartGeom.Positions[T.Item1];
            P[2] = PartGeom.Positions[T.Item2];
            P[3] = PartGeom.Positions[T.Item3];
            U[1] = PartGeom.TextureCoordinates[T.Item1];
            U[2] = PartGeom.TextureCoordinates[T.Item2];
            U[3] = PartGeom.TextureCoordinates[T.Item3];
            N[1] = PartGeom.Normals[T.Item1];
            N[2] = PartGeom.Normals[T.Item2];
            N[3] = PartGeom.Normals[T.Item3];

            if (Keyboard.IsKeyDown(Key.LeftCtrl)) // zet de gegevens van de geselecteerde driehoek in de text
            {
                editText.AppendText("v, 1, " + P[1].X + ", " + P[1].Y + ", " + P[1].Z + "\r\n");
                editText.AppendText("v, 2, " + P[2].X + ", " + P[2].Y + ", " + P[2].Z + "\r\n");
                editText.AppendText("v, 3, " + P[3].X + ", " + P[3].Y + ", " + P[3].Z + "\r\n");
                editText.AppendText("vt, 1, " + U[1].X + ", " + U[1].Y + "\r\n");
                editText.AppendText("vt, 2, " + U[2].X + ", " + U[2].Y + "\r\n");
                editText.AppendText("vt, 3, " + U[3].X + ", " + U[3].Y + "\r\n");
                editText.AppendText("vn, 1, " + N[1].X + ", " + N[1].Y + ", " + N[1].Z + "\r\n");
                editText.AppendText("vn, 2, " + N[2].X + ", " + N[2].Y + ", " + N[2].Z + "\r\n");
                editText.AppendText("vn, 3, " + N[3].X + ", " + N[3].Y + ", " + N[3].Z + "\r\n");

                editText.AppendText("f, 1/1/1, 2/2/2, 3/3/3" + "\r\n");
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt)) // verf een driehoek
            {
                U[1] = U[1] * H + G;
                U[2] = U[2] * H + G;
                U[3] = U[3] * H + G;

                drawingGrid.Children.Add(DrawTriangles(U[1].ToPoint(), U[2].ToPoint(), U[3].ToPoint(), partMats.MatsColor("rood").ToColor(), true));

                Q = (MeshNode)viewRoom.FindHits(C)[0].ModelHit;
                M = (PhongMaterialCore)Q.Material;
                M.DiffuseMap = null;

                PngBitmapEncoder Encoder = new PngBitmapEncoder();
                RenderTargetBitmap MapsBitmap = new RenderTargetBitmap(1000, 1000, 96, 96, PixelFormats.Default);
                MemoryStream MS = new MemoryStream();

                MapsBitmap.Render(viewDrawing);
                Encoder.Frames.Add(BitmapFrame.Create(MapsBitmap));
                Encoder.Save(MS);

                M.DiffuseMap = MS;
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift)) // verwijder een driehoek
            {
                int J = PartGeom.TriangleIndices.Count;
                IntCollection TC = new IntCollection();
                for (int I = 0; I < J; I++)
                {
                    TC.Add(PartGeom.TriangleIndices[I]);
                }
                for (int I = 0; I < J - 2; I++)
                {
                    if (TC[I] == T.Item1 && TC[I + 1] == T.Item2 && TC[I + 2] == T.Item3)
                    {
                        for (int K = I; K < J - 3; K++)
                        {
                            TC[K] = TC[K + 3];
                        }
                        goto verder;
                    }
                }
            verder:;
                PartGeom.TriangleIndices.Clear();
                for (int I = 0; I < J - 3; I++)
                {
                    PartGeom.TriangleIndices.Add(TC[I]);
                }
                PartGeom.UpdateVertices();
                modsPrts.ModelPart.Geometry = PartGeom;
                modsPrts.ModelPart.Material = PartMat;
                modsPrts.ModelPart.RenderWireframe = true;
                modsPrts.ModelPart.WireframeColor = new Color4(0, 0, 0, 1).ToColor();
                RoomMod.Clear();
                RoomMod.AddNode((HelixToolkit.Wpf.SharpDX.Model.Scene.SceneNode)modsPrts.ModelPart);
            }
        }
        catch
        {
            return;
        }
    }

    public void ModsWires() // toon draadmodel
    {
        // W[0] = "wires"
        // W[1] = [kleur]

        Color4 C;

        if (W[1] == "") C = new Color4(0, 0, 0, 1); else C = partMats.MatsColor(W[1]);

        foreach (var N in RoomMod.SceneNode.Traverse(true))
        {
            if (N is MeshNode M)
            {
                M.RenderWireframe = true;
                M.WireframeColor = C;
                M.IsHitTestVisible = true;
            }
        }
    }

    public static RoomMods roomMods = new RoomMods(); // het modellen object
}