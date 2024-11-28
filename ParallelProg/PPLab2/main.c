// Var 14 - среднее-арифметическое пар соседних элементов

#include <stdlib.h>
#include <stdio.h>
#include <time.h>
#include <mpi.h>

/// @brief Функция вычисления пар среднего арифметического пар соседних элементов
/// @param array Массив данных
/// @param size Размер массива
/// @return Среднее арифметическое пар соседних элементов
float average_pairs(float *array, int size)
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

    float* array; // Массив данных
    float* partArray; // Массив для счета
    float procArraySum;
    float globalSum;

    MPI_Init(&argc, &argv);
    MPI_Comm_rank(MPI_COMM_WORLD, &procRank);
    MPI_Comm_size(MPI_COMM_WORLD, &procSize);

    int arrSize = procSize * 100; // Количество элементов в массиве
    int partArraySize = arrSize / procSize;

    if (procRank == 0)
    {
        startTime = MPI_Wtime();

        array = CreateArrayWithGenarg(arrSize);
    }

    partArray = malloc(sizeof(float) * partArraySize); 
    
    // Рассылка частей массива
    MPI_Scatter(array, partArraySize, MPI_FLOAT, partArray, partArraySize, MPI_FLOAT, 0, MPI_COMM_WORLD);

    procArraySum = average_pairs(partArray, partArraySize);
    printf("Proccess %d has counted: %f\n", procRank, procArraySum);

    float* totalProcArraySum = NULL;

    if (procRank == 0)
        totalProcArraySum = malloc(sizeof(float) * procSize);

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

        free(array);
        free(totalProcArraySum);
    }

    MPI_Finalize();

    return 0;
}