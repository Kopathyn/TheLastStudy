#include <stdio.h>
#include "mpi.h"
#include <math.h>

static double HyperSinSh(double x) { return (exp(x) - exp(-x)) / 2; }

static double HyperCosCh(double x) { return (exp(x) + exp(-x)) / 2; }

static double f(double a) { return HyperCosCh(0.9 * a) * cos(0.9 * a); }

static double fi(double a) { return ((HyperSinSh(0.9 * a) * cos(0.9 * a) + HyperCosCh(0.9 * a) * sin(0.9 * a)) / (0.9 * 2)); }

int main(int argc, char* argv[])
{
    int intervals, procRang, numprocs, i;
    double myfunk, funk, step, sum, x;

    double xl = -1.2,	// ������
        xh = 0.8;	// �����

    double startwtime, endwtime; // ���������� �������
    int  namelen;
    MPI_Status stats;

    MPI_Init(&argc, &argv); // ������������� � ����������� cmd
    MPI_Comm_size(MPI_COMM_WORLD, &numprocs); // ��������� ���������� ��������� �������������
    MPI_Comm_rank(MPI_COMM_WORLD, &procRang); // ��������� ����� ��������

    while (1)
    {
        if (procRang == 0)
        {
            printf("Enter amount of intervals:>> ");
            fflush(stdout);
            scanf("%d", &intervals);
        }

        startwtime = MPI_Wtime(); // �������� ������� ������

        /* ������� �������� ���������� ���������� ���������� */

        // buffer - ���������� � �������
        // count - ���������� ������
        // datatype - ��� ������
        // root - �� ������ �������� ��������
        // comm - ������������
        MPI_Bcast(&intervals, 1, MPI_INT, 0, MPI_COMM_WORLD);

        if (intervals == 0)
            break;


        step = (xh - xl) / (double)intervals; // ��� ��������������
        sum = 0.0;

        // ���������� ����������
        for (i = procRang + 1; i <= intervals; i += numprocs)
        {
            x = xl + step * ((double)i - 0.5);
            sum += f(x);
        }

        myfunk = step * sum;

        MPI_Reduce(&myfunk, &funk, 1, MPI_DOUBLE, MPI_SUM, 0, MPI_COMM_WORLD);

        if (procRang == 0)
        {
            char filePath[] = "output.txt";
            char openType[] = "w";
            FILE* outputFile = fopen(filePath, openType);

            fprintf(outputFile, "Done with bcast and reduce functions\n");
            fprintf(outputFile, "Integral is approximately % .16f, Error % .16f\n", funk, funk - fi(xh) + fi(xl));

            endwtime = MPI_Wtime();
            fprintf(outputFile, "Time of calculation = %f\n", endwtime - startwtime);

            fclose(outputFile);
        }
    }
    // �������� MPI ���������
    MPI_Finalize();

    return 0;
}