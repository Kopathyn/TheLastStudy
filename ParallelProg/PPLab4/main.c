#include <omp.h>
#include <stdlib.h>
#include <stdio.h>
#include <math.h>

static double HyperSinSh(double x) { return (exp(x) - exp(-x)) / 2; }

static double HyperCosCh(double x) { return (exp(x) + exp(-x)) / 2; }

static double f(double a) { return HyperCosCh(0.9 * a) * cos(0.9 * a); }

static double fi(double a) { return ((HyperSinSh(0.9 * a) * cos(0.9 * a) + HyperCosCh(0.9 * a) * sin(0.9 * a)) / (0.9 * 2)); }

const int n = 1000000;

int main(int argc, char* argv[])
{
    int threadsMaxCount = 8;
    if (argc == 2) 
        threadsMaxCount = atoi(argv[1]);

    printf("Max threads = %d\n", omp_get_max_threads());

    double startTime = omp_get_wtime();

    double xl = -1.2, xh = 0.8;
    double sum = 0.0;
    double step = (xh - xl) / (double)n;

#pragma omp parallel num_threads(threadsMaxCount)
    {
#pragma omp master // Главный поток
        {
            printf("Num threads = %d\n", omp_get_num_threads());
        }

        printf("This thread #%d\n", omp_get_thread_num());

        double localSum = 0;

#pragma omp for schedule(dynamic, n) // Динамическое деление цикла на части и исполнение
        for (int i = 1; i <= n; i++)
        {
            double x = xl + step * ((double)i - 0.5);
            localSum += f(x);
        }
        localSum *= step;

#pragma omp atomic
        sum += localSum;
    }

    double endTime = omp_get_wtime();

    FILE* fileOutput = fopen("output.txt", "w");
    fprintf(fileOutput, "Integral is approximately: %.16f\nError: %.16f\n", sum, fabs(sum - fi(xh) + fi(xl)));
    fprintf(fileOutput, "Time of calculation %lf\n", endTime - startTime);

    fclose(fileOutput);
}