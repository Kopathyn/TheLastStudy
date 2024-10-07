; Напишите функцию (drop L N P). 
; которая удаляет N элементов с начала списка L,
; удовлетворяющих некоторому предикату P.

; Предикат
(defun is-even (n) (= 0 (mod n 2)))

(defun diapaz (n) (and (>= n 7) (<= n 10)))
; Диапазон 7 - 10

; Реализация drop
(defun drop (L N P)
  (if L

  (if (= N 0)
      L
      (if (funcall P (car L))
          (drop (cdr L) (- N 1) P)
          (cons (car L) (drop (cdr L) N P))
        )
    )

    nil)
)

(drop '(1 2 7 4 5 8) 3 'diapaz)