using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

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
    /// <returns>
    /// true - успешное завершение
    /// false - завершение с ошибкой
    /// </returns>
    public bool Run(string str)
    {
        string state = "q0";

        for (int i = 0; i < str.Length; i++)
        {
            char symbol = str[i];
            string matchedAlphabet = MatchSymbol(symbol);

            if (matchedAlphabet == null)
            {
                Console.WriteLine("Автомат завершил работу с ошибкой.");
                File.WriteAllText(_filePath, "Строка введена некорректно");
                return false;
            }

            // Поиск соответствующего перехода
            bool found = false;
            foreach (var transition in _transitionTable[state])
            {
                if (transition[1] == matchedAlphabet || transition[1] == symbol.ToString())
                {
                    found = true;
                    state = transition[0];

                    if (transition.Length == 3)
                    {
                        if (transition[2] == "Push")
                            _stack.Push(symbol);
                        else if (transition[2] == "Pop")
                            _stack.Pop();
                    }

                    break;
                }
            }

            if (!found)
            {
                Console.WriteLine("Автомат завершил работу с ошибкой.");
                File.WriteAllText(_filePath, "Строка введена некорректно");
                return false;
            }
        }

        // Проверка на завершение
        if (_transitionTable[state].Any(transition => transition[0] == "HALT" && _stack.Count == 0))
        {
            Console.WriteLine("Автомат завершил работу успешно.");
            return true;
        }
        else
        {
            Console.WriteLine("Автомат завершил работу с ошибкой.");
            File.WriteAllText(_filePath, "Строка введена некорректно");
            return false;
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
        "\\s",        // Пробел (7)
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
                new string[] { "HALT", "" }
            }
        }
    };

    // Стек(Магазин) автомата
    private Stack<char> _stack = new();

    #endregion
}