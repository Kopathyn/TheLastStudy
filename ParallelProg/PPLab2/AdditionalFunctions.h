#pragma once

#include <stdio.h>
#include <mpi.h>
#include <stdlib.h>

/// <summary>
/// ������������� ������� ��������� �����
/// ��������� ����� ������������ �������� �� ��������� (genarg)
/// </summary>
/// <param name="numAmount">���������� ��������</param>
/// <returns>������ ��������� ��������</returns>
static float* InitArray_genarg(int numAmount)
{
	int numRange = 100;
	int startWith = 0;

	int n, i, st;
	double x, dp, rmax, max, min;

	float* array = (float*)malloc(numAmount * sizeof(float));

	n = numAmount;
	dp = numRange;
	st = startWith;
	srand(st);

	for (i = 0; i < n; i++)
		array[i] = (float)rand() / RAND_MAX * dp * 2.0f - dp;

	return array;
}

/// <summary>
/// ����� �������
/// </summary>
/// <param name="array">������</param>
/// <param name="size">������ �������</param>
static void PrintArray(float* array, int size)
{
	for (int i = 0; i < size; i++)
		printf("%.3f ");

	printf("\n");
}