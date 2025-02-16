using static EditText;
using static TextParser;
using static TabsFile;

class ShowGame
{
    public string[] gameChoice = new string[8];

    public void GameChoice()
    {
        // W[0] = "choice"
        // V[1] = keuze nummer
        // W[2] = keuze tekst
        // W[3] = vervolg 

        int I = (int)V[1];

        if (I < 8) gameChoice[(int)V[1]] = W[2];
    }

    public void GameGoto(int N)
    {
        string F = gameChoice[N - 34]; // key.1 = 35

        F = FileRoot() + GetFilePath(textFile) + @"\" + F + ".txt";
        editText.TextOpen(F);
        parserRuns = false;
        textParser.ParserWait(10);
        textParser.ParserOpen(F, 0, -1);
    }

    public static ShowGame showGame = new ShowGame();
}