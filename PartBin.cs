using HelixToolkit.Wpf.SharpDX;
using SharpDX;

using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;

using static ModsPrts;
using static PartLego;
using static PartMats;
using static RoomMods;
using static TabsFile;
using static TextParser;

class PartBins
{
    public void BinOpen(string S) // onderdeel uit een .bin bestand tekenen
    {
        if (File.Exists(S))
        {
            int I1;
            int I2;
            int I3;

            MeshBuilder B = new MeshBuilder();
            FileInfo F = new FileInfo(S);
            long N;
            Transform3DGroup T = new Transform3DGroup();

            using (BinaryReader R = new BinaryReader(File.Open(S, FileMode.Open)))
            {
                N = F.Length / 12; // aantal posities

                for (int I = 0; I < N; I++)
                {
                    PA[I] = new Vector3(R.ReadSingle(), R.ReadSingle(), R.ReadSingle());
                }
            }

            N /= 3; // aantal driehoeken

            for (int I = 0; I < N; I++)
            {
                I1 = I * 3 + 0;
                I2 = I * 3 + 1;
                I3 = I * 3 + 2;
                
                B.AddTriangle(PA[I1], PA[I2], PA[I3]);
                
                //if (PA[I1].X < PA[I3].X) B.TextureCoordinates[I1] = new Vector2(0, 0); else B.TextureCoordinates[I1] = new Vector2(1, 1);
                //if (PA[I1].Y < PA[I2].Y) B.TextureCoordinates[I2] = new Vector2(1, 0); else B.TextureCoordinates[I2] = new Vector2(0, 1);
                //if (PA[I1].X < PA[I3].X) B.TextureCoordinates[I3] = new Vector2(1, 1); else B.TextureCoordinates[I3] = new Vector2(0, 0);
            }

            //B.CreateNormals = false;

            modsPrts.ModelPart.Geometry = B.ToMeshGeometry3D();
            modsPrts.ModelPart.Material = PartMat;

            for (int I = FI; I >= 0; I--)
            {
                modsPrts.PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
                modsPrts.PartsTranslate(T, TX[I], TY[I], TZ[I]);
                modsPrts.PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
            }

            
                modsPrts.ModelPart.Transform = T;
            
            //string Q = S.Substring(GetFileRoot().Length + 2);
            
            //Q = Q.Replace(@"\part-2.bin", "");
            //Q = Q.Replace(@"\part.bin", "");
            //Q = Q.Replace(@"\", "_");
            //Q = Q.Replace("-", "");
            //Q = Q.Replace("#", "");
            //modsPrts.ModelPart.Name = Q;
            
            modsPrts.PartsAdd();

            partLego.LegoAdd();
        }
        else
        {
            MessageBox.Show("onderdeel " + S + " niet gevonden");
            parserRuns = true;
        }
    }

    public void BinSave(string S) // exporteert een onderdeel naar een .bin bestand
    {
        BinaryWriter F = new BinaryWriter(File.Open(S, FileMode.Create));

        HelixToolkit.Wpf.SharpDX.GeometryModel3D M;
        HelixToolkit.Wpf.SharpDX.MeshGeometry3D P;

        int K = RoomMod.SceneNode.Items.Count;
        int N;
        int O;

        for (int J = 0; J < K; J++)
        {
            M = (HelixToolkit.Wpf.SharpDX.GeometryModel3D)RoomMod.SceneNode.Items[J];
            P = (HelixToolkit.Wpf.SharpDX.MeshGeometry3D)M.Geometry;
            N = P.TriangleIndices.Count;

            for (int I = 0; I < N; I++)
            {
                O = P.TriangleIndices[I];
                F.Write(P.Positions[O].X);
                F.Write(P.Positions[O].Y);
                F.Write(P.Positions[O].Z);
            }
        }

        F.Close();
    }

    public static PartBins partBins = new PartBins();
}