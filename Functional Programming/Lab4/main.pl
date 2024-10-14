/*У Алисы в стране чудес была украдена мука, ее нашли в домике, где жил
Мартовский заяц, Болванщик и Соня. Мартовский заяц сказал: «Муку украл Болванщик».
Болванщик и Соня дали показания, но они утеряны. В ходе заседания выяснилось, что
укравший муку дал правдивые показания.
Вопрос: кто украл муку?*/

flour(house).

house(march_hare, live).
house(hatter, live).
house(sofia, live).

flour_statement(march_hare, hatter).

truth(X) :- flour_statement(X, _).

thief(X) :- house(X, live), truth(X).
