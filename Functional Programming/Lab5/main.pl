:- dynamic horse_position/2.

% проверка на выход за пределы шахматной доски
check_board(X, Y) :- X > 0, X < 9,
    Y > 0, Y < 9.

% Определение всех возможных ходов
move_horse(X, Y, NewX, NewY) :- NewX is X - 1, NewY is Y + 2, check_board(NewX, NewY).
move_horse(X, Y, NewX, NewY) :- NewX is X + 1, NewY is Y + 2, check_board(NewX, NewY).
move_horse(X, Y, NewX, NewY) :- NewX is X + 2, NewY is Y + 1, check_board(NewX, NewY).
move_horse(X, Y, NewX, NewY) :- NewX is X + 2, NewY is Y - 1, check_board(NewX, NewY).
move_horse(X, Y, NewX, NewY) :- NewX is X + 1, NewY is Y - 2, check_board(NewX, NewY).
move_horse(X, Y, NewX, NewY) :- NewX is X - 1, NewY is Y - 2, check_board(NewX, NewY).
move_horse(X, Y, NewX, NewY) :- NewX is X - 2, NewY is Y - 1, check_board(NewX, NewY).
move_horse(X, Y, NewX, NewY) :- NewX is X - 2, NewY is Y + 1, check_board(NewX, NewY).

horse_movement(X, Y, NewX, NewY) :-
    move_horse(X, Y, NewX, NewY),
    not(horse_position(NewX, NewY)),
    format("Новая позиция (~w,~w)~n", [NewX, NewY]),
    assert(horse_position(NewX, NewY)).

generate_3movements_horse(FirstX, FirstY) :-
    format("~nНачальная позиция (~w,~w)~n", [FirstX, FirstY]),
    check_board(FirstX, FirstY),
    assert(horse_position(FirstX, FirstY)),

    % Первый ход
    format("~nПервый ход:~n"),
    horse_movement(FirstX, FirstY, SecondX, SecondY),

    % Второй ход
    format("~nВторой ход:~n"),
    horse_movement(SecondX, SecondY, ThirdX, ThirdY),

    % Третий ход
    format("~nТретий ход:~n"),
    horse_movement(ThirdX, ThirdY, FinalX, FinalY).