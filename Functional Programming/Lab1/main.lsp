(defvar students
	(list
		`("Иванов" ("Информатика" 5) ("Физика" 4)) 
		`("Петров" ("Геометрия" 3) ("География" 4))
		`("Сидоров" ("Математика" 3) ("Физкультура" 5))
	)
)

(defun avg(students_list)
	(if
		(not (null students_list))
		(
			let
			(
				(head (car students_list))
				(body (cdr students_list))
			)
			
			(
				let
				(
					(surname (first head))
					(subjects (cdr head))
				)

				(
					let 
					(
						(first_subject  (car (cdar subjects))) 
						(second_subject (cadar (cdr subjects)))
					)
					(cons
						(list surname (/ (* 1.0(+ first_subject second_subject)) 2))
						(avg body)
					)
				)
			)
		)
		nil
	)
)

(avg students)