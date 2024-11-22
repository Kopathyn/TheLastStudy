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

    double xl = -1.2,	// ������
        xh = 0.8;	// �����

    double startwtime, endwtime; // ���������� �������
    int  namelen;
    char processor_name[MPI_MAX_PROCESSOR_NAME];
    MPI_Status stats;

    MPI_Init(&argc, &argv); // ������������� � ����������� cmd
    MPI_Comm_size(MPI_COMM_WORLD, &numprocs); // ��������� ���������� ��������� �������������
    MPI_Comm_rank(MPI_COMM_WORLD, &procRang); // ��������� ����� ��������

    // ���� �� ��������, ���� �������� 0 (������� �������)
    if (procRang == 0)
    {
        startwtime = MPI_Wtime(); // �������� ������� ������

        /* ������� �������� ���������� ���������� ���������� */

        for (i = 1; i < numprocs; i++)
        {
            // buf - ���������� � �������
            // count - ���������� ������
            // datatype - ��� ������
            // dest - ����� ��������
            // tag - ��� �������� ���������
            // comm - ����� ������������

            MPI_Send(&intervals, 1, MPI_INT, i, 1, MPI_COMM_WORLD);
        }
    }

    // �� ������� �������� �������� ���������
    // buf - ���� �������� ������
    // count - ���������� ������
    // datatype - ��� ������
    // source - �� ����� �������
    // tag - ��� ���������
    // comm - ������������
    // status - ������ ����������
    else
        MPI_Recv(&intervals, 1, MPI_INT, 0, 1, MPI_COMM_WORLD, &stats);

    step = (xh - xl) / (double)intervals; // ��� ��������������
    sum = 0.0;

     //���������� ����������
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

         // �������� ���������� ����������
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

    // �������� MPI ���������
    MPI_Finalize();

    return 0;
}