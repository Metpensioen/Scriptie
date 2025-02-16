using HelixToolkit.Wpf.SharpDX;

using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;

using static ModsPrts;
using static PartMats;
using static TabsFile;
using static TextFunctions;
using static TextParser;

class PartLego
{
    public struct TLegoParts
    {
        public string Name;
        public int Numb;
    }

    public static Boolean[] INV = new Boolean[20];
    public static int MI; // matrix index
    public static Boolean[] REV = new Boolean[20];
    public static Matrix3D[] ML = new Matrix3D[20];
    public static MeshBuilder PartBuilder;
    public bool LegoGide = false;
    public bool LegoData = false;
    public static TLegoParts[] LegoParts = new TLegoParts[1000];

    public void LegoInit()
    {
        PartBuilder = new MeshBuilder();
        StepHeight = 0;
        LegoGide = false;
        LegoData = false;
        MI = 0;
        for (int I = 0; I < 20; I++)
        {
            INV[I] = false;
            REV[I] = false;
        }
        for (int J = 0; J < 1000; J++)
        {
            LegoParts[J].Name = "";
            LegoParts[J].Numb = 0;
        }
    }

    public void LegoDone()
    {
        if (PartBuilder != null)
        {
            if (PartBuilder.ToMeshGeometry3D().Positions.Count > 0)
            {
                modsPrts.ModelPart.Geometry = PartBuilder.ToMeshGeometry3D();
                modsPrts.ModelPart.Material = PartMat;

                Transform3DGroup T = new Transform3DGroup();

                FI += 1;

                for (int I = FI; I >= 0; I--)
                {
                    modsPrts.PartsScale(T, new Vector3D(SX[I], SY[I], SZ[I]));
                    modsPrts.PartsTranslate(T, TX[I], TY[I], TZ[I]);
                    modsPrts.PartsRotate(T, new Point3D(TX[I], TY[I], TZ[I]), (float)RX[I], (float)RY[I], (float)RZ[I]);
                }

                FI -= 1;
                modsPrts.ModelPart.Transform = T;
                modsPrts.PartsAdd();

                PartBuilder = new MeshBuilder();
            }
        }
    }

    public void LegoCalc(int N)
    {
        Point3D P = new Point3D(PA[N].X, PA[N].Y, PA[N].Z);

        for (int I = MI; I >= 0; I--)
        {
            P = Point3D.Multiply(P, ML[I]);
        }

        PA[N].X = (float)(TX[0] + LegoSize(P.X));
        PA[N].Y = (float)(TY[0] + LegoSize(P.Y));
        PA[N].Z = (float)(TZ[0] + LegoSize(P.Z));
    }

    public void LegoFace(int P1, int P2, int P3, int P4)
    {
        if (P4 == 0)
        {
            PartBuilder.AddTriangle(PA[P1], PA[P2], PA[P3]);
        }
        else
        {
            PartBuilder.AddQuad(PA[P1], PA[P2], PA[P3], PA[P4]);
        }
    }

    public void LegoComment() // commentaar
    {
        // W[0] = "0"

        if (W[1] == "BFC")
        {
            if (W[2] == "INVERTNEXT")
            {
                INV[MI] = true;
            }
            else if (W[2] == "CERTIFY")
            {
                if (W[3] == "CW")
                {
                    if (INV[MI])
                    {
                        REV[MI] = false;
                    }
                    else
                    {
                        REV[MI] = true;
                    }
                }
                else if (W[3] == "CCW")
                {
                    if (INV[MI])
                    {
                        REV[MI] = true;
                    }
                    else
                    {
                        REV[MI] = false;
                    }
                }
            }
        }
    }

    public void LegoFile()
    {
        // V0] = 1

        MI += 1;

        ML[MI].OffsetX = V[2];
        ML[MI].OffsetY = V[3];
        ML[MI].OffsetZ = V[4];
        ML[MI].M11 = V[5];
        ML[MI].M21 = V[6];
        ML[MI].M31 = V[7];
        ML[MI].M12 = V[8];
        ML[MI].M22 = V[9];
        ML[MI].M32 = V[10];
        ML[MI].M13 = V[11];
        ML[MI].M23 = V[12];
        ML[MI].M33 = V[13];
        ML[MI].M14 = 0;
        ML[MI].M24 = 0;
        ML[MI].M34 = 0;
        ML[MI].M44 = 1;

        INV[MI] = INV[MI - 1];
        string F = W[14];
        parserFile[FI] = @"d:\data\dat\used\parts\" + F;

        if (!File.Exists(parserFile[FI]))
        {
            string B = @"d:\data\dat\parts\" + F;

            if (File.Exists(B))
            {
                File.Copy(B, parserFile[FI]);
            }
            else
            {
                parserFile[FI] = @"d:\data\dat\p\" + F;
            
                if (!File.Exists(parserFile[FI]))
                {
                    B = @"\d:\data\dat\p\" + F;
                    if (File.Exists(B))
                    {
                        File.Copy(B, parserFile[FI]);
                    }
                }
            }
            
            if (!File.Exists(parserFile[FI]))
            {
                MessageBox.Show(parserFile[FI] + " niet gevonden");
                parserRuns = false;
                return;
            }
        }

        textParser.ParserOpen(parserFile[FI], 0, -1);

        if (MI > 1)
        {
            MI -= 1;
            INV[MI] = INV[MI - 1];
        }
    }

    public void LegoTriangle()
    {
        // V[0] = 3

        for (int I = 1; I <= 3; I++)
        {
            PA[I].X = (float)V[I * 3 - 1];
            PA[I].Y = (float)V[I * 3 + 0];
            PA[I].Z = (float)V[I * 3 + 1];
            LegoCalc(I);
        }

        double D = ML[0].Determinant;
        for (int I = 1; I <= MI; I++)
        {
            D *= ML[I].Determinant;
        }

        if (!REV[MI])
        {
            if (D > 0)
            {
                LegoFace(1, 2, 3, 0);
            }
            else
            {
                LegoFace(3, 2, 1, 0);
            }
        }
        else
        {
            if (D > 0)
            {
                LegoFace(3, 2, 1, 0);
            }
            else
            {
                LegoFace(1, 2, 3, 0);
            }
        }

        if (W[14] != "") LegoDone();
    }

    public void LegoRectangle()
    {
        // V[0] = 4
        // V[1] = kleurnummer
        // V[2] = X
        // V[3] = Y
        // V[5] = Z

        for (int I = 1; I <= 4; I++)
        {
            PA[I].X = (float)V[I * 3 - 1]; // 2, 5,  8, 11
            PA[I].Y = (float)V[I * 3 + 0]; // 3, 6,  9, 12
            PA[I].Z = (float)V[I * 3 + 1]; // 4, 7, 10, 13
            LegoCalc(I);
        }

        double D = ML[0].Determinant;
        for (int I = 1; I <= MI; I++)
        {
            D *= ML[I].Determinant;
        }

        if (!REV[MI])
        {
            if (D > 0)
            {
                LegoFace(1, 2, 3, 4);
            }
            else
            {
                LegoFace(4, 3, 2, 1);
            }
        }
        else
        {
            if (D > 0)
            {
                LegoFace(4, 3, 2, 1);
            }
            else
            {
                LegoFace(1, 2, 3, 4);
            }
        }

        if (W[14] != "") LegoDone();
    }

    public void LegoAdd()
    {
        int I;
        string S = FileName(parserFile[FI]);

        if (S.Contains("part-")) return;
        S = GetFilePath(parserFile[FI]);
        string T = FilePrev(S);
        string P = FilePath(parserFile[FI - 1]);
        if (P.Contains(T)) S = P;
        S += PartMat.Name; // voeg de kleur toe
        S = S.Substring(2); // verwijder de eerste letter

        for (I = 0; I < 1000; I++)
        {
            if (LegoParts[I].Name == S)
            {
                LegoParts[I].Numb++;
                return;
            }
        }
        I = 0;
        while (LegoParts[I].Name != "") I++;
        LegoParts[I].Name = S;
        LegoParts[I].Numb = 1;
    }

    public void LegoList() // maakt een lijst met de onderdel
    {
        string R = FileRoot();
        string F = R + GetFilePath(parserFile[FI]) + "\\list.txt";
        string S = "onderdeel, aantal \r\n\r\n";
        int N = 0;

        int I = 0;

        while (LegoParts[I].Name != "") // zolang er onderdelen zijn
        {
            S += LegoParts[I].Name + ", " + LegoParts[I].Numb.ToString() + "\r\n";
            N += LegoParts[I].Numb;
            I++; // volgende onderdeel
        }
        S = S + "\r\n" + "totaal, " + FSL(N, 3, ' ');
        File.WriteAllText(F, S); // schrijf de onderdelen gegevens naar het bestand
        for (int J = 0; J < 1000; J++)
        {
            LegoParts[J].Name = "";
            LegoParts[J].Numb = 0;
        }
    }

    public void LegoPart() // voeg lego onderdeel aan model toe
    {
        // W[0] = "lego"
        // W[1] = // onderdeel naam: \type\afmeting\mod\kleur

        string S = W[1]; 
        int I = S.LastIndexOf(@"\") + 1; // laatste \ 

        W[11] = S.Substring(I).Trim(); // onderdeel kleur
        W[1] = S.Substring(0, I) + "part.bin"; // onderdeel bestand
        textParser.ParserFiles();
    }

    public Double LegoSize(Double R)
    {
        if (R > 0.1) R = (R - 0.1) / 1000; else R = (R + 0.1) / 1000;

        return R;
    }

    public static PartLego partLego = new PartLego();
}