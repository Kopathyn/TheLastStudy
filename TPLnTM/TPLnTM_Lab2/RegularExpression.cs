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
            Regex regex = new Regex(@"
            ^\s*
            (?<variable>[A-Za-z][A-Za-z0-9]*)       
            \s*=\s*
            (?<expression>
                (?<number>
                    \d+\.\d+([eE][+]?\d+)? |
                    \d+[eE][+]?\d+      |
                    \b\d+\b(?!\.)
                )
                |
                (?<var>[A-Za-z][A-Za-z0-9]*)   
                |
                (?<operator>[*+()])             
                |
                (?<whitespace>\s+)              
            )+
            \s*$",
                RegexOptions.IgnorePatternWhitespace
            );

            Match formatMatch = regex.Match(str);
            if (!formatMatch.Success)
                return null;

            string variable = formatMatch.Groups["variable"].Value;

            var tokens = new List<string> { variable, "=" };

            // Обрабатываем захваченные группы выражения
            var numberCaptures = formatMatch.Groups["number"].Captures.Cast<Capture>();
            var varCaptures = formatMatch.Groups["var"].Captures.Cast<Capture>();
            var operatorCaptures = formatMatch.Groups["operator"].Captures.Cast<Capture>();

            // Объединяем и сортируем токены по позиции в строке
            var allCaptures = numberCaptures.Concat(varCaptures).Concat(operatorCaptures)
                                          .OrderBy(c => c.Index)
                                          .Select(c => c.Value);

            tokens.AddRange(allCaptures);

            return ConvertToOPZ(tokens);
        }

        /// <summary>
        /// Преобразование в обратную польскую запись (с учетом =)
        /// </summary>
        private static string ConvertToOPZ(List<string> tokens)
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
                    while (stack.Count > 0 && GetOperatorPriority(stack.Peek()) >= GetOperatorPriority(token))
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
                    return 3;
                case "+":
                    return 2;
                default:
                    return 0;
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