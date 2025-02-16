using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using static RoomCamera;
using static RoomLamp;

class TabsRoom : TabItem
{
    public TextBox CameraInfo = new TextBox();
    public Label CameraLabel = new Label();

    public Label LampA1Label = new Label();
    public Label LampD1Label = new Label();
    public Label LampD2Label = new Label();

    public Label LampRXLabel = new Label();
    public Label LampRYLabel = new Label();
    public Label ModelLabel = new Label();

    public class ClassCameraYSlider : Slider
    {
        public Slider Init()
        {
            Minimum = -1;
            Maximum = 1;
            Value = 0;

            ValueChanged += ClassSlider_ValueChanged;

            return this;
        }

        private void ClassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float C = (float)e.NewValue;
            Point3D P;

            P = roomCamera.Position;
            P.Y = C;
            roomCamera.Position = P;
        }
    }

    public ClassCameraYSlider CameraYSlider = new ClassCameraYSlider();

    public class ClassCameraY2Slider : Slider
    {
        public Slider Init()
        {
            Minimum = -1;
            Maximum = 1;
            Value = 0;

            ValueChanged += ClassSlider_ValueChanged;

            return this;
        }

        private void ClassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float C = (float)e.NewValue;
            Vector3D V;

            V = roomCamera.LookDirection;
            V.Y = C;
            roomCamera.LookDirection = V;

        }
    }

    public static ClassCameraY2Slider CameraY2Slider = new ClassCameraY2Slider();

    public class LampA1CSlider : Slider
    {
        public Slider A1CSliderInit()
        {
            Minimum = 0;
            Maximum = 1;

            ValueChanged += This_ValueChanged;

            return this;
        }

        private void This_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float C = (float)e.NewValue;

            roomLamp.LampA1.Color = new Color4(C, C, C, 1).ToColor();
        }
    }

    public LampA1CSlider lampA1CSlider = new LampA1CSlider();

    public class LampD1CSlider : Slider
    {
        public Slider D1CSliderInit()
        {
            Minimum = 0;
            Maximum = 1;
            Value = 1;

            ValueChanged += This_ValueChanged;

            return this;
        }

        private void This_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float C = (float)e.NewValue;

            roomLamp.LampD1.Color = new Color4(C, C, C, 1).ToColor();
        }
    }

    public LampD1CSlider lampD1CSlider = new LampD1CSlider();

    public class ClassLampD1XSlider : Slider
    {
        public Slider Init()
        {
            Minimum = -1;
            Maximum = 1;

            ValueChanged += ClassSlider_ValueChanged;

            return this;
        }

        private void ClassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            roomLamp.LampTurn();
        }
    }

    public ClassLampD1XSlider LampD1XSlider = new ClassLampD1XSlider();

    public class ClassLampD1YSlider : Slider
    {
        public Slider Init()
        {
            Minimum = -1;
            Maximum = 1;

            ValueChanged += ClassSlider_ValueChanged;

            return this;
        }

        private void ClassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            roomLamp.LampTurn();
        }
    }

    public ClassLampD1YSlider LampD1YSlider = new ClassLampD1YSlider();

    public class ClassLampD1ZSlider : Slider
    {
        public Slider Init()
        {
            Minimum = -1;
            Maximum = 1;

            ValueChanged += ClassSlider_ValueChanged;

            return this;
        }

        private void ClassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            roomLamp.LampTurn();
        }
    }

    public ClassLampD1ZSlider LampD1ZSlider = new ClassLampD1ZSlider();

    public class LampD2CSlider : Slider
    {
        public Slider D2CSliderInit()
        {
            Minimum = 0;
            Maximum = 1;

            ValueChanged += This_ValueChanged;

            return this;
        }

        private void This_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float C = (float)e.NewValue;

            roomLamp.LampD2.Color = new Color4(C, C, C, 1).ToColor();
        }
    }

    public LampD2CSlider lampD2CSlider = new LampD2CSlider();

    public class ClassLampD2XSlider : Slider
    {
        public Slider Init()
        {
            Minimum = -1;
            Maximum = 1;

            ValueChanged += ClassSlider_ValueChanged;

            return this;
        }

        private void ClassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            roomLamp.LampTurn();
        }
    }

    public ClassLampD2XSlider LampD2XSlider = new ClassLampD2XSlider();

    public class ClassLampD2YSlider : Slider
    {
        public Slider Init()
        {
            Minimum = -1;
            Maximum = 1;

            ValueChanged += ClassSlider_ValueChanged;

            return this;
        }

        private void ClassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            roomLamp.LampTurn();
        }
    }

    public ClassLampD2YSlider LampD2YSlider = new ClassLampD2YSlider();

    public class ClassLampD2ZSlider : Slider
    {
        public Slider Init()
        {
            Minimum = -1;
            Maximum = 1;

            ValueChanged += ClassSlider_ValueChanged;

            return this;
        }

        private void ClassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            roomLamp.LampTurn();
        }
    }

    public ClassLampD1ZSlider LampD2ZSlider = new ClassLampD1ZSlider();

    public TabItem RoomInit()
    {
        Header = "Room";
        Background = Brushes.Black;
        Foreground = Brushes.White;

        StackPanel RoomStack = new StackPanel();

        CameraLabel.Content = "camera";
        RoomStack.Children.Add(CameraLabel);
        RoomStack.Children.Add(CameraInfo);

        RoomStack.Children.Add(CameraYSlider.Init());
        RoomStack.Children.Add(CameraY2Slider.Init());

        LampA1Label.Content = "lamp A1";
        RoomStack.Children.Add(LampA1Label);
        RoomStack.Children.Add(lampA1CSlider.A1CSliderInit());

        LampD1Label.Content = "lamp D1";
        RoomStack.Children.Add(LampD1Label);
        RoomStack.Children.Add(lampD1CSlider.D1CSliderInit());
        RoomStack.Children.Add(LampD1XSlider.Init());
        RoomStack.Children.Add(LampD1YSlider.Init());
        RoomStack.Children.Add(LampD1ZSlider.Init());

        LampD2Label.Content = "lamp D2";
        RoomStack.Children.Add(LampD2Label);
        RoomStack.Children.Add(lampD2CSlider.D2CSliderInit());
        RoomStack.Children.Add(LampD2XSlider.Init());
        RoomStack.Children.Add(LampD2YSlider.Init());
        RoomStack.Children.Add(LampD2ZSlider.Init());

        Content = RoomStack;

        return this;
    }

    public static TabsRoom tabsRoom = new TabsRoom();
}