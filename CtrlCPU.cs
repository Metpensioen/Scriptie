using System;

using static GridView;
using static TextParser;
using static ViewText;

class CtrlCPU
{
    public string CPU_Type = "";

    public Byte CPU_A = 0;
    public Byte CPU_B = 0;
    public Byte CPU_C = 0;
    public Byte CPU_D = 0;
    public Byte CPU_E = 0;
    public Byte CPU_F = 0;
    public Byte CPU_H = 0;
    public Byte CPU_I = 0;
    public Byte CPU_L = 0;

    public Byte CPU_A2 = 0;
    public Byte CPU_B2 = 0;
    public Byte CPU_C2 = 0;
    public Byte CPU_D2 = 0;
    public Byte CPU_E2 = 0;
    public Byte CPU_F2 = 0;
    public Byte CPU_H2 = 0;
    public Byte CPU_L2 = 0;

    public UInt16 CPU_SP = 0;
    public UInt16 CPU_PC = 0;
    public UInt16 CPU_IX = 0;
    public UInt16 CPU_IY = 0;

    public Byte[] CPU_MEM = new Byte[0xFFFF];

    public void CPU_Open()
    {
        // W[0] = ".CPU"
        // W[1] = type

        CPU_Type = W[1];

        gridView.Children.Clear();
        gridView.Children.Add(viewText);
        viewText.Clear();
        viewText.AppendText(CPU_Type + "\n");
    }

    public void CPU_ADD()
    {
        // W[0] = "add"
        // W[1] = register of geheugenadres
        // W[2] = register, geheugenadres of waarde

        if (W[1] == "a,")
        {
            if (W[2].StartsWith("$"))
            {
                CPU_A += Convert.ToByte("0x" + W[2].Substring(1), 16);
                viewText.AppendText("A = " + CPU_A + "\n");
            }
        }
    }

    public void CPU_DI()
    {

    }

    public void CPU_EI()
    {

    }

    public void CPU_EXX()
    {

    }

    public void CPU_HALT()
    {

    }

    public void CPU_LD()
    {
        // W[0] = "ld"
        // W[1] = register of geheugenadres
        // W[2] = register, geheugenadres of waarde

        if (W[1] == "a,")
        {
            if (W[2].StartsWith("$"))
            {
                string S = "0x" + W[2].Substring(1);
                CPU_A = Convert.ToByte(S, 16);
                viewText.AppendText("A = " + CPU_A + "\n");
            }
        }
        else if (W[1].StartsWith("("))
        {
            Int32 I = Convert.ToInt32("0x" + W[1].Substring(2, 4), 16);
            CPU_MEM[I] = CPU_A;
            viewText.AppendText("MEM[" + I + "] = " + CPU_MEM[I] + "\n");
        }
    }

    public void CPU_LDIR()
    {

    }

    public void CPU_NOP()
    {

    }

    public void CPU_POP()
    {

    }
    
    public void CPU_PUSH()
    {

    }

    public void CPU_RET()
    {

    }

    public void CPU_RLA()
    {

    }



    public static CtrlCPU ctrlCPU = new CtrlCPU();
}