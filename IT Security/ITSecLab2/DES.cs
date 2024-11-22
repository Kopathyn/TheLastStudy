using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ITSecLab2
{
    /// <summary>
    /// Класс, описывающий метод шифрования DES(Data Encryption Standard)
    /// </summary>
    public class DES
    {
        /// <summary>
        /// Конвертация сообщения в биты
        /// </summary>
        private string ConvertToBits(byte[] message)
        {
            string bits = "";

            for (int i = 0; i < message.Length; i++)
                bits += (Convert.ToString(message[i], 2).PadLeft(8, '0'));

            return bits;
        }

        /// <summary>
        /// Первичная перестановка 64 битного блока
        /// </summary>
        private string Initial_Permutation(string bit_message)
        {
            string IPmessage = "";

            for (int i = 0; i < _blockSize; i++)
                IPmessage += bit_message[_initPermTable[i] - 1];

            return IPmessage;
        }

        /// <summary>
        /// Конечная перестановка 64 битного блока
        /// </summary>
        private string End_Permutation(string bit_message)
        {
            string EPmessage = "";

            for (int i = 0; i < _blockSize; i++)
                EPmessage += bit_message[_endPermTable[i] - 1];

            return EPmessage;
        }

        /// <summary>
        /// Функция преобразования сообщения до кратности 64
        /// </summary>
        private string MessageToBlocks(string bitMessage)
        {
            if ((bitMessage.Length) % _blockSize != 0)
            {
                while ((bitMessage.Length) % _blockSize != 0)
                    bitMessage += "0";

                return bitMessage;
            }
            else
                return bitMessage;
        }

        /// <summary>
        /// Преобразование ключа
        /// </summary>
        private string bitBlockKey(string _key)
        {
            string Key = ConvertToBits(Encoding.UTF8.GetBytes(_key));

            if (Key.Length < _keySize)
            {
                while ((Key.Length) % _keySize != 0)
                    Key += "0";

                string newKey = "";

                for (int i = 0; i < _cdKeyTable.Length; i++)
                    newKey += Key[_cdKeyTable[i] - 1];

                return newKey;
            }
            else if (Key.Length > _keySize)
            {
                string cutKey = _key.Substring(0, _keySize);
                string newKey = "";

                for (int i = 0; i < _cdKeyTable.Length; i++)
                    newKey += cutKey[_cdKeyTable[i] - 1];

                return newKey;
            }
            else
                return Key;
        }

            /// <summary>
            /// Сложение по модулю 2
            /// </summary>
        private string XOR(string a, string b)
        {
            string result = "";

            for (int i = 0; i < a.Length; i++)
            {
                bool x = Convert.ToBoolean(Convert.ToInt32(a[i].ToString()));
                bool y = Convert.ToBoolean(Convert.ToInt32(b[i].ToString()));

                if (x ^ y)
                    result += "1";
                else
                    result += "0";
            }
            
            return result;
        }

        /// <summary>
        /// Основная функция шифрования
        /// </summary>
        private string EncryptFunction(string _someMesPart, string _keyPart)
        {
            string extendedBlock = "";

            // Перестановка и расширение блока сообщения
            for (int i = 0; i < _extendPermTable.Length; i++)
                extendedBlock += _someMesPart[_extendPermTable[i] - 1];

            string perms = "";
            for (int i = 0; i < 8; i++)
            {
                string sixBlock = extendedBlock.Substring((i * 6), 6);

                string sLine = sixBlock.First().ToString() + sixBlock.Last().ToString();

                string sCol = sixBlock.Substring(1, 4);

                int line = Convert.ToInt32(sLine, 2);
                int col = Convert.ToInt32(sCol.ToString(), 2);

                perms += Convert.ToString(_sPermTable[i][line][col], 2).PadLeft(8, '0');// PadLeft now 4 was 8
            }

            string final = "";
            for (int i = 0; i < _pBlockTable.Length; i++)
                final += perms[_pBlockTable[i] - 1];

            return final;
        }

        /// <summary>
        /// Функция циклического сдвига влево
        /// </summary>
        private string ShiftLeft(string byteArray)
        {
            char temp = byteArray[0];
            string newByteArray = "";

            for (int i = 0; i < byteArray.Length - 1; i++)
                newByteArray += byteArray[i + 1];

            newByteArray += temp;

            return newByteArray;
        }

        /// <summary>
        /// Функция сдвига ключа
        /// </summary>
        /// <param name="_key">Текущий ключ</param>
        /// <param name="round">Какой раунд</param>
        private string KeyShiftLeft(string _key, int round)
        {
            string firstKeyPart = _key.Substring(0, _key.Length / 2);
            string secondKeyPary = _key.Substring(_key.Length / 2, _key.Length / 2);

            for (int i = 0; i < _keyShifts[round]; i++)
            {
                firstKeyPart = ShiftLeft(firstKeyPart);
                secondKeyPary = ShiftLeft(secondKeyPary);
            }

            string shiftedKey = ConnectBlocks(firstKeyPart, secondKeyPary);

            return shiftedKey;
        }

        private string ShiftRight(string byteArray)
        {
            char temp = byteArray[byteArray.Length - 1];
            string newByteArray = "";

            for (int i = byteArray.Length - 1; i > 0; i--)
                newByteArray += byteArray[i - 1];

            newByteArray += temp;

            return newByteArray;
        }

        private string KeyShiftRight(string _key, int round)
        {
            string firstKeyPart = _key[0..(_key.Length / 2)];
            string secondKeyPary = _key[(_key.Length / 2).._key.Length];

            for (int i = 0; i < _keyShifts[round]; i++)
            {
                firstKeyPart = ShiftRight(firstKeyPart);
                secondKeyPary = ShiftRight(secondKeyPary);
            }

            string shiftedKey = ConnectBlocks(firstKeyPart, secondKeyPary);

            return shiftedKey;
        }

        /// <summary>
        /// Функция нахождения ключа раунда
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private string FindRoundKey(string Key)
        {
            string roundKey = "";

            for (int i = 0; i < _finalCDKeyTable.Length; i++)
                roundKey += Key[_finalCDKeyTable[i] - 1];

            return roundKey;
        }

        /// <summary>
        /// Функция соединения двух блоков в один
        /// </summary>
        private string ConnectBlocks(string blockOne, string blockTwo)
        {
            string ConnectedBlock = "";

            //foreach (var x in blockOne)
            //    ConnectedBlock += x;

            //foreach (var x in blockTwo)
            //    ConnectedBlock += x;

            ConnectedBlock = blockOne + blockTwo;

            return ConnectedBlock;
        }

        /// <summary>
        /// Алгоритм шифрования
        /// </summary>
        public string EncryptMessage(string message, string key)
        {
            //Преобразование в биты
            byte[] oldByteMessage = Encoding.UTF8.GetBytes(message);
            string bitMessage = ConvertToBits(oldByteMessage);

            // Преобразование сообщения для кратности 64
            bitMessage = MessageToBlocks(bitMessage);

            string bitKey = bitBlockKey(key);

            // Количество блоков
            int amountOfBlocks = bitMessage.Length / _blockSize;

            // Запись зашифрованного сообщения
            string encryptedMessage = "";

            // Шифрование блоков
            for (int i = 0; i < amountOfBlocks; i++)
            {
                string messageBlock = bitMessage.Substring((i * _blockSize), _blockSize);
                messageBlock = Initial_Permutation(messageBlock);

                // Деление сообщения на блоки по 32 бита
                string leftMessagePart = messageBlock.Substring(0, _blockSize / 2);
                string rightMessagePart = messageBlock.Substring(_blockSize / 2, _blockSize / 2);

                for (int j = 0; j < _rounds; j++)
                {
                    bitKey = KeyShiftLeft(bitKey, j);
                    string roundKey = FindRoundKey(bitKey);

                    string _buffer = XOR(rightMessagePart, EncryptFunction(leftMessagePart, roundKey));
                    rightMessagePart = leftMessagePart;
                    leftMessagePart = _buffer;
                }

                // Соединение двух блоков в один
                encryptedMessage += End_Permutation(ConnectBlocks(rightMessagePart, leftMessagePart));
            }

            byte[] byteMessage = new byte[encryptedMessage.Length / 8];
            for (int i = 0; i < (encryptedMessage.Length / 8); i++)
            {
                string part = encryptedMessage.Substring((i * 8), 8);
                byteMessage[i] = Convert.ToByte(part, 2);
            }

            encryptedMessage = Encoding.UTF8.GetString(byteMessage);

            return encryptedMessage;
        }

        public string DecryptMessage(string message, string key)
        {
            //Преобразование в биты
            string bitMessage = ConvertToBits(Encoding.UTF8.GetBytes(message));

            // Преобразование сообщения для кратности 64
            bitMessage = MessageToBlocks(bitMessage);

            string bitKey = bitBlockKey(key);

            // Количество блоков
            int amountOfBlocks = bitMessage.Length / _blockSize;

            // Запись зашифрованного сообщения
            string encryptedMessage = "";

            // Шифрование блоков
            for (int i = 0; i < amountOfBlocks; i++)
            {
                string messageBlock = bitMessage.Substring((i * _blockSize), _blockSize);
                messageBlock = Initial_Permutation(messageBlock);

                // Деление сообщения на блоки по 32 бита
                string leftMessagePart = messageBlock.Substring(0, _blockSize / 2);
                string rightMessagePart = messageBlock.Substring(_blockSize / 2, _blockSize / 2);

                for (int j = _rounds - 1; j >= 0; j--)
                {
                    bitKey = KeyShiftLeft(bitKey, j);
                    string roundKey = FindRoundKey(bitKey);

                    string _buffer = XOR(leftMessagePart, EncryptFunction(rightMessagePart, roundKey)); ;
                    leftMessagePart = rightMessagePart;
                    rightMessagePart = _buffer; 
                }

                // Соединение двух блоков в один
                encryptedMessage += End_Permutation(ConnectBlocks(leftMessagePart, rightMessagePart));
            }



            byte[] byteMessage = new byte[encryptedMessage.Length / 8];
            for (int i = 0; i < (encryptedMessage.Length / 8); i++)
            {
                string part = encryptedMessage.Substring((i * 8), 8);
                byteMessage[i] = Convert.ToByte(part, 2);
            }

            encryptedMessage = Encoding.UTF8.GetString(byteMessage);

            return encryptedMessage;
        }
        
        // Переменные шифра

        private const int _blockSize = 64; // Размер одного блока сообщения
        private const int _halfBlockSize = 32; // Размер части блока сообщения
        private const int _rounds = 16; // Количество раундов
        private const int _keySize = 64; // Размер ключа

        #region Tables

        // Обработка блоков сообщений

        // Таблица первичной перестановки
        private int[] _initPermTable = 
        [
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        ];

        // Таблица конечной перестановки
        private int[] _endPermTable = 
        [
            40, 8, 48, 16, 56, 24, 64, 32,
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41, 9, 49, 17, 57, 25
        ];

        // Таблица расширения 32 битного блока сообщения в 48 битный
        private int[] _extendPermTable = 
        [
            32, 1, 2, 3, 4, 5,
            4, 5, 6, 7, 8, 9,
            8, 9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32, 1
        ];

        // Таблицы преобразований блоков S
        private int[][][] _sPermTable =
        [           
            [
                [14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7],
                [0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8],
                [4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0],
                [15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13]
            ],

            [
                [15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10],
                [3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5],
                [0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15],
                [13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9]
            ],

            [
                [10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8],
                [13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1],
                [13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7],
                [1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12]
            ],

            [
                [7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15],
                [13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9],
                [10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4],
                [3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14]
            ],

            [
                [2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9],
                [14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6],
                [4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14],
                [11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3]
            ],

             [
                [12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11],
                [10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8],
                [9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6],
                [4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13]
             ],

             [
                [4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1],
                [13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6],
                [1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2],
                [6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12]
             ],

             [
                [13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7],
                [1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2],
                [7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8],
                [2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11]
             ]
        ];

        // Сдвиг блока в функции шифрования
        private int[] _pBlockTable =
        [
            16, 7,  20, 21, 29, 12, 28, 17,
            1,  15, 23, 26, 5,  18, 31, 10,
            2,  8,  24, 14, 32, 27, 3,  9,
            19, 13, 30, 6,  22, 11, 4,  25
        ];


        // Обработка ключей

        // Таблица преобразования ключа
        private int[] _cdKeyTable =
        [
            57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18,
            10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22,
            14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4
        ];

        // Окончательная перестановка ключа
        private int[] _finalCDKeyTable =
        [
            14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4,
            26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40,
            51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
        ];

        // Сдвиг ключа
        private int[] _keyShifts = 
        [
            1, 1, 2, 2, 2, 2, 2, 2,
            1, 2, 2, 2, 2, 2, 2, 1
        ];

        #endregion
    };
}