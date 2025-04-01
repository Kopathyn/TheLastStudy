namespace TPLnTM_Lab1
{
    public class ReversePolishNotation
    {
        private Stack<char> _operatorStack;
        private string _outputString;
        private int _bracketCounter;

        public string OutputString => _outputString;

        public ReversePolishNotation()
        {
            _outputString = string.Empty;
            _operatorStack = new Stack<char>();
            _bracketCounter = 0;
        }

        /// <summary> 
        /// Добавление операнда 
        /// </summary>
        public bool ActionOne(char symbol)
        {
            _outputString += symbol;
            return true;
        }

        /// <summary> 
        /// Добавление открывающей скобки в стек
        /// </summary>
        public bool ActionTwo(char openBracket)
        {
            _operatorStack.Push(openBracket);
            _bracketCounter++;

            return true;
        }

        /// <summary>
        /// Обработка закрывающей скобки
        /// </summary>
        public bool ActionThree()
        {
            _bracketCounter--;

            if (_bracketCounter < 0) 
                return false;

            _outputString += " ";

            while (_operatorStack.Count > 0 && _operatorStack.Peek() != '(')
                _outputString += _operatorStack.Pop();

            if (_operatorStack.Count == 0)
                return false;

            _operatorStack.Pop();
            return true;
        }

        /// <summary>
        /// Обработка оператора
        /// </summary>
        /// <param name="symbol"></param>
        public bool ActionFour(char symbol)
        {
            _outputString += " ";

            int currentPriority = GetOperatorPriority(symbol);

            while (_operatorStack.Count > 0 && GetOperatorPriority(_operatorStack.Peek()) >= currentPriority)
                _outputString += _operatorStack.Pop() + " ";

            _operatorStack.Push(symbol);
            return true;
        }

        /// <summary>
        /// Завершение работы
        /// </summary>
        public bool ActionFive()
        {
            if (_bracketCounter != 0) 
                return false;

            while (_operatorStack.Count > 0)
            {
                char op = _operatorStack.Pop();

                if (op == '(') 
                    return false;

                _outputString += " " + op;
            }

            return true;
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
    }
}