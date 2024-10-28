#include <stdio.h>
#include <mpi.h>
#include <stdlib.h>
#define dp 50.0f
/* Function for init of matrix*/
void init_matrix(int m1, int n1, float* arr1)
{
	int i, j;
	for (i = 0; i < m1; i++)
		for (j = 0; j < n1; j++)
			arr1[i * n1 + j] = (float)rand() / RAND_MAX * dp * 2.0f - dp;
}
/* Function for max and min the each row of matrix */
/* Matrix and two vectors */
void mamirows(int m1, int n1, float* arr1,
	float* arr2, float* arr3)
{
	int i, j; float ma, mi, memb;
	for (i = 0; i < m1; i++)
	{
		ma = mi = arr1[i * n1 + 0];
		for (j = 1; j < n1; j++)
		{
			memb = arr1[i * n1 + j];
			if (memb < mi) mi = memb;
			else if (memb > ma) ma = memb;
		}
		arr2[i] = mi; arr3[i] = ma;
	}
}
/* Function for output matrix */
void prnmatr(int m1, int n1, float* arr1, char* zag)
{
	int i, j;
	printf("%s\n", zag);
	for (i = 0; i < m1; i++)
	{
		for (j = 0; j < n1; j++) printf("%8.2f", arr1[i * n1 + j]);
			printf("\n");
	}; printf("\n");
}
/* Function for output vector */
void prnvect(int n1, float* arr1, char* zag, int rnk)
{
	int i, j;
	printf("%s %d\n", zag, rnk);
	for (j = 0; j < n1; j++) printf("%8.2f", arr1[j]);
	printf("\n");
}
int main(int argc, char** argv)
{
	int m, n;
	int rank, size;
	int nach, count, i, scol, ost;
	/* Enter m and n from the command line */
	m = atoi(argv[1]); n = atoi(argv[2]);
	double time1, time2;
	float*  ma = 0;/* Input matrix */
	float* vmin = 999999; float* vmax = 0; float* smin = 999999; float* smax = 0;/* Output vectors */
	/* Auxilary arrays for processes */
	int* adispls = 0, * acounts = 0, * vcounts = 0, * vdispls = 0;
	MPI_Init(&argc, &argv);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	printf("Proccess %d of %d is started\n", rank, size);
	/* Parts rows for process */
	count = m / size; ost = m % size;
	if (rank == 0)/* Process 0 - master */
	{
		/* storage for output vectors*/
		smin = (float*)calloc(m, sizeof(float)); // for sequential
		smax = (float*)calloc(m, sizeof(float)); // calculation
		vmin = (float*)calloc(m, sizeof(float)); // for parallel
		vmax = (float*)calloc(m, sizeof(float)); // calculation
		/* storage for the input matrix and its initialization*/
		ma = (float*)calloc(m * n, sizeof(float));
		init_matrix(m, n, ma);
		prnmatr(m, n, ma, "Input matrix MA");
		/* Sequential calculate */
		mamirows(m, n, ma, smin, smax);
		prnvect(m, smin, "Sequential minimum SMIN, rank", rank);
		prnvect(m, smax, "Sequential maximum SMAX, rank", rank);
		time1 = MPI_Wtime(); //Time begining of parallel programm
		/* auxilary arrays for distributing matrix rows to processes */
		adispls = (int*)malloc(size * sizeof(int));
		acounts = (int*)malloc(size * sizeof(int));
		vdispls = (int*)malloc(size * sizeof(int));
		vcounts = (int*)malloc(size * sizeof(int));
		for (i = 0; i < size; i++)
		{
			scol = i < ost ? count + 1 : count;
			acounts[i] = scol * n; vcounts[i] = scol;
			nach = i * scol + (i >= ost ? ost : 0);
			adispls[i] = nach * n; vdispls[i] = nach;
		}
		/*output of auxilary arrays for test */
		for (i = 0; i < size; i++) printf("%10d", acounts[i]); printf(" acounts\n");
		for (i = 0; i < size; i++) printf("%10d", adispls[i]); printf(" adispls\n");
		for (i = 0; i < size; i++) printf("%10d", vcounts[i]); printf(" vcounts\n");
		for (i = 0; i < size; i++) printf("%10d", vdispls[i]); printf(" vdispls\n");
	} /* End of work process 0 */
	/* All processes */
	/* Rows in part of matrix ma for rank */
	scol = rank < ost ? count + 1 : count;
	/* Offset (in rows) part for rank in matrix ma */
	nach = rank * scol + (rank >= ost ? ost : 0);
	float* map;/* pointer on part matrix in rank */
	float* bmin, * bmax;/* pointers on output vectors in rank */
	map = (float*)calloc(scol * n, sizeof(float));
	bmin = (float*)calloc(scol, sizeof(float));
	bmax = (float*)calloc(scol, sizeof(float));
	/* Distribution of the rows of the matrix to other processes */
	MPI_Scatterv(ma, acounts, adispls, MPI_FLOAT, map, scol * n, MPI_FLOAT, 0, MPI_COMM_WORLD);
	/* Calculation in the one rank */
	printf("Process %d\n", rank);
	prnmatr(scol, n, map, "Part matrix MA");
	mamirows(scol, n, map, bmin, bmax);
	prnvect(scol, bmin, "Part minimum BMIN, rank", rank);
		prnvect(scol, bmax, "Part maximum BMAX, rank", rank);
	/* Assembly of partial vectors into master-process */
	MPI_Gatherv(bmin, scol, MPI_FLOAT, vmin, vcounts, vdispls, MPI_FLOAT, 0, MPI_COMM_WORLD);
	MPI_Gatherv(bmax, scol, MPI_FLOAT, vmax, vcounts, vdispls, MPI_FLOAT, 0, MPI_COMM_WORLD);
	/* Storage release of local arrays in each process */
	free(map); free(bmin); free(bmax);
	if (rank == 0) /* Only master-process */
	{
		/* Results of the parallel calculanions */
		printf("Results of parallel.\n");
		prnvect(m, vmin, "Parallel minimum VMIN, rank", rank);
		prnvect(m, vmax, "Parallel maximum VMAX, rank", rank);
		time2 = MPI_Wtime(); //Time ending programm
		printf("\ntime= %f\n", time2 - time1); // Time calculating
		/* Storage release of arrays in master-process */
		free(ma); free(smin); free(smax); free(vmin); free(vmax);
		free(adispls); free(acounts); free(vdispls); free(vcounts);
	} /* End of work master-process */
	MPI_Finalize();
	return 0;
}