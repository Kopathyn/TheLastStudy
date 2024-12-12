namespace ITSecLab3
{
    class ElGamal
    {
        private long Power(long x, long n)
        {
            if (n == 0)
            {
                return 1;
            }

            //у четного числа последний бит равен нулю
            if ((n & 1) == 0)
            {
                //смещение на один бит вправо равносильно делению на два
                var p = Power(x, n >> 1);
                return p * p;
            }
            else
            {
                return x * Power(x, n - 1);
            }
        }

        /// <summary>
        /// Функция генерации открытого ключа
        /// </summary>
        /// <returns>Связку открытого ключа (y, g, p)</returns>
        public long[] GeneratePublicKey()
        {
            long buf = Power(_g, _privateKey);
            long y = buf % _p;

            long[] PublicKey = [y, _g, _p];
            return PublicKey;
        }

        /// <summary>
        /// Функция генерации закрытого ключа
        /// </summary>
        /// <returns>Закрытый ключ</returns>
        private int GeneratePrivateKey(int p = _p)
        {
            return _randomizer.Next(1, (p - 1));
        }

        public string Encrypt(string message, long[] publicKey)
        {
            long yKey = publicKey[0];
            long gKey = publicKey[1];
            long pKey = publicKey[2];

            string _message = message.ToUpper();
            long[] messageInCoding = new long[message.Length];

            // Из букв в кодировку
            foreach (char letter in _message)
            {
                int value;

                if (_russianDictionary.TryGetValue(letter, out value))
                    messageInCoding.Append(value);

                else if (_englishDictionary.TryGetValue(letter, out value))
                    messageInCoding.Append(value);

                else if (_specialSymbolsEncoding.TryGetValue(letter, out value))
                    messageInCoding.Append(value);
            }

            int SessionKey = _randomizer.Next(1, (int)pKey - 1); // Сессионный ключ

            string CipherText = "";

            for (int i = 0; i < messageInCoding.Length; i++)
            {
                double a = Power(gKey, SessionKey) % pKey;
                CipherText += a.ToString();

                double b = Power((int)yKey, messageInCoding[i]) % pKey;
                CipherText += b.ToString();
            }
            
            return CipherText;
        }

        #region PrivateFields

        private const int _p = 21841;
        private const int _g = 39;
        private const int _privateKey = 15126;

        private Dictionary<char, int> _russianDictionary = new Dictionary<char, int>
        {
            {'А', 10}, {'Б', 11}, {'В', 12}, {'Г', 13}, {'Д', 14}, {'Е', 15}, {'Ж', 16},
            {'З', 17}, {'И', 18}, {'Й', 19}, {'К', 20}, {'Л', 21}, {'М', 22}, {'Н', 23},
            {'О', 24}, {'П', 25}, {'Р', 26}, {'С', 27}, {'Т', 28}, {'У', 29}, {'Ф', 30},
            {'Х', 31}, {'Ц', 32}, {'Ч', 33}, {'Ш', 34}, {'Щ', 35}, {'Ъ', 36}, {'Ы', 37},
            {'Ь', 38}, {'Э', 39}, {'Ю', 40}, {'Я', 41}
        };

        private Dictionary<char, int> _englishDictionary = new Dictionary<char, int>
        {
            {'A', 42}, {'B', 43}, {'C', 44}, {'D', 45}, {'E', 46}, {'F', 47}, {'G', 48},
            {'H', 49}, {'I', 50}, {'J', 51}, {'K', 52}, {'L', 53}, {'M', 54}, {'N', 55},
            {'O', 56}, {'P', 57}, {'Q', 58}, {'R', 59}, {'S', 60}, {'T', 61}, {'U', 62},
            {'V', 63}, {'W', 64}, {'X', 65}, {'Y', 66}, {'Z', 67}
        };

        private Dictionary<char, int> _specialSymbolsEncoding = new Dictionary<char, int>
        {
         {' ', 68},  {',', 69}, {'.', 70}
        };

        private Random _randomizer = new Random();

        #endregion
    }
}