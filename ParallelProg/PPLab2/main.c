// Var 14 - среднее-арифметическое пар соседних элементов

#include <stdlib.h>
#include <stdio.h>
#include <time.h>
#include "mpi.h"

/// <summary>
/// Создание динамического массива случайных чисел
/// </summary>
/// <param name="NumAmount">Количество элементов</param>
/// <returns>Массив случайных чисел</returns>
float* CreateArrayWithGenarg(int NumAmount)
{
    srand(time(0));
    float* RandNumsArray = (float*)malloc(sizeof(float) * NumAmount);

    for (int i = 0; i < NumAmount; i++)
        RandNumsArray[i] = (float)rand() / RAND_MAX * 2.0f;

    return RandNumsArray;
}

/// <summary>
/// Вывод массива в файл
/// </summary>
void PrintArray(FILE* file, float* array, int size)
{

    for (int i = 0; i < size; i++)
        fprintf(file, "%.3f ", array[i]);

    fprintf(file, "\n\n");
}

int main(int argc, char* argv[])
{
    // Переменные времени
    double startTime, endTime;

    int procRank; // Ранг процесса
    int procSize; // Кол-во процессов

    char filePath[] = "output.txt";
    char openType[] = "w";
    FILE* outputFile = fopen(filePath, openType);

    MPI_Init(&argc, &argv);
    MPI_Comm_rank(MPI_COMM_WORLD, &procRank);
    MPI_Comm_size(MPI_COMM_WORLD, &procSize);

    int initArraySize = procSize * 10; // Количество элементов в массиве
    int finalArraySize = initArraySize / 2;

    int partArraySize = initArraySize / procSize; // Количество элементов для части массива
    int halfPartArraySize = partArraySize / 2;

    // Обеспечение четности частей основного массива
    // для процессов
    if (partArraySize % 2 != 0)
        partArraySize--;

    float* initArray = NULL; // Массив данных
    float* finalArray = NULL; // Массив результатов
    if (procRank == 0)
    {
        startTime = MPI_Wtime();

        initArray = CreateArrayWithGenarg(initArraySize);
        finalArray = (float*)malloc(sizeof(float) * finalArraySize);

        fprintf(outputFile, "Initial Array:\n");
        PrintArray(outputFile, initArray, initArraySize);
    }

    // Инициализация массива, в котором будет часть исходного массива
    float* partArray = (float*)malloc(sizeof(float) * partArraySize);

    // Рассылка частей основного массива процессам
    MPI_Scatter(initArray, partArraySize, MPI_FLOAT, partArray, partArraySize, MPI_FLOAT, 0, MPI_COMM_WORLD);

    float* avg = (float*)malloc(sizeof(float) * halfPartArraySize);

    for (int i = 0, j = 0; i < partArraySize; i += 2, j++)
    {
        avg[j] = (partArray[i] + partArray[i + 1]) / 2;
    }

    // Отправка результатов в конечный массив
    MPI_Gather(avg, halfPartArraySize, MPI_FLOAT, finalArray, halfPartArraySize, MPI_FLOAT, 0, MPI_COMM_WORLD);

    free(avg);
    free(partArray);

    if (procRank == 0)
    {
        endTime = MPI_Wtime();

        fprintf(outputFile, "Result Array:\n");
        PrintArray(outputFile, finalArray, finalArraySize);

        fprintf(outputFile, "Time wasted: %f\n", endTime - startTime);

        free(initArray);
        free(finalArray);
    }

    MPI_Finalize();

    fclose(outputFile);

    return 0;
}