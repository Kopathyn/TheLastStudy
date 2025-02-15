using System.Linq;

namespace TPLnTM_Lab1
{
    /// <summary>
    /// Узел дерева
    /// </summary>
    public class Node
    {
        public string oper; // Операция
        public int level = 0;   // Уровень узла
        public Node left;   // Левое поддерево
        public Node right;  // Правое поддерево

        public Node()
        {
            left = null;
            right = null;
        }
    }

    /// <summary>
    /// Класс преобразования математического выражения в дерево
    /// </summary>
    public class ExpressionTree
    {

        #region PublicFunctions

        /// <summary>
        /// Обработка выражения
        /// </summary>
        /// <param name="node">Узел дерева</param>
        /// <param name="expr">Выражение или его часть</param>
        public void ProcessExpression(Node node, string expr)
        {
            node.level++;

            /* Проверка на скобки */

            if (expr[0] == '(' && expr[expr.Length - 1] == ')')
            {
                ProcessExpression(node, expr.Substring(1, expr.Length - 2));
                return;
            }

            /* Поиск знаков операции */

            int pos = -1; // Позиция операции
            int priority = 4; // Приоритет операции

            for (int i = 0; i < expr.Length; i++)
            {
                char currentChar = expr[i];
                int currentPriority = GetPriority(currentChar);

                if (currentPriority < priority && !IsInsideBrackets(expr, i))
                {
                    pos = i;
                    priority = currentPriority;
                }
            }

            if (pos != -1)
            {
                node.oper = expr[pos].ToString();
                node.left = new Node();
                node.right = new Node();

                ProcessExpression(node.left, expr.Substring(0, pos));
                ProcessExpression(node.right, expr.Substring(pos + 1));
            }
            else
            {
                node.oper = expr;

                if (IsVariable(expr))
                    _variableTable.Add(expr, "double");
                else
                {
                    if (expr.Contains('.'))
                        _variableTable.Add(expr, "const float");
                    else
                        _variableTable.Add(expr, "const int");
                }

                node.left = null;
                node.right = null;
            }
        }

        /// <summary>
        /// Генерация кода для мат. выражения
        /// </summary>
        public void GenerateAssemblyCode(Node node, List<string> assemblyCode)
        {
            if (node == null) return;

            // Обработка вершин
            if (node.left != null && node.right != null)
            {
                if (node.oper == "*" || node.oper == "/")
                {
                    GenerateAssemblyCode(node.right, assemblyCode);
                    assemblyCode.Add($"STORE ${node.level}");
                    GenerateAssemblyCode(node.left, assemblyCode);
                    assemblyCode.Add($"MPY ${node.level}");
                }
                else if (node.oper == "+" || node.oper == "-")
                {
                    GenerateAssemblyCode(node.right, assemblyCode);
                    assemblyCode.Add($"STORE ${node.level}");
                    GenerateAssemblyCode(node.left, assemblyCode);
                    assemblyCode.Add($"ADD ${node.level}");
                }
                else if (node.oper == "=")
                {
                    GenerateAssemblyCode(node.right, assemblyCode);
                    assemblyCode.Add($"STORE {node.left.oper}");
                }
            }
            else // Обработка листьев
            {
                if (node.oper == "=" || node.oper == "+" || node.oper == "-" || node.oper == "*" || node.oper == "/")
                    return;

                if (IsVariable(node.oper))
                    assemblyCode.Add($"LOAD {node.oper}");
                else
                    assemblyCode.Add($"LOAD ={node.oper}");
            }
        }

        /// <summary>
        /// Оптимизация сгенерированного кода
        /// </summary>
        public void OptimizeCode(List<string> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                string current = instructions[i];

                // Проверка на коммутативность для операций + и *
                if (current.StartsWith("LOAD") && i + 1 < instructions.Count)
                {
                    string next = instructions[i + 1];
                    if (next.StartsWith("ADD") || next.StartsWith("MPY"))
                    {
                        string loadValue = current.Split(' ')[1];
                        string operationValue = next.Split(' ')[1];
                        string newCurrent = $"LOAD {operationValue}";
                        string newNext = $"{next.Split(' ')[0]} {loadValue}";
                        instructions[i] = newCurrent;
                        instructions[i + 1] = newNext;
                        i++; // Переходим к следующей паре команд
                    }
                }

                // Удаление последовательности STORE α; LOAD α;
                if (current.StartsWith("STORE") && i + 1 < instructions.Count)
                {
                    string next = instructions[i + 1];
                    if (next.StartsWith("LOAD") && current.Split(' ')[1] == next.Split(' ')[1])
                    {
                        instructions.RemoveAt(i); // Удаляем STORE
                        instructions.RemoveAt(i); // Удаляем LOAD
                        i--; // Корректируем индекс после удаления
                    }
                }

                // Удаление последовательности LOAD α; STORE β;
                if (current.StartsWith("LOAD") && i + 1 < instructions.Count)
                {
                    string next = instructions[i + 1];
                    if (next.StartsWith("STORE") && i + 2 < instructions.Count && instructions[i + 2].StartsWith("LOAD"))
                    {
                        string loadValue = current.Split(' ')[1];
                        string storeValue = next.Split(' ')[1];
                        // Заменяем все вхождения storeValue на loadValue
                        for (int j = i + 2; j < instructions.Count; j++)
                        {
                            if (instructions[j].Contains(storeValue))
                            {
                                instructions[j] = instructions[j].Replace(storeValue, loadValue);
                            }
                        }
                        instructions.RemoveAt(i); // Удаляем LOAD
                        instructions.RemoveAt(i); // Удаляем STORE
                        i--; // Корректируем индекс после удаления
                    }
                }
            }
        }

            #region OutputFunctions

        /// <summary>
        /// Вывод дерева в файл
        /// </summary>
            public void PrintTreeToFile(Node node, StreamWriter writer, int level)
        {
            if (node == null) return;

            PrintTreeToFile(node.right, writer, level + 1);
            writer.WriteLine(new string(' ', level * 4) + node.oper);
            PrintTreeToFile(node.left, writer, level + 1);
        }

        /// <summary>
        /// Вывод таблицы имен в файл
        /// </summary>
        /// <param name="filePath">Путь до файла куда записывать</param>
        public void PrintVariableTableToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Таблица имен:");
                foreach (var variable in _variableTable)
                {
                    writer.WriteLine($"{variable.Key} : {variable.Value}");
                }
            }
        }

        /// <summary>
        /// Объединение всех выводов в один
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="root">Корень математического дерева</param>
        public void PrintAllInfo(string filePath, Node root)
        {
            List<string> assemblyCode = new();
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Дерево:\n");
                PrintTreeToFile(root, writer, 0);

                writer.WriteLine("Таблица имен:");
                foreach (var variable in _variableTable)
                    writer.WriteLine($"{variable.Key} : {variable.Value}");

                writer.WriteLine("\nНеоптимизированный код:");

                GenerateAssemblyCode(root, assemblyCode);

                foreach (var codeStr in assemblyCode)
                    writer.WriteLine($"{codeStr}");

                writer.WriteLine("\nОптимизированный код:");

                OptimizeCode(assemblyCode);

                foreach (var codeStr in assemblyCode)
                    writer.WriteLine($"{codeStr}");
            }
        }

        #endregion

        #endregion

        #region PrivateFunctions

        /// <summary>
        /// Получение приоритета мат. знака
        /// </summary>
        /// <param name="operation">Мат. знак</param>
        /// <returns>Значение приоритета</returns>
        private int GetPriority(char operation)
        {
            switch (operation)
            {
                case '=':
                    return 1;
                case '+':
                    return 2;
                case '-':
                    return 2;
                case '*':
                    return 3;
                case '\\':
                    return 3;
                default:
                    return 4;
            }
        }

        /// <summary>
        /// Проверка на то, является ли выражение в скобках
        /// </summary>
        /// <param name="expr">Выражение</param>
        /// <param name="index">Индекс, до какого необходимо проверить</param>
        private bool IsInsideBrackets(string expr, int index)
        {
            int openBrackets = 0;
            for (int i = 0; i < index; i++)
            {
                if (expr[i] == '(') openBrackets++;
                if (expr[i] == ')') openBrackets--;
            }
            return openBrackets > 0;
        }

        /// <summary>
        /// Проверка на то, являелся ли подстрока переменной
        /// </summary>
        /// <param name="expr">Строка или продстрока</param>
        private bool IsVariable(string expr)
        {
            // Проверяем, является ли выражение идентификатором переменной
            return !string.IsNullOrEmpty(expr) && char.IsLetter(expr[0]);
        }

        #endregion

        private Dictionary<string, string> _variableTable = new Dictionary<string, string>(); // Таблица имен

    }
}