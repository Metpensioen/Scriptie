using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using static EditText;
using static GridView;
using static TextParser;

class ViewText : TextBox
{
    public void TextStyle(TextBox T)
    {
        T.BorderThickness = new Thickness(0);
        T.Margin = new Thickness(8, 0, 0, 0);
        T.FontFamily = new FontFamily("Consolas");
        T.FontSize = 16.0;
        T.AcceptsReturn = true;
        T.TextWrapping = TextWrapping.Wrap; //TextWrapping.NoWrap; //TextWrapping.Wrap;
        T.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        T.Background = Brushes.Transparent;
        T.Foreground = Brushes.White;
        T.CaretBrush = Brushes.Yellow;
        T.SelectionBrush = Brushes.Yellow;
        T.SelectionOpacity = 0.5; // default = 0.4
    }

    public TextBox TextInit() // Start het tekst veld
    {
        TextStyle(this);

        return this;
    }

    public void TextOpen()
    {
        if (!gridView.Children.Contains(this))
        {
            gridView.Children.Clear();
            gridView.Children.Add(this);
        }
    }

    public void TextType(string T) // tekst schrijven
    {
        // W[0] = "type"
        // W[1] = tekst

        TextOpen();

        int n = T.Length;

        for (int i = 0; i < n; i++)
        {
            this.AppendText(T[i].ToString());
            textParser.ParserWait(100);
        }

        this.AppendText("\n");
    }

    public void TextLoad()
    {
        TextOpen();

        Text = File.ReadAllText(textFile);
    }

    public static ViewText viewText = new ViewText();
}