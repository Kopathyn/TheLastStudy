#include <stdio.h>
#include <stdlib.h>
#include <omp.h>
#include <time.h>
#include <unistd.h>
#include <semaphore.h>

#define BUFFER_SIZE 2

int buffer[BUFFER_SIZE];
int in = 0;
int out = 0;
int iterations;

sem_t bufferEmptySlots;
sem_t bufferFullSlots;
sem_t mutex;

void producer(int iter)
{
    int value = iter;
    /* Уменьшение значений семафоров
    * Если семафор > 0, продолжение выполнения
    * Меньше, ожидание
    * Захватывание потоков
    */
    sem_wait(&bufferEmptySlots);
    sem_wait(&mutex);

    buffer[in] = value;
    printf("Producer: Iteration %d, produced value %d at position %d\n", iter, value, in);
    in = (in + 1) % BUFFER_SIZE;

    /* Увеличение значений семафоров
    * Освобождение потоков
    */
    sem_post(&mutex);
    sem_post(&bufferFullSlots);
    sleep(rand() % 3 + 1); // Задержка от 1 до 3 секунд
}

void consumer(int iter)
{
    sem_wait(&bufferFullSlots);
    sem_wait(&mutex);

    int value = buffer[out];
    printf("Consumer: Iteration %d, consumed value %d from position %d\n", iter, value, out);
    out = (out + 1) % BUFFER_SIZE;

    sem_post(&mutex);
    sem_post(&bufferEmptySlots);
    sleep(rand() % 3 + 1); // Задержка от 1 до 3 секунд
}

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        fprintf(stderr, "Usage: %s <number_of_iterations>\n", argv[0]);
        return 1;
    }

    iterations = atoi(argv[1]);
    srand(time(NULL));

    /* Инициализация семафоров */
    sem_init(&bufferEmptySlots, 0, BUFFER_SIZE); // Пустые места в буфере
    sem_init(&bufferFullSlots, 0, 0); // Заполненные места в буфере
    sem_init(&mutex, 0, 1); // Исключение при доступе к буферу

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

    /* Завершение работы семафоров */
    sem_destroy(&bufferEmptySlots);
    sem_destroy(&bufferFullSlots);
    sem_destroy(&mutex);

    return 0;
}