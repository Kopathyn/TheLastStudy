using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TPLnTM_Lab2;

class Program
{
    static void Main()
    {
        string inputPath = @"..\..\..\input.txt";
        string outputPath = @"..\..\..\output.txt";

        string input = File.ReadAllText(inputPath);

        bool result = RegularExpression.CheckString(input);

        if (result)
        {
            Node node = new();
            ExpressionTree expressionTree = new ExpressionTree();

            expressionTree.ProcessExpression(node, input);

            //using (StreamWriter writer = new StreamWriter(outputPath))
            //{
            //    writer.WriteLine("Дерево имен");
            //    expressionTree.PrintTreeToFile(node, writer, 0);
            //}

            expressionTree.PrintVariableTableToFile(outputPath);
        }
        else
        {
            File.WriteAllText(outputPath, "Некорректное выражение.");
        }

        Process.Start("explorer.exe", outputPath);
    }
}