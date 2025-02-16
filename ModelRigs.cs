using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Animations;
using HelixToolkit.Wpf.SharpDX.Assimp;

using SharpDX;

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;

using static RoomCamera;
using static RoomMods;
using static ModsPrts;
using static PartBins;
using static TabsCalc;
using static TabsFile;
using static TabsRigs;
using static TabsRoom;
using static TextFunctions;
using static TextParser;
using static ViewRoom;

class ModsRigs
{
    public static HelixToolkitScene[] ModelRig = new HelixToolkitScene[4];
    public static int RI; // rig index

    public struct TPosed
    {
        public int B; // bot index
        public int R; // rig index
        public int T; // type
    }

    public void RigsInit()
    {
        RI = 0;
        AE = false;
        for (int I = 0; I < 4; I++) QC[I] = 1;
        Posed.B = 0;
        Posed.R = 0;
        Posed.T = 0;
        SE = false;

        RigsTransformer.Children.Clear();
        RigsTransformer.Children.Add(new RotateTransform3D());
        RigsTransformer.Children.Add(new TranslateTransform3D());
        RigsTransformer.Children.Add(new TranslateTransform3D());
        RigsTransformer.Children.Add(new TranslateTransform3D());
        RigsTransformer.Children.Add(new ScaleTransform3D());

        RigsPoser.Rendering += RigsPoser_Rendering;
        RigsTransformer.Changed += RigsTransformer_Changed;
    }

    public void RigsAnimate() // beweging starten
    {
        // W[0] = "animate"
        // V[1] = [rigindex]
        // V[2] = [animatieindex]
        // V[3] = [quaternioncorrectie]
        // W[4] = [wis]

        int R = RI;
        int A = 0;
        if (W[1] != "") R = (int)V[1];
        if (W[2] != "") A = (int)V[2];
        if (W[3] != "") QC[R] = (int)V[3]; else QC[R] = 1;
        if (ModelRig[R] != null)
        {
            if (!ModelRig[R].HasAnimation)
            {
                MessageBox.Show("Rig is niet beweegbaar");
                parserRuns = false;
                return;
            }

            if (ModelRig[R].Animations[A] != null)
            {
                RigsUpdt[R] = new NodeAnimationUpdater(ModelRig[R].Animations[A]);
            }

            if (W[4] != "")
            {
                Vector3 T = new Vector3();
                SharpDX.Quaternion Q = new SharpDX.Quaternion();

                RigsUpdt[R].Animation.StartTime = 0;
                RigsUpdt[R].Animation.EndTime = 2;

                int L;
                int N = RigsUpdt[R].Animation.NodeAnimationCollection.Count;
                Keyframe K = new Keyframe();
                for (int I = 0; I < N; I++) // voor ieder bot
                {
                    L = 5;
                    MM[I] = RigsUpdt[R].Animation.NodeAnimationCollection[I].Node.ModelMatrix;
                    RigsUpdt[R].Animation.NodeAnimationCollection[I].KeyFrames.Clear();

                    if (V[4] == 0)
                    {
                        T = new Vector3(0, 0, 0);
                        Q = new SharpDX.Quaternion(0, 0, 0, 1);
                    }
                    else if (V[4] == 1)
                    {
                        T = MM[I].TranslationVector;
                        Q = new SharpDX.Quaternion(0, 0, 0, 1);
                    }
                    else if (V[4] == 2)
                    {
                        T = new Vector3(0, 0, 0);
                        Q = MatrixToQuaternion(MM[I]);
                    }
                    else if (V[4] == 3)
                    {
                        T = MM[I].TranslationVector;
                        Q = MatrixToQuaternion(MM[I]);
                    }

                    for (int J = 0; J < L; J++)
                    {
                        K.BoneIndex = I;
                        K.Translation = T;
                        K.Rotation = Q;
                        K.Rotation.W *= QC[R];
                        K.Scale = new Vector3(1, 1, 1);
                        K.Time = J * 0.5f;
                        RigsUpdt[R].Animation.NodeAnimationCollection[I].KeyFrames.Add(K);
                    }
                }
            }

            AE = true;
        }
    } // Animate

    public void RigsBone() // importeer bot
    {
        // W[0] = "bone"
        // V[1] = index
        // W[2] = oude naam
        // W[3] = nieuwe naam


        RigsUpdt[RI].Animation.NodeAnimationCollection[(int)V[1]].Node.Name = W[3];
    }

    public void RigsBones() // exporteer botten
    {
        // W[0] = "bones"
        // W[1] = [bestandnaam]
        // W[2] = [type]

        int I;
        int N;

        tabsRigs.BoneList.Items.Clear();

        if (W[2] == "")
        {
            try
            {
                N = ModelRig[RI].Animations[0].NodeAnimationCollection.Count;
            }
            catch
            {
                return;
            }
            for (I = 0; I < N; I++)
            {
                tabsRigs.BoneList.Items.Add(I + ", " + ModelRig[RI].Animations[0].NodeAnimationCollection[I].Node.Name);
            }
        }
        else
        {
            I = 0;
            foreach (var Node in ModelRig[RI].Root.Traverse(true))
            {
                if (Node.IsAnimationNode)
                {
                    tabsRigs.BoneList.Items.Add(I + ", " + Node.Name);
                    I += 1;
                }
            }
        }
    }

    public void RigsExport() // exporteerd onderdelen of rigs
    {
        // W[0] = "export"
        // W[1] = bestandnaam
        // W[2] = [objecten]
        // W[3] = [schaal]

        string E;
        if (W[1] == "") W[1] = ModelFile;
        string S = tabsFile.FileSave(W[1]);
        string T = FileType(S);

        switch (T)
        {
            case ".bin":
                partBins.BinSave(S);
                return;
            case ".dae": T = "collada"; break;
            case ".obj": T = "obj"; break;
            case ".objnomtl": S = S.Substring(0, S.Length - 5); T = "objnomtl"; break;
        }

        HelixToolkit.Wpf.SharpDX.Assimp.Exporter rigExporter = new HelixToolkit.Wpf.SharpDX.Assimp.Exporter();

        if (ModelRig[RI] != null)
        {
            E = rigExporter.ExportToFile(S, ModelRig[RI], T).ToString();
        }
        else
        {
            E = rigExporter.ExportToFile(S, RoomMod.SceneNode, T).ToString();
        }

        if (E != "Succeed") MessageBox.Show(E);
    }

    public int RigsFindBone(string S)
    {
        int N;
        N = RigsUpdt[RI].Animation.NodeAnimationCollection.Count; // aantal botten
        int I = 0;
        while (I < N && RigsUpdt[RI].Animation.NodeAnimationCollection[I].Node.Name != S)
        {
            I += 1;
        }

        if (I == N)
        {
            MessageBox.Show("bot " + S + " miet gevonden");
            parserRuns = false;
            I = 0;
        }

        return I;
    }

    public void RigsKeyFrame() // importeert een keyframe
    {
        // W[0] = "keyframe"
        // W[1] = botnaam
        // V[2] = keyframeindex
        // V[3] = TX

        Keyframe K = new Keyframe();
        int N; //  = RigsUpdt[RI].Animation.NodeAnimationCollection.Count;
        int I = (int)V[2];

        K.BoneIndex = RigsFindBone(W[1]);
        // SharpDX.Matrix X = RigsUpdt[RI].Animation.NodeAnimationCollection[K.BoneIndex].Node.ModelMatrix;
        // K.Translation = X.TranslationVector + new Vector3(V[3], V[4], V[5]);
        K.Translation = new Vector3((float)V[3], (float)V[4], (float)V[5]);
        K.Rotation = new SharpDX.Quaternion((float)V[6], (float)V[7], (float)V[8], (float)V[9]);
        K.Scale = new Vector3((float)V[10], (float)V[11], (float)V[12]);
        K.Time = (float)V[13];
        N = RigsUpdt[RI].Animation.NodeAnimationCollection[K.BoneIndex].KeyFrames.Count;
        if (I < N)
        {
            RigsUpdt[RI].Animation.NodeAnimationCollection[K.BoneIndex].KeyFrames[I] = K;
        }
        else
        {
            RigsUpdt[RI].Animation.NodeAnimationCollection[K.BoneIndex].KeyFrames.Add(K);
        }
    }

    public void RigsKeyFrames() // exporteert keyframes
    {
        V[3] = RigsUpdt[RI].Animation.StartTime;
        V[4] = RigsUpdt[RI].Animation.EndTime;

        string F = "' " + DateTime.Now + "\r\n" + "keytimes, " + V[3] + ", " + V[4] + "\r\n";
        string B; // botnaam
        Keyframe K; // keyframe
        int L; // aantal keyframes
        Vector3 T;
        SharpDX.Quaternion R;
        Vector3 S;
        int N = RigsUpdt[RI].Animation.NodeAnimationCollection.Count;

        for (int I = 0; I < N; I++)
        {
            B = RigsUpdt[RI].Animation.NodeAnimationCollection[I].Node.Name;
            L = RigsUpdt[RI].Animation.NodeAnimationCollection[I].KeyFrames.Count;
            for (int J = 0; J < L; J++)
            {
                K = RigsUpdt[RI].Animation.NodeAnimationCollection[I].KeyFrames[J];
                T = K.Translation;
                R = K.Rotation;
                S = K.Scale;
                F += "keyframe, " + B + SL(J.ToString(), ' ', 3) +
                    FL(T.X) + FL(T.Y) + FL(T.Z) +
                    FL(R.X) + FL(R.Y) + FL(R.Z) + FL(R.W) +
                    FL(S.X) + FL(S.Y) + FL(S.Z) + FL(K.Time) + "\r\n";
            }
        }

        File.WriteAllText(GetFileAddress(W[1]), F);
    }

    public void RigsLoad(string S)
    {
        W[1] = S;

        if (RI > 0) if (parserRuns) textParser.ParserFiles();
    }

    public void RigsMerg() // combineer model
    {
        // W[0] = "merge"
        // W[1] = bestandnaam

        string E;
        string S = GetFileAddress(W[1]);

        if (File.Exists(S))
        {
            using (Importer RigImporter = new Importer())
            {
                E = RigImporter.Load(S, out ModelRig[0]).ToString();
            }
            if (E == "Succeed")
            {
                if (ModelRig[0].HasAnimation)
                {
                    ModelRig[RI].Animations = ModelRig[0].Animations;
                }
                else
                {
                    RoomMod.AddNode(ModelRig[0].Root);
                    ModelRig[RI].Root = (HelixToolkit.Wpf.SharpDX.Model.Scene.SceneNode)RoomMod;
                }
            }
            else
            {
                MessageBox.Show(E);
            }
        }
        else
        {
            MessageBox.Show("bestand " + W[1] + " niet gevonden");
        }
    }

    public void RigsNavi() // Start navigeren
    {
        // W(0) = "navigate"
        // V(1) = [model index]
        // V(2) = [naar x als absolute waarde]
        // V(3) = [naar y]
        // V(4) = [naar z]
        // V(5) = [camera y-as hoek]
        // V(6) = [snelheid (m/s)]
        // V(7) = [cameraschaal]

        Point3D D = new Point3D(); // delta afstand
        Point3D M; // model positie

        // CS = ViewRoom.Camera.Position.Z / 2.4f; // camera schaal

        if (V[1] > 0) // als een model moet navigeren
        {
            if (W[2] == "") //  geen bestemming
            {
                MA = 0;
                MT = 1.2; // keyframe tijd in secondes
                MZ = 1.2 * CS; // verplaatsing langs z as
                modsRigs.RigsLoad(IdleFile);
            }
            else // model heeft bestemming
            {
                M = ModelRig[(int)V[1]].Root.ModelMatrix.TranslationVector.ToPoint3D();
                D.X = V[2] - M.X; // model is in M en moet D overbruggen
                D.Y = V[3] - M.Y; // hoogte gebeurd nog niets mee
                D.Z = V[4] - M.Z; //
                if (V[5] != 0) CA = V[5]; // camera hoek
                MA = 0; //' MA = Math.Atan(D.X / D.Z) * (180 / Math.PI) // bereken model hoek
                if (D.Z < 0) MA += 180;
                MZ = Math.Pow(Math.Pow(D.X, 2) + Math.Pow(D.Z, 2), 0.5f) * CS; // navigeer afstand langs z as
                if (V[6] == 0) V[6] = 1.38f; // snelheid in m/s
                MT = MZ / V[6]; // tijd
                modsRigs.RigsLoad(IdleFile);
                //ModelRigs.View();
                // Walk();
            }
        }
        else // navigeert alleen de camera
        {
            if (W[2] == "") // geen bestemming
            {
                MA = 0;
                MT = 1.2;
                //MY = 0;
                //MA = 180;
            }
            else // wel bestemming
            {
                MY = V[3] / 2;
                MZ = V[4];
                MT = 1.2;
                roomCamera.Position = new Point3D(0, MY, 0);
                roomCamera.LookDirection = new Vector3D(0, -MY / 3, -MZ);
                // CS = V[2] / 2.4;
            }
        }
        Navigates = true;
        tabsRoom.IsSelected = true;
        viewRoom.Focus();
    }

    public void RigsOpen(string S)
    {
        if (File.Exists(S))
        {
            string E;
            Transform3DGroup T = new Transform3DGroup();
            Importer RigImporter = new Importer();

            ModelFile = S; // onthoud bestandnaam voor eventuele uitgestelde export
            RI += 1;
            E = RigImporter.Load(S, out ModelRig[RI]).ToString();

            if (E == "Succeed")
            {
                for (int I = FI; I > 0; I--)
                {
                    modsPrts.PartsTranslate(T, TX[I], TY[I], TZ[I]);
                    modsPrts.PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
                    modsPrts.PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
                }

                ModelRig[RI].Root.ModelMatrix = T.ToMatrix();

                RoomMod.AddNode(ModelRig[RI].Root);
            }
            else
            {
                MessageBox.Show(E);
                parserRuns = false;
            }
        }
        else
        {
            MessageBox.Show("bestand " + S + " niet gevonden");
            parserRuns = false;
        }
    }

    public void RigsPose() // zet bot
    {
        // W[0] = "pose"
        // V[1] = rigindex
        // W[2] = botindex of naam
        // V[3] = posetype
        // V[4] = TX
        // V[5] = TY
        // V[6] = TZ
        // V[7] = RX
        // V[8] = RY
        // V[9] = RZ
        // V[10] = SX
        // V[11] = SY
        // V[12] = SZ

        SE = false;
        Posed.R = (int)V[1];
        if (W[2] == V[2].ToString()) Posed.B = (int)V[2]; else Posed.B = RigsFindBone(W[2]);
        Posed.T = (int)V[3];
        if (W[10] == "") V[10] = 1;
        if (W[11] == "") V[11] = 1;
        if (W[12] == "") V[12] = 1;

        tabsRigs.TXSlider.Value = V[4];
        tabsRigs.TYSlider.Value = V[5];
        tabsRigs.TZSlider.Value = V[6];
        tabsRigs.RXSlider.Value = V[7];
        tabsRigs.RYSlider.Value = V[8];
        tabsRigs.RZSlider.Value = V[9];
        tabsRigs.SXSlider.Value = V[10];
        tabsRigs.SYSlider.Value = V[11];
        tabsRigs.SZSlider.Value = V[12];

        SE = true;
        PoseBone();
    }

    public void RigsPoses() // houding exporteren
    {
        // W[0] = "poses"
        // W[1] = bestandnaam

        if (RigsUpdt[RI] == null) return;

        int N = RigsUpdt[RI].Animation.NodeAnimationCollection.Count; // aantal botten
        string B; // botnaam
        Keyframe K;
        Vector3 R;
        Vector3 T;
        Vector3 S;
        SharpDX.Matrix M;
        string F;

        F = "' " + DateTime.Now + "\n";

        for (int I = 0; I < N; I++) // voor alle botten
        {
            M = RigsUpdt[RI].Animation.NodeAnimationCollection[I].Node.ModelMatrix;
            B = RigsUpdt[RI].Animation.NodeAnimationCollection[I].Node.Name;
            K = RigsUpdt[RI].Animation.NodeAnimationCollection[I].KeyFrames[0];
            T = K.Translation - M.TranslationVector;
            T *= 0.02f;
            R = QuaternionToEuler(MatrixToQuaternion(M));
            R = QuaternionToEuler(K.Rotation) - R;
            S = K.Scale;
            F += "pose, " + RI + ", " + SR(B + ", ", ' ', 16) + "12" + FL(T.X) + FL(T.Y) + FL(T.Z) + FL(R.X) + FL(R.Y) + FL(R.Z) + FL(S.X) + FL(S.Y) + FL(S.Z) + "\n";
        }

        File.WriteAllText(GetFileAddress(W[1]), F);
    }

    public void RigsTime()
    {
        if (W[1] != "") RigsUpdt[RI].Animation.StartTime = (float)V[1];
        if (W[2] != "") RigsUpdt[RI].Animation.EndTime = (float)V[2];
        //if (W[3] != "") RigsUpdt[RI].Speed = (float)V[3];
    }

    public void PoseBone()
    {
        if (SE)
        {
            Vector3 T = new Vector3((float)tabsRigs.TXSlider.Value, (float)tabsRigs.TYSlider.Value, (float)tabsRigs.TZSlider.Value);
            Vector3 R = new Vector3((float)tabsRigs.RXSlider.Value, (float)tabsRigs.RYSlider.Value, (float)tabsRigs.RZSlider.Value);
            SharpDX.Quaternion Q = EulerToQuaternion(R);
            Vector3 S = new Vector3((float)tabsRigs.SXSlider.Value, (float)tabsRigs.SYSlider.Value, (float)tabsRigs.SZSlider.Value);
            string N = RigsUpdt[Posed.R].Animation.NodeAnimationCollection[Posed.B].Node.Name;
            int J = RigsUpdt[Posed.R].Animation.NodeAnimationCollection[Posed.B].KeyFrames.Count;
            Keyframe K = new Keyframe();
            for (int I = 0; I < 5; I++)
            {
                K.BoneIndex = Posed.B;
                K.Translation = MM[Posed.B].TranslationVector + T;
                K.Rotation = Q;
                K.Scale = S;
                K.Time = I * 0.5f;
                if (I >= J)
                    RigsUpdt[Posed.R].Animation.NodeAnimationCollection[Posed.B].KeyFrames.Add(K);
                else
                    RigsUpdt[Posed.R].Animation.NodeAnimationCollection[Posed.B].KeyFrames[I] = K;
            }
            tabsRigs.PoseText.Text = "pose, " + Posed.R + ", " + N + ", " + Posed.T +
                FL((float)tabsRigs.TXSlider.Value) + FL((float)tabsRigs.TYSlider.Value) + FL((float)tabsRigs.TZSlider.Value) +
                FL((float)tabsRigs.RXSlider.Value) + FL((float)tabsRigs.RYSlider.Value) + FL((float)tabsRigs.RZSlider.Value) +
                FL((float)tabsRigs.SXSlider.Value) + FL((float)tabsRigs.SYSlider.Value) + FL((float)tabsRigs.SZSlider.Value);
        }
    }

    public void RigsPoser_Rendering(object sender, System.Windows.Media.RenderingEventArgs e)
    {
        if (AE)
        {
            for (int I = 0; I < 4; I++)
            {
                if (RigsUpdt[I] != null)
                {
                    try
                    {
                        RigsUpdt[I].Update(Stopwatch.GetTimestamp(), Stopwatch.Frequency);
                    }
                    catch
                    {

                    }
                }
            }
        }
    }

    public void RigsTransformer_Changed(object sender, EventArgs e)
    {
        if (Navigates && KeyNumb != 0)
        {
            SharpDX.Matrix M = RigsTransformer.ToMatrix();

            if (RigsUpdt[RI] != null)
            {
                ModelRig[RI].Root.ModelMatrix = M;
            }

            MP = M.TranslationVector.ToVector3D();
            roomCamera.CameraMove();
        }
    }

    public static ModsRigs modsRigs = new ModsRigs();

    // 

    public static SharpDX.Quaternion EulerToQuaternion(Vector3 H)
    {
        SharpDX.Quaternion Q;

        float cx = (float)Math.Cos(Rad(H.X) / 2);
        float sx = (float)Math.Sin(Rad(H.X) / 2);
        float cy = (float)Math.Cos(Rad(H.Y) / 2);
        float sy = (float)Math.Sin(Rad(H.Y) / 2);
        float cz = (float)Math.Cos(Rad(H.Z) / 2);
        float sz = (float)Math.Sin(Rad(H.Z) / 2);

        Q.W = cy * cz * cx - sy * sz * sx;
        Q.X = cy * cz * sx + sy * sz * cx;
        Q.Y = sy * cz * cx + cy * sz * sx;
        Q.Z = cy * sz * cx - sy * cz * sx;

        return Q;
    }

    public static Vector3 QuaternionToEuler(SharpDX.Quaternion Q)
    {
        Vector3 V;
        double Sqw = Q.W * Q.W;
        double Sqx = Q.X * Q.X;
        double Sqy = Q.Y * Q.Y;
        double Sqz = Q.Z * Q.Z;
        double Unit = Sqx + Sqy + Sqz + Sqw;
        double Test = Q.X * Q.Y + Q.Z * Q.W;

        if (Test > 0.499 * Unit)
        {
            V.X = 0;
            V.Y = (float)(2 * Math.Atan2(Q.X, Q.W));
            V.Z = (float)Math.PI / 2;
        }
        else if (Test < -0.499 * Unit)
        {
            V.X = 0;
            V.Y = (float)(-2 * Math.Atan2(Q.X, Q.W));
            V.Z = (float)-Math.PI / 2;
        }
        else
        {
            V.X = (float)(Math.Atan2(2 * Q.X * Q.W - 2 * Q.Y * Q.Z, -Sqx + Sqy - Sqz + Sqw));
            V.Y = (float)(Math.Atan2(2 * Q.Y * Q.W - 2 * Q.X * Q.Z, Sqx - Sqy - Sqz + Sqw));
            V.Z = (float)(Math.Asin(2 * Test / Unit));
        }
        V.X = (float)Deg(V.X);
        V.Y = (float)Deg(V.Y);
        V.Z = (float)Deg(V.Z);

        return V;
    }

    public static SharpDX.Quaternion MatrixToQuaternion(SharpDX.Matrix X)
    {
        SharpDX.Quaternion Q;
        double T;
        double S;

        T = X.M11 + X.M22 + X.M33;

        if (T > 0)
        {
            S = Math.Sqrt(T + 1) * 2; // S=4*qw 
            Q.W = (float)(0.25 * S);
            Q.X = (float)((X.M32 - X.M23) / S);
            Q.Y = (float)((X.M13 - X.M31) / S);
            Q.Z = (float)((X.M21 - X.M12) / S);
        }
        else if ((X.M11 > X.M22) && (X.M11 > X.M33))
        {
            S = Math.Sqrt(1 + X.M11 - X.M22 - X.M33) * 2; // S=4*qx 
            Q.W = (float)((X.M32 - X.M23) / S);
            Q.X = (float)(0.25 * S);
            Q.Y = (float)((X.M12 + X.M21) / S);
            Q.Z = (float)((X.M13 + X.M31) / S);
        }
        else if (X.M22 > X.M33)
        {
            S = Math.Sqrt(1 + X.M22 - X.M11 - X.M33) * 2; // S=4*qy
            Q.W = (float)((X.M13 - X.M31) / S);
            Q.X = (float)((X.M12 + X.M21) / S);
            Q.Y = (float)(0.25 * S);
            Q.Z = (float)((X.M23 + X.M32) / S);
        }
        else
        {
            S = Math.Sqrt(1 + X.M33 - X.M11 - X.M22) * 2; // S=4*qz
            Q.W = (float)((X.M21 - X.M12) / S);
            Q.X = (float)((X.M13 + X.M31) / S);
            Q.Y = (float)((X.M23 + X.M32) / S);
            Q.Z = (float)(0.25 * S);
        }

        return Q;
    }
}