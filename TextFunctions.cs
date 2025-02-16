// funkties

using System;

public class TextFunctions
{
    public static string HexB(byte B) // converteer een byte naar een hexadecimale string
    {
        string H = "0123456789ABCDEF";

        return H[B / 16].ToString() + H[B % 16].ToString();
    }

    public static string HexW(int W) // converteer een woord naar een hexadecimale string
    {
        byte I = (byte)(W / 256);
        byte J = (byte)(W % 256);

        return HexB(I) + HexB(J);
    }

    public static string FL(double F) // vul een floating point links aan met spaties
    {
        string S = String.Format("{0:0.0000}", F);

        while (S.Length < 8) S = ' ' + S;

        return ',' + S;
    }

    public static string FSL(double F, int I, char C) // vul een floating point links aan met karakters
    {
        string S = F.ToString();

        while (S.Length < I) S = C + S;

        return S;
    }

    public static string SL(string S, char C, int I) // vul string links
    {
        while (S.Length < I) S = C + S;

        if (S.Length > I) S = S.Substring(0, I);

        return S;
    }

    public static string SR(string S, char C, int I) // vul string rechts
    {
        while (S.Length < I) S += C;

        if (S.Length > I) S = S.Substring(0, I);

        return S;
    }
}