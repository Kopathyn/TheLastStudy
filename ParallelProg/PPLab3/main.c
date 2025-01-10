#include <stdlib.h>
#include <stdio.h>
#include "mpi.h"

void PrintMatrix(int* matrix, int rows, int columns, int procRank)
{
    printf("Process %d\nMatrix is:\n", procRank);
    for (int i = 0; i < rows * columns; i++)
    {
        if (i % columns == 0)
            printf("\n");

        printf(" %d ", matrix[i]);
    }

    printf("\n\n");
}

void CreateMatrixWithIntNums(int* matrix, int rows, int columns)
{
    for (int i = 0; i < rows * columns; i++)
        matrix[i] = i + 1;
}

void BuildVectorType(int rows, int cols, MPI_Datatype* messageType)
{
    int oddCount = (rows * cols) / 2; // Количество нечетных столбцов
    int blocklength = 1; // Количество элементов в каждом блоке
    int stride = 2; // Шаг между блоками (нечетные столбцы)

    // Создаем производный тип данных для нечетных столбцов
    MPI_Type_vector(oddCount, blocklength, stride, MPI_INT, messageType);
    MPI_Type_commit(messageType);
}

void ReloadMatrix(int** matrix, int size)
{
    free(*matrix);

    *matrix = (int*)calloc(size, sizeof(int));
}

int main(int argc, char* argv[])
{
    int procRank; // Ранг процесса
    int procSize; // Кол-во процессов

    const int rows = 10, cols = 10; // Количество строк и столбцов
    int matrixSize = rows * cols;

    MPI_Status stats;
    MPI_Init(&argc, &argv);
    MPI_Comm_rank(MPI_COMM_WORLD, &procRank);
    MPI_Comm_size(MPI_COMM_WORLD, &procSize);

    MPI_Datatype oddCols;

    BuildVectorType(rows, cols, &oddCols);

    int* intMatrix = (int*)calloc(matrixSize, sizeof(int));
    int* nulledMatrix = (int*)calloc(matrixSize, sizeof(int));

    if (procRank == 0)
    {
        /* Инициализируем и отправляем матрицу целых чисел для обнуления */
        CreateMatrixWithIntNums(intMatrix, rows, cols);

        PrintMatrix(intMatrix, rows, cols, procRank);

        /* Отправляем матрицу целых чисел 2 раза */
        MPI_Send(intMatrix, 1, oddCols, 1, 0, MPI_COMM_WORLD); // Для принятия производным типом данных
        MPI_Send(intMatrix, 1, oddCols, 1, 1, MPI_COMM_WORLD); // Для принятия базовым типом данных

        //ReloadMatrix(&intMatrix, matrixSize);
        //MPI_Recv(intMatrix, 1, oddCols, 1, 2, MPI_COMM_WORLD, &stats);

        //PrintMatrix(intMatrix, rows, cols, procRank);
    }

    if (procRank == 1)
    {
        /* Получаем матрицу производным типом данных */
        MPI_Recv(nulledMatrix, 1, oddCols, 0, 0, MPI_COMM_WORLD, &stats);

        printf("MPI Vector:\n");
        PrintMatrix(nulledMatrix, rows, cols, procRank);

        /* Получаем матрицу базовым типом данных */

        ReloadMatrix(&nulledMatrix, matrixSize);
        MPI_Recv(nulledMatrix, matrixSize, MPI_INT, 0, 1, MPI_COMM_WORLD, &stats);

        printf("INT:\n");
        PrintMatrix(nulledMatrix, rows, cols, procRank);

        /* Отправляем матрицу базовым типом данных */
        //ReloadMatrix(&nulledMatrix, matrixSize);
        //CreateMatrixWithIntNums(nulledMatrix, rows, cols);
        //MPI_Send(nulledMatrix, matrixSize, MPI_INT, 0, 2, MPI_COMM_WORLD);
    }

    free(intMatrix);
    free(nulledMatrix);
    MPI_Type_free(&oddCols);
    MPI_Finalize();

    return 0;
}