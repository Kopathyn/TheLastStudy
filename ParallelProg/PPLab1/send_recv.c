#include <stdio.h>
#include "mpi.h"
#include <math.h>

static double HyperSinSh(double x) { return (exp(x) - exp(-x)) / 2; }

static double HyperCosCh(double x) { return (exp(x) + exp(-x)) / 2; }

static double f(double a) { return HyperCosCh(0.9 * a) * cos(0.9 * a); }

static double fi(double a) { return (( HyperSinSh(0.9 * a) * cos (0.9 * a) + HyperCosCh(0.9 * a) * sin(0.9 * a) ) /  (0.9 * 2)); }

int main(int argc, char* argv[])
{
    int intervals = 100, procRang, numprocs, i;
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

    // Если на процессе, ранг которого 0 (главный процесс)
    if (procRang == 0)
    {
        startwtime = MPI_Wtime(); // Фиксация времени начала

        /* Каждому процессу отправляем количество интервалов */

        for (i = 1; i < numprocs; i++)
        {
            // buf - переменная с данными
            // count - количество данных
            // datatype - тип данных
            // dest - номер процесса
            // tag - тэг различия сообщений
            // comm - какой коммуникатор

            MPI_Send(&intervals, 1, MPI_INT, i, 1, MPI_COMM_WORLD);
        }
    }

    // От каждого процесса получаем результат
    // buf - куда записать данные
    // count - количество данных
    // datatype - тип данных
    // source - на какой процесс
    // tag - тэг сообщения
    // comm - коммуникатор
    // status - статус исполнения
    else
        MPI_Recv(&intervals, 1, MPI_INT, 0, 1, MPI_COMM_WORLD, &stats);

    step = (xh - xl) / (double)intervals; // Шаг интегрирования
    sum = 0.0;

     //Выполнение вычислений
    for (i = procRang + 1; i <= intervals; i += numprocs)
    {
        x = xl + step * ((double)i - 0.5);
        sum += f(x);
    }

    myfunk = step * sum;

    /* Sending the local sum to node 0 */
    if (procRang != 0)
        MPI_Send(&myfunk, 1, MPI_DOUBLE, 0, 1, MPI_COMM_WORLD);

    if (procRang == 0)
    {
        funk = myfunk;

         // Получаем результаты вычислений
        for (i = 1; i < numprocs; i++)
        {
            MPI_Recv(&myfunk, 1, MPI_DOUBLE, i, 1, MPI_COMM_WORLD, &stats);
            funk += myfunk;
        }

        char filePath[] = "output.txt";
        char openType[] = "w";

        FILE* outputFile = fopen(filePath, openType);

        fprintf(outputFile, "Done with send and recv functions\n");
        fprintf(outputFile, "Integral is approximately % .16f, Error % .16f\n", funk, funk - fi(xh) + fi(xl));

        endwtime = MPI_Wtime();
        fprintf(outputFile, "Time of calculation = %f\n", endwtime - startwtime);

        fclose(outputFile);
    }

    // Закрытие MPI программы
    MPI_Finalize();

    return 0;
}