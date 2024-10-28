/*У Алисы в стране чудес была украдена мука, ее нашли в домике, где жил
Мартовский заяц, Болванщик и Соня. Мартовский заяц сказал: «Муку украл Болванщик».
Болванщик и Соня дали показания, но они утеряны. В ходе заседания выяснилось, что
укравший муку дал правдивые показания.
Вопрос: кто украл муку?*/

% сказал правду только один человек - вор
% показания только вида "украл X"

% Соня украла

% Утверждения о доме
% Кто в нем живет
house(march_hare, live).
house(hatter, live).
house(sofia, live).

flour_statement(X, Y) :- X == Y, 

flour_statement(march_hare, hatter).

% Определение правды в данном контексте
truth(X) :- flour_statement(X, _).

% Определение вора
thief(X) :- house(X, live), truth(X).

% Перебор показаний болванщика и Сони