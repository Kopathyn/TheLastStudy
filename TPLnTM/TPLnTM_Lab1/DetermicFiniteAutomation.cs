using System.Text.RegularExpressions;
using TPLnTM_Lab1;

public class DeterministicFiniteAutomatonWithStack
{
    private static ReversePolishNotation _rpnCreator = new();

    private string _filePath;

    /// <summary>
    /// Состояния автомата
    /// </summary>
    private static string[] _states =
    {
        "q0", "q1", "q2", "q3", "q4", "q5", "q6",
        "q7", "q8", "q9", "q10", "q11", "q12"
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

    /// <summary>
    /// Внедреные действия
    /// </summary>
    private static Action<char>[] _implementedActions =
    {
        x => _rpnCreator.ActionOne(x),
        x => _rpnCreator.ActionTwo(x),
        x => _rpnCreator.ActionThree(),
        x => _rpnCreator.ActionFour(x),
        x => _rpnCreator.ActionFive()
    };

    /// <summary>
    /// Таблица переходов
    /// </summary>
    private Dictionary<Tuple<string, string>, Tuple<string, Action<char>>> _newTransitionTable = new()
    {
        // q0
        { new(_states[0], _alphabet[0]), new(_states[1], _implementedActions[0]) },
        { new(_states[0], _alphabet[7]), new(_states[0], null) },

        // q1
        { new(_states[1], _alphabet[0]), new(_states[1], _implementedActions[0]) },
        { new(_states[1], _alphabet[1]), new(_states[1], _implementedActions[0]) },
        { new(_states[1], _alphabet[7]), new(_states[2], null) },
        { new(_states[1], _alphabet[6]), new(_states[3], _implementedActions[3]) },

        // q2
        { new(_states[2], _alphabet[7]), new(_states[2], null) },
        { new(_states[2], _alphabet[6]), new(_states[3], _implementedActions[3]) },

        // q3
        { new(_states[3], _alphabet[7]), new(_states[3],  null) },
        { new(_states[3], _alphabet[3]), new(_states[3],  _implementedActions[1]) },
        { new(_states[3], _alphabet[1]), new(_states[4],  _implementedActions[0]) },
        { new(_states[3], _alphabet[0]), new(_states[5],  _implementedActions[0]) },

        // q4
        { new(_states[4], _alphabet[2]), new(_states[3], _implementedActions[3]) },
        { new(_states[4], _alphabet[1]), new(_states[4], _implementedActions[0]) },
        { new(_states[4], _alphabet[4]), new(_states[6], _implementedActions[2]) },
        { new(_states[4], _alphabet[5]), new(_states[7], _implementedActions[0]) },
        { new(_states[4], "E"), new(_states[9], _implementedActions[0]) },
        { new(_states[4], "e"), new(_states[9], _implementedActions[0]) },
        { new(_states[4], _alphabet[7]), new(_states[12], null) },
        { new(_states[4], string.Empty), new("HALT", _implementedActions[4]) },

        // q5
        { new(_states[5], _alphabet[2]), new(_states[3], _implementedActions[3]) },
        { new(_states[5], _alphabet[0]), new(_states[5], _implementedActions[0]) },
        { new(_states[5], _alphabet[1]), new(_states[5], _implementedActions[0]) },
        { new(_states[5], _alphabet[4]), new(_states[6], _implementedActions[2]) },
        { new(_states[5], _alphabet[7]), new(_states[12], null) },
        { new(_states[5], string.Empty), new("HALT", _implementedActions[4]) },

        // q6
        { new(_states[6], _alphabet[2]), new(_states[3], _implementedActions[3]) },
        { new(_states[6], _alphabet[4]), new(_states[6], _implementedActions[2]) },
        { new(_states[6], _alphabet[7]), new(_states[6], null) },
        { new(_states[6], string.Empty), new("HALT", _implementedActions[4]) },

        // q7
        { new(_states[7], _alphabet[1]), new(_states[8], _implementedActions[0]) },

        // q8
        { new(_states[8], _alphabet[2]), new(_states[3], _implementedActions[3]) },
        { new(_states[8], _alphabet[4]), new(_states[6], _implementedActions[2]) },
        { new(_states[8], _alphabet[1]), new(_states[8], _implementedActions[0]) },
        { new(_states[8], "E"), new(_states[9], _implementedActions[0]) },
        { new(_states[8], "e"), new(_states[9], _implementedActions[0]) },
        { new(_states[8], _alphabet[7]), new(_states[12], null) },
        { new(_states[8], string.Empty), new("HALT", _implementedActions[4]) },

        // q9
        { new(_states[9], "+"), new(_states[10], _implementedActions[0]) },
        { new(_states[9], "-"), new(_states[10], _implementedActions[0]) },

        // q10
        { new(_states[10], _alphabet[1]), new(_states[11], _implementedActions[0]) },

        // q11
        { new(_states[11], _alphabet[2]), new(_states[3], _implementedActions[3]) },
        { new(_states[11], _alphabet[4]), new(_states[6], _implementedActions[2]) },
        { new(_states[11], _alphabet[1]), new(_states[11], _implementedActions[0]) },
        { new(_states[11], _alphabet[7]), new(_states[12], null) },
        { new(_states[11], string.Empty), new("HALT", _implementedActions[4]) },

        // q12
        { new(_states[12], _alphabet[2]), new(_states[3], _implementedActions[3]) },
        { new(_states[12], _alphabet[4]), new(_states[6], _implementedActions[2]) },
        { new(_states[12], _alphabet[7]), new(_states[12], null) },
        { new(_states[12], string.Empty), new("HALT", _implementedActions[4]) }
    };

    public DeterministicFiniteAutomatonWithStack(string path)
    {
        _filePath = path;
    }

    /// <summary>
    /// Запуск ДМПА
    /// </summary>
    /// <param name="str">Строка</param>
    /// <returns>Преобразование строки в ОПЗ</returns>
    public string Run(string str)
    {
        string currentState = _states[0];
        string currentAlphabet = "";

        for (int i = 0; i < str.Length; i++)
        {
            char symbol = str[i];

            currentAlphabet = FindAlphabetSymbol(symbol);

            if (currentAlphabet == null)
            {
                ErrorMessage(currentState, symbol);
                return null;
            }

            Tuple<string, Action<char>> transition;

            if (_newTransitionTable.TryGetValue(new(currentState, currentAlphabet), out transition) ||
                _newTransitionTable.TryGetValue(new(currentState, symbol.ToString()), out transition))
            {
                currentState = transition.Item1;
                transition.Item2?.Invoke(symbol);
            }
            else
            {
                ErrorMessage(currentState, symbol);
                return null;
            }
        }

        if (_newTransitionTable.TryGetValue(new(currentState, string.Empty), out var lastTransition))
        {
            currentState = lastTransition.Item1;
            lastTransition.Item2?.Invoke(' ');
        }
        else
        {
            ErrorMessage(currentState, ' ');
            return null;
        }

        if (currentState == "HALT")
            return _rpnCreator.OutputString;
        else
        {
            ErrorMessage(currentState, ' ');
            return null;
        }
    }

    /// <summary>
    /// Поиск к какой части алфавита относится символ
    /// </summary>
    /// <param name="symbol">Символ</param>
    /// <returns>Часть алфавита, которой принадлежит</returns>
    private string FindAlphabetSymbol(char symbol)
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
}