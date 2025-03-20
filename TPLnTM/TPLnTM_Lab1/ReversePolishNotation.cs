using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPLnTM_Lab1
{
    public class ReversePolishNotation
    {
        public ReversePolishNotation() 
        { 
            _outputString = null; _operatorStack = new(); 
        }

        /// <summary>
        /// Добавление части операнда в строку
        /// </summary>
        public void ActionOne(char symbol) => _outputString += (symbol);

        /// <summary>
        /// Добавление открывающей скобки в стек
        /// </summary>
        /// <param name="symbol"></param>
        public void ActionTwo(char openBracket) => _operatorStack.Push(openBracket);

        /// <summary>
        /// Обработка закрывающей скобки
        /// </summary>
        public void ActionThree()
        {
            _outputString += " ";

            while (_operatorStack.Count > 0 && _operatorStack.Peek() != '(')
                _outputString += _operatorStack.Pop();

            if (_operatorStack.Count > 0)
                _operatorStack.Pop();
        }

        /// <summary>
        /// Обработка оператора
        /// </summary>
        /// <param name="symbol"></param>
        public void ActionFour(char symbol)
        {
            _outputString += " "; 
            
            int currentPriority = GetOperatorPriority(symbol);

            while (_operatorStack.Count > 0 && GetOperatorPriority(_operatorStack.Peek()) >= currentPriority)
                _outputString += _operatorStack.Pop() + " ";

            _operatorStack.Push(symbol);
        }

        /// <summary>
        /// Завершение работы
        /// </summary>
        public void ActionFive()
        {
            while (_operatorStack.Count > 0)
                _outputString += " " + _operatorStack.Pop();
        }

        /// <summary>
        /// Метод для получения приоритета оператора
        /// </summary>
        /// <param name="op">Оператор</param>
        /// <returns>Приоритет оператора</returns>
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

        private Stack<char> _operatorStack;
        private string _outputString;

        public string OutputString { get { return _outputString; } }
    }
}
