using System.Diagnostics;
using TPLnTM_Lab2;

class Program
{
    static void Main()
    {
        string inputPath = @"..\..\..\input.txt";
        string outputPath = @"..\..\..\output.txt";

        string input = File.ReadAllText(inputPath);

        string result = RegularExpression.CheckString(input);

        Console.WriteLine($"Обработка строки: {input}");
        if (result != null)
        {
            Node node = new();
            ExpressionTree expressionTree = new ExpressionTree();

            node = expressionTree.ProcessExpression(result);

            expressionTree.PrintAllInfo(outputPath, node);
        }
        else
        {
            File.WriteAllText(outputPath, "Некорректное выражение.");
        }

        Process.Start("explorer.exe", outputPath);
    }
}