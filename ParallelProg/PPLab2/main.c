//#pragma region Task_Var14
//// ��������� ������� �������������� ��� �������� ��������� (0,1; 1,2; � �.�). 
//// ���������� ������ ����� ��������� ������ �������.
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
//	// ������������� MPI
//	MPI_Init(&argc, &argv);
//
//	int rank; // ���� ����������� ��������
//	MPI_Comm_rank(MPI_COMM_WORLD, &rank);
//
//	int procAmount; // ���-�� ��������� � �������������
//	MPI_Comm_size(MPI_COMM_WORLD, &procAmount);
//
//	printf("Proccess %d of %d is started\n", rank, procAmount);
//
//	float* array; // ������ ��������� �����
//	int arrayElem = 20; // ���������� ��������� � �������
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