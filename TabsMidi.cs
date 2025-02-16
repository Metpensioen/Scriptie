using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media;
using System;

class TabsMidi : TabItem
{
    [DllImport("winmm.dll", EntryPoint = "midiInGetNumDevs", CharSet = CharSet.Ansi)]
    public static extern int MidiInGetNumDevs();

    [DllImport("winmm.dll", EntryPoint = "midiOutGetNumDevs", CharSet = CharSet.Ansi)]
    public static extern int MidiOutGetNumDevs();

    [DllImport("winmm.dll", EntryPoint = "midiInGetDevCaps", CharSet = CharSet.Ansi)]
    public static extern int MidiInGetDevCaps(uint uDeviceID, ref MidiInCaps InCaps, int cbMidiInCaps);

    [DllImport("winmm.dll", EntryPoint = "midiOutGetDevCaps", CharSet = CharSet.Ansi)]
    public static extern int MidiOutGetDevCaps(uint uDeviceID, ref MidiOutCaps OutCaps, int cbMidiOutCaps);

    [DllImport("winmm.dll", EntryPoint = "midiInOpen", CharSet = CharSet.Ansi)]
    public static extern int MidiInOpen(ref int lphMidiIn, uint uDeviceID, MidiInCallback dwCallback, int dwInstance, int dwFlags);

    [DllImport("winmm.dll", EntryPoint = "midiInStart", CharSet = CharSet.Ansi)]
    public static extern int MidiInStart(int hMidiIn);

    [DllImport("winmm.dll", EntryPoint = "midiOutClose", CharSet = CharSet.Ansi)]
    public static extern int MidiOutClose(int hMidiIn);

    [DllImport("winmm.dll", EntryPoint = "midiOutOpen", CharSet = CharSet.Ansi)]
    public static extern int MidiOutOpen(ref int lphMidiIn, uint uDeviceID, MidiInCallback dwCallback, int dwInstance, int dwFlags);

    [DllImport("winmm.dll")]
    private static extern int midiOutShortMsg(int handle, int message);

    public struct MidiInCaps
    {
        public short wMid;
        public short wPid;
        public int vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;
        public int dwSupport;
    }

    public struct MidiOutCaps
    {
        public short wMid;
        public short wPid;
        public int vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;
        public int dwSupport;
    }

    public static int hMidIn;
    public static int OutHandle;
    public static object in_lock_obj = new Object();
    public delegate int MidiInCallback(int hMidiIn, int wMsg, int dwInstance, int dwParam1, int dwParam2);
    public static TextBox MidiInText = new TextBox();
    public static TextBox MidiOutText = new TextBox();
    //public static byte StatusByte;
    //public static byte DataByte1;
    //public static byte DataByte2;
    public ListBox MidiInList = new ListBox();
    public ListBox MidiOutList = new ListBox();
    public static MidiInCallback ptrCallback = new MidiInCallback(InCallback);

    public TabItem MidiInit()
    {
        StackPanel MidiStack = new StackPanel();

        Label MidiInLabel = new Label();
        Label MidiOutLabel = new Label();

        MidiInLabel.Content = "input";
        MidiOutLabel.Content = "output";

        MidiStack.Children.Add(MidiInLabel);
        MidiStack.Children.Add(MidiInList);
        MidiStack.Children.Add(MidiInText);
        MidiStack.Children.Add(MidiOutLabel);
        MidiStack.Children.Add(MidiOutList);
        MidiStack.Children.Add(MidiOutText);

        Content = MidiStack;
        Header = "Midi";
        Background = Brushes.Black;
        Foreground = Brushes.White;

        return this;
    }

    public void MidiOpen()
    {
        // W[0] = ""midi""

        MidiInCaps InCaps = new MidiInCaps();
        MidiOutCaps OutCaps = new MidiOutCaps();

        IsSelected = true;

        MidiInList.Items.Clear();
        int N = MidiInGetNumDevs();
        for (int I = 0; I < N; I++)
        {
            MidiInGetDevCaps((uint)I, ref InCaps, Marshal.SizeOf(typeof(MidiInCaps)));
            MidiInList.Items.Add(InCaps.szPname);
        }
        MidiInList.SelectedIndex = 0;

        MidiOutList.Items.Clear();
        N = MidiOutGetNumDevs();
        for (int I = 0; I < N; I++)
        {
            MidiOutGetDevCaps((uint)I, ref OutCaps, Marshal.SizeOf(typeof(MidiOutCaps)));
            MidiOutList.Items.Add(OutCaps.szPname);
        }

        MidiOutList.SelectedIndex = 0;

        MidiInOpen(ref hMidIn, 0, ptrCallback, 0, 0x30000 | 0x20);
        MidiInStart(hMidIn);

        MidiOutClose(OutHandle);
        MidiOutOpen(ref OutHandle, 0, null, 0, 0x30000);
    }

    public static byte[] data = new byte[4];
    public static uint msg = BitConverter.ToUInt32(data, 0);

    private static int InCallback(int hMidiIn, int wMsg, int dwInstance, int dwParam1, int dwParam2)
    {
        //lock (in_lock_obj)
        {
            data[0] = (byte)(dwParam1 & 0xFF); // 0x90 = 144 = note on
            data[1] = (byte)((dwParam1 & 0xFF00) >> 8);
            data[2] = (byte)((dwParam1 & 0xFF0000) >> 16);
            if (data[2] > 0) data[2] = 127;
            //if (DataByte2 > 0) DataByte2 = 127; // maximaal volume
            //midiOutShortMsg(OutHandle, (int)msg);
            midiOutShortMsg(OutHandle, dwParam1);
            //Console.WriteLine(wMsg + " | " + dwParam1);
            View(dwParam1);
        }
        return 0;
    }

    public static void View(int I)
    {
        tabsMidi.Dispatcher.Invoke(() =>
        {
            MidiInText.Text = I.ToString();
        });
    }

    public static TabsMidi tabsMidi = new TabsMidi();
}