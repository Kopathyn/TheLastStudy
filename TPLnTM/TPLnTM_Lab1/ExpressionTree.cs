using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPLnTM_Lab1;
using System.Text.RegularExpressions;
using System.Numerics;

namespace TPLnTM_Lab1
{
    /// <summary>
    /// Узел дерева
    /// </summary>
    public class Node
    {
        public string oper; // Операция
        public int level;   // Уровень узла
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
            // Проверяем, заключено ли выражение в скобки
            if (expr[0] == '(' && expr[expr.Length - 1] == ')')
            {
                // Удаляем скобки и продолжаем обработку
                ProcessExpression(node, expr.Substring(1, expr.Length - 2));
                return;
            }

            // Ищем операцию с наивысшим приоритетом
            int pos = -1;
            int priority = 4; // Начальное значение приоритета (выше, чем у всех операций)

            // Ищем операцию присваивания, сложения и умножения
            for (int i = 0; i < expr.Length; i++)
            {
                char currentChar = expr[i];
                int currentPriority = GetPriority(currentChar);

                // Проверяем, не заключен ли знак операции в скобки
                if (currentPriority < priority && !IsInsideBrackets(expr, i))
                {
                    pos = i;
                    priority = currentPriority;
                }
            }

            // Если операция найдена
            if (pos != -1)
            {
                node.oper = expr[pos].ToString();
                node.left = new Node();
                node.right = new Node();

                // Рекурсивно обрабатываем левую и правую части
                ProcessExpression(node.left, expr.Substring(0, pos));
                ProcessExpression(node.right, expr.Substring(pos + 1));
            }
            else
            {
                // Если операция не найдена, это лист дерева
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
        /// Генерация неоптимизированного кода выражения
        /// </summary>
        public void GenerateAssemblyCode(Node node, StreamWriter writer)
        {
            if (node == null) return;

            // Обработка правого поддерева
            GenerateAssemblyCode(node.right, writer);
            // Обработка левого поддерева
            GenerateAssemblyCode(node.left, writer);

            if (Regex.IsMatch(node.oper, @"[\\+\\-\\\\\\*\\=]"))
                return;

            if (IsVariable(node.oper))
                writer.WriteLine($"LOAD {node.oper}");
            else
                writer.WriteLine($"LOAD ={node.oper}");
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
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Дерево:\n");
                PrintTreeToFile(root, writer, 0);

                writer.WriteLine("Таблица имен:");
                foreach (var variable in _variableTable)
                    writer.WriteLine($"{variable.Key} : {variable.Value}");

                writer.WriteLine("\nНеоптимизированный код:");
                GenerateAssemblyCode(root, writer);
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