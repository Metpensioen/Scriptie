using HelixToolkit.Wpf.SharpDX;

using Microsoft.VisualBasic;

using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using static EditText;
using static GridView;
using static PartMats;
using static TabsCalc;
using static TabsDraw;
using static TabsFile;
using static TextParser;
using static ViewData;
using static ViewImage;

class ViewDrawing : ScrollViewer
{
    public static double GS = 1; // grid afmeting
    public static double GI = 1;
    public static int drawingThickness = 1;
    public static Color drawingColor;
    public static Point drawingLast;
    public static Grid drawingGrid = new Grid();
    public static bool drawingReady = false; // voor foto maken

    public ScrollViewer DrawingInit() // tekenveld starten
    {
        Background = Brushes.Transparent;

        Content = drawingGrid;

        viewDrawing.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        viewDrawing.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

        PreviewMouseDown += This_PreviewMouseDown;
        PreviewMouseMove += This_PreviewMouseMove;
        drawingGrid.LayoutUpdated += DrawingGridLayoutUpdated;

        return this;
    }

    private void DrawingGridLayoutUpdated(object sender, EventArgs e)
    {
        drawingReady = true;
    }

    public void DrawingOpen() // tekening starten
    {
        // W[0] = "draw"
        // W[1] = [achtergrond kleur of afbeelding]

        drawingGrid.Children.Clear();

        string S = W[1];

        if (S.IndexOf(".") > -1) // als de achtergrond een afbeelding is
        {
            ImageBrush I = new ImageBrush(viewImage.ImageLoad(GetFileAddress(S)))
            {
                Stretch = Stretch.Uniform
            };

            drawingGrid.Background = I;
        }
        else // geen afbeelding
        {
            if (S == "") S = "wit";

            drawingGrid.Background = new SolidColorBrush(partMats.MatsColor(S).ToColor());
        }

        gridView.Children.Clear();
        gridView.Children.Add(viewDrawing);
    }

    public void DrawingAngle() // hoeken met de x, y en z as bepalen
    {
        // W[0] = "angle"
        // V[1] = x
        // V[2] = y
        // V[3] = z

        RX[0] = V[1];
        RY[0] = V[2];
        RZ[0] = V[3];
    }

    public void DrawingCircle() // cirkel tekenen
    {
        // W[0] = "circle"
        // V[1] = straal
        // V[2] = aantal segmenten
        // V[3] = [X middelpunt]
        // V[4] = [Y middelpunt]
        // V[5] = [beginhoek]
        // V[6] = [eindhoek]
        // W[7] = [dikte]
        // W[8] = [kleur]
        // W[9] = [vullen]

        int I;
        Point P = new Point();

        V[1] *= SX[0] * GS; // straal
        V[3] *= SX[0] * GS; // middelpunt x
        V[4] *= SY[0] * GS; // middelpunt y

        if (V[2] == 0) V[2] = 12; // als het aantal segmenten niet is opgegeven worden 12 segmenten genomen

        if (W[5] == "") // als de beginhoek niet is opgegeven wordt een volledige cirkel van 0 tot 360 ° getekend
        {
            V[5] = 0; // beginhoek
            V[6] = 360; // eindhoek
        }

        FI += 1;

        TX[FI] = V[3]; // middelpunt x
        TY[FI] = V[4]; // middelpunt y
        RZ[FI] = 0;    // hoek

        for (I = FI; I >= 0; I--)
        {
            P.X += TX[I];
            P.Y += TY[I];
            V[5] += RZ[I];
            V[6] += RZ[I];

            RotateTransform E = new RotateTransform(RZ[I], TX[I], TY[I]); // verdraai middelpunt om draaipunt

            P = E.Transform(P);
        }

        FI -= 1;

        drawingGrid.Children.Add(DrawSegments((float)V[1], (int)V[2], P, (float)V[5], (float)V[6], (int)V[7], partMats.MatsColor(W[8]).ToColor(), W[9] != ""));
    }

    public void DrawingClear() // tekening wissen
    {
        // W[0] = "clear"
        // V[1] = [wachttijd in milisecondes]

        if (V[1] > 0) textParser.ParserWait((int)V[1]);

        drawingGrid.Children.Clear();
    }

    public void DrawingColor() // kleur selecteren
    {
        // W[0] = "color"
        // W[1] = kleur

        drawingColor = partMats.MatsColor(W[1]).ToColor();

        int I = 0;
        int N = tabsDraw.StackColor.Items.Count;

        while (tabsDraw.StackColor.Items[I].ToString() != W[1] && I < N)
        {
            I++;
        }

        tabsDraw.StackColor.SelectedIndex = I;
    }

    public void DrawingGraph() // grafiek tekenen
    {
        int N = viewData.dataTable[viewData.TI].Rows.Count;
        double Xmax = 0;
        double Ymax = 0;

        DataRow R;
        Polyline P = new Polyline();

        for (int I = 0; I < N; I++)
        {
            R = viewData.dataTable[viewData.TI].Rows[I];
            PA[I].X = (float)R[1];
            PA[I].Y = (float)R[2];
            if (PA[I].X > Xmax) Xmax = PA[I].X;
            if (PA[I].Y > Ymax) Ymax = PA[I].Y;
        }

        Xmax = 800 / Xmax;
        Ymax = 800 / Ymax;

        for (int I = 0; I < N; I++)
        {
            P.Points.Add(new Point(450 + TX[0] + PA[I].X * SX[0] * Xmax, 450 - TY[0] - PA[I].Y * SY[0] * Ymax));
        }

        P.StrokeThickness = 1;
        P.Stroke = Brushes.Black;
        drawingGrid.Children.Add(P);
        drawingGrid.Children.Add(DrawLines(TX[0], TY[0], TX[0], TY[0] + 800 * SY[0], 1, "zwart")); // Y-as
        drawingGrid.Children.Add(DrawLines(TX[0], TY[0], TX[0] + 800 * SX[0], TY[0], 1, "zwart")); // X-as
    }

    public void DrawingGrid() // raster tekenen
    {
        // W[0] = "grid"
        // V[1] = raster afstand
        // W[2] = [toon]
        // V[3] = max
        // W[4] = [gebruik]

        GS = V[1] * SX[0];
        double max = V[3] * SX[0];
        double x1;
        double y1;
        double x2;
        double y2;

        if (W[2] != "") // tonen
        {
            int M = (int)(450 % GS); // marge van 1e gridlijn
            int N = (int)(max / GS) + 1; // aantal gridlijnen
            int L = (int)(max); // lengte van gridlijnen

            for (int I = 0; I < N; I++)  // horizontale lijnen
            {
                x1 = -L / 2;
                y1 = L / 2 - I * GS;
                x2 = L / 2;
                y2 = y1;

                drawingGrid.Children.Add(DrawLines(x1, y1, x2, y2, 1, "grijs-l"));
            }

            for (int I = 0; I < N; I++) // verticale lijnen
            {
                x1 = -L / 2 + I * GS;
                y1 = L / 2;
                x2 = x1;
                y2 = -L / 2;

                drawingGrid.Children.Add(DrawLines(x1, y1, x2, y2, 1, "grijs-l"));
            }
        }

        if (W[4] == "") GS = 1; // gebruik raster afstand niet
    }

    public void DrawingIso() // tekent een isometrisch raster
    {
        // W[0] = "iso"
        // V[1] = raster afstand

        GS = V[1];
        GI = Math.Pow(3, 0.5) / 2.0f;

        int M = (int)(450 % (GS * GI));
        int N = (int)(1000 / (GS * GI));

        for (int I = 0; I < N; I++) // verticale lijnen
        {
            drawingGrid.Children.Add(DrawLines(M + I * GS * GI - 450, 450, M + I * GS * GI - 450, -450, 1, "grijs-l"));
        }

        M = (int)(450 % GS);
        N = (int)(2000 / GS);

        for (int I = 0; I < N; I++)
        {
            drawingGrid.Children.Add(DrawLines(-450, 1000 - M - I * GS + DYT(450, 30), 450, 1000 - M - I * GS + DYT(450, -30), 1, "grijs-l"));
            drawingGrid.Children.Add(DrawLines(-450, 1000 - M - I * GS + DYT(450, -30), 450, 1000 - M - I * GS + DYT(450, 30), 1, "grijs-l"));
        }
    }

    public void DrawingLine()  // tekent lijn
    {
        // W[0] = "lijn"
        // V[1] = lengte langs x-as
        // V[2] = x begin punt
        // V[3] = y begin punt
        // V[4] = [Z hoek]
        // V[5] = [dikte]
        // W[6] = [kleur]

        V[1] *= SX[0] * GS;
        V[2] *= SX[0] * GS;
        V[3] *= SY[0] * GS;

        Point[] P = new Point[2]; // lijn bestaat uit 2 punten

        P[0].X = 0; // beginpunt
        P[0].Y = 0;

        P[1].X = P[0].X + V[1]; // eindpunt
        P[1].Y = P[0].Y;

        FI++;

        TX[FI] = V[2];
        TY[FI] = V[3];
        RZ[FI] = V[4];

        for (int I = FI; I >= 0; I--)
        {
            P[0].X += TX[I];
            P[0].Y += TY[I];
            P[1].X += TX[I];
            P[1].Y += TY[I];

            RotateTransform E = new RotateTransform(RZ[I], TX[I], TY[I]); // verdraai begin- en eindpunt om draaipunt

            P[0] = E.Transform(P[0]);
            P[1] = E.Transform(P[1]);
        }

        FI--;

        drawingGrid.Children.Add(DrawLines(P[0].X, P[0].Y, P[1].X, P[1].Y, (int)V[5], W[6])); // voeg lijn toe aan tekening
    }

    public void DrawingOffset() // verplaatst een tekening
    {
        // W[0] = "offset"
        // V[1] = x
        // V[2] = y
        // V[3] = z
        // W[4] = [show]

        TX[0] = V[1] * SX[0] * GS * GI;
        TY[0] = V[2] * SY[0] * GS;
        TZ[0] = V[3] * SZ[0] * GS;

        if (W[4] != "")
        {
            drawingGrid.Children.Add(DrawLines(-450, TY[0], 450, TY[0], 1, "zwart")); // horizontaal
            drawingGrid.Children.Add(DrawLines(TX[0], 450, TX[0], -450, 1, "zwart")); // verticaal
        }
    }

    public void DrawingPaint() // start tekenen met de muis
    {
        // W[0] = "paint"
        // V[1] = dikte
        // W[2] = kleur

        drawingThickness = (int)V[1];
        drawingColor = partMats.MatsColor(W[2]).ToColor();
    }

    public void DrawingPict() // plaatst een foto op een tekening
    {
        // W[0] = "pict"
        // W[1] = bestand naam
        // V[2] = breedte
        // V[3] = hoogte
        // V[4] = links
        // V[5] = top

        string file = GetFileAddress(W[1]);

        if (File.Exists(file))
        {
            Rectangle rectangle = new Rectangle();
            ImageBrush imageBrush = new ImageBrush(viewImage.ImageLoad(file));

            if (V[2] == 0) V[2] = 900;
            if (V[3] == 0) V[3] = 900;

            imageBrush.Stretch = Stretch.Uniform;
            rectangle.VerticalAlignment = VerticalAlignment.Top;
            rectangle.HorizontalAlignment = HorizontalAlignment.Left;
            rectangle.Width = V[2] * SX[0];
            
            if (V[3] == 0) V[3] = V[2] * (float)(imageBrush.ImageSource.Height / imageBrush.ImageSource.Width);
            
            rectangle.Height = V[3] * SY[0];

            rectangle.Margin = new Thickness(450 - rectangle.Width / 2 + V[4] * SX[0], 450 - rectangle.Height / 2 - V[5] * SY[0], 0, 0);
            rectangle.Fill = imageBrush;

            drawingGrid.Children.Add(rectangle);
        }
    }

    public void DrawingPixel()
    {
        // W[0] = "pixel"
        // W[1] = kleur
        // V[2] = x
        // V[3] = y

        Point[] P = new Point[4];

        P[0] = drawingLast;
        P[0].X += HorizontalOffset;
        P[0].Y += VerticalOffset;

        if (W[0] == "pixel")
        {
            P[0].X = (int)V[2];
            P[0].Y = (int)V[3];
            drawingColor = partMats.MatsColor(W[1]).ToColor();
        }
        else editText.AppendText("pixel, " + tabsDraw.StackColor.SelectedItem.ToString() + ", " + P[0].X + ", " + P[0].Y + "\n");

        P[0].X = P[0].X - 450; // rb
        P[0].Y = 450 - P[0].Y;

        if (P[0].X < 0) P[0].X = ((int)(P[0].X / GS) - 1) * GS; else P[0].X = ((int)(P[0].X / GS)) * GS;
        if (P[0].Y >= 0) P[0].Y = ((int)(P[0].Y / GS) + 1) * GS; else P[0].Y = ((int)(P[0].Y / GS)) * GS;

        P[1].X = P[0].X + GS; // lb
        P[1].Y = P[0].Y;
        P[2].X = P[0].X + GS; // lo
        P[2].Y = P[0].Y - GS;
        P[3].X = P[0].X;       // ro
        P[3].Y = P[0].Y - GS;
        drawingGrid.Children.Add(DrawRects(P[0], P[1], P[2], P[3], 1, drawingColor, true));

    }

    public void DrawingRectangle() // tekent een rechthoek 
    {
        // W(0) = "rectangle"
        // V(1) = width
        // V(2) = height
        // V(3) = X left
        // V(4) = Y top
        // V(5) = hoek z
        // W[6] = [dikte]
        // W(7) = [kleur]
        // W(8) = [vullen]

        Point[] P = new Point[4]; // een rechthoek heeft 4 punten

        V[1] *= SX[0] * GS;
        V[2] *= SY[0] * GS;
        V[3] *= SX[0] * GS;
        V[4] *= SY[0] * GS;

        P[0].X = 0; // links boven
        P[0].Y = 0;

        P[1].X = P[0].X + V[1]; // rechts boven
        P[1].Y = P[0].Y;

        P[2].X = P[1].X; // rechts onder
        P[2].Y = P[1].Y - V[2];

        P[3].X = P[0].X; // links onder
        P[3].Y = P[2].Y;

        FI++;

        TX[FI] = V[3];
        TY[FI] = V[4];
        RZ[FI] = V[5];

        for (int I = FI; I >= 0; I--)
        {
            P[0].X += TX[I];
            P[0].Y += TY[I];
            P[1].X += TX[I];
            P[1].Y += TY[I];
            P[2].X += TX[I];
            P[2].Y += TY[I];
            P[3].X += TX[I];
            P[3].Y += TY[I];

            RotateTransform E = new RotateTransform(RZ[I], TX[I], TY[I]); // verdraai begin- en eindpunt om draaipunt

            P[0] = E.Transform(P[0]);
            P[1] = E.Transform(P[1]);
            P[2] = E.Transform(P[2]);
            P[3] = E.Transform(P[3]);
        }

        FI--;

        drawingGrid.Children.Add(DrawRects(P[0], P[1], P[2], P[3], (int)V[6], partMats.MatsColor(W[7]).ToColor(), W[8] != ""));
    }

    public void DrawingCalc() // berekent variabelen
    {
        V[6] = calcVars[1] / calcVars[2];
        DrawingVar();
    }

    public void DrawingScale() // stelt een schaal in
    {
        // W[0] = "scale"
        // V[1] = x fractie
        // V[2] = [y fractie]
        // V[3] = [z fractie]

        SX[0] = V[1];
        if (W[2] == "") SY[0] = V[1]; else SY[0] = V[2];
        if (W[3] == "") SZ[0] = V[1]; else SZ[0] = V[3];
    }

    public void DrawingSpiro() // teken spirograaf
    {
        // W[0] = "spiro"
        // V[1] = ring tanden
        // V[2] = wiel tanden
        // V[3] = pen straal
        // V[4] = hoek
        // W[5] = kleur

        Polyline P = new Polyline();

        double R = V[1];
        double K = V[2];
        double S = K - 1.2 * V[3];
        double N = V[4];

        double H;

        R -= K;

        for (int I = 0; I < N; I++)
        {
            H = R / K * I;

            P.Points.Add(new Point(450 + (DXC(R, 90 + I) - DXC(S, 90 + H)) * SX[0], 450 - (DYS(R, 90 + I) + DYS(S, 90 + H)) * SY[0]));
        }

        P.StrokeThickness = 1;
        P.Stroke = new SolidColorBrush(partMats.MatsColor(W[5]).ToColor());
        drawingGrid.Children.Add(P);
    }

    public void DrawingText() // tekent een tekst
    {
        // W[0] = "text"
        // W[1] = tekst
        // V[2] = X
        // V[3] = Y
        // V[4] = [fontsize)
        // W[5] = [fontname]
        // W[6] = [bold]
        // W[7] = [italic]
        // W[8] = [kleur]
        // V[9] = [wait ms]

        Point P = new Point()
        {
            X = V[2] * GS * SX[0],
            Y = V[3] * GS * SY[0]
        };


        for (int I = FI; I >= 0; I--)
        {
            P.X += TX[I];
            P.Y += TY[I];

            RotateTransform E = new RotateTransform(-RZ[I], TX[I], TY[I]); // verdraai begin- en eindpunt om draaipunt
            P = E.Transform(P);
        }

        string S = W[1];

        if (S.Substring(0, 1) == "=")
        {
            //S = V[1].ToString();
            S = Strings.Format(V[1], "0.000");
        }

        drawingGrid.Children.Add(DrawTexts(S, P.X, P.Y, V[4], W[5], W[6] != "", W[7] != "", partMats.MatsColor(W[8]).ToColor(), ""));

        if (W[9] != "") textParser.ParserWait((int)V[9]);
    }

    public void DrawingTriangle() // tekent een driehoek
    {
        // W[0] = "triangle"
        // V[1] = breedte
        // V[2] = hoogte
        // V[3] = top links
        // V[4] = tx
        // V[5] = ty
        // V[6] = hoek
        // V[7] = kleur
        // V(8) = vul

        Point[] P = new Point[4];

        V[1] *= SX[0] * GS;
        V[2] *= SY[0] * GS;
        V[3] *= SY[0] * GS;
        V[4] *= SX[0] * GS;
        V[5] *= SY[0] * GS;

        P[1] = new Point(-V[1] / 2.0f, 0); // links onder
        P[2] = new Point(V[1] / 2.0f, 0); // rechts onder
        P[3] = new Point(-V[1] / 2.0f + V[3], V[2]); // top

        FI += 1;

        TX[FI] = V[4];
        TY[FI] = V[5];
        RZ[FI] = V[6];

        for (int I = FI; I >= 0; I--)
        {
            P[1].X += TX[I];
            P[1].Y += TY[I];
            P[2].X += TX[I];
            P[2].Y += TY[I];
            P[3].X += TX[I];
            P[3].Y += TY[I];

            RotateTransform E = new RotateTransform(RZ[I], TX[I], TY[I]); // verdraai begin- en eindpunt om draaipunt

            P[1] = E.Transform(P[1]);
            P[2] = E.Transform(P[2]);
            P[3] = E.Transform(P[3]);
        }

        FI -= 1;

        drawingGrid.Children.Add(DrawTriangles(P[1], P[2], P[3], partMats.MatsColor(W[7]).ToColor(), W[8] != ""));
    }

    public void DrawingVar() // tekent een variabele tekst
    {
        // W[0] = "var"
        // V[1] = index
        // W[2] = type
        // W[3] = header
        // W[4] = waarde
        // V[5] = x
        // V[6] = y
        // V[7] = [afmeting]

        calcVars[(int)V[1]] = V[4];

        string S = "";
        switch (W[2])
        {
            case "0": S = Strings.Format(V[4], ""); break;
            case "1": S = Strings.Format(V[4], "0.0"); break;
            case "3": S = Strings.Format(V[4], "0.000"); break;
        }

        Point P = new Point()
        {
            X = V[5] * GS * SX[0],
            Y = V[6] * GS * SY[0]
        };

        for (int I = FI; I >= 0; I--)
        {
            P.X += TX[I];
            P.Y += TY[I];
        }

        if (V[7] == 0) V[7] = 28;

        drawingGrid.Children.Add(DrawTexts(W[3] + " = " + S, P.X, P.Y, V[7], "", false, false, partMats.MatsColor("zwart").ToColor(), "R"));
    }

    public void This_PreviewMouseDown(object sender, MouseEventArgs e)
    {
        drawingLast = e.GetPosition(this);

        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            DrawingPixel();
        }
    }

    public void This_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Polyline L = new Polyline();

            PointCollection P = new PointCollection { drawingLast, e.GetPosition(this) };

            //PointCollection P = new PointCollection();
            //P.Add(drawingLast);
            //P.Add(e.GetPosition(this));

            L.StrokeThickness = drawingThickness;
            L.Stroke = new SolidColorBrush(drawingColor);
            L.Points = P;

            drawingLast = e.GetPosition(this);

            drawingGrid.Children.Add(L);
        }
    }

    public static ViewDrawing viewDrawing = new ViewDrawing();

    // funkties

    public static Line DrawLines(double PX1, double PY1, double PX2, double PY2, int T, string C) // tekent een lijn
    {
        if (T == 0) T = 1;

        Line L = new Line
        {
            X1 = 450 + PX1,
            Y1 = 450 - PY1,
            X2 = 450 + PX2,
            Y2 = 450 - PY2,

            StrokeThickness = T,
            Stroke = new SolidColorBrush(partMats.MatsColor(C).ToColor())
        };

        return L;
    }

    public static Polyline DrawRects(Point P1, Point P2, Point P3, Point P4, int T, Color C, bool F) // tekent een rechthoek
    {
        Polyline P = new Polyline();

        P1.X = 450 + P1.X; // links boven
        P1.Y = 450 - P1.Y;
        P2.X = 450 + P2.X; // rechts boven
        P2.Y = 450 - P2.Y;
        P3.X = 450 + P3.X; // rechts onder
        P3.Y = 450 - P3.Y;
        P4.X = 450 + P4.X; // links onder
        P4.Y = 450 - P4.Y;

        P.Points.Add(P1);
        P.Points.Add(P2);
        P.Points.Add(P3);
        P.Points.Add(P4);
        P.Points.Add(P1);

        if (F)
        {
            P.Fill = new SolidColorBrush(C);
        }
        else
        {
            if (T == 0) T = 1;
            P.StrokeThickness = T;
            P.Stroke = new SolidColorBrush(C);
        }

        return P;
    }

    public static Polyline DrawSegments(double radius, int n, Point center, double ab, double ae, int t, Color c, bool fill) // tekent een segment
    {
        int g = 360 / n; // aantal graden per segment

        int j = (int)Math.Abs(ae - ab) / g; // aantal segmenten

        if (ae < ab) n = -n;

        Polyline p = new Polyline();

        for (int i = 0; i <= j; i++)
        {
            p.Points.Add(new Point((int)(center.X + DXC(radius, ab + 360 / n * i)) + 450, (int)(-center.Y - DYS(radius, ab + 360 / n * i)) + 450));
        }

        p.Points.Add(new Point((int)(center.X + DXC(radius, ae)) + 450, (int)(-center.Y - DYS(radius, ae)) + 450)); // laatste punt

        p.Stroke = new SolidColorBrush(c);

        if (t == 0) t = 1;

        p.StrokeThickness = t;

        if (fill) p.Fill = new SolidColorBrush(c);

        return p;
    }

    public static TextBlock DrawTexts(string S, double XLB, double YLB, double FS, string FN, bool B, bool I, Color C, string L)  // tekent een tekst
    {
        TextBlock T = new TextBlock()
        {
            Background = Brushes.Transparent,
            Foreground = new SolidColorBrush(C),
            Text = S
        };

        if (FS == 0) FS = 28;

        T.FontSize = FS * SX[0];

        if (FN == "") T.FontFamily = new FontFamily("Consolas"); else T.FontFamily = new FontFamily(FN);

        if (B) T.FontWeight = FontWeights.UltraBold;
        if (I) T.FontStyle = FontStyles.Italic;

        double X;
        double Y;

        Y = (double)(T.FontSize / 1.6f);

        if (L != "R")
        {
            X = (double)(S.Length / 2f) * (T.FontSize / 1.8f);
        }
        else
        {
            X = 0;
        }

        T.Margin = new Thickness(450 + XLB - X, 450 - YLB - Y, 0, 0);

        return T;
    }

    public static Polyline DrawTriangles(Point P1, Point P2, Point P3, Color C, bool F) // tekent een driehoek
    {
        Polyline P = new Polyline();

        P1.X += 450;
        P1.Y = 450 - P1.Y;
        P2.X += 450;
        P2.Y = 450 - P2.Y;
        P3.X += 450;
        P3.Y = 450 - P3.Y;

        P.Points.Add(P1);
        P.Points.Add(P2);
        P.Points.Add(P3);
        P.Points.Add(P1);

        if (F) P.Fill = new SolidColorBrush(C);
        else
        {
            P.Stroke = new SolidColorBrush(C);
            P.StrokeThickness = 1;
        }

        return P;
    }

}