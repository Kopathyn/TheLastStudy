// Var 14 - среднее-арифметическое пар соседних элементов

#include <stdlib.h>
#include <stdio.h>
#include <time.h>
#include "mpi.h"

/// @brief Функция вычисления пар среднего арифметического пар соседних элементов
/// @param array Массив данных
/// @param size Размер массива
/// @return Среднее арифметическое пар соседних элементов
float average_pairs(float* array, int size)
{
    double sum = 0;
    int counter = 0;

    while (counter < size - 1)
    {
        sum += (array[counter] + array[counter + 1]) / 2;
        counter += 2;
    }

    if (size % 2 != 0)
        sum += array[size - 1] / 2;

    return sum;
}

/// @brief Создание динамического массива случайных чисел
/// @param NumAmount Количество чисел
/// @return Массив случайных чисел
float* CreateArrayWithGenarg(int NumAmount)
{
    srand(time(0));
    float* RandNumsArray = malloc(sizeof(float) * NumAmount);

    for (int i = 0; i < NumAmount; i++)
        RandNumsArray[i] = (float)rand() / RAND_MAX * 2.0f;

    return RandNumsArray;
}

/// @brief Вывод массива данных в консоль
/// @param array Массив данных
/// @param size Размер массива
void PrintArray(float* array, int size)
{
    for (int i = 0; i < size; i++)
        printf("%f ", array[i]);

    printf("\n");
}

int main(int argc, char* argv[])
{
    // Переменные времени
    double startTime, endTime;

    int procRank; // Ранг процесса
    int procSize; // Кол-во процессов

    float* initArray = NULL; // Массив данных
    float* partArray; // Массив для счета

    float procArraySum; // Подсчитанное процессом
    float globalSum; // Общая сумма

    MPI_Init(&argc, &argv);
    MPI_Comm_rank(MPI_COMM_WORLD, &procRank);
    MPI_Comm_size(MPI_COMM_WORLD, &procSize);

    int initArraySize = procSize * 1000; // Количество элементов в массиве
    int partArraySize = initArraySize / procSize; // Количество элементов для части массива

    if (procRank == 0)
    {
        startTime = MPI_Wtime();

        initArray = CreateArrayWithGenarg(initArraySize);

        printf("Init count avg: %f\n", average_pairs(initArray, initArraySize));
    }

    // Инициализация массива, в котором будет часть исходного массива
    partArray = malloc(sizeof(float) * partArraySize);

    // Рассылка частей массива
    MPI_Scatter(initArray, partArraySize, MPI_FLOAT, partArray, partArraySize, MPI_FLOAT, 0, MPI_COMM_WORLD);

    // Подсчет
    procArraySum = average_pairs(partArray, partArraySize);
    printf("Proccess %d has counted: %f\n", procRank, procArraySum);

    // Массив хранения средних значений частей массива
    float* totalProcArraySum = NULL; 
    if (procRank == 0)
        totalProcArraySum = malloc(sizeof(float) * procSize);

    // Отправка в totalProcArraySum
    MPI_Gather(&procArraySum, 1, MPI_FLOAT, totalProcArraySum, 1, MPI_FLOAT, 0, MPI_COMM_WORLD);

    free(partArray);

    if (procRank == 0)
    {
        endTime = MPI_Wtime();

        float totalSum = 0;

        for (int i = 0; i < procSize; i++)
            totalSum += totalProcArraySum[i];

        printf("Sum of all pairs: %f\n", totalSum);
        printf("Time wasted: %f\n", endTime - startTime);

        free(initArray);
        free(totalProcArraySum);
    }

    MPI_Finalize();

    return 0;
}