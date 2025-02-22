﻿using System;
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
    /// Обработка стека
    /// </summary>
    private void StackProcessing(_states state, char symbol)
    {
        if (state == _states.q5)
        {
            if (symbol == '(')
                _stack.Push(symbol);
            else if (symbol == ')')
                _stack.Pop();
        }        
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
        _states state = _states.q0;

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

            if (_transitionTable[state].ContainsKey(matchedAlphabet))
            {
                state = _transitionTable[state][matchedAlphabet];

                StackProcessing(state, symbol);
            }
            else
            {
                Console.WriteLine("Автомат завершил работу с ошибкой.");
                File.WriteAllText(_filePath, "Строка введена некорректно");
                return false;
            }
        }

        // Проверка на завершение
        if (_transitionTable[state].ContainsValue(_states.HALT) && _stack.Count == 0)
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

    // Таблица переходов
    private Dictionary<_states, Dictionary<string, _states>> _transitionTable = new Dictionary<_states, Dictionary<string, _states>>
    {
        {
            _states.q0,
            new Dictionary<string, _states>
            {
                {_alphabet[0], _states.q1 }
            }
        },

        {
            _states.q1,
            new Dictionary<string, _states>
            {
                {_alphabet[0], _states.q1},
                {_alphabet[1], _states.q1},
                {_alphabet[6], _states.q2}
            }
        },

        {
            _states.q2,
            new Dictionary<string, _states>
            {
                {_alphabet[0], _states.q4 },
                {_alphabet[1], _states.q3 },
                {_alphabet[3], _states.q5 }
            }
        },

        {
            _states.q3,
            new Dictionary<string, _states>
            {
                {_alphabet[1], _states.q3 },
                { _alphabet[2], _states.q2 },
                { _alphabet[4], _states.q5 },
                {_alphabet[5], _states.q3 },
                {"", _states.HALT}
            }
        },

        {
            _states.q4,
            new Dictionary<string, _states>
            {
                {_alphabet[0], _states.q4 },
                {_alphabet[1], _states.q4 },
                {_alphabet[2], _states.q2 },
                {_alphabet[4], _states.q5 },
                {"", _states.HALT}
            }
        },

        {
            _states.q5,
            new Dictionary<string, _states>
            {
                {_alphabet[0], _states.q4 },
                {_alphabet[1], _states.q3 },
                {_alphabet[2], _states.q2 },
                {_alphabet[4], _states.q5 },
                {"", _states.HALT}
            }
        }
    };

    // Стек(Магазин) автомата
    private Stack<char> _stack = new(); 

    // Состояния автомата
    private enum _states 
    {
        q0, q1, q2, q3, q4, q5, HALT
    }

    // Алфавит автомата в РВ
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

#endregion
}