using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using static EditText;
using static GridMenu;
using static GridView;
using static TabsFile;
using static TextFunctions;
using static TextParser;
using static ViewDrawing;
using static ViewPlayer;

class ViewImage : Image
{
    public string imageFile = "";
    public long imageSize;
    public BitmapImage imageMap = new BitmapImage();
    public RotateTransform imageRotate = new RotateTransform();
    public ScaleTransform imageScale = new ScaleTransform();
    public TranslateTransform imageMove = new TranslateTransform();
    public TransformGroup imageTrans = new TransformGroup();
    public Point imagePos = new Point();
    public string imageTags = "001";

    public void ImageReset()
    {
        imageScale.ScaleX = 1;
        imageScale.ScaleY = 1;
        imageRotate.CenterX = 450;
        imageRotate.CenterY = 450;
        imageRotate.Angle = 0;
        imageMove.X = 0;
        imageMove.Y = 0;
    }

    public Image ImageInit()
    {
        HorizontalAlignment = HorizontalAlignment.Left; // default = stretch
        VerticalAlignment = VerticalAlignment.Top; // default = stretch

        ImageReset();

        imageTrans.Children.Add(imageRotate);
        imageTrans.Children.Add(imageScale);
        imageTrans.Children.Add(imageMove);

        RenderTransform = imageTrans;

        PreviewMouseLeftButtonDown += This_PreviewMouseLeftButtonDown;
        PreviewMouseRightButtonDown += This_PreviewMouseLeftButtonDown;
        PreviewMouseLeftButtonUp += This_PreviewMouseLeftButtonUp;
        PreviewMouseMove += This_PreviewMouseMove;
        PreviewMouseWheel += This_PreviewMouseWheel;

        return this;
    }

    public void ImageCopy()
    {
        // W[0] = "pictcopy"
        // W[1] = bron afbeelding bestand naam
        // V[2] = bron rechthoek links
        // V[3] = bron rechthoek top
        // V[4] = bron rechthoek breedte
        // V[5] = bron rechthoek hoogte
        // V[6] = doel links 
        // V[7] = doel top
        // V[8] = doel midden x
        // V[9] = doel midden y
        // V[10] = hoek
        // V[11] = schaal x
        // V[12] = schaal y

        Stretch = Stretch.None;
        string B = GetFileAddress(W[1]);
        ImageLoad(B); // open de bron afbeelding
        WriteableBitmap WB = new WriteableBitmap(1000, 1000, 96, 96, PixelFormats.Bgra32, null);

        Int32Rect R = new Int32Rect((int)V[2], (int)V[3], (int)V[4], (int)V[5]);
        byte[] C = new byte[imageMap.PixelWidth * imageMap.PixelHeight * 4];
        Int32Rect D = new Int32Rect(0, 0, (int)V[4], (int)V[5]);

        int X = imageMap.PixelWidth * 4;

        try
        {
            imageMap.CopyPixels(R, C, X, 0);
            WB.WritePixels(D, C, X, 0);
        }
        catch
        {
            return;
        }

        viewImage.Source = WB;

        imageScale.ScaleX = (int)V[11];
        imageScale.ScaleY = (int)V[12];
        imageRotate.CenterX = (int)V[8];
        imageRotate.CenterY = (int)V[9];
        imageRotate.Angle = (int)V[10];
        imageMove.X = (int)V[6];
        imageMove.Y = (int)V[7];

        //gridEdit.Children.Clear();
        //gridEdit.Children.Add(editPict); 

        gridView.Children.Clear();
        gridView.Children.Add(viewDrawing);
        gridView.Children.Add(viewImage);
    }

    public void ImageCrop(int x1, int y1, int x2, int y2)
    {
        try
        {
            CroppedBitmap c = new CroppedBitmap(imageMap, new Int32Rect(x1, y1, x2, y2));

            viewImage.Source = null;

            viewImage.Source = c;
        }
        catch { }
    }

    public void ImageInfo()
    {
        string s;

        if (imageMap != null)
        {
            //s = imageTags; // .Replace("lauren", "-");
            //s = imageFile + ", " + imageMap.PixelWidth + ", " + imageMap.PixelHeight + ", " + s;// imageSize + s; // + imageTags).ToString();
            s = FileName(imageFile) + ", " + imageMap.PixelWidth + ", " + imageMap.PixelHeight + ", " + imageSize + imageTags;
            menuText.Text = s;
        }
        else
        {
           menuText.Text = "";
        }
    }

    public BitmapImage ImageLoad(string S)
    {
        BitmapImage b = new BitmapImage();
        FileStream F;

        try
        {
            F = new FileStream(S, FileMode.Open);
        }
        catch
        {
            return null;
        }

        using (F)
        {
            imageSize = F.Length;

            try
            {
                b.BeginInit();
                b.CacheOption = BitmapCacheOption.OnLoad;
                b.StreamSource = F;
                b.EndInit();
            }
            catch { }
        }

        imageMap = b;

        return b;
    }

    public void ImageOpen(string file)
    {
        if (File.Exists(file))
        {
            imageFile = file; // onthoud de foto naam
            Source = ImageLoad(file); // afbeelding bestand lezen
            ImageReset(); // transformatie initialiseren
            imageTags = ImageTagsGet(file);
            ImageInfo(); // gegevens tonen

            gridView.Children.Clear();
            gridView.Children.Add(viewImage);
        }
    }

    public void ImageSize(string s) // foto formaat aanpassen
    {
        BitmapImage b = new BitmapImage();
        FileStream F = new FileStream(s, FileMode.Open);

        using (F)
        {
            b.BeginInit();
            b.CacheOption = BitmapCacheOption.OnLoad;
            b.StreamSource = F;
            b.EndInit();
        }

        double scale;
        double max = 900;

        if (b.PixelWidth > b.PixelHeight) scale = max / b.PixelWidth; else scale = max / b.PixelHeight;

        TransformedBitmap t = new TransformedBitmap(b, new ScaleTransform(scale, scale));

        JpegBitmapEncoder ejpg = new JpegBitmapEncoder();

        ejpg.Frames.Add(BitmapFrame.Create(t));

        FileStream g = new FileStream(s, FileMode.Create);

        using (g)
        {
            ejpg.Save(g);
        }
    }

    public string ImageTagsGet(string S) // lees labels uit jpg bestand
    {
        if (FileType(S) == ".jpg")
        {
            try
            {
                TagLib.File F = TagLib.File.Create(S);
                S = "";
                var I = F as TagLib.Image.File;
                string[] T = I.ImageTag.Keywords;
                int N = T.GetUpperBound(0);
                for (int J = 0; J <= N; J++)
                {
                    T[J].Trim();
                    if (T[J] != "") S += T[J] + ", ";
                }
            }
            catch { S = ""; }
        }
        else S = "";

        return S;
    }

    public void ImageTagsSet(string file, string tags) // schrijf labels in jpg bestand
    {
        string S;
        string P;
        // string E;
        int I;

        if (tags == "")
        {
            I = editText.GetLineIndexFromCharacterIndex(editText.CaretIndex); // regel nummer
            textParser.ParserLine(editText.GetLineText(I), ','); // zoek woorden
            S = W[0]; // bestandnaam
            P = FileRoot() + GetFilePath(textFile) + "\\" + S;

            if (S.EndsWith("mp4"))
            {
                viewPlayer.PlayerSetTags(P);
                return;
            }
        }

        try
        {
            List<string> list = new List<string>();

            textParser.ParserLine(tags, ',');

            I = 0;

            while (W[I] != "")
            {
                S = W[I];
                list.Add(S);
                I++;
            }

            ReadOnlyCollection<string> list2 = new ReadOnlyCollection<string>(list);

            FileStream input = new FileStream(file, FileMode.Open);

            BitmapDecoder decoder = new JpegBitmapDecoder(input, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

            BitmapFrame bitmapFrame = decoder.Frames[0]; // modify the metadata

            BitmapMetadata metaData = new BitmapMetadata("jpg") //BitmapMetadata metaData = (BitmapMetadata)bitmapFrame.Metadata.Clone();
            {
                Keywords = list2
            };

            BitmapEncoder encoder = new JpegBitmapEncoder(); // get an encoder to create a new jpg file with the new metadata.      

            //encoder.Frames.Add(BitmapFrame.Create(bitmapFrame, bitmapFrame.Thumbnail, metaData, bitmapFrame.ColorContexts));
            encoder.Frames.Add(BitmapFrame.Create(bitmapFrame, null, metaData, bitmapFrame.ColorContexts));

            input.Close();

            FileStream output = new FileStream(file, FileMode.Create); // Save the new image 

            using (output)
            {
                encoder.Save(output);
            }
        }
        catch { }
    }

    public void ImageShow()
    {
        // W[0] = "show"
        // W[1] = bestand

        string S = GetFileAddress(W[1]);

        ImageOpen(S);
        textParser.ParserWait(100);
    }

    // events

    public void This_PreviewMouseLeftButtonDown(object sender, MouseEventArgs e) // als de linker muisknop wordt ingedrukt
    {
        imagePos = e.GetPosition(this);
    }

    public void This_PreviewMouseRightButtonDown(object sender, MouseEventArgs e) // als de rechter muisknop wordt ingedrukt
    {
        imagePos = e.GetPosition(this);
    }

    public void This_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) // verdraai
        {
            Point P = e.GetPosition(this);

            imageRotate.Angle += (P.X - imagePos.X) / 10;
            imagePos = P;
        }
        else if (e.RightButton == MouseButtonState.Pressed) // verplaats
        {
            Point P = e.GetPosition(this);

            imageMove.X += P.X - imagePos.X;
            imageMove.Y += P.Y - imagePos.Y;
            imagePos = P;
        }
    }

    public void This_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        float S;

        if (e.Delta > 0) S = 0.1f; else S = -0.1f;

        imageScale.ScaleX += S;
        imageScale.ScaleY += S;
    }

    public void This_PreviewMouseLeftButtonUp(object sender, MouseEventArgs e) // als de linker muisknop wordt losgelaten
    {
        Point P;

        P = e.GetPosition(this);

        if (Keyboard.IsKeyDown(Key.LeftShift)) // schrijf de textuur waarden naar een tekst bestand om een foto te onthoeken
        {
            editText.AppendText("vt, n" + FL(P.X / this.ActualWidth) + FL(P.Y / this.ActualHeight) + "\n");
        }

        if (Keyboard.IsKeyDown(Key.LeftCtrl)) // bepaal de plaats van een onderrand van een foto om die daarna af te knippen met F3
        {
            if (P.Y < 30) // rand rondom verwijderen
            {
                ImageCrop((int)P.X, (int)P.Y, (int)(this.ActualWidth - 2 * P.X), (int)(this.ActualHeight - 2 * P.Y));
                textParser.ParserWait(100);
                photoFile = imageFile;
                gridView.ShowPhoto();
            }
            else // alleen onderste rand verwijderen
            {
                ImageCrop(0, 0, (int)this.ActualWidth, (int)P.Y);
                textParser.ParserWait(100);
                photoFile = imageFile;
                gridView.ShowPhoto();
            }

            textParser.ParserWait(100);
            ImageOpen(photoFile);
        }

        if (Keyboard.IsKeyDown(Key.LeftAlt)) // pixelkleur als rgba waardes naar een tekst bestand schrijven
        {
            CroppedBitmap cb = new CroppedBitmap(imageMap, new Int32Rect((int)P.X, (int)P.Y, 1, 1));
            var b = new byte[4];
            cb.CopyPixels(b, 4, 0);
            editText.AppendText("'" + HexB(b[2]) + HexB(b[1]) + HexB(b[0]) + HexB(b[3]) + "\n"); // bgra !!! -- > rgba
        }
    }

    public static ViewImage viewImage = new ViewImage();
}