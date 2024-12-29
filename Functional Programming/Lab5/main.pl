% Объявление динмаической переменной, которая содержит 2 переменные
% И которая будет содержать в себе изменения о положении коня
:-dynamic horse_position/2.

% Проверка на выход за пределы шахматной доски
check_board(X, Y) :- X > 0, X < 9,
    Y > 0, Y < 9.

% Шаги вверх

% Шаг вверх влево 
move_horse(X, Y) :- New_X is X - 1, New_Y is Y + 2,
	% Проверка на выход за перделы доски
	check_board(New_X, New_Y),
	% Проверка, что на этом месте не были
	not(horse_position(New_X, New_Y)),
	format("Step up to the left.~nNew position (~w,~w)~n",
		[New_X, New_Y]),
	% Запоминание нового местоположения коня
	assert(horse_position(New_X, New_Y)).

% Шаг вниз влево
move_horse(X, Y) :- New_X is X - 1, New_Y is Y - 2,
	check_board(New_X, New_Y),
	not(horse_position(New_X, New_Y)),
	format("~nDown left.~nNew position (~w,~w)~n",
		[New_X, New_Y]),
	assert(horse_position(New_X, New_Y)).

% Шаг вниз вправо
move_horse(X, Y) :- New_X is X + 1, New_Y is Y - 2,
	check_board(New_X, New_Y),
	not(horse_position(New_X, New_Y)),
	format("~nDown right.~nNew position (~w,~w)~n",
		[New_X, New_Y]),
	assert(horse_position(New_X, New_Y)).

% Шаг вверх вправо
move_horse(X, Y) :- New_X is X + 1, New_Y is Y + 2,
	check_board(New_X, New_Y),
	not(horse_position(New_X, New_Y)),
	format("~nUp to the right.~nNew position (~w,~w)~n",
		[New_X, New_Y]),
	assert(horse_position(New_X, New_Y)).
	
% Шаг вправо вверх
move_horse(X, Y) :- New_X is X + 2, New_Y is Y + 1,
	check_board(New_X, New_Y),
	not(horse_position(New_X, New_Y)),
	format("~nRight up.~nNew position (~w,~w)~n",
		 [New_X, New_Y]),
	assert(horse_position(New_X, New_Y)).

% Шаг вправо вниз
move_horse(X, Y) :- New_X is X + 2, New_Y is Y - 1,
	check_board(New_X, New_Y),
	not(horse_position(New_X, New_Y)),
	format("~nRight down.~nNew position (~w,~w)~n",
		[New_X, New_Y]),
	assert(horse_position(New_X, New_Y)).

% Шаг влево вниз
move_horse(X, Y) :- New_X is X - 2, New_Y is Y - 1,
	check_board(New_X, New_Y),
	not(horse_position(New_X, New_Y)),
	format("~nLeft down.~nNew position (~w,~w)~n",
		[New_X, New_Y]),
	assert(horse_position(New_X, New_Y)).

% Шаг влево вверх
move_horse(X, Y) :- New_X is X - 2, New_Y is Y + 1,
	check_board(New_X, New_Y),
	not(horse_position(New_X, New_Y)),
	format("~nLeft up.~nNew position (~w,~w)~n",
		[New_X, New_Y]),
	assert(horse_position(New_X, New_Y)).

horse_movement(X, Y) :-
	format("~nNew movement~n"),
	move_horse(X, Y).

three_horse_moves(FirstX, FirstY) :-
	format("~nStart position (~w,~w)~n", [FirstX, FirstY]),
	check_board(FirstX, FirstY),
	assert(horse_position(FirstX, FirstY)),

	% Расчёт первого хода
	format("~nFirst step:~n"),
	move_horse(FirstX, FirstY),

	% Расчёт второго хода
	format("~nSecond step:~n"),
	horse_position(SecondX, SecondY),
	horse_movement(SecondX, SecondY),

	% Расчёт третьего хода
	format("~nThird step:~n"),
	horse_position(ThirdX, ThirdY),
	horse_movement(ThirdX, ThirdY).