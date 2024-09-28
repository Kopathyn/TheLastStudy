using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSecLab1
{   
    /// <summary>
    /// Класс, описывающий шифрование таблицей Тритемия
    /// </summary>
    public class TabulaRectaEncryption
    {
        /// <summary>
        /// Создание таблицы Тритемия
        /// </summary>
        public TabulaRectaEncryption() 
        {
            for (int i = 0; i < _alphabet.Length; i++)
            {
                for (int j = 0; j < _alphabet.Length; j++)
                    if (i + j < _alphabet.Length)
                        _table[i, j] = _alphabet[i + j];

                for (int j = 0; j < _alphabet.Length; j++)
                    if (_table[i, j] == '\0')
                        for (int k = 0; k < i; k++)
                            _table[i, j + k] = _alphabet[k];
            }
        }

        public void PrintTable()
        {
            for (int i = 0; i < 26; i++)
            {
                Console.WriteLine(" ");
                for (int j = 0; j < 26; j++)
                    Console.Write(_table[i, j]);
            }
        }

        /// <summary>
        /// Шифрование переданного сообщение
        /// </summary>
        /// <param name="message">Изначальное ообщение</param>
        /// <returns>Зашифрованное сообщение</returns>
        public string EncryptMessage(string message)
        {
            message = message.ToUpper();
            string encryptedMessage = ""; //Запись зашифрованного сообщения

            for (int i = 0; i < message.Length; i++)
            {
                //Чтобы не шифровались пробелы
                if (message[i] == ' ')
                {
                    encryptedMessage += ' ';
                    continue;
                }

                //Место буквы в алфавите
                int alphabetLetterPlace = 0; 

                //Ищем букву сообщения в алфавите
                for (int j = 0; j < _alphabet.Length; j++)
                    if (_alphabet[j] == message[i])
                        alphabetLetterPlace = j;

                //Обработка сообщений, где количество символов больше 26
                if (i >= 26)
                    encryptedMessage += _table[i - 26, alphabetLetterPlace];
                else
                    encryptedMessage += _table[i, alphabetLetterPlace];
            }

            return encryptedMessage;
        }

        /// <summary>
        /// Расшифровка сообщения
        /// </summary>
        /// <param name="message">Зашифрованное сообщение</param>
        /// <returns>Изначальное сообщение</returns>
        public string DecryptMessage(string message)
        {
            message = message.ToUpper();
            string decryptedMessage = ""; //Инициализация для изначального сообщения

            for (int i = 0; i < message.Length; i++)
            {
                //Обработка пробела
                if (message[i] == ' ')
                {
                    decryptedMessage += ' ';
                    continue;
                }

                //Положение символа в таблице
                int tableLetterPlace = 0;
                
                //В каждой строке ищем положение символа из сообщения
                for (int j = 0; j < _alphabet.Length; j++)
                {
                    if (i >= 26)
                    {
                        if (_table[i - 26, j] == message[i])
                            tableLetterPlace = j;
                    }
                    else
                    {
                        if (_table[i, j] == message[i])
                            tableLetterPlace = j;
                    }
                }

                decryptedMessage += _table[0, tableLetterPlace];
            }

            return decryptedMessage;
        }

        private char[,] _table = new char[26, 26]; 
        private string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    }
}