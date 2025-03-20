using System;
using System.Text.RegularExpressions;
using TPLnTM_Lab1;

public class Transition
{
    public string CurrentState;
    public string CurrentAlphabet;
    public string StackAction;
}

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
    /// Сообщение об ошибке
    /// </summary>
    private void ErrorMessage(string state, char symbol)
    {
        Console.WriteLine("Автомат завершил работу с ошибкой.\n" +
            $"Состояние {state}, Символ {symbol}");

        File.WriteAllText(_filePath, "Строка введена некорректно");
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
            bool transitionFlag = false;

            string matchedAlphabet = MatchSymbol(symbol);

            if (matchedAlphabet == null)
            {
                ErrorMessage(state, symbol);
                return null;
            }
            else if (matchedAlphabet == _alphabet[7])
            {
                transitionFlag = true;
                continue;
            }

            foreach (var transition in _transitionTable[state])
            {
                if (transition[1] == matchedAlphabet || transition[1] == symbol.ToString())
                {
                    transitionFlag = true;
                    state = transition[0];

                    if (transition.Length == 3)
                    {
                        string transitionAction = transition[2];

                        switch (transitionAction)
                        {
                            case "Push":
                                _stack.Push(symbol);
                                _rpnCreator.ActionTwo(symbol);
                                break;
                            case "Pop":
                                _stack.Pop();
                                _rpnCreator.ActionThree();
                                break;
                            case "1":
                                _rpnCreator.ActionOne(symbol);
                                break;
                            case "4":
                                _rpnCreator.ActionFour(symbol);
                                break;

                            default:
                                ErrorMessage(state, symbol);
                                break;
                        }

                        break;
                    }

                    break;
                }
            }

            if (!transitionFlag)
            {
                ErrorMessage(state, symbol);
                return null;
            }
        }

        // Проверка на завершение
        if (_transitionTable[state].Any(transition => transition[0] == "HALT" && _stack.Count == 0))
        {
            _rpnCreator.ActionFive();

            Console.WriteLine("Автомат завершил работу успешно.");
            return _rpnCreator.OutputString;
        }
        else
        {
            ErrorMessage(state, ' ');
            return null;
        }
    }

    #region PrivateFields

    private static ReversePolishNotation _rpnCreator = new();

    private string _filePath;

    /// <summary>
    /// Состояния автомата
    /// </summary>
    private string[] _states = 
    {
        "q0", "q1", "q2", "q3", "q4", "q5", "q6", 
        "q7", "q8", "q9", "q10", "q11", "q12",
    }; 

    /// <summary>
    /// Алфавит автомата в РВ
    /// </summary>
    /// <value> 
    ///   Буквы [0];
    ///   Цифры [1];
    ///   Знаки сложения и умножения [2];
    ///   Скобки [3, 4];
    ///   "Запятая" [5];
    ///   Знак равенства [6];
    ///   Пробел [7];
    /// </value>
    private static string[] _alphabet =
    {
        "\\p{L}",
        "\\d",
        "[\\+\\*]",
        "\\(", "\\)",
        "\\.",
        "\\=",
        "\\s"
    };

    // Стек(Магазин) автомата
    private static Stack<char> _stack = new();

    private static Action<char>[] _stackActions =
    { 
        x => _stack.Push(x),
        x => _stack.Pop()
    };

    private static Action<char>[] _implementedActions =
    {
        x => _rpnCreator.ActionOne(x),
        x => _rpnCreator.ActionTwo(x),
        x => _rpnCreator.ActionThree(),
        x => _rpnCreator.ActionFour(x),
        x => _rpnCreator.ActionFive()
    };


    private Dictionary<Tuple<string, string, Action<char>>, Tuple<string, Action<char>, Action<char>>> _newTransitionTable = new()
    {
        // q0
        { new("q0", _alphabet[0], null), new("q1", null, _implementedActions[0]) },
        { new("q0", _alphabet[7], null), new("q0", null, null) },

        // q1
        { new("q1", _alphabet[0], null), new("q1", null, _implementedActions[0]) },
        { new("q1", _alphabet[1], null), new("q1", null, _implementedActions[0]) },
        { new("q1", _alphabet[7], null), new("q2", null, null) },
        { new("q1", _alphabet[6], null), new("q3", null, _implementedActions[3]) },

        // q2
        { new("q2", _alphabet[7], null), new("q2", null, null) },
        { new("q2", _alphabet[6], null), new("q3", null, _implementedActions[3]) },

        // q3
        { new("q3", _alphabet[7], null), new("q3", null, null) },
        { new("q3", _alphabet[3], _stackActions[0]), new("q3", null, _implementedActions[1]) },
        { new("q3", _alphabet[1], null), new("q4", null, _implementedActions[0]) },
        { new("q3", _alphabet[0], null), new("q5", null, _implementedActions[0]) },

        // q4
        { new("q4", _alphabet[2], null), new("q3", null, _implementedActions[3]) },
        { new("q4", _alphabet[1], null), new("q4", null, _implementedActions[0]) },
        { new("q4", _alphabet[4], _stackActions[1]), new("q6", null, _implementedActions[2]) },
        { new("q4", _alphabet[5], null), new("q7", null, _implementedActions[0]) },
        { new("q4", "E", null), new("q9", null, _implementedActions[0]) },
        { new("q4", "e", null), new("q9", null, _implementedActions[0]) },
        { new("q4", _alphabet[7], null), new("q12", null, null) },
        { new("q4", "", null), new("HALT", null, _implementedActions[4]) },

        // q5
        { new("q5", _alphabet[2], null), new("q3", null, _implementedActions[3]) },
        { new("q5", _alphabet[0], null), new("q5", null, _implementedActions[0]) },
        { new("q5", _alphabet[1], null), new("q5", null, _implementedActions[0]) },
        { new("q5", _alphabet[4], _stackActions[1]), new("q6", null, _implementedActions[2]) },
        { new("q5", _alphabet[7], null), new("q12", null, null) },
        { new("q5", "", null), new("HALT", null, _implementedActions[4]) },

        // q6
        { new("q6", _alphabet[2], null), new("q3", null, _implementedActions[3]) },
        { new("q6", _alphabet[4], _stackActions[1]), new("q6", null, _implementedActions[2]) },
        { new("q6", _alphabet[7], null), new("q6", null, null) },
        { new("q6", "", null), new("HALT", null, _implementedActions[4]) },

        // q7
        { new("q7", _alphabet[1], null), new("q8", null, _implementedActions[0]) },

        // q8
        { new("q8", _alphabet[2], null), new("q3", null, _implementedActions[3]) },
        { new("q8", _alphabet[4], _stackActions[1]), new("q6", null, _implementedActions[2]) },
        { new("q8", _alphabet[1], null), new("q8", null, _implementedActions[0]) },
        { new("q8", "E", null), new("q9", null, _implementedActions[0]) },
        { new("q8", "e", null), new("q9", null, _implementedActions[0]) },
        { new("q8", _alphabet[7], null), new("q12", null, null) },
        { new("q8", "", null), new("HALT", null, _implementedActions[4]) },

        // q9
        { new("q9", "+", null), new("q10", null, _implementedActions[0]) },
        { new("q9", "-", null), new("q10", null, _implementedActions[0]) },

        // q10
        { new("q10", _alphabet[1], null), new("q11", null, _implementedActions[0]) },

        // q11
        { new("q11", _alphabet[2], null), new("q3", null, _implementedActions[3]) },
        { new("q11", _alphabet[4], _stackActions[1]), new("q6", null, _implementedActions[2]) },
        { new("q11", _alphabet[1], null), new("q11", null, _implementedActions[0]) },
        { new("q11", _alphabet[7], null), new("q12", null, null) },
        { new("q11", "", null), new("HALT", null, _implementedActions[4]) },

        // q12
        { new("q12", _alphabet[2], null), new("q3", null, _implementedActions[3]) },
        { new("q12", _alphabet[4], _stackActions[1]), new("q6", null, _implementedActions[2]) },
        { new("q12", _alphabet[7], null), new("q12", null, null) },
        { new("q12", "", null), new("HALT", null, _implementedActions[4]) }
    };

    // Таблица переходов
    private Dictionary<string, string[][]> _transitionTable = new Dictionary<string, string[][]>
    {
        {
            "q0",
            new string[][] 
            {
                new[] { "q1", _alphabet[0], "1" },
                new[] { "q0", _alphabet[7] }
            }
        },
        {
            "q1",
            new string[][]
            {
                new string[] { "q1", _alphabet[0], "1"},
                new string[] { "q1", _alphabet[1], "1" },
                new string[] { "q2", _alphabet[7] },
                new[] { "q3", _alphabet[6], "4" }
            }
        },
        {
            "q2",
            new string[][]
            {
                new string[] { "q2", _alphabet[7] },
                new string[] { "q3", _alphabet[6], "4" }
            }
        },
        {
            "q3",
            new string[][]
            {
                new[] { "q3", _alphabet[7] },
                new[] { "q3", _alphabet[3], "Push" },
                new string[] { "q4", _alphabet[1], "1" },
                new string[] { "q5", _alphabet[0], "1" }
            }
        },
        {
            "q4",
            new string[][]
            {
                new[] { "q3", _alphabet[2], "4" },
                new string[] { "q4", _alphabet[1], "1" },
                new[] { "q6", _alphabet[4], "Pop" },
                new string[] { "q7", _alphabet[5], "1" },
                new[] { "q9", "E", "1" },
                new[] { "q9", "e", "1" },
                new[] { "q12", _alphabet[7] },
                new string[] { "HALT", "" }
            }
        },
        {
            "q5",
            new string[][]
            {
                new[] { "q3", _alphabet[2], "4" },
                new[] { "q5",  _alphabet[0], "1" },
                new[] { "q5", _alphabet[1] , "1" },
                new[] { "q6", _alphabet[4], "Pop" },
                new[] { "q12", _alphabet[7] },
                new string[] { "HALT", "" }
            }
        },
        {
            "q6",
            new string[][] 
            {
                new[] { "q3", _alphabet[2], "4" },
                new string[] { "q6", _alphabet[4], "Pop" },
                new[] { "q6", _alphabet[7] },
                new string[] { "HALT", "" }
            }
        },
        {
            "q7",
            new string[][]
            {
                new string[] { "q8", _alphabet[1], "1" }
            }
        },
        {
            "q8",
            new string[][]
            {
                new[] { "q3", _alphabet[2], "4" },
                new[] { "q6", _alphabet[4], "Pop" },
                new[] { "q8", _alphabet[1], "1" },
                new[] { "q9", "E" , "1" },
                new[] { "q9", "e", "1" },
                new[] { "q12", _alphabet[7] },
                new string[] { "HALT", "" }
            }
        },
        {
            "q9",
            new string[][]
            {
                new[] { "q10", "+", "1"}, //?
                new[] { "q10", "-", "1" }
            }
        },
        {
            "q10",
            new string[][]
            {
                new[] { "q11", _alphabet[1], "1" }
            }
        },
        {
            "q11",
            new string[][]
            {
                new[] { "q3", _alphabet[2], "4" },
                new[] { "q6", _alphabet[4], "Pop" },
                new[] { "q11", _alphabet[1], "1" },
                new[] { "q12", _alphabet[7] },
                new string[] { "HALT", "" }
            }
        },
        {
            "q12",
            new string[][]
            {
                new[] { "q3", _alphabet[2], "4" },
                new[] { "q6", _alphabet[4], "Pop" },
                new[] { "q12", _alphabet[7] },
                new string[] { "HALT", "" }
            }
        }
    };


    #endregion
}