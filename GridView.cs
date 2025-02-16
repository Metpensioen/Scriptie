using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using static EditText;
using static ViewData;
using static ViewDrawing;
using static ViewHTML;
using static ViewImage;
using static ViewPlayer;
using static ViewRoom;
using static ViewText;
using static TabsFile;
using static TextParser;

class GridView : Grid
{
    public static string photoFile = "";

    public Grid ShowInit() // toonveld starten
    {
        Children.Add(viewData.DataInit());
        Children.Add(viewDrawing.DrawingInit());
        Children.Add(viewHTML.HTMLInit());
        Children.Add(viewPlayer.PlayerInit());
        Children.Add(viewRoom.RoomInit());
        Children.Add(viewImage.ImageInit());
        Children.Add(viewText.TextInit());

        return this;
    }

    public void ShowPhoto() // foto maken
    {
        // W[0] = "photo"
        // W[1] = [bestandnaam]

        textParser.ParserWait(100);

        int B = (int)ActualWidth; // breedte
        int H = (int)ActualHeight; // hoogte

        if (W[0] == "photo")
        {
            photoFile = tabsFile.FileSave(W[1]);

            if (W[2] != "") B = (int)V[2];
            if (W[3] != "") H = (int)V[3];
        }
        string T;

        UIElement U = new UIElement();

        if (Children.Contains(viewRoom)) // als het een 3d scene is
        {
            while (!roomReady) textParser.ParserDoEvents();

            U = viewRoom;
        }
        else if (Children.Contains(viewDrawing)) // als het een tekening is
        {
            while (!drawingReady) textParser.ParserDoEvents();

            U = drawingGrid;
        }
        else if (Children.Contains(viewImage)) // als het een afbeelding is
        {
            U = viewImage;
            B = (int)viewImage.ActualWidth;
            H = (int)viewImage.ActualHeight;
        }
        else if (Children.Contains(viewPlayer))
        {
            U = viewPlayer;
            B = (int)viewPlayer.ActualWidth;
            H = (int)viewPlayer.ActualHeight;
        }
        else if (Children.Contains(HTMLView))
        {
            string S = HTMLView.Source.ToString();
            T = FileType(S);

            string FN = GetFilePath(textFile);
            int I = FN.LastIndexOf(@"\");
            if (I > 3) FN = FN.Substring(0, I);
            FN = FileRoot() + FN + @"\foto";
            Directory.CreateDirectory(FN);

            using (WebClient client = new WebClient())
            {
                FN += "\\" + tabsFile.FileDate(System.DateTime.Now) + T;
                client.DownloadFile(new Uri(S), FN);
            }
            HTMLView.GoBack();
            return;
        }
        else
        {
            MessageBox.Show("geen bron");
        }

        if (B == 0 || H == 0) return;

        if (File.Exists(photoFile))
        {
            System.IO.File.Delete(photoFile);
        }

        RenderTargetBitmap Bitmap = new RenderTargetBitmap(B, H, 96, 96, PixelFormats.Pbgra32);

        //Size visualSize = new Size(B, H);
        //U.Measure(visualSize);
        //U.Arrange(new Rect(visualSize));

        Bitmap.Render(U); // Bitmap.Render(gridView); rendert ook het menu

        if (W[0] != "photo") photoFile = photoFile.Replace(".png", ".jpg");

        FileStream F = new FileStream(photoFile, FileMode.Create);

        T = FileType(photoFile);

        if (T == ".jpg")
        {
            JpegBitmapEncoder EJPG = new JpegBitmapEncoder();

            EJPG.Frames.Add(BitmapFrame.Create(Bitmap));

            using (F)
            {
                EJPG.Save(F);
            }
        }
        else if (T == ".png")
        {
            PngBitmapEncoder EPNG = new PngBitmapEncoder();
            EPNG.Frames.Add(BitmapFrame.Create(Bitmap));

            using (F)
            {
                EPNG.Save(F);
            }
        }
        else
        {
            MessageBox.Show("geen afbeelding type");
        }

        F.Dispose();

        if (textFile.Contains("index.txt")) editText.TextNext();
    }

    public static GridView gridView = new GridView();
}