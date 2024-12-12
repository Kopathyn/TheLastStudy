// Var 6 - Выбрать нечетные столбцы матрицы. (if colums % 2 != 0 => add(colList, colums)
#include <stdlib.h>
#include <stdio.h>
#include <time.h>
#include "mpi.h"

#pragma region BasicMatrixFuncions

/// <summary>
/// Вывод матрицы в консоль
/// </summary>
/// <param name="matrix">Матрица</param>
/// <param name="rows">Количество строк</param>
/// <param name="columns">Количество столбцов</param>
void PrintMatrix(int* matrix, int rows, int columns)
{
    for (int i = 0; i < rows * columns; i++)
    {
        if (i % rows == 0)
            printf("\n");

        printf("%d ", matrix[i]);
    }

    printf("\n");
}

/// <summary>
/// Очистка памяти матрицы
/// </summary>
/// <param name="matrix">Матрица</param>
/// <param name="rows">Количество строк</param>
void KillMatrix(int* matrix)
{
    free(matrix);
}

#pragma endregion

/// <summary>
/// Создание матрицы случайных чисел
/// </summary>
/// <param name="rows">Количество строк</param>
/// <param name="columns">Количество столбцов</param>
/// <returns>Матрица случайных чисел</returns>
void CreateMatrixWithIntNums(int* matrix, int rows, int columns)
{

    for (int i = 0; i < rows * columns; i++)
        //matrix[i][j] = (float)rand() / (RAND_MAX * 2.0f);
        matrix[i] = i;
}

/// <summary>
/// Обнуление матрицы
/// </summary>
/// <param name="matrix">Исходная матрица</param>
/// <param name="rows">Количество строк</param>
/// <param name="columns">Количество столбцов</param>
int* nullMatrix(int* matrix, int rows, int columns)
{
    for (int i = 0; i < rows * columns; i++)
        matrix[i] = 0;

    return matrix;
}

int main(int argc, char* argv[])
{
    // Переменные времени
    //double startTime, endTime;

    int procRank; // Ранг процесса
    int procSize; // Кол-во процессов

    const int rows = 10, cols = 10;
    int matrixSize = rows * cols;

    int* firstMatrix = (int*)malloc(sizeof(int) * (rows * cols));    

    MPI_Status stats;
    MPI_Init(&argc, &argv);
    MPI_Comm_rank(MPI_COMM_WORLD, &procRank);
    MPI_Comm_size(MPI_COMM_WORLD, &procSize);

    // Проверка на 2 процесса
    if (procSize > 2)
    {
        printf("Program created to work with 2 processes!\n");
        MPI_Finalize();
        return -1;
    }

    if (procRank == 0)
    {
        CreateMatrixWithIntNums(firstMatrix, rows, cols);

        printf("Process %d\nMatrix is:\n", procRank);
        PrintMatrix(firstMatrix, rows, cols);
        printf("\n");

        MPI_Send(firstMatrix, matrixSize, MPI_INT, 1, 0, MPI_COMM_WORLD);
    }

    if (procRank == 1)
    {
        // Получаем от 0 процесса матрицу
        MPI_Recv(firstMatrix, matrixSize, MPI_INT, 0, 0, MPI_COMM_WORLD, &stats);

        printf("Process %d\n", procRank);

        printf("Nulling matrix...\n");
        printf("\n");

        firstMatrix = nullMatrix(firstMatrix, rows, cols);

        printf("Matrix is nulled! Printing...\n");
        PrintMatrix(firstMatrix, rows, cols);
    }

    KillMatrix(firstMatrix);
       
    MPI_Finalize();

    return 0;
}