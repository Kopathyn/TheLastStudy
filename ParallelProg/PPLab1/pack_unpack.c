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
    double startwtime, endwtime; // ���������� �������

    MPI_Init(&argc, &argv); // ������������� � ����������� cmd
    MPI_Comm_size(MPI_COMM_WORLD, &numprocs); // ��������� ���������� ��������� �������������
    MPI_Comm_rank(MPI_COMM_WORLD, &procRang); // ��������� ����� ��������

    startwtime = MPI_Wtime(); // �������� ������� ������

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

        // inbuf - ����� �������� �������
        // incount - ���������� ������� ���������
        // datatype - ��� ������ �������� ������� ������
        // outbuf - �������� ������
        // outsize - ������ ��������� �������
        // position - ��������� � ������ � ������
        // comm - ������������
        MPI_Pack(&xl, 1, MPI_DOUBLE, &pack_buf, 100, &position, MPI_COMM_WORLD);
        MPI_Pack(&xh, 1, MPI_DOUBLE, &pack_buf, 100, &position, MPI_COMM_WORLD);
        MPI_Pack(&c, 1, MPI_DOUBLE, &pack_buf, 100, &position, MPI_COMM_WORLD);
    }

    MPI_Bcast(&intervals, 1, MPI_INT, 0, MPI_COMM_WORLD);
    MPI_Bcast(&pack_buf, 100, MPI_PACKED, 0, MPI_COMM_WORLD);
    

    if (procRang != 0)
    {
        // inbuf - ����� �������� ������
        // insize - ������ �������� ������
        // position - ��������� � ������ � ������
        // outbuf - �������� ������
        // outcount - ���������� ������������� ���������
        // datatype - ��� ������ �������� ������� ������
        // comm - ������������
        MPI_Unpack(pack_buf, 100, &position, &xl, 1, MPI_DOUBLE, MPI_COMM_WORLD);
        MPI_Unpack(pack_buf, 100, &position, &xh, 1, MPI_DOUBLE, MPI_COMM_WORLD);
        MPI_Unpack(pack_buf, 100, &position, &c, 1, MPI_DOUBLE, MPI_COMM_WORLD);
    }

    step = (xh - xl) / (double)intervals; // ��� ��������������
    sum = 0.0;

    // ���������� ����������
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

    // �������� MPI ���������
    MPI_Finalize();

    return 0;
}