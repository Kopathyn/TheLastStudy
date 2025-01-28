using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TPLnTM_Lab2
{
    public class RegularExpression
    {
        /// <summary>
        /// Функция проверки строки на "ПЕРЕМЕННАЯ = ВЫРАЖЕНИЕ"
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Результат</returns>
        static public bool CheckString(string str)
        {
            string pattern = @"^([a-zA-Z_][a-zA-Z0-9_]*=([0-9]+(\.[0-9]+)?|[a-zA-Z_][a-zA-Z0-9_]*|(\([^\)]*\))|([\+\*\\\-]))*)$"; 

            if (Regex.IsMatch(str, pattern))
                return true;
            else
                return false;
        }
    }

    public class Node
    {
        public string oper; // Операция
        public Node left;   // Левое поддерево
        public Node right;  // Правое поддерево

        public Node()
        {
            left = null;
            right = null;
        }
    }

    public class VariableInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public VariableInfo(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }

    public class ExpressionTree
    {
        private HashSet<VariableInfo> variableTable = new HashSet<VariableInfo>();

        public void ProcessExpression(Node node, string expr)
        {
            if (string.IsNullOrEmpty(expr))
                return;

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

                // Добавляем переменную в таблицу имен
                if (IsVariable(expr))
                {
                    variableTable.Add(new VariableInfo(expr, "double"));
                }
                else
                {
                    if (expr.Contains('.'))
                        variableTable.Add(new VariableInfo(expr, "const float"));
                    else
                        variableTable.Add(new VariableInfo(expr, "const int"));

                }

                node.left = null;
                node.right = null;
            }
        }

        private int GetPriority(char operation)
        {
            switch (operation)
            {
                case '=':
                    return 1; // Присваивание
                case '+':
                    return 2; // Сложение
                case '*':
                    return 3; // Умножение
                default:
                    return 4; // Не операция
            }
        }

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

        private bool IsVariable(string expr)
        {
            // Проверяем, является ли выражение идентификатором переменной
            // Здесь можно добавить более сложную логику для проверки
            return !string.IsNullOrEmpty(expr) && char.IsLetter(expr[0]);
        }

        public void PrintTreeToFile(Node node, StreamWriter writer, int level)
        {
            if (node == null) return;

            PrintTreeToFile(node.right, writer, level + 1);
            writer.WriteLine(new string(' ', level * 4) + node.oper);
            PrintTreeToFile(node.left, writer, level + 1);
        }

        public void PrintVariableTableToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Таблица имен:");
                foreach (var variable in variableTable)
                {
                    writer.WriteLine($"{variable.Name} : {variable.Type}");
                }
            }
        }
    }
}