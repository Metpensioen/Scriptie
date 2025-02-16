using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX.Model;

using Microsoft.VisualBasic;

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using static RoomMods;
using static TabsFile;
using static TextFunctions;
using static TextParser;

class TabsParts : TabItem
{
    public static MeshNode TabsPart;
    public static PhongMaterialCore TabsPhong;
    public static TextBlock AmbientColor = new TextBlock();
    public static TextBlock DiffuseColor = new TextBlock();
    public static TextBlock DiffusePath = new TextBlock();
    public static TextBlock SpecularColor = new TextBlock();

    public ListBox PartsList = new ListBox();

    public TabItem PartsInit()
    {
        Header = "Parts";
        Background = Brushes.Black;
        Foreground = Brushes.White;

        StackPanel PartsStack = new StackPanel();
        Label PartsLabel = new Label();
        PartsLabel.Content = "parts";

        PartsList.Height = 400;
        PartsList.FontFamily = new FontFamily("Consolas");
        PartsList.FontSize = 14;

        PartsStack.Children.Add(PartsLabel);
        PartsStack.Children.Add(PartsList);

        PartsStack.Children.Add(AmbientColor);
        PartsStack.Children.Add(DiffuseColor);
        PartsStack.Children.Add(DiffusePath);
        PartsStack.Children.Add(SpecularColor);

        Content = PartsStack;

        PartsList.MouseLeftButtonUp += PartsList_MouseLeftButtonUp;
        PartsList.MouseRightButtonUp += PartsList_MouseRightButtonUp;

        return this;
    }

    public void PartsPart() // toon, verberg, hernoem onderdeel
    {
        // W[0] = "part"
        // V[1] = [onderdeel nummer]
        // W[2] = [hide]
        // W[3] = [nieuwe naam]

        int I = 0;

        foreach (var Node in RoomMod.SceneNode.Items.Traverse(true))
        {
            if (Node is MeshNode N)
            {
                I += 1;
                if (I == V[1])
                {
                    if (V[2] == 0) N.Visible = false; else N.Visible = true;
                    if (W[3] != "") N.Name = W[3];
                }
            }
        }
    }

    public void PartsLists() // maak onderdelen lijst
    {
        // W[0] = "parts"
        // W[1] = [bestandnaam]
        // W[2] = [onderdeel of model]

        string S = W[1];
        bool L = S != "";
        if (L)
        {
            S = GetFileAddress(S);
            FileSystem.FileOpen(1, S, OpenMode.Output);
        }

        tabsParts.PartsList.Items.Clear();
        int I = 0;
        string N;
        PhongMaterialCore P = new PhongMaterialCore();
        foreach (var Node in RoomMod.SceneNode.Items.Traverse(true))
        {
            if (Node is MeshNode M)
            {
                I += 1;
                if (M.Material is PhongMaterialCore phong)
                {
                    P = (PhongMaterialCore)M.Material;
                }
                N = Node.Name;
                N = SR(N + ", ", ' ', 20);
                S = M.Material.Name;
                S = SR(S + ", ", ' ', 16);
                string J = SL(I.ToString(), ' ', 2) + ", 1, ";
                tabsParts.PartsList.Items.Add(J + N + S + P.DiffuseMapFilePath);
                if (L) FileSystem.PrintLine(1, "part, " + J + N + S + P.DiffuseMapFilePath);
            }
        }
        FileSystem.FileClose(1);
    }

    public void PartsList_MouseLeftButtonUp(object Sender, MouseButtonEventArgs e)
    {
        if (PartsList.SelectedIndex == -1) return;
        string S = PartsList.SelectedItem.ToString();
        int N = int.Parse(S.Substring(0, S.IndexOf(",")));
        int I = 0;
        MeshNode M;
        foreach (var Node in RoomMod.SceneNode.Traverse(false))
        {
            if (Node.GetType().ToString().Contains("MeshNode"))
            {
                I++;
                if (I == N)
                {
                    M = (MeshNode)Node;
                    if (M.Material.GetType().ToString().Contains("Phong"))
                    {
                        TabsPart = M;
                        TabsPhong = (PhongMaterialCore)M.Material;
                        AmbientColor.Text = "ambient  " + TabsPhong.AmbientColor;
                        DiffuseColor.Text = "diffuse  " + TabsPhong.DiffuseColor;
                        DiffusePath.Text = "dif path " + TabsPhong.DiffuseMapFilePath;
                        SpecularColor.Text = "specular " + TabsPhong.SpecularColor;
                    }
                }
            }
        }
    }

    public void PartsList_MouseRightButtonUp(object Sender, MouseButtonEventArgs e)
    {
        string S = PartsList.SelectedItem.ToString();
        int N = int.Parse(S.Substring(0, S.IndexOf(",")));
        int I = 0;
        int J;
        int K;
        MeshNode M;

        foreach (var Node in RoomMod.SceneNode.Traverse(false))
        {
            if (Node.GetType().ToString().Contains("MeshNode"))
            {
                I++;
                if (I == N)
                {
                    M = (MeshNode)Node;
                    M.Visible = !M.Visible;
                    J = S.IndexOf(",") + 1;
                    K = S.IndexOf(",", J + 1);
                    if (M.Visible)
                    {
                        S = S.Substring(0, J) + " 1" + S.Substring(K);
                    }
                    else
                    {
                        S = S.Substring(0, J) + " 0" + S.Substring(K);
                    }
                    I = PartsList.SelectedIndex;
                    PartsList.Items[I] = S;
                    return;
                }
            }
        }
    }

    public static TabsParts tabsParts = new TabsParts();
}