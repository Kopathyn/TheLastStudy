//#pragma region Task_Var14
//// Вычислить среднее арифметическое пар соседних элементов (0,1; 1,2; и т.д). 
//// Обеспечить четное число элементов частей массива.
//#pragma endregion
//
//#include "AdditionalFunctions.h"
//
//float Calc_Avg(float* nums, int size)
//{
//	float sum, result;
//
//	for (int i = 0; i < size; i++)
//		sum = nums[i];
//
//	return result = sum / size;
//}
//
//int main(int argc, char** argv)
//{
//	// Инициализация MPI
//	MPI_Init(&argc, &argv);
//
//	int rank; // Ранг вызывающего процесса
//	MPI_Comm_rank(MPI_COMM_WORLD, &rank);
//
//	int procAmount; // Кол-во процессов в коммуникаторе
//	MPI_Comm_size(MPI_COMM_WORLD, &procAmount);
//
//	printf("Proccess %d of %d is started\n", rank, procAmount);
//
//	float* array; // Массив случайных чисел
//	int arrayElem = 20; // Количество элементов в массиве
//
//	array = InitArray_genarg(arrayElem);
//
//	float* res = (float*)malloc(4 * sizeof(float));
//	MPI_Scatter(array, 4, MPI_FLOAT, res, 4, MPI_FLOAT, 0, MPI_COMM_WORLD);
//	Calc_Avg(array, 4);
//	float* res = (float*)malloc(arrayElem * sizeof(float));
//
//	MPI_Gather();
//
//	MPI_Finalize();
//
//	return 0;
//}