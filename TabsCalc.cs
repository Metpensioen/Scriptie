using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using static TabsCalc;


class TabsCalc : TabItem
{
    public TextBox calcView = new TextBox();
    public static double[] calcVars = new double[24];

    public TabItem CalcInit()
    {
        StackPanel CalcStack = new StackPanel();

        CalcStack.Children.Add(calcView);

        Background = Brushes.Black;
        Foreground = Brushes.White;
        Header = "Calc";
        Content = CalcStack;

        PreviewKeyUp += CalcKey;

        return this;
    }

    public double CalcCalc(string S)
    {
        S += ";";
        int K = 0;
        int L = 0;
        double[] F = new double[8]; // waardes
        string P = "+-*/^"; // operators
        string[] O = new string[8]; // operators
        string[] Q = new string[8]; // waardes
        string N = "0123456789."; // cijfers

        for (int I = 0; I < S.Length; I++) // voor alle karakters in S
        {
            if (N.Contains(S.Substring(I, 1))) // als het karakter in N voorkomt is het een cijfer
            {
                Q[K] += S[I]; // voeg het cijfer toe aan getal Q
            }
            else // als het karakter geen cijfer is
            {
                if (Q[K] != null) // als Q een waarde heeft
                {
                    double.TryParse(Q[K], out F[K]); // converteer waarde string Q naar double F
                    K++;
                }
                if (S[I] == 'v')
                {
                    try
                    {
                        F[L] = calcVars[int.Parse(S.Substring(I + 1, 1))];
                    }
                    catch
                    {
                        return 0;
                    }
                    K++;
                    I++;
                }
                else
                {
                    if (S[I] != ';') O[L] += S[I];
                    if (O[L] != null) if (P.Contains(O[L])) L++;
                    if (O[L] == "sin" || O[L] == "cos" || O[L] == "atan")
                    {
                        K++;
                        L++;
                    }
                }
            }
        }

        double R = F[0]; // resultaat
        L = 0;
        while (O[L] != null) // zolang er operators zijn
        {
            switch (O[L])
            {
                case "+": R += F[L + 1]; break;
                case "-": R -= F[L + 1]; break;
                case "*": R *= F[L + 1]; break;
                case "/": R /= F[L + 1]; break;
                case "^": R = Math.Pow(R, F[L + 1]); break;
                case "sin": R = Math.Sin(F[L + 1] * Math.PI / 180); break;
                case "cos": R = Math.Cos(F[L + 1] * Math.PI / 180); break;
                case "atan": R = Math.Atan(F[L + 1]) * 180 / Math.PI; break;
            }
            L++;
        }

        return R;
    }

    public void CalcKey(object sender, KeyEventArgs e) // als op de enter toets wordt gedrukt
    {
        if (e.Key == Key.Enter)
        {
            calcView.Text = CalcCalc(calcView.Text).ToString(); // bereken de regel uit het tekstvlak
        }
    }

    public static TabsCalc tabsCalc = new TabsCalc();

    // funkties

    public static double DXC(double Hyp, double Ang) // bereken x uit de cosinus van de hoek en de schuine zijde
    {
        return Hyp * Math.Cos(Rad(Ang));
    }

    public static double DYS(double Hyp, double Ang) // bereken de overstaande y uit de sinus van de hoek en de schuine zijde
    {
        return Hyp * Math.Sin(Rad(Ang));
    }

    public static double DYT(double a, double Ang) // bereken de overstaande zijde y uit de tangenes van de hoek en de aanliggende zijde
    {
        return a * Math.Tan(Rad(Ang));
    }

    public static double Deg(double Rad) // converteer radialen naar graden
    {
        return Rad * 180 / Math.PI;
    }

    public static double Rad(double Deg) // converteer graden naar radialen
    {
        return Deg * (Single)Math.PI / 180;
    }

}