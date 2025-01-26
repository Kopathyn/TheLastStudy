using System;
using System.Collections.Generic;

public class DeterministicFiniteAutomatonWithStack
{
    // Определяем состояния автомата
    private enum State
    {
        Initial,
        State1,
        State2
    }

    // Определяем символы входного алфавита
    private enum Symbol
    {
        A,
        B
    }

    // Определяем действия автомата
    private enum Action
    {
        Accept,
        Reject
    }

    // Определяем стек
    private Stack<Symbol> stack = new Stack<Symbol>();

    // Определяем текущее состояние
    private State currentState = State.Initial;

    // Определяем таблицу переходов
    private Dictionary<State, Dictionary<Symbol, State>> transitionTable = new Dictionary<State, Dictionary<Symbol, State>>
    {
        {
            State.Initial, 
            new Dictionary<Symbol, State>
            {
                { Symbol.A, State.State1 },
                { Symbol.B, State.State2 }
            }
        },
        {
            State.State1, 
            new Dictionary<Symbol, State>
            {
                { Symbol.A, State.State1 },
                { Symbol.B, State.State2 }
            }
        },
        {
            State.State2, 
            new Dictionary<Symbol, State>
            {
                { Symbol.A, State.State2 },
                { Symbol.B, State.State2 }
            }
        }
    };

    // Определяем начальное состояние автомата
    private State InitialState { get { return State.Initial; } }

    // Определяем конечное состояние автомата
    private Action FinalState { get { return Action.Accept; } }

    // Определяем метод для добавления символа в стек
    private void Push(Symbol symbol)
    {
        stack.Push(symbol);
    }

    // Определяем метод для удаления символа из стека
    private Symbol Pop()
    {
        return stack.Pop();
    }

    // Определяем метод для проверки, пуст ли стек
    private bool IsEmpty()
    {
        return stack.Count == 0;
    }

    // Определяем метод для перехода в новое состояние
    private void Transition(Symbol symbol)
    {
        State nextState;
        if (transitionTable.TryGetValue(currentState, out var table) && table.TryGetValue(symbol, out nextState))
        {
            currentState = nextState;
        }
        else
        {
            Console.WriteLine("Invalid transition");
            Environment.Exit(1);
        }
    }

    // Определяем метод для обработки входного символа
    private void HandleInput(Symbol symbol)
    {
        Transition(symbol);
        if (currentState == State.State2 && IsEmpty())
        {
            Console.WriteLine("Accepted");
            Environment.Exit(0);
        }
    }

    //// Определяем метод для запуска автомата
    //public void Run(string input)
    //{
    //    foreach (var symbol in input)
    //    {
    //        if (symbol == 'A' || symbol == 'B')
    //        {
    //            Push(symbol);
    //            HandleInput(symbol);
    //        }
    //    }
    //}
}