using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TPLnTM_Lab2
{
    public class RegularExpression
    {
        /// <summary>
        /// Функция проверки строки на "ПЕРЕМЕННАЯ = ВЫРАЖЕНИЕ"
        /// </summary>
        static public string CheckString(string str)
        {
            // Проверка общего формата строки
            Regex regex = new Regex(@"
            ^\s*
            (?<variable>[A-Za-z][A-Za-z0-9]*)       # Переменная слева
            \s*=\s*
            (?<expression>                           # Выражение справа
                (
                    (?<number>
                        \d+\.\d+([eE][+-]?\d+)? |   # float
                        \d+[eE][+-]?\d+      |      # экспонента
                        \b\d+\b(?!\.)               # int
                    )
                    |
                    (?<variable>[A-Za-z][A-Za-z0-9]*)  # Переменные
                    |
                    (?<operator>[+*=()])             # Операторы и скобки
                    |
                    (?<whitespace>\s+)               # Пробелы (игнорируются)
                )+
            )
            \s*$",
                RegexOptions.IgnorePatternWhitespace
            );

            Match formatMatch = regex.Match(str);
            if (!formatMatch.Success) 
                return null;

            string variable = formatMatch.Groups[1].Value;
            string expression = formatMatch.Groups[2].Value;

            // Извлечение токенов из выражения
            Regex tokenRegex = new Regex(
                @"(?x)
                (?<number>  \d+\.\d+([eE][+]?\d+)?
                            | \d+[eE][+]?\d+
                            | \b\d+\b(?!\.)
                )
                | (?<variable> [A-Za-z][A-Za-z0-9]*)
                | (?<operator> [+*()])
                | (?<whitespace> \s+)
                ");

            var tokens = new List<string> { variable, "=" };

            foreach (Match m in tokenRegex.Matches(expression))
                if (m.Groups["number"].Success || m.Groups["variable"].Success || m.Groups["operator"].Success)
                    tokens.Add(m.Value);

            return ConvertToRPN(tokens);
        }

        /// <summary>
        /// Преобразование в обратную польскую запись
        /// </summary>
        private static string ConvertToRPN(List<string> tokens)
        {
            List<string> output = new List<string>();
            Stack<string> stack = new Stack<string>();

            foreach (string token in tokens)
            {
                if (token == "=")
                    stack.Push(token);

                else if (IsConstant(token) || IsVariable(token))
                    output.Add(token);

                else if (token == "(")
                    stack.Push(token);

                else if (token == ")")
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                        output.Add(stack.Pop());

                    stack.Pop();
                }
                else
                {
                    int currentPriority = GetOperatorPriority(token); 

                    while (stack.Count > 0 && GetOperatorPriority(stack.Peek()) >= currentPriority)
                        output.Add(stack.Pop());

                    stack.Push(token);
                }
            }

            while (stack.Count > 0)
                output.Add(stack.Pop());

            return string.Join(" ", output);
        }

        /// <summary>
        /// Метод для получения приоритета опереции
        /// </summary>
        /// <param name="op">Операция</param>
        /// <returns>Приоритет</returns>
        private static int GetOperatorPriority(string op)
        {
            switch (op)
            {
                case "*":
                    return 3; // Наивысший приоритет
                case "+":
                    return 2; // Средний приоритет
                default:
                    return 0; // Не оператор
            }
        }

        /// <summary>
        /// Проверка на то, являелся ли подстрока переменной
        /// </summary>
        /// <param name="expr">Строка или продстрока</param>
        private static bool IsVariable(string expr)
        {
            return !string.IsNullOrEmpty(expr) && char.IsLetter(expr[0]);
        }

        /// <summary>
        /// Проверка на то, является ли строка константой
        /// </summary>
        /// <param name="expr">Строка или подстрока</param>
        private static bool IsConstant(string expr)
        {
            return double.TryParse(expr, CultureInfo.InvariantCulture, out _);
        }            
    }
}