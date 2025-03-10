using System.Globalization;
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
        /// Обработка выражения в обратной польской записи
        /// </summary>
        /// <param name="expr">Выражение в ОПЗ</param>
        /// <returns>Корень дерева</returns>
        public Node ProcessExpression(string expr)
        {
            Stack<Node> stack = new Stack<Node>();
            string[] tokens = expr.Split(' ');

            foreach (var token in tokens)
            {
                if (IsVariable(token) || IsConstant(token))
                {
                    Node node = new Node { oper = token };
                    stack.Push(node);

                    if (IsVariable(token))
                        _variableTable.Add(token, "double");

                    else if (token.Contains('.'))
                        _variableTable.Add(token, "const float");

                    else _variableTable.Add(token, "const int");
                }
                else
                {
                    Node node = new Node { oper = token };
                    node.right = stack.Pop();
                    node.left = stack.Pop();
                    stack.Push(node);
                }
            }

            return stack.Pop();
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
                if (node.oper == "*")
                {
                    GenerateAssemblyCode(node.right, assemblyCode);
                    assemblyCode.Add($"STORE ${node.level}");
                    GenerateAssemblyCode(node.left, assemblyCode);
                    assemblyCode.Add($"MPY ${node.level}");
                }
                else if (node.oper == "+")
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
                if (node.oper == "=" || node.oper == "+" || node.oper == "*")
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
        public void OptimizeAssemblyCode(List<string> assemblyCode)
        {
            for (int i = 0; i < assemblyCode.Count; i++)
            {
                string current = assemblyCode[i];

                // Правила 1 и 2
                if (current.StartsWith("LOAD") && i + 1 < assemblyCode.Count)
                {
                    string next = assemblyCode[i + 1];
                    if (next.StartsWith("ADD") || next.StartsWith("MPY"))
                    {
                        string loadValue = current.Split(' ')[1];
                        string operationValue = next.Split(' ')[1];
                        string newCurrent = $"LOAD {operationValue}";
                        string newNext = $"{next.Split(' ')[0]} {loadValue}";
                        assemblyCode[i] = newCurrent;
                        assemblyCode[i + 1] = newNext;
                        i++;
                    }
                }

                // Правило 3
                if (current.StartsWith("STORE") && i + 1 < assemblyCode.Count)
                {
                    string next = assemblyCode[i + 1];
                    if (next.StartsWith("LOAD") && current.Split(' ')[1] == next.Split(' ')[1])
                    {
                        assemblyCode.RemoveAt(i);
                        assemblyCode.RemoveAt(i);
                        i--;
                    }
                }

                // Правило 4
                if (current.StartsWith("LOAD") && i + 1 < assemblyCode.Count)
                {
                    string next = assemblyCode[i + 1];
                    if (next.StartsWith("STORE") && i + 2 < assemblyCode.Count && assemblyCode[i + 2].StartsWith("LOAD"))
                    {
                        string loadValue = current.Split(' ')[1];
                        string storeValue = next.Split(' ')[1];

                        // Замена вхождений b на a
                        for (int j = i + 2; j < assemblyCode.Count; j++)
                        {
                            if (assemblyCode[j].Contains(storeValue))
                            {
                                assemblyCode[j] = assemblyCode[j].Replace(storeValue, loadValue);
                            }
                        }
                        assemblyCode.RemoveAt(i);
                        assemblyCode.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        #region OutputFunctions

        /// <summary>
        /// Вывод дерева в файл
        /// </summary>
        public void PrintTreeToFile(Node node, StreamWriter writer, int level = 0)
        {
            if (node == null) return;

            PrintTreeToFile(node.right, writer, level + 1);
            writer.WriteLine(new string(' ', level * 4) + node.oper);
            PrintTreeToFile(node.left, writer, level + 1);
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
                //writer.WriteLine("Дерево:\n");
                //PrintTreeToFile(root, writer, 0);

                writer.WriteLine("\nТаблица имен:");
                foreach (var variable in _variableTable)
                    writer.WriteLine($"{variable.Key} : {variable.Value}");

                writer.WriteLine("\nНеоптимизированный код:");

                GenerateAssemblyCode(root, assemblyCode);

                foreach (var codeStr in assemblyCode)
                    writer.WriteLine($"{codeStr}");

                writer.WriteLine("\nОптимизированный код:");

                OptimizeAssemblyCode(assemblyCode);

                foreach (var codeStr in assemblyCode)
                    writer.WriteLine($"{codeStr}");
            }
        }

        #endregion

        #endregion

        #region PrivateFunctions

        /// <summary>
        /// Проверка на то, являелся ли подстрока переменной
        /// </summary>
        /// <param name="expr">Строка или продстрока</param>
        private bool IsVariable(string expr)
        {
            return !string.IsNullOrEmpty(expr) && char.IsLetter(expr[0]);
        }

        /// <summary>
        /// Проверка на то, является ли строка константой
        /// </summary>
        /// <param name="expr">Строка или подстрока</param>
        private bool IsConstant(string expr)
        {
            return double.TryParse(expr, CultureInfo.InvariantCulture, out _);
        }

        #endregion

        private Dictionary<string, string> _variableTable = new Dictionary<string, string>(); // Таблица имен

    }
}