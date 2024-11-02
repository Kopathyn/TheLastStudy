#include <stdio.h>
#include "mpi.h"
#include <math.h>


static double HyperSinSh(double x) { return (exp(x) - exp(-x)) / 2; }

static double HyperCosCh(double x) { return (exp(x) + exp(-x)) / 2; }

static double f(double a, double c) { return HyperCosCh(c * a) * cos(c * a); }

static double fi(double a, double c) { return ((HyperSinSh(c * a) * cos(c * a) + HyperCosCh(c * a) * sin(c * a)) / 2); }

int main(int argc, char* argv[])
{
    int procRang, numprocs;
    double myfunk, funk, step, sum, x;
    double xl = 0, xh = 0;
    double c;
    int intervals;
    double startwtime, endwtime; // Переменные времени

    MPI_Init(&argc, &argv); // Инициализация с параметрами cmd
    MPI_Comm_size(MPI_COMM_WORLD, &numprocs); // Получение количества процессов коммуникатора
    MPI_Comm_rank(MPI_COMM_WORLD, &procRang); // Получение ранга процесса

    startwtime = MPI_Wtime(); // Фиксация времени начала

    char pack_buf[100];
    int position = 0;

    if (procRang == 0)
    {
        printf("Intervals>> ");
        scanf("%d", &intervals);

        printf("Low border>> ");
        scanf("%lf", &xl);

        printf("High border>> ");
        scanf("%lf", &xh);

        printf("C >> ");
        scanf("%lf", &c);

        // inbuf - адрес входного буффера
        // incount - количество входных элементов
        // datatype - тип данных элемента входных данных
        // outbuf - выходной буффер
        // outsize - размер выходного буффера
        // position - положение в буфере в байтах
        // comm - коммуникатор
        MPI_Pack(&xl, 1, MPI_DOUBLE, &pack_buf, 100, &position, MPI_COMM_WORLD);
        MPI_Pack(&xh, 1, MPI_DOUBLE, &pack_buf, 100, &position, MPI_COMM_WORLD);
        MPI_Pack(&c, 1, MPI_DOUBLE, &pack_buf, 100, &position, MPI_COMM_WORLD);
    }

    MPI_Bcast(&intervals, 1, MPI_INT, 0, MPI_COMM_WORLD);
    MPI_Bcast(&pack_buf, 100, MPI_PACKED, 0, MPI_COMM_WORLD);
    

    if (procRang != 0)
    {
        // inbuf - адрес входного буфера
        // insize - размер входного буфера
        // position - положение в буфере в байтах
        // outbuf - выходной буффер
        // outcount - количество распакованных элементов
        // datatype - тип данных элемента входных данных
        // comm - коммуникатор
        MPI_Unpack(pack_buf, 100, &position, &xl, 1, MPI_DOUBLE, MPI_COMM_WORLD);
        MPI_Unpack(pack_buf, 100, &position, &xh, 1, MPI_DOUBLE, MPI_COMM_WORLD);
        MPI_Unpack(pack_buf, 100, &position, &c, 1, MPI_DOUBLE, MPI_COMM_WORLD);
    }

    step = (xh - xl) / (double)intervals; // Шаг интегрирования
    sum = 0.0;

    // Выполнение вычислений
    for (int i = procRang + 1; i <= intervals; i += numprocs)
    {
        x = xl + step * ((double)i - 0.5);
        sum += f(x, c);
    }

    myfunk = step * sum;

    MPI_Reduce(&myfunk, &funk, 1, MPI_DOUBLE, MPI_SUM, 0, MPI_COMM_WORLD);

    if (procRang == 0)
    {
        char filePath[] = "output.txt";
        char openType[] = "w";
        FILE* outputFile = fopen(filePath, openType);

        fprintf(outputFile, "Done with pack and unpack functions\n");
        fprintf(outputFile, "Integral is approximately % .16f, Error % .16f\n", funk, funk - fi(xh, c) + fi(xl, c));

        endwtime = MPI_Wtime();
        fprintf(outputFile, "Time of calculation = %f\n", endwtime - startwtime);

        fclose(outputFile);
    }

    // Закрытие MPI программы
    MPI_Finalize();

    return 0;
}