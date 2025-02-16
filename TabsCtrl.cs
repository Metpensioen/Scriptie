using Microsoft.VisualBasic; // nodig voor Asc

using System;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using static GridView;
using static TabsFile;
using static TextFunctions;
using static TextParser;
using static ViewText;

class TabsCtrl : TabItem
{
    public StackPanel CtrlStack = new StackPanel();
    public TextBox CtrlView = new TextBox();
    public TextBox CtrlIn = new TextBox();
    public SerialPort CtrlPort = new SerialPort();
    public byte[] Mem = new byte[0x10000];

    public TabItem CtrlInit()
    {
        CtrlStack.Children.Add(CtrlView);
        CtrlStack.Children.Add(CtrlIn);

        Background = Brushes.Black;
        Foreground = Brushes.White;

        Header = "Ctrl";
        Content = CtrlStack;

        PreviewKeyUp += CtrlKey;
        CtrlPort.DataReceived += CtrlRead;

        return this;
    }

    public void CtrlInput() // definieert een ingang
    {
        // W[0] = "input"
        // V[1] = index
        // W[2] = type
        // V[3] = adres

    }

    public void CtrlKey(object sender, KeyEventArgs e) // als een toets wordt losgelaten
    {
        if (e.Key == Key.Enter)
        {
            byte[] B = new byte[1];

            B[0] = (byte)Strings.Asc(CtrlView.Text[0]);
            CtrlSend(B, 1);
        }
    }

    public void CtrlMem()
    {
        // W[0] = "mem"
        // V[1] = grootte
        // W[2] = [hex bestand]

        int N = (int)V[1];
        string S;
        byte T;
        int H;

        for (int I = 0; I < N; I++)
        {
            Mem[I] = 0;
        }

        if (W[2] != "")
        {
            string Hex = File.ReadAllText(GetFileAddress(W[2])).Trim();
            H = Hex.Length / 2;

            for (int I = 0; I < H; I++)
            {
                S = "0x" + Hex.Substring(I * 2, 2);
                Mem[I] = Convert.ToByte(S, 16);
            }
        }


        N /= 16;

        gridView.Children.Clear();
        gridView.Children.Add(viewText);

        for (int I = 0; I < N; I++)
        {
            S = HexW(I * 16) + " ";
            
            for (int J = 0; J < 16; J++)
            {
                S += HexB(Mem[I * 16 + J]) + " ";
            }
            S += " | ";
            
            for (int J = 0; J < 16; J++)
            {
                T = Mem[I * 16 + J];
                if (T > 19 && T < 127)
                {
                    S += Convert.ToChar(T);
                }
                else
                {
                    S += ".";
                }
            }
            viewText.AppendText(S + "\n");
        }
    }


    public void CtrlOpen() // opent een compoort en start het besturen daarvan
    {
        // W[0] = "control"
        // V[1] = compoort nummer
        // V[2] = baudrate
        // V[3] = aantal stopbits

        if (V[3] == 0) V[3] = 1;

        if (CtrlPort.IsOpen) CtrlPort.Close();
        CtrlPort.PortName = "com" + (int)V[1];
        CtrlPort.BaudRate = (int)V[2];
        CtrlPort.DataBits = 8;
        CtrlPort.Parity = Parity.None;
        CtrlPort.StopBits = (StopBits)V[3];
        CtrlPort.Handshake = Handshake.None;
        CtrlPort.ReadTimeout = 500;
        CtrlPort.ReceivedBytesThreshold = 1;
        try
        {
            CtrlPort.Open();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        tabsCtrl.IsSelected = true;
    }

    public void CtrlOutput()
    {
        // W[0] = "output"
        // V[1] = index
        // W[2] = type
        // V[3] = adres

    }

    public void CtrlRead(object sender, SerialDataReceivedEventArgs e)
    {
        if (CtrlPort.IsOpen)
        {
            this.Dispatcher.Invoke(() =>
            {
                CtrlIn.Text = CtrlPort.ReadByte().ToString();
            });
        }
    }

    public void CtrlRung()
    {
        // W[0] = "rung"
        // W[1] = definitie

    }

    public void CtrlSend(byte[] B, int N) // stuurt 1 of meer bytes naar de compoort
    {
        if (CtrlPort.IsOpen)
        {
            CtrlPort.Write(B, 0, N);
        }
    }

    public static TabsCtrl tabsCtrl = new TabsCtrl();
}