using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        string inputPath = @"..\..\..\input.txt";
        string outputPath = @"..\..\..\output.txt";

        string variableDescription = File.ReadAllText(inputPath);

        /*
         * ...
         * Some operations
         * ...
         */

        File.WriteAllText(outputPath, variableDescription);

        Process.Start("explorer.exe", outputPath);
    }
}