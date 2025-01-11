#include <stdio.h>
#include <stdlib.h>
#include <omp.h>
#include <time.h>

#define BUFFER_SIZE 2

int buffer[BUFFER_SIZE];
int in = 0;
int out = 0;
int iterations;

void producer(int iter)
{
    int value = iter;
    buffer[in] = value;
    printf("Producer: Iteration %d, produced value %d at position %d\n", iter, value, in);
    in = (in + 1) % BUFFER_SIZE;
    sleep(rand() % 3 + 1); // Задержка от 1 до 3 секунд
}

void consumer(int iter)
{
    int value = buffer[out];
    printf("Consumer: Iteration %d, consumed value %d from position %d\n", iter, value, out);
    out = (out + 1) % BUFFER_SIZE;
    sleep(rand() % 3 + 1); // Задержка от 1 до 3 секунд
}

int main(int argc, char* argv[])
{
    int threadsMaxCount = omp_get_max_threads();

    if (argc != 2)
    {
        fprintf(stderr, "Usage: %s <number_of_iterations>\n", argv[0]);
        return 1;
    }

    iterations = atoi(argv[1]);
    srand(time(NULL));

#pragma omp parallel 
    {
#pragma omp sections
        {
#pragma omp section
            {

                for (int i = 0; i < iterations; i++)
                {
                    producer(i);
                }
            }

#pragma omp section
            {
                for (int i = 0; i < iterations; i++)
                {
                    consumer(i);
                }
            }

        }
    }

    return 0;
}