using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using static ModsRigs;
using static ViewRoom;

class TabsRigs : TabItem
{
    public ListBox BoneList = new ListBox();
    public Slider TXSlider = new Slider(); // translatie
    public Slider TYSlider = new Slider();
    public Slider TZSlider = new Slider();
    public Slider RXSlider = new Slider(); // rotatie
    public Slider RYSlider = new Slider();
    public Slider RZSlider = new Slider();
    public Slider SXSlider = new Slider(); // schaal
    public Slider SYSlider = new Slider();
    public Slider SZSlider = new Slider();
    public TextBox PoseText = new TextBox();

    public TabItem RigsInit()
    {
        BoneList.Height = 200;
        BoneList.BorderThickness = new Thickness(0);

        StackPanel RigsStack = new StackPanel();

        RigsStack.Children.Add(BoneList);

        TXSlider.Minimum = -10;
        TXSlider.Maximum = 10;
        TYSlider.Minimum = -10;
        TYSlider.Maximum = 10;
        TZSlider.Minimum = -10;
        TZSlider.Maximum = 10;
        RXSlider.Minimum = -180;
        RXSlider.Maximum = 180;
        RYSlider.Minimum = -180;
        RYSlider.Maximum = 180;
        RZSlider.Minimum = -180;
        RZSlider.Maximum = 180;
        SXSlider.Minimum = 0;
        SXSlider.Maximum = 10;
        SYSlider.Minimum = 0;
        SYSlider.Maximum = 10;
        SZSlider.Minimum = 0;
        SZSlider.Maximum = 10;

        TXSlider.Value = 0;
        TYSlider.Value = 0;
        TZSlider.Value = 0;
        RXSlider.Value = 0;
        RYSlider.Value = 0;
        RZSlider.Value = 0;
        SXSlider.Value = 0;
        SYSlider.Value = 0;
        SZSlider.Value = 0;

        RigsStack.Children.Add(TXSlider);
        RigsStack.Children.Add(TYSlider);
        RigsStack.Children.Add(TZSlider);
        RigsStack.Children.Add(RXSlider);
        RigsStack.Children.Add(RYSlider);
        RigsStack.Children.Add(RZSlider);
        RigsStack.Children.Add(SXSlider);
        RigsStack.Children.Add(SYSlider);
        RigsStack.Children.Add(SZSlider);
        RigsStack.Children.Add(PoseText);

        Header = "Rigs";
        Background = Brushes.Black;
        Foreground = Brushes.White;
        Content = RigsStack;

        TXSlider.ValueChanged += TXSlider_ValueChanged;
        TYSlider.ValueChanged += TYSlider_ValueChanged;
        TZSlider.ValueChanged += TZSlider_ValueChanged;
        RXSlider.ValueChanged += RXSlider_ValueChanged;
        RYSlider.ValueChanged += RYSlider_ValueChanged;
        RZSlider.ValueChanged += RZSlider_ValueChanged;
        SXSlider.ValueChanged += SXSlider_ValueChanged;
        SYSlider.ValueChanged += SYSlider_ValueChanged;
        SZSlider.ValueChanged += SZSlider_ValueChanged;

        BoneList.PreviewMouseUp += BoneList_PreviewMouseUp;
        return this;
    }

    public void BoneList_PreviewMouseUp(object sender, EventArgs e)
    {
        if (BoneList.SelectedIndex == -1) return;
        SE = false;
        string S = BoneList.SelectedItem.ToString();
        int I = S.IndexOf(',');
        int.TryParse(S.Substring(0, I), out Posed.B);
        if (Posed.R == 0) Posed.R = 1;

        TXSlider.Value = 0;
        TYSlider.Value = 0;
        TZSlider.Value = 0;
        RXSlider.Value = 0;
        RYSlider.Value = 0;
        RZSlider.Value = 0;
        SXSlider.Value = 1;
        SYSlider.Value = 1;
        SZSlider.Value = 1;

        SE = true;

        modsRigs.PoseBone();
    }

    private void TXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        modsRigs.PoseBone();
    }

    private void TYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        modsRigs.PoseBone();
    }

    private void TZSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        modsRigs.PoseBone();
    }

    private void RXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        modsRigs.PoseBone();
    }

    private void RYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        modsRigs.PoseBone();
    }

    private void RZSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        modsRigs.PoseBone();
    }

    private void SXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        modsRigs.PoseBone();
    }

    private void SYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        modsRigs.PoseBone();
    }

    private void SZSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        modsRigs.PoseBone();
    }

    public static TabsRigs tabsRigs = new TabsRigs();
}