import random

class ElGamal:
    def __init__(self):
        self.p = 21841
        self.g = 39
        self.private_key = 15126

        self.russian_dictionary = {
            'А': 10, 'Б': 11, 'В': 12, 'Г': 13, 'Д': 14, 'Е': 15, 'Ж': 16,
            'З': 17, 'И': 18, 'Й': 19, 'К': 20, 'Л': 21, 'М': 22, 'Н': 23,
            'О': 24, 'П': 25, 'Р': 26, 'С': 27, 'Т': 28, 'У': 29, 'Ф': 30,
            'Х': 31, 'Ц': 32, 'Ч': 33, 'Ш': 34, 'Щ': 35, 'Ъ': 36, 'Ы': 37,
            'Ь': 38, 'Э': 39, 'Ю': 40, 'Я': 41
        }

        self.english_dictionary = {
            'A': 42, 'B': 43, 'C': 44, 'D': 45, 'E': 46, 'F': 47, 'G': 48,
            'H': 49, 'I': 50, 'J': 51, 'K': 52, 'L': 53, 'M': 54, 'N': 55,
            'O': 56, 'P': 57, 'Q': 58, 'R': 59, 'S': 60, 'T': 61, 'U': 62,
            'V': 63, 'W': 64, 'X': 65, 'Y': 66, 'Z': 67
        }

        self.special_symbols_encoding = {
            ' ': 68, ',': 69, '.': 70
        }

    def power(self, x, n):
        if n == 0:
            return 1
        if n % 2 == 0:
            p = self.power(x, n // 2)
            return p * p
        else:
            return x * self.power(x, n - 1)

    def generate_public_key(self):
        buf = self.power(self.g, self.private_key)
        y = buf % self.p
        return [y, self.g, self.p]

    def generate_private_key(self):
        return random.randint(1, self.p - 1)

    def encrypt(self, message, public_key):
        y_key = public_key[0]
        g_key = public_key[1]
        p_key = public_key[2]

        message = message.upper()
        message_in_coding = []

        # Из букв в кодировку
        for letter in message:
            if letter in self.russian_dictionary:
                message_in_coding.append(self.russian_dictionary[letter])
            elif letter in self.english_dictionary:
                message_in_coding.append(self.english_dictionary[letter])
            elif letter in self.special_symbols_encoding:
                message_in_coding.append(self.special_symbols_encoding[letter])

        session_key = random.randint(1, p_key - 1)  # Сессионный ключ
        cipher_text = ""

        for value in message_in_coding:
            a = self.power(g_key, session_key) % p_key
            cipher_text += str(a) + ' '

            b = (value * self.power(y_key, session_key))% p_key
            cipher_text += str(b) + ' '

        return cipher_text
    
    def decrypt(self, message, public_key):
        decrypted_message = []
        decrypted_message_in_coding = ""
        p_key = public_key[2]

        for i in range(0, len(message), 2):
            a = int(message[i])
            b = int(message[i + 1])
            
            helper = pow(a, self.private_key) % p_key

            function = b * (pow(helper, p_key - 2) % p_key) % p_key

            decrypted_message.append(function)
        
        for number in decrypted_message:
            if number in self.russian_dictionary.values():
                decrypted_message_in_coding += str(get_key(self.russian_dictionary, number))
            elif get_key(self.english_dictionary, number):
                decrypted_message_in_coding += str(get_key(self.english_dictionary, number))
            elif number in self.special_symbols_encoding.values():
                decrypted_message_in_coding += str(get_key(self.special_symbols_encoding, number))

        return decrypted_message_in_coding
    
def get_key(my_dict, val):
    for key, value in my_dict.items():
        if val == value:
            return key