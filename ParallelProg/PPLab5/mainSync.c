#include <stdio.h>
#include <stdlib.h>
#include <omp.h>
#include <unistd.h>
#include <semaphore.h>

#define BUFFER_SIZE 5

int buffer[BUFFER_SIZE];
int in = 0;
int out = 0;
int iterations;

sem_t empty;
sem_t full;
sem_t mutex;

void producer(int iter) 
{
    int value = iter;
    /* Уменьшение значений семафоров 
    * Если семафор > 0, продолжение выполнения
    * Меньше, ожидание
    * Захватывание потоков
    */
    sem_wait(&empty);
    sem_wait(&mutex);

    buffer[in] = value;
    printf("Producer: Iteration %d, produced value %d at position %d\n", iter, value, in);
    in = (in + 1) % BUFFER_SIZE;

    /* Увеличение значений семафоров 
    * Освобождение потоков
    */
    sem_post(&mutex);
    sem_post(&full);
    sleep(rand() % 3 + 1); // Задержка от 1 до 3 секунд
}

void consumer(int iter) 
{
    sem_wait(&full);
    sem_wait(&mutex);

    int value = buffer[out];
    printf("Consumer: Iteration %d, consumed value %d from position %d\n", iter, value, out);
    out = (out + 1) % BUFFER_SIZE;

    sem_post(&mutex);
    sem_post(&empty);
    sleep(rand() % 3 + 1); // Задержка от 1 до 3 секунд
}

int main(int argc, char *argv[]) 
{
    if (argc != 2) 
    {
        fprintf(stderr, "Usage: %s <number_of_iterations>\n", argv[0]);
        return 1;
    }

    iterations = atoi(argv[1]);
    srand(time(NULL));

    /* Инициализация семафоров */
    sem_init(&empty, 0, BUFFER_SIZE); // Пустые места в буфере
    sem_init(&full, 0, 0); // Заполненные места в буфере
    sem_init(&mutex, 0, 1); // Исключение при доступе к буферу

    #pragma omp parallel
    {
        for (int i = 0; i < iterations; i++) 
        {
            producer(i);
            consumer(i);
        }
    }

    /* Завершение работы семафоров */
    sem_destroy(&empty);
    sem_destroy(&full);
    sem_destroy(&mutex);

    return 0;
}