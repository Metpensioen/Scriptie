using HelixToolkit.Wpf.SharpDX;

using System.Windows.Controls;
using System.Windows.Media;

using static PartMats;
using static ViewDrawing;

class TabsDraw : TabItem
{
    public TextBox StackThick = new TextBox();
    public ComboBox StackColor = new ComboBox();

    public TabItem DrawInit()
    {
        StackPanel DrawStack = new StackPanel();

        Label ThickLabel = new Label() {
            Background = Brushes.Black,
            Foreground = Brushes.White,
            Content = "thickness"
        };

        Label ColorLabel = new Label()
        {
            Background = Brushes.Black,
            Foreground = Brushes.White,
            Content = "color"
        };

        StackColor.Items.Add("beige");
        StackColor.Items.Add("beige-d");
        StackColor.Items.Add("blauw");
        StackColor.Items.Add("blauw-l");
        StackColor.Items.Add("blauw-t");
        StackColor.Items.Add("bruin");
        StackColor.Items.Add("geel");
        StackColor.Items.Add("grijs");
        StackColor.Items.Add("grijs-db");
        StackColor.Items.Add("grijs-d");
        StackColor.Items.Add("grijs-l");
        StackColor.Items.Add("grijs-x");
        StackColor.Items.Add("groen");
        StackColor.Items.Add("groen-d");
        StackColor.Items.Add("helder");
        StackColor.Items.Add("rood");
        StackColor.Items.Add("rood-l");
        StackColor.Items.Add("rood-t");
        StackColor.Items.Add("roze");
        StackColor.Items.Add("trans");
        StackColor.Items.Add("wit");
        StackColor.Items.Add("wit-m");
        StackColor.Items.Add("zilver");
        StackColor.Items.Add("zwart");

        DrawStack.Children.Add(ThickLabel);
        DrawStack.Children.Add(StackThick);
        DrawStack.Children.Add(ColorLabel);
        DrawStack.Children.Add(StackColor);

        Header = "Draw";
        Background = Brushes.Black;
        Foreground = Brushes.White;
        Content = DrawStack;

        StackColor.SelectionChanged += StackColor_SelectionChanged;
        StackThick.TextChanged += StackThick_TextChanged;

        return this;
    }

    void StackColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        drawingColor = partMats.MatsColor(StackColor.SelectedItem.ToString()).ToColor();
    }

    void StackThick_TextChanged(object sender, TextChangedEventArgs e)
    {
        drawingThickness = int.Parse(StackThick.Text);
    }

    public static TabsDraw tabsDraw = new TabsDraw();
}