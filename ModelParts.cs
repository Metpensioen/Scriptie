using HelixToolkit.Wpf.SharpDX;
using SharpDX;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

using static PartMats;
using static RoomMods;
using static TabsCalc;
using static TextParser;
using static ViewRoom;

class ModsPrts
{
    public struct TMove
    {
        public int T; // tijd in millisecondes
        public int R; // herhalen
        public double X;
        public double Y;
        public double Z;
    }

    public TMove PMove;

    public struct TTurn
    {
        public int T;
        public int R;
        public double CX;
        public double CY;
        public double CZ;
        public double AX;
        public double AY;
        public double AZ;
        public double B;
        public double E;
        public bool K;
    }

    public TTurn PTurn;

    public Transform3DGroup PartTransform = new Transform3DGroup();
    public MeshGeometryModel3D ModelPart = new MeshGeometryModel3D();

    public void PartsInit()
    {
        PN = -1;
        partMats.MatsInit();
        PMove.T = 0;
        PTurn.T = 0;
        viewRoom.StopSpin();
    }

    public void PartsAdd()
    {
        Transform3DGroup T = new Transform3DGroup();

        T.Children.Add(ModelPart.Transform);
        T.Children.Add(PartMove());
        T.Children.Add(PartTurn());
        //ModelPart.Transform = T;
        RoomMod.AddNode((HelixToolkit.Wpf.SharpDX.Model.Scene.SceneNode)ModelPart);
        int N = RoomMod.SceneNode.ItemsCount - 1;
        RoomMod.SceneNode.Items[N].Name = ModelPart.Name;
        ModelPart = new MeshGeometryModel3D();
        textParser.ParserWait(PMove.T);
    }

    public void PartsBend() //' tekent een gebogen buis
    {
        // W(0) = "bend"
        // W(1) = diameter
        // W(2) = segmenten
        // W(3) = hoek
        // W(4) = boogstraal
        // W(5) = spoed

        MeshBuilder B = new MeshBuilder();
        double D = 360.0f / V[2]; // hoek per segment
        double H;
        Vector3Collection P = new Vector3Collection();
        Transform3DGroup T = new Transform3DGroup();
        double R = V[4] * 0.886f;
        double S = V[5] / V[2];
        int N = (int)(V[3] / D) + 1;
        double X = DXC(R, -D / 2);
        double Y = DYS(R, D / 2);

        for (int I = 0; I <= N; I++)
        {
            H = (I - 0.5f) * D;
            P.Add(new Vector3((float)(DXC(R, H) - X), (float)(DYS(R, H) + Y), (float)(I * S)));
        }

        B.AddTube(P, V[1], 24, false, false, false);
        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        FI += 1;
        TX[FI] = V[6];
        TY[FI] = V[7];
        TZ[FI] = V[8];
        RX[FI] = V[9];
        RY[FI] = V[10];
        RZ[FI] = V[11];
        if (W[12] != "") SX[FI] = V[12]; else SX[FI] = 1;
        if (W[13] != "") SY[FI] = V[13]; else SY[FI] = 1;
        if (W[14] != "") SZ[FI] = V[14]; else SZ[FI] = 1;

        for (int I = FI; I > 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        FI -= 1;
        ModelPart.Transform = T;
        ModelPart.Name = "curve";

        PartsAdd();
    }

    public void PartsBox() // doos
    {
        // W(0) = "box"
        // V[1] = x
        // V[2] = y
        // V[3] = z
        // V[4] = tx

        MeshBuilder B = new MeshBuilder();
        Transform3DGroup T = new Transform3DGroup();

        B.AddBox(new Vector3(0, 0, 0), V[1], V[2], V[3]);
        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        FI += 1;
        TX[FI] = V[4];
        TY[FI] = V[5];
        TZ[FI] = V[6];
        RX[FI] = V[7];
        RY[FI] = V[8];
        RZ[FI] = V[9];
        if (W[10] != "") SX[FI] = V[10]; else SX[FI] = 1;
        if (W[11] != "") SY[FI] = V[11]; else SY[FI] = 1;
        if (W[12] != "") SZ[FI] = V[12]; else SZ[FI] = 1;

        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        if (FI > 0) FI -= 1;
        ModelPart.Transform = T;
        ModelPart.Name = "box";

        PartsAdd();
    }

    public void PartsCurve() // teken een kromming
    {

    }

    public void PartsCylinder() // cilinder
    {
        // W[0] = "cylinder"
        // W[1] = diameter
        // W[2] = hoogte
        // W[3] = segmenten

        MeshBuilder B = new MeshBuilder();
        Transform3DGroup T = new Transform3DGroup();

        V[2] = V[2] / 2.0f;

        B.AddCylinder(new Vector3(0, (float)-V[2], 0), new Vector3(0, (float)V[2], 0), V[1] / 2.0f, (int)V[3], true, true);
        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;
        ModelPart.Name = "cylinder";

        FI += 1;
        TX[FI] = V[4];
        TY[FI] = V[5];
        TZ[FI] = V[6];
        RX[FI] = V[7];
        RY[FI] = V[8];
        RZ[FI] = V[9];
        if (W[10] != "") SX[FI] = V[10]; else SX[FI] = 1;
        if (W[11] != "") SY[FI] = V[11]; else SY[FI] = 1;
        if (W[12] != "") SZ[FI] = V[12]; else SZ[FI] = 1;

        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        if (FI > 0) FI -= 1;
        ModelPart.Transform = T;
        ModelPart.Name = "cylinder";

        PartsAdd();
    }

    public void PartsDisk() // schijf
    {
        // W[0] = 'disk"

        MeshBuilder B = new MeshBuilder();
        Transform3DGroup T = new Transform3DGroup();
        List<Vector3> L = new List<Vector3>();
        List<Vector3> N = new List<Vector3>();
        List<Vector2> U = new List<Vector2>();
        double X;
        double Y;
        double R = V[1]; // straal
        int J = (int)V[2]; // segmenten
        double A = 360f / J; // segmenthoek

        for (int I = 0; I < J; I++)
        {
            X = DXC(R, -A * I);
            Y = DYS(R, -A * I);
            L.Add(new Vector3((float)X, (float)V[4], (float)Y));
            N.Add(new Vector3(0, 1, 0));
            U.Add(new Vector2((float)((X + R) / (R * 2)), (float)((-Y + R) / (R * 2))));

            L.Add(new Vector3(0, (float)V[4], 0));
            N.Add(new Vector3(0, 1, 0));
            U.Add(new Vector2(0.5f, 0.5f));

            X = DXC(R, -A * (I + 1));
            Y = DYS(R, -A * (I + 1));
            L.Add(new Vector3((float)X, (float)V[4], (float)Y));
            N.Add(new Vector3(0, 1, 0));
            U.Add(new Vector2((float)((X + R) / (R * 2)), (float)((-Y + R) / (R * 2))));
        }

        B.AddTriangles(L, N, U);
        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        FI += 1;
        TX[FI] = V[3];
        TY[FI] = V[4];
        TZ[FI] = V[5];
        RX[FI] = V[6];
        RY[FI] = V[7];
        RZ[FI] = V[8];
        if (W[9] != "") SX[FI] = V[9]; else SX[FI] = 1;
        if (W[10] != "") SY[FI] = V[10]; else SY[FI] = 1;
        if (W[11] != "") SZ[FI] = V[11]; else SZ[FI] = 1;

        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        if (FI > 0) FI -= 1;
        ModelPart.Transform = T;
        ModelPart.Name = "disk";

        PartsAdd();
    }

    public void PartsFace() // vlak
    {
        // W[0] = "f"
        // W[1] = punt 1
        // W[2] = punt 2
        // W[3] = punt 3
        // W[4] = punt 4

        int J;
        int K;
        int M;
        int[] P = new int[5];
        int[] U = new int[5];
        int[] N = new int[5];

        if (W[4] == "") M = 3; else M = 4;

        for (int I = 1; I <= M; I++)
        {
            J = W[I].IndexOf(@"/");
            K = W[I].LastIndexOf(@"/");

            try
            {
                if (J > -1) P[I] = int.Parse(W[I].Substring(0, J)); else P[I] = int.Parse(W[I]);
                if (J > -1 && K > -1) U[I] = int.Parse(W[I].Substring(J + 1, K - J - 1));
                if (J > -1 && K > -1) N[I] = int.Parse(W[I].Substring(K + 1));
            }
            catch { MessageBox.Show(W[I] + " is fout"); }
            { }
        }

        MeshBuilder B = new MeshBuilder();
        if (M == 3)
        {
            B.AddTriangle(PA[P[1]], PA[P[2]], PA[P[3]], TA[U[1]], TA[U[2]], TA[U[3]]);
        }
        else
        {
            B.AddQuad(PA[P[1]], PA[P[2]], PA[P[3]], PA[P[4]], TA[U[1]], TA[U[2]], TA[U[3]], TA[U[4]]);
        }
        HelixToolkit.Wpf.SharpDX.MeshGeometry3D G = B.ToMeshGeometry3D();
        J = G.Normals.Count;
        if (M == 3)
        {
            G.Normals[J - 3] = NA[N[1]];
            G.Normals[J - 2] = NA[N[2]];
            G.Normals[J - 1] = NA[N[3]];
        }
        else
        {
            G.Normals[J - 4] = NA[N[1]];
            G.Normals[J - 3] = NA[N[2]];
            G.Normals[J - 2] = NA[N[3]];
            G.Normals[J - 1] = NA[N[4]];
        }

        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;
        ModelPart.Name = "face";

        Transform3DGroup T = new Transform3DGroup();
        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        ModelPart.Transform = T;

        PartsAdd();
    }

    public void PartsPillar() // pilaar
    {
        // W[0] = "pillar"
        // V[1] = x
        // V[2] = y
        // V[3] = u

        MeshBuilder B = new MeshBuilder();
        Vector3 D = new Vector3(0, 1, 0);
        Vector3 O = new Vector3(0, 0, 0);
        List<Vector2> P = new List<Vector2>();
        int S = 13; // 12 segmenten
        Transform3DGroup T = new Transform3DGroup();
        List<double> U = new List<double>();

        for (int I = 0; I <= PN; I++)
        {
            P.Add(new Vector2(PA[I].X, PA[I].Y));
            U.Add(PA[I].Z);
        }

        if (W[3] == "")
        {
            B.AddRevolvedGeometry(P, U, O, D, S);
        }
        else
        {
            B.AddRevolvedGeometry(P, null, O, D, S);
        }

        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        FI += 1;
        TX[FI] = V[4];
        TY[FI] = V[5];
        TZ[FI] = V[6];
        RX[FI] = V[7];
        RY[FI] = V[8];
        RZ[FI] = V[9];
        if (W[10] != "") SX[FI] = V[10]; else SX[FI] = 1;
        if (W[11] != "") SY[FI] = V[11]; else SY[FI] = 1;
        if (W[12] != "") SZ[FI] = V[12]; else SZ[FI] = 1;

        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        FI -= 1;
        ModelPart.Transform = T;

        PartsAdd();
    }

    public void PartsPlane() // vlak
    {
        // W[0] = "plane"
        // V[1] = x
        // V[2] = z

        Vector3[] P = new Vector3[4];

        P[0] = new Vector3((float)-V[1] / 2, 0, (float)-V[2] / 2);
        P[1] = new Vector3((float)-V[1] / 2, 0, (float)V[2] / 2);
        P[2] = new Vector3((float)V[1] / 2, 0, (float)V[2] / 2);
        P[3] = new Vector3((float)V[1] / 2, 0, (float)-V[2] / 2);

        MeshBuilder B = new MeshBuilder();

        B.AddQuad(P[0], P[1], P[2], P[3]);

        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        FI += 1;
        TX[FI] = V[3];
        TY[FI] = V[4];
        TZ[FI] = V[5];
        RX[FI] = V[6];
        RY[FI] = V[7];
        RZ[FI] = V[8];
        if (W[9] != "") SX[FI] = V[9]; else SX[FI] = 1;
        if (W[10] != "") SY[FI] = V[10]; else SY[FI] = 1;
        if (W[11] != "") SZ[FI] = V[11]; else SZ[FI] = 1;

        Transform3DGroup T = new Transform3DGroup();
        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        FI -= 1;
        ModelPart.Transform = T;

        PartsAdd();
    }

    public void PartsPyramid()
    {
        // W[0] = "pyramid"
        // V[1] = 
        // V[2] = 

        MeshBuilder B = new MeshBuilder();

        B.AddPyramid(new SharpDX.Vector3(0, (float)V[2] / 3, 0), new SharpDX.Vector3(1, 0, 0), new SharpDX.Vector3(0, 1, 0), V[1], V[2], true);

        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        FI += 1;
        TX[FI] = V[3];
        TY[FI] = V[4];
        TZ[FI] = V[5];
        RX[FI] = V[6];
        RY[FI] = V[7];
        RZ[FI] = V[8];
        if (W[9] != "") SX[FI] = V[9]; else SX[FI] = 1;
        if (W[10] != "") SY[FI] = V[10]; else SY[FI] = 1;
        if (W[11] != "") SZ[FI] = V[11]; else SZ[FI] = 1;

        Transform3DGroup T = new Transform3DGroup();
        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        FI -= 1;
        ModelPart.Transform = T;

        PartsAdd();
    }

    public void PartsQuad() // tekent een vierhoek
    {
        //' W(0) = "quad"
        //' W(1) = x1
        //' enz
        //' W(12) = z4

        MeshBuilder B = new MeshBuilder();
        Vector3[] P = new Vector3[5];
        Transform3DGroup T = new Transform3DGroup();

        P[1] = new Vector3((float)V[1], (float)V[2], (float)V[3]);
        P[2] = new Vector3((float)V[4], (float)V[5], (float)V[6]);
        P[3] = new Vector3((float)V[7], (float)V[8], (float)V[9]);
        P[4] = new Vector3((float)V[10], (float)V[11], (float)V[12]);

        B.AddQuad(P[1], P[2], P[3], P[4]);
        B.CreateNormals = true;
        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }

        ModelPart.Transform = T;
        ModelPart.Name = "quad";

        PartsAdd();
    }

    public void PartsSegment() // tekent een segment
    {
        // W(0) = "segment"
        // V(1) = buiten straal
        // V(2) = binnen straal
        // V(3) = hoogte
        // V(4) = segmenten voor 360°
        // V(5) = hoek
        // V(6) = dx
        // V(7) = dy
        // V(8) = dz
        // V(9) = hx
        // V(10) = hy
        // V(11) = hz

        MeshBuilder B = new MeshBuilder();
        int N;
        SharpDX.Vector3[] P = new SharpDX.Vector3[200];
        Transform3DGroup T = new Transform3DGroup();

        FI += 1;
        TX[FI] = V[6];
        TY[FI] = V[7];
        TZ[FI] = V[8];
        RX[FI] = V[9];
        RY[FI] = V[10];
        RZ[FI] = V[11];

        V[3] /= 2; // hoogte
        if (V[4] == 0) V[4] = 24; // aantal segmenten in 360°
        V[4] = 360 / V[4]; // graden per segment
        if (V[5] == 0) V[5] = 360; // hoek
        N = (int)Math.Abs(V[5] / V[4]); // aantal segmenten voor hoek
        V[4] *= -1;
        for (int I = 0; I <= N; I++) // bereken punten vanaf 12 uur met klok mee
        {
            P[I + 0] = new SharpDX.Vector3((float)DXC(V[2], I * V[4] + 90), (float)V[3], (float)-DYS(V[2], I * V[4] + 90)); // bovenkant binnen
            P[I + 40] = new SharpDX.Vector3((float)DXC(V[2], I * V[4] + 90), (float)-V[3], (float)-DYS(V[2], I * V[4] + 90)); // onderkant binnen
            P[I + 80] = new SharpDX.Vector3((float)DXC(V[1], I * V[4] + 90), (float)-V[3], (float)-DYS(V[1], I * V[4] + 90)); // onderkant buiten
            P[I + 120] = new SharpDX.Vector3((float)DXC(V[1], I * V[4] + 90), (float)V[3], (float)-DYS(V[1], I * V[4] + 90)); // bovenkant buiten
        }
        for (int I = 0; I <= N - 1; I++)
        {
            B.AddQuad(P[I + 120], P[I], P[I + 1], P[I + 121]); // bovenkant
            B.AddQuad(P[I + 81], P[I + 41], P[I + 40], P[I + 80]); // onderkant
            B.AddQuad(P[I + 121], P[I + 81], P[I + 80], P[I + 120]); // buitenkant
            B.AddQuad(P[I + 0], P[I + 40], P[I + 41], P[I + 1]); // binnenkant
        }
        B.AddQuad(P[120], P[80], P[40], P[0]); // voorkant
        B.AddQuad(P[N], P[N + 40], P[N + 80], P[N + 120]); // achterkant
        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;
        for (int I = FI; I > 0; I--)
        {
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        FI -= 1;
        ModelPart.Transform = T;
        PartsAdd();
    }

    public void PartsSlope() // helling
    {
        // W[0] = "slope"
        // V[1] = x breedte
        // V[2] = y hoogte
        // V[3] = z diepte
        // V[4] = x translatie
        // V[5] = y translatie
        // V[6] = z translatie
        // V[7] = x rotatie
        // V[8] = y rotatie
        // V[9] = z rotatie

        for (int I = 1; I <= 3; I++) V[I] /= 2f;

        Vector3[] PV = new Vector3[6];

        PV[0].X = (float)-V[1]; //loa
        PV[0].Y = (float)-V[2];
        PV[0].Z = (float)-V[3];
        PV[1].X = (float)-V[1]; //lov
        PV[1].Y = (float)-V[2];
        PV[1].Z = (float)V[3];
        PV[2].X = (float)V[1]; //rov
        PV[2].Y = (float)-V[2];
        PV[2].Z = (float)V[3];
        PV[3].X = (float)V[1]; //roa
        PV[3].Y = (float)-V[2];
        PV[3].Z = (float)-V[3];
        PV[4].X = (float)-V[1]; //lba
        PV[4].Y = (float)V[2];
        PV[4].Z = (float)-V[3];
        PV[5].X = (float)V[1]; //rba
        PV[5].Y = (float)V[2];
        PV[5].Z = (float)-V[3];

        MeshBuilder B = new MeshBuilder();

        B.AddQuad(PV[3], PV[2], PV[1], PV[0]); // grondvlak
        B.AddQuad(PV[3], PV[0], PV[4], PV[5]); // achtervlak
        B.AddQuad(PV[4], PV[1], PV[2], PV[5]); // schuine bovenlak
        B.AddTriangle(PV[0], PV[1], PV[4]); // links
        B.AddTriangle(PV[2], PV[3], PV[5]); // rechts
                
        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        FI += 1;
        TX[FI] = V[4];
        TY[FI] = V[5];
        TZ[FI] = V[6];
        RX[FI] = V[7];
        RY[FI] = V[8];
        RZ[FI] = V[9];
        if (W[10] != "") SX[FI] = V[10]; else SX[FI] = 1;
        if (W[11] != "") SY[FI] = V[11]; else SY[FI] = 1;
        if (W[12] != "") SZ[FI] = V[12]; else SZ[FI] = 1;

        Transform3DGroup T = new Transform3DGroup();

        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        FI -= 1;
        ModelPart.Transform = T;

        PartsAdd();
    }

    public void PartsSphere()
    {
        MeshBuilder B = new MeshBuilder();
        Transform3DGroup T = new Transform3DGroup();

        if (W[2] == "") V[2] = 32;

        B.AddSphere(new Vector3(0, 0, 0), V[1] / 2.0, (int)V[2], (int)V[2]);
        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        FI += 1;
        TX[FI] = V[3];
        TY[FI] = V[4];
        TZ[FI] = V[5];
        RX[FI] = V[6];
        RY[FI] = V[7];
        RZ[FI] = V[8];
        if (W[9] != "") SX[FI] = V[9]; else SX[FI] = 1;
        if (W[10] != "") SY[FI] = V[10]; else SY[FI] = 1;
        if (W[11] != "") SZ[FI] = V[11]; else SZ[FI] = 1;
        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }
        FI -= 1;
        ModelPart.Transform = T;
        PartsAdd();
    }

    public void PartsTrng() // tekent een ruimtelijke driehoek
    {
        // W(0) = "trng"
        // W(1) = x1
        // enz
        // W(9) = z3

        MeshBuilder B = new MeshBuilder();
        Vector3[] P = new Vector3[4];
        Transform3DGroup T = new Transform3DGroup();

        P[1] = new Vector3((float)V[1], (float)V[2], (float)V[3]);
        P[2] = new Vector3((float)V[4], (float)V[5], (float)V[6]);
        P[3] = new Vector3((float)V[7], (float)V[8], (float)V[9]);

        B.AddTriangle(P[1], P[2], P[3]);
        B.CreateNormals = true;
        ModelPart.Geometry = B.ToMeshGeometry3D();
        ModelPart.Material = PartMat;

        for (int I = FI; I >= 0; I--)
        {
            PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
            PartsTranslate(T, TX[I], TY[I], TZ[I]);
            PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
        }

        ModelPart.Transform = T;
        ModelPart.Name = "trng";
        PartsAdd();
    }

    public void PartsWall()
    {

    }

    public void PartsMove() // initieert verplaatsing van een onderdeel
    {
        // W(0) = "move"
        // V(1) = X
        // V(2) = Y
        // V(3) = Z
        // V(4) = tijd in secondes
        // V(5) = herhalen, 0 = niet, 1 = wel, 2 =  heen en weer

        PMove.X = V[1];
        PMove.Y = V[2];
        PMove.Z = V[3];
        PMove.T = (int)V[4];
        PMove.R = (int)V[5];
    }

    public Transform3DGroup PartMove()
    {
        Transform3DGroup T = new Transform3DGroup();

        if (PMove.T > 0)
        {
            TranslateTransform3D TX = new TranslateTransform3D();
            TranslateTransform3D TY = new TranslateTransform3D();
            TranslateTransform3D TZ = new TranslateTransform3D();

            DoubleAnimation AX = new DoubleAnimation(PMove.X, 0, new System.Windows.Duration(TimeSpan.FromMilliseconds(PMove.T)));
            DoubleAnimation AY = new DoubleAnimation(PMove.Y, 0, new System.Windows.Duration(TimeSpan.FromMilliseconds(PMove.T)));
            DoubleAnimation AZ = new DoubleAnimation(PMove.Z, 0, new System.Windows.Duration(TimeSpan.FromMilliseconds(PMove.T)));

            if (PMove.R > 0)
            {
                AX.RepeatBehavior = RepeatBehavior.Forever;
                AY.RepeatBehavior = RepeatBehavior.Forever;
                AZ.RepeatBehavior = RepeatBehavior.Forever;
                if (PMove.R > 1)
                {
                    AX.AutoReverse = true;
                    AY.AutoReverse = true;
                    AZ.AutoReverse = true;
                }
            }
            TX.BeginAnimation(TranslateTransform3D.OffsetXProperty, AX);
            TY.BeginAnimation(TranslateTransform3D.OffsetYProperty, AY);
            TZ.BeginAnimation(TranslateTransform3D.OffsetZProperty, AZ);
            T.Children.Add(TX);
            T.Children.Add(TY);
            T.Children.Add(TZ);
        }
        return T;
    }

    public void PartsNormal()
    {
        NA[(int)V[1]].X = (float)V[2];
        NA[(int)V[1]].Y = (float)V[3];
        NA[(int)V[1]].Z = (float)V[4];
    }

    public void PartsPoint()
    {
        // W[0] = "point"
        // V[1] = [index of x]
        // V[2] = [x of y]
        // V[3] = [y of z]
        // V[4] = [z]

        if (W[4] != "") // wel index
        {
            PN = (int)V[1];
            PA[PN].X = (float)V[2];
            PA[PN].Y = (float)V[3];
            PA[PN].Z = (float)V[4];
        }
        else
        {
            PN += 1;
            PA[PN].X = (float)V[1];
            PA[PN].Y = (float)V[2];
            PA[PN].Z = (float)V[3];
        }
    }

    public void PartsTexture()
    {
        TA[(int)V[1]].X = (float)V[2];
        TA[(int)V[1]].Y = (float)V[3];
    }

    public void PartsText()
    {
        // W[0] = "text3d"
        // W[1] = tekst

        // uit helix example simpledemo

        //var textBuilder = new MeshBuilder();

        //textBuilder.ExtrudeText("HelixToolkit.SharpDX", "Arial", System.Windows.FontStyles.Normal, System.Windows.FontWeights.Bold,
        //    14, new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        //ModelPart = textBuilder.ToMesh();
    }

    public void PartsRotate(Transform3DGroup T, Point3D P, Single DX, Single DY, Single DZ) // draait een onderdeel
    {
        T.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), DY), P));
        T.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), DX), P));
        T.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), DZ), P));
    }

    public void PartsScale(Transform3DGroup T, Vector3D P)
    {
        T.Children.Add(new ScaleTransform3D(P));
    }

    public void PartsTranslate(Transform3DGroup T, Double X, Double Y, Double Z) // verplaatst een onderdeel
    {
        T.Children.Add(new TranslateTransform3D(X, Y, Z));
    }

    public void PartsTurn()
    {
        for (int I = 0; I <= FI; I++)
        {
            V[1] += TX[I];
            V[2] += TY[I];
            V[3] += TZ[I];
        }
        PTurn.CX = V[1];
        PTurn.CY = V[2];
        PTurn.CZ = V[3];
        PTurn.B = V[4];
        PTurn.E = V[5];
        PTurn.AX = V[6];
        PTurn.AY = V[7];
        PTurn.AZ = V[8];
        PTurn.T = (int)V[9];
        PTurn.R = (int)V[10];
        if (W[11] == "")
        {
            PTurn.K = false;
            PartTransform.Children.Clear();
        }
        else
        {
            PTurn.K = true;
        }

    }

    public Transform3DGroup PartTurn()
    {
        Transform3DGroup T = new Transform3DGroup();

        if (PTurn.T > 0)
        {
            DoubleAnimation A = new DoubleAnimation();
            RotateTransform3D R = new RotateTransform3D();

            A.From = PTurn.B;
            A.To = PTurn.E;
            A.Duration = TimeSpan.FromSeconds(PTurn.T);
            if (PTurn.R > 0)
            {
                A.RepeatBehavior = RepeatBehavior.Forever;
                if (PTurn.R > 1)
                {
                    A.AutoReverse = true;
                }
            }
            R.CenterX = PTurn.CX;
            R.CenterY = PTurn.CY;
            R.CenterZ = PTurn.CZ;
            R.Rotation = new AxisAngleRotation3D(new Vector3D(PTurn.AX, PTurn.AY, PTurn.AZ), 0);
            R.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, A);

            //int N = PartTransform.Children.Count;

            if (PTurn.K)
            {
                T.Children.Add(R);
                T.Children.Add(PartTransform);
            }
            else
            {
                T.Children.Add(R);
                PartTransform.Children.Clear();
                PartTransform.Children.Add(R);
            }
        }

        return T;
    }

    public static ModsPrts modsPrts = new ModsPrts(); // het onderdelen object
}