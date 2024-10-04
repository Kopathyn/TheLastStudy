; Уравнение A/x + B = C / x
; x = (C - A) / B

; Уравнение в виде списка
(setq Equation '(A / x + B = C / x))

; Выделение левой части относительно заданного символа
; lst - список
; symb - символ
(defun left(lst symb)
    (if lst
        (
            let
            (
                (head (first lst))
            )

            (if (eq head symb)
                nil
                (cons head (left (rest lst) symb))
            )
        )
        nil
    )
)

; Выделение правой части относительно заданного символа
; lst - список
; symb - символ
(defun right (lst symb)
    (if lst
        (
            let
            (
                (tail (rest lst))
            )
            (if (eq (first lst) symb) 
            tail
            (right tail symb) 
            )
        )
        nil
    )
)

; Решение
(defun res(e)
    (let 
        (
            (leftExpr (left e '=))
        )
        (if leftExpr
            (let 
            (
                (aVal (first leftExpr))
                (cVal (first (right e '=)))
                (bVal (first (right leftExpr '+)))
                (var (first (right(right e '=) '/)))
            )
                (if (and cVal aVal bVal var)
                    (list var '= (list cVal '- aVal)  '/ bVal)
                nil
                )
            )
        nil
        )
    )
)

; Запуск решения
; Т.к. в нем сообщение об ошибке
(defun res2(e)
    (let 
        (
            (r (res e))
        )
        (if r r "Ошибка")
    )
)

(write (res2 Equation))