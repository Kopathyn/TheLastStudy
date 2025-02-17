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
        bool reuslt = DFA.Run(str);

        if (reuslt)
        {
            Node node = new();
            ExpressionTree expressionTree = new ExpressionTree();

            expressionTree.ProcessExpression(node, str);

            expressionTree.PrintAllInfo(outputPath, node);
        }

        Process.Start("explorer.exe", outputPath);
    }
}