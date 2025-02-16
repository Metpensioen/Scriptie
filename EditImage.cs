using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

class EditImage : Image
{
    public Image ImageInit()
    {
        Stretch = Stretch.None;

        PreviewMouseDown += ImagePreviewMouseDown;
        PreviewMouseMove += ImagePreviewMouseMove;

        return this;
    }

    public void ImagePixel(Point P)
    {
        Int32Rect R = new Int32Rect((int)P.X, (int)P.Y, 20, 20);

        BitmapSource B = (BitmapSource)editImage.Source;

        byte[] C = new byte[B.PixelWidth * B.PixelHeight * 4];

        int X = B.PixelWidth * 4;

        try
        {
            B.CopyPixels(R, C, X, 0);
        }
        catch { }
    }

    public void ImagePreviewMouseDown(object sender, MouseEventArgs e)
    {
        ImagePixel(e.GetPosition(this));
    }

    public void ImagePreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) ImagePixel(e.GetPosition(this));
    }

    public static EditImage editImage = new EditImage();
}