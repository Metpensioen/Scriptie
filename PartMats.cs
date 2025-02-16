using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;

using SharpDX;
using System;
using System.IO;
using System.Windows;

using static RoomMods;
using static TabsFile;
using static TextParser;

class PartMats
{
    public static PhongMaterialCore PartMat;

    public void MatsInit()
    {
        PartMat = new PhongMaterialCore()
        { 
        AmbientColor = new Color4(1, 1, 1, 1),
        DiffuseColor = new Color4(1, 1, 1, 1),
        EmissiveColor = new Color4(0, 0, 0, 1),
        ReflectiveColor = new Color4(0, 0, 0, 1),
        SpecularColor = new Color4(0, 0, 0, 1),
        SpecularShininess = 100,
        Name = "",

        //DisplacementMapScaleMask = new Vector4(0.2F, 0.2F, 0.2F, 0),
        //EnableTessellation = true,
        //MaxDistanceTessellationFactor = 1,
        //MinDistanceTessellationFactor = 3,
        //MaxTessellationDistance = 450,
        //EnableAutoTangent = true
        };
    }

    public void MatsExport() // materialen exporteren
    {
        // W[0] = "materials"
        // W[1] = bestandnaam
        // W[2] = submap "ds", "mh",

        string D;
        string F;
        string L = "";
        string O;
        string P;
        string S = GetFileAddress(W[1]);
        string T;
        int I = 0;
        int J;
        int K;
        foreach (var Node in RoomMod.SceneNode.Items.Traverse(true))
        {
            if (Node is MeshNode N)
            {
                I += 1;
                PhongMaterialCore M = (PhongMaterialCore)N.Material;
                D = M.DiffuseMapFilePath;
                J = D.LastIndexOf(@"\");
                K = D.LastIndexOf(@"/");
                if (J < K) J = K;
                F = D.Substring(J + 1);
                T = FileType(F).Substring(1);
                P = @"d:\data\" + T + @"\" + W[2] + @"\" + N.Name + "." + T;
                O = M.NormalMapFilePath;
                if (O != null)
                {
                    T = FileType(O);
                    O = @"d:\data\" + T + @"\" + W[2] + @"\" + N.Name + "." + T;
                }
                L += "material, " + @"d:\data\mtl\" + W[2] + @"\" + N.Name + ".mtl, " + I + ", " + N.Material.Name + ", " + P + "\r\n";
                T = "Ka " + M.AmbientColor.Red + " " + M.AmbientColor.Green + " " + M.AmbientColor.Blue + "\r\n";
                T += "Kd " + M.DiffuseColor.Red + " " + M.DiffuseColor.Green + " " + M.DiffuseColor.Blue + "\r\n";
                T += "Ke " + M.EmissiveColor.Red + " " + M.EmissiveColor.Green + " " + M.EmissiveColor.Blue + "\r\n";
                T += "Kr " + M.ReflectiveColor.Red + " " + M.ReflectiveColor.Green + " " + M.ReflectiveColor.Blue + "\r\n";
                T += "Ks " + M.SpecularColor.Red + " " + M.SpecularColor.Green + " " + M.SpecularColor.Blue + "\r\n";
                T += "Ns 20" + "\r\n";
                if (O != null) T += "map_Kn " + O + "\r\n";
                if (P != null) T += "map_Kd " + P + "\r\n";

                File.WriteAllText(@"d:\data\mtl\#\" + N.Name + ".mtl", T);
            }
        }
        File.WriteAllText(S, L);
    }

    public void MatsImport()
    {
        // W[0] = "material"
        // W[1] = [kleur of bestandnaam]
        // V[2] = [onderdeelnummer]

        MatsInit();

        string S = W[1];

        if (!S.Contains(".")) // materiaal is geen bestand
        {
            if (W[1].Contains(@"kleur\"))
            {
                string T = GetFileAddress(S);
                if (File.Exists(T)) T = File.ReadAllText(GetFileAddress(S)); else return;

                PartMat.DiffuseColor = MatsRGBA(T);
            }
            else if (S.StartsWith("0x"))
            {
                PartMat.DiffuseColor = MatsRGBA(S.Substring(2));   
            }
            else
            {
                PartMat.DiffuseColor = MatsColor(S);
            }
            PartMat.DiffuseMap = null;
            PartMat.DiffuseMapFilePath = "";
            PartMat.Name = S;
        }
        else // materiaal is wel een bestand
        {
            S = FileFind(S);

            if (FileType(S) == ".mtl") // .mtl bestand
            {
                string M = FileName(S); // materiaal naam
                
                PartMat = new PhongMaterialCore()
                { 
                    Name = M
                };

                int J = (int)V[2]; // onderdeel nummer
                FI += 1;
                textParser.ParserOpen(S, 0, -1);
                FI -= 1;
                int I = 0;
                foreach (var Node in RoomMod.SceneNode.Items.Traverse(true))
                {
                    if (Node is MeshNode N)
                    {
                        I += 1;
                        if (J == I)
                        {
                            try
                            {
                                N.Material = PartMat;
                            }
                            catch { }
                            return;
                        }
                    }
                }
            }
            else // afbeelding betand
            {
                if (File.Exists(S))
                {
                    PartMat.DiffuseMap = MatsMap(S);
                    PartMat.DiffuseMapFilePath = S;
                    PartMat.Name = FileName(S);
                }
            }
        }

        if (W[2] != "")
        {
            int I = (int)V[2];
            int L = RoomMod.SceneNode.ItemsCount; // laatste
            if (I == 0) I = L;
            int J = 0;

            foreach (var Node in RoomMod.SceneNode.Items.Traverse(true))
            {
                if (Node is MeshNode N)
                {
                    J++;
                    if (J == I)
                    {
                        try
                        {
                            N.Material = PartMat;
                        }
                        catch { }

                        return;
                    }
                }
            }
        }
    }

    public Color4 MatsColor(string C) // kleuren
    {
        string S;

        if (C.StartsWith("0x"))
        {
            S = C.Substring(2);
        }
        else
        {
            switch (C.ToLower())
            {
                case "beige": S = "B0A06FFF"; break;
                case "beige-d": S = "897D62FF"; break;
                case "blauw": S = "0055BFFF"; break;
                case "blauw-l": S = "97CBD9FF"; break;
                case "blauw-t": S = "0055BF7F"; break;
                case "bruin": S = "583927FF"; break;
                case "geel": S = "F2CD37FF"; break;
                case "geel-t": S = "F5CD2F7F"; break;
                case "grijs": S = "9BA19DFF"; break;
                case "grijs-db": S = "6C6E68FF"; break;
                case "grijs-d": S = "6D6E5CFF"; break;
                case "grijs-l": S = "BCB4A5FF"; break;
                case "grijs-x": S = "E0E0E0FF"; break;
                case "groen": S = "237841FF"; break;
                case "groen-d": S = "002000FF"; break;
                case "helder": S = "FCFCFC7F"; break;
                case "rood": S = "C91A09FF"; break;
                case "rood-l": S = "FFBDBDFF"; break;
                case "rood-t": S = "C91A097F"; break;
                case "roze": S = "FC97ACFF"; break;
                case "trans": S = "00000000"; break;
                case "wit": S = "FFFFFFFF"; break;
                case "wit-m": S = "EEEEEEFF"; break; // melk wit
                case "zilver": S = "CECECEFF"; break;
                default: S = "252525FF"; break; // zwart
            }
        }

        return MatsRGBA(S);
    }

    public Color4 MatsRGBA(string S)
    {
        return new Color4((float)Convert.ToInt32("0x" + S.Substring(0, 2), 16) / 255.0f,
                          (float)Convert.ToInt32("0x" + S.Substring(2, 2), 16) / 255.0f,
                          (float)Convert.ToInt32("0x" + S.Substring(4, 2), 16) / 255.0f,
                          (float)Convert.ToInt32("0x" + S.Substring(6, 2), 16) / 255.0f);
    }

    public void MatsBump()
    {
        W[1] = GetFileAddress(W[1]);
        PartMat.DisplacementMapFilePath = W[1];
        PartMat.DisplacementMap = partMats.MatsMap(W[1]);
    }

    public void MatsKd()
    {
        W[1] = GetFileAddress(W[1]);
        PartMat.DiffuseMapFilePath = W[1];
        PartMat.DiffuseMap = partMats.MatsMap(W[1]);
    }

    public void MatsKn()
    {
        string S = GetFileAddress(W[1]);
        PartMat.NormalMapFilePath = S;
        PartMat.NormalMap = partMats.MatsMap(S);
    }

    public void MatsKs()
    {
        W[1] = GetFileAddress(W[1]);
        PartMat.SpecularColorMapFilePath = W[1];
        PartMat.SpecularColorMap = partMats.MatsMap(W[1]);
    }

    public MemoryStream MatsMap(string S)
    {
        MemoryStream M = new MemoryStream();

        try
        {
            using (FileStream F = new FileStream(S, FileMode.Open))
            {
                F.CopyTo(M);
            }
        }
        catch
        {
            MessageBox.Show(S + " niet gevonden");

            parserRuns = false;
        }

        return M;
    }

    public static PartMats partMats = new PartMats();
}