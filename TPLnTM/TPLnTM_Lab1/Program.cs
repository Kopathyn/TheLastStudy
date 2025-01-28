using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TPLnTM_Lab1;

public class Program
{
    static void Main(string[] args)
    {
        string inputPath = @"..\..\..\input.txt";
        string outputPath = @"..\..\..\output.txt";

        string str = File.ReadAllText(inputPath);

        DeterministicFiniteAutomatonWithStack DFA = new(outputPath);

        bool reuslt = DFA.Run(str);

        if (reuslt)
        {
            Node node = new();
            ExpressionTree expressionTree = new ExpressionTree();

            expressionTree.ProcessExpression(node, str);

            expressionTree.OutputAll(outputPath, node);


        }
         
        Process.Start("explorer.exe", outputPath);
    }
}