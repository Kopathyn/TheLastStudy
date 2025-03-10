using System;
using System.Text.RegularExpressions;

public class Program
{
    public static void Main()
    {
        string input = "cost = +";

        Regex regex = new Regex(@"
            ^\s*
            (?<double>[A-Za-z][A-Za-z0-9]*)   # Переменная
            \s*=\s*
            (
                (?:                           # Элементы выражения
                    \s*
                    (
                        (?<const_int>\b\d+\b(?!\.))      # Целые числа
                        |
                        (?<const_float>                  # Числа с плавающей точкой
                            \d+\.\d+([eE][+-]?\d+)?     # С дробной частью
                            |
                            \d+[eE][+]?\d+             # Экспоненциальный формат
                        )
                        |
                        (?<double>[A-Za-z][A-Za-z0-9]*)            # Переменные
                        |
                        [+*()]                        # Операторы и скобки
                        |
                        \s+                             # Пробелы
                    )
                )+
            )
            \s*$", RegexOptions.IgnorePatternWhitespace);
        Match match = regex.Match(input);

        if (match.Success)
        {
            Console.WriteLine($"Переменная (double):");
            foreach (Capture capture in match.Groups["double"].Captures)
                Console.WriteLine(capture.Value);

            Console.WriteLine("Целые числа (const int):");
            foreach (Capture capture in match.Groups["const_int"].Captures)
                Console.WriteLine(capture.Value);

            Console.WriteLine("Числа с плавающей точкой (const float):");
            foreach (Capture capture in match.Groups["const_float"].Captures)
                Console.WriteLine(capture.Value);
        }
        else
        {
            Console.WriteLine("Строка не соответствует формату.");
        }
    }
}