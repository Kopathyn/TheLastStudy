; Напишите функцию (drop L N P). 
; которая удаляет N элементов с начала списка L,
; удовлетворяющих некоторому предикату P.

; Предикат
(defun is-even (n) (= 0 (mod n 2)))

; Реализация drop
(defun drop (L N P)
  (if (= N 0)
      L
      (if (funcall P (car L))
          (drop (cdr L) (- N 1) P)
          (cons (car L) (drop (cdr L) N P))
        )
    )
)

(write (drop '(1 2 3 4 5) 1 'is-even))