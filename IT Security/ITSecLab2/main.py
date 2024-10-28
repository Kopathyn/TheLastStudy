
class DES:
    def __init__(self):
        # Таблица CD перестановки бит ключа
        self.cd = [57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18,
                   10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
                   63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22,
                   14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4]
        self.cd = [x - 1 for x in self.cd]

        # Таблица для финальной версии раундового ключа k_i
        self.final_cd = [14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4,
                         26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40,
                         51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32]
        self.final_cd = [x - 1 for x in self.final_cd]

        # Таблица значения циклического смещения в таблицах C и D
        self.cd_shift = [1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1]

        # Таблица стартовой перестановки
        self.start_ip = [58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
                         62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
                         57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3,
                         61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7]
        self.start_ip = [x - 1 for x in self.start_ip]

        # Вектор Е (функция расширения)
        self.vector_e = [32, 1, 2, 3, 4, 5,
                         4, 5, 6, 7, 8, 9,
                         8, 9, 10, 11, 12, 13,
                         12, 13, 14, 15, 16, 17,
                         16, 17, 18, 19, 20, 21,
                         20, 21, 22, 23, 24, 25,
                         24, 25, 26, 27, 28, 29,
                         28, 29, 30, 31, 32, 1]

        # Преобразования S, таблицы
        self.s_i = [
                        [  # S1
                            [14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7],
                            [0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8],
                            [4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0],
                            [15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13]
                        ],
                        [  # S2
                            [15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10],
                            [3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5],
                            [0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15],
                            [13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9]
                        ],
                        [  # S3
                            [10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8],
                            [13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1],
                            [13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7],
                            [1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12]
                        ],
                        [  # S4
                            [7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15],
                            [13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9],
                            [10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4],
                            [3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14]
                        ],
                        [  # S5
                            [2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9],
                            [14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6],
                            [4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14],
                            [11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3]
                        ],
                        [  # S6
                            [12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11],
                            [10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8],
                            [9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6],
                            [4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13]
                        ],
                        [  # S7
                            [4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1],
                            [13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6],
                            [1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2],
                            [6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12]
                        ],
                        [  # S8
                            [13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7],
                            [1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2],
                            [7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8],
                            [2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11]
                        ]
                    ]

        # Количество циклов шифрования
        self.i = 16

        # Пользовательский ключ
        self.input_key = None

        # Расширенный ключ
        self.extended_key = bytearray()

        # Раундовые ключи
        self.round_keys = []

    # проверка на не ASCII-символы + длину в 8 символов
    def set_user_key(self, user_key):
        if not user_key.isascii() or (len(user_key) != 8):
            raise ValueError("Ключ должен состоять из 8 ASCII-символов")
        self.input_key = user_key.encode()

    # преобразуем 56-битовый ключ в 64-битовый
    # В каждый 8-й бит ставим такое значение, чтобы в каждом байте было нечётное кол-во единиц
    def extend_key(self):
        if not self.input_key:
            raise ValueError("Не задан ключ user_key")
        for ch in self.input_key:
            if bin(ch).count("1") % 2 == 0:
                ch += 128 # 2^7
            self.extended_key.append(ch & 0xff)

    def key_generate(self):
        cd_i = self.cd

        # расширяем ключ до 64 бит
        self.extend_key()

        # Представляем расширенный ключ в виде бинарной строки (для удобства восприятия)
        bit_ex_key = (bin(int.from_bytes(self.extended_key, byteorder='big'))[2:])[::-1]
        if len(bit_ex_key) < 64:
            bit_ex_key += ("0" * (64 - len(bit_ex_key)))

        # Генерируем 16 ключей (по количеству циклов)
        for i in range(self.i):

            shift = self.cd_shift[i]
            # Получаем новые таблицы перестановки C D
            # Таблицу C (первая половина CD) циклически смещаем влево на i элементов
            cd_i[:len(cd_i) // 2] = cd_i[shift:len(cd_i) // 2] + cd_i[:shift]
            # Таблицу D (вторая половина CD) циклически смещаем влево на i элементов
            cd_i[len(cd_i) // 2:] = cd_i[len(cd_i) // 2 + shift:] + cd_i[len(cd_i) // 2: len(cd_i) // 2 + shift]

            # Получаем вектор CD на основе расширенного 64-битного ключа
            cd_vector = ""
            for bytepos in range(len(cd_i)):
                cd_vector += bit_ex_key[cd_i[bytepos]]

            # Получаем раундовый ключ k_i из вектора CD в соответствии с таблицей перестановки final_cd
            key_i = ""
            for bytepos in range(len(self.final_cd)):
                key_i += cd_vector[self.final_cd[bytepos]]
            # Добавляем полученный ключ
            self.round_keys.append(int(key_i[::-1], 2))


try:
    a = DES()
    my_key = input("Введите ключ, состоящий из 8 ASCII-символов: ")
    a.set_user_key(my_key)
    a.key_generate()
    print(a.round_keys)

except Exception as e:
    print(f"\033[1;31m Error! \033[0m{e}")
