#include "mpi.h"
#include <stdio.h>
#include <math.h>

#define C 0.9

static double HyperSinSh(double x) { return (exp(x) - exp(-x)) / 2; }

static double HyperCosCh(double x) { return (exp(x) + exp(-x)) / 2; }

static double f(double a) { return HyperCosCh(C * a) * cos(C * a); }

static double fi(double a) { return (( HyperSinSh(C * a) * cos (C * a) + HyperCosCh(C * a) * sin(C * a) ) /  2); }

int main(int argc, char* argv[])
{
    int intervals, procRang, numprocs, i;
    double myfunk, funk, step, sum, x;

    double xl = -1.2,	// начало
        xh = 0.8;	// конец

    double startwtime, endwtime; // Переменные времени
    int  namelen;
    char processor_name[MPI_MAX_PROCESSOR_NAME];
    MPI_Status stats;

    MPI_Init(&argc, &argv); // Инициализация с параметрами cmd
    MPI_Comm_size(MPI_COMM_WORLD, &numprocs); // Получение количества процессов коммуникатора
    MPI_Comm_rank(MPI_COMM_WORLD, &procRang); // Получение ранга процесса
    MPI_Get_processor_name(processor_name, &namelen); // Получение имени процессора

    fprintf(stderr, "Process %d on %s\n", procRang, processor_name);
    fflush(stderr);

    intervals = 0; // Количество интервалов
    while (1)
    {
        // Если на процессе, ранг которого 0 (главный процесс)
        if (procRang == 0)
        {
            printf("Enter the number of intervals (0 quit) "); fflush(stdout);
            scanf("%d", &intervals);

            startwtime = MPI_Wtime(); // Фиксация времени начала

            /* Каждому процессу отправляем количество интервалов */

            for (i = 1; i < numprocs; i++)
            {
                MPI_Send(&intervals,                /* Перменная, в которой данные  */
                    1,                  /* Количество данных*/
                    MPI_INT,         /* Тип данных */
                    i,                  /* Какому процессу передаем       */
                    1,                  /* Тэг для различия сообщений */
                    MPI_COMM_WORLD);    /* Какой коммуникатор  */
            }
        }

        // От каждого процесса получаем результат
        else
            MPI_Recv(&intervals, 1, MPI_INT, 0, 1, MPI_COMM_WORLD, &stats);

        if (intervals == 0)
            break;

        else
        {
            step = (xh - xl) / (double)intervals; // Шаг интегрирования
            sum = 0.0;

            for (i = procRang + 1; i <= intervals; i += numprocs)
            {
                x = xl + step * ((double)i - 0.5);
                sum += f(x);
            }

            myfunk = step * sum;
            printf("Process %d SUMM %.16f\n", procRang, myfunk);

            /* Sending the local sum to node 0 */
            if (procRang != 0)
            {
                MPI_Send(&myfunk,                /* buffer               */
                    1,                  /* one data            */
                    MPI_DOUBLE,         /* type                */
                    0,                  /* to which node       */
                    1,                  /* number of message   */
                    MPI_COMM_WORLD);    /* common communicator  */
            }

            if (procRang == 0)
            {
                funk = myfunk;
                for (i = 1; i < numprocs; i++)
                {
                    MPI_Recv(&myfunk, 1, MPI_DOUBLE, i, 1, MPI_COMM_WORLD, &stats);
                    funk += myfunk;
                };

                printf("Integral is approximately  %.16f, Error   %.16f\n", funk, funk - fi(xh) + fi(xl));
                endwtime = MPI_Wtime();
                printf("Time of calculation = %f\n", endwtime - startwtime);
            }
        }
    }
    // Закрытие MPI программы
    MPI_Finalize();

    return 0;
}