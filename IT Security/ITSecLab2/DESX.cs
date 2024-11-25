using System.Text;

namespace ITSecLab2
{
    /// <summary>
    /// Класс, описывающиЙ метод шифрования DESX(Data Encryption Standard Extended)
    /// </summary>
    public class DESX : DES
    {
        private int _oneTwoKeyLenght = 64;
        private int _thirdKeyLenght = 56;

        /// <summary>
        /// Функция проверки и изменения ключа
        /// </summary>
        private string KeyCheck(string bitKey, int WhichKey)
        {
            string Key = "";

            switch (WhichKey)
            {
                case 1 or 2:
                    if (bitKey.Length > _oneTwoKeyLenght)
                    {
                        string newKey = bitKey.Substring(0, _oneTwoKeyLenght);
                        Key = newKey;
                    }
                    else if (bitKey.Length < _oneTwoKeyLenght)
                    {
                        while (bitKey.Length % _oneTwoKeyLenght != 0)
                            bitKey += "0";

                        Key = bitKey;
                    }
                    else
                    {
                        Key = bitKey;
                    }
                    break;

                case 3:
                    if (bitKey.Length > _thirdKeyLenght)
                    {
                        string newKey = bitKey.Substring(0, _thirdKeyLenght);
                        Key = newKey;
                    }
                    else if (bitKey.Length < _thirdKeyLenght)
                    {
                        while (bitKey.Length % _thirdKeyLenght != 0)
                            bitKey += "0";

                        Key = bitKey;
                    }
                    else
                    {
                        Key = bitKey;
                    }
                    break;

                default:
                    Console.WriteLine("ERROR!");
                    Environment.Exit(-1);
                    break;
            }

            return Key;
        }

        /// <summary>
        /// Шифрование сообщения
        /// </summary>
        public string Encryption(byte[] message, byte[] keyOne, byte[] keyTwo, byte[] keyThree)
        {
            // Подготовка ключей
            string bitKeyOne = KeyCheck(ConvertToBits(keyOne), 1);
            string bitKeyTwo = KeyCheck(ConvertToBits(keyTwo), 2);
            string bitKeyThree = KeyCheck(ConvertToBits(keyThree), 3);

            // Преобразование сообщения
            string bitMessage = ConvertToBits(message);
            string BitBlockMessage = MakeRightMessageBlock(bitMessage);

            int AmountOfBlocks = BitBlockMessage.Length / _blockSize; // Количество блоков

            string encryptedMessage = "";
            for (int i = 0; i < AmountOfBlocks; i++)
            {
                string someMessageBlock = BitBlockMessage.Substring((i * _blockSize), _blockSize);

                string MessageXORKey = XOR(someMessageBlock, bitKeyOne);

                string EncryptedByDES = EncryptMessage(MessageXORKey, bitKeyThree);
                string EncryptedXORKey = XOR(bitKeyTwo, EncryptedByDES);

                encryptedMessage += EncryptedXORKey;
            }

            return encryptedMessage;
        }

        /// <summary>
        /// Расшифрование сообщения
        /// </summary>
        public string Decryption(string message, byte[] keyOne, byte[] keyTwo, byte[] keyThree)
        {
            // Подготовка ключей
            string bitKeyOne = KeyCheck(ConvertToBits(keyOne), 1);
            string bitKeyTwo = KeyCheck(ConvertToBits(keyTwo), 2);
            string bitKeyThree = KeyCheck(ConvertToBits(keyThree), 3);

            // Преобразование сообщения
            string BitBlockMessage = message;

            int AmountOfBlocks = BitBlockMessage.Length / _blockSize; // Количество блоков

            string decryptedMessage = "";
            for (int i = 0; i < AmountOfBlocks; i++)
            {
                string someMessageBlock = BitBlockMessage.Substring((i * _blockSize), _blockSize);

                string MessageXORKey = XOR(someMessageBlock, bitKeyTwo);

                string EncryptedByDES = DecryptMessage(MessageXORKey, bitKeyThree);
                string EncryptedXORKey = XOR(bitKeyOne, EncryptedByDES);

                decryptedMessage += EncryptedXORKey;
            }

            byte[] byteMessage = new byte[decryptedMessage.Length / 8];
            for (int i = 0; i < (decryptedMessage.Length / 8); i++)
            {
                string part = decryptedMessage.Substring((i * 8), 8);
                byteMessage[i] = Convert.ToByte(part, 2);
            }

            decryptedMessage = Encoding.UTF8.GetString(byteMessage);

            return decryptedMessage;
        }
    }
}