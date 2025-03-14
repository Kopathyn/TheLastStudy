﻿using System.Text.RegularExpressions;

public class DeterministicFiniteAutomatonWithStack
{
    public DeterministicFiniteAutomatonWithStack(string path)
    {
        _filePath = path;
    }

    /// <summary>
    /// Проверка на вхождение символа в алфавит
    /// </summary>
    /// <param name="symbol">Символ</param>
    /// <returns>Часть алфавита, которой принадлежит</returns>
    private string MatchSymbol(char symbol)
    {
        foreach (var regex in _alphabet)
            if (Regex.IsMatch(symbol.ToString(), regex))
                return regex;

        return null;
    }

    /// <summary>
    /// Запуск ДМПА
    /// </summary>
    /// <param name="str">Строка</param>
    /// <returns>Преобразование строки в ОПЗ</returns>
    public string Run(string str)
    {
        string state = "q0";

        /* Для формирования ОПЗ */
        Stack<char> operatorStack = new Stack<char>();
        string outputString = "";

        for (int i = 0; i < str.Length; i++)
        {
            char symbol = str[i];

            string matchedAlphabet = MatchSymbol(symbol);

            if (matchedAlphabet == null)
            {
                Console.WriteLine("Автомат завершил работу с ошибкой.");
                File.WriteAllText(_filePath, "Строка введена некорректно");
                return null;
            }

            bool transitionFlag = false;
            foreach (var transition in _transitionTable[state])
            {
                if (transition[1] == matchedAlphabet || transition[1] == symbol.ToString())
                {
                    transitionFlag = true;
                    state = transition[0];

                    if (transition.Length == 3)
                    {
                        if (transition[2] == "Push")
                        {
                            _stack.Push(symbol);

                            operatorStack.Push(symbol);
                        }

                        else if (transition[2] == "Pop")
                        {
                            _stack.Pop();

                            outputString += " ";
                            while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                                outputString += operatorStack.Pop();

                            if (operatorStack.Count > 0)
                                operatorStack.Pop();
                        }

                        break;
                    }

                    // Обработка символов в соответствии с алгоритмом
                    if (char.IsLetterOrDigit(symbol) || symbol == '.')
                        outputString += symbol;

                    else // Если символ - знак операции
                    {
                        outputString += " ";

                        int currentPriority = GetOperatorPriority(symbol);

                        while (operatorStack.Count > 0 && GetOperatorPriority(operatorStack.Peek()) >= currentPriority)
                            outputString += operatorStack.Pop() + " ";

                        operatorStack.Push(symbol);
                    }

                    break;
                }
            }

            if (!transitionFlag)
            {
                Console.WriteLine("Автомат завершил работу с ошибкой.");
                File.WriteAllText(_filePath, "Строка введена некорректно");
                return null;
            }
        }

        // Извлечение оставшихся операторов из стека в выходную строку
        while (operatorStack.Count > 0)
            outputString += " " + operatorStack.Pop();

        // Проверка на завершение
        if (_transitionTable[state].Any(transition => transition[0] == "HALT" && _stack.Count == 0))
        {
            Console.WriteLine("Автомат завершил работу успешно.");
            return outputString;
        }
        else
        {
            Console.WriteLine("Автомат завершил работу с ошибкой.");
            File.WriteAllText(_filePath, "Строка введена некорректно");
            return null;
        }
    }

    // Метод для получения приоритета оператора
    private int GetOperatorPriority(char op)
    {
        switch (op)
        {
            case '*':
                return 3; // Наивысший приоритет
            case '+':
                return 2; // Средний приоритет
            default:
                return 0; // Не оператор
        }
    }

    #region PrivateFields

    private string _filePath;

    // Алфавит автомата в РВ
    private static string[] _alphabet =
    {
        "\\p{L}",     // Буквы (0)
        "\\d",        // Цифры (1)
        "[\\+\\*]",   // Знаки сложения и умножения (2)
        "\\(", "\\)", // Скобки (3, 4)
        "\\.",        // "Запятая" (5)
        "\\=",        // Знак равенства (6)
    };

    // Таблица переходов
    private Dictionary<string, string[][]> _transitionTable = new Dictionary<string, string[][]>
    {
        {
            "q0",
            new string[][] { new string[] { "q1", _alphabet[0] } }
        },
        {
            "q1",
            new string[][]
            {
                new string[] { "q1", _alphabet[0]  },
                new string[] { "q1", _alphabet[1] },
                new string[] { "q2", _alphabet[6] }
            }
        },
        {
            "q2",
            new string[][]
            {
                new string[] { "q3", _alphabet[1] },
                new string[] { "q4", _alphabet[0] },
                new string[] { "q5", _alphabet[3], "Push" }
            }
        },
        {
            "q3",
            new string[][]
            {
                new string[] { "q2", _alphabet[2] },
                new string[] { "q3", _alphabet[1] },
                new string[] { "q5", _alphabet[4], "Pop" },
                new string[] { "q6", "E" },
                new string[] { "q6", "e" },
                new string[] { "q7", _alphabet[5] },
                new string[] { "HALT", "" }
            }
        },
        {
            "q4",
            new string[][]
            {
                new string[] { "q2", _alphabet[2] },
                new string[] { "q4", _alphabet[0] },
                new string[] { "q4", _alphabet[1] },
                new string[] { "q5", _alphabet[4], "Pop" },
                new string[] { "HALT", "" }
            }
        },
        {
            "q5",
            new string[][]
            {
                new string[] { "q2", _alphabet[2] },
                new string[] { "q3", _alphabet[1] },
                new string[] { "q4", _alphabet[0] },
                new string[] { "q5", _alphabet[3], "Push" },
                new string[] { "q5", _alphabet[4], "Pop" },
                new string[] { "HALT", "" }
            }
        },
        {
            "q6",
            new string[][] { new string[] { "q7", "+" } }
        },
        {
            "q7",
            new string[][]
            {
                new string[] { "q2", _alphabet[2] },
                new string[] { "q7", _alphabet[1] },
                new string[] { "q7", _alphabet[4], "Pop" },
                new string[] { "HALT", "" }
            }
        }
    };

    // Стек(Магазин) автомата
    private Stack<char> _stack = new();

    #endregion
}