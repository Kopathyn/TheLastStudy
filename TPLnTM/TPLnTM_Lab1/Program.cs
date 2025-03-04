using System.Diagnostics;
using TPLnTM_Lab1;

public class Program
{
    static void Main(string[] args)
    {
        string inputPath = @"..\..\..\input.txt";
        string outputPath = @"..\..\..\output.txt";

        string str = File.ReadAllText(inputPath);

        DeterministicFiniteAutomatonWithStack DFA = new(outputPath);

        Console.WriteLine($"Обработка строки: {str}");
        string result = DFA.Run(str);

        if (result != null)
        {
            Node node = new();
            ExpressionTree expressionTree = new ExpressionTree();

            node = expressionTree.ProcessExpression(result);

            expressionTree.PrintAllInfo(outputPath, node);
        }

        Process.Start("explorer.exe", outputPath);
    }
}