from Elgamal import *

import os
import subprocess

message_path = 'C:\\Users\\chemo\\Documents\\Development\\TheLastStudy\\IT Security\\ITSecLab3\\Python\\Message.txt'
key_path = 'C:\\Users\\chemo\\Documents\\Development\\TheLastStudy\\IT Security\\ITSecLab3\\Python\\key.txt'
encrypted_path = 'C:\\Users\\chemo\\Documents\\Development\\TheLastStudy\\IT Security\\ITSecLab3\\Python\\EncryptedMessage.txt'
decrypted_path = 'C:\\Users\\chemo\\Documents\\Development\\TheLastStudy\\IT Security\\ITSecLab3\\Python\\DecrytpedMessage.txt'

encoding_tool = ElGamal()

# Читаем ключ
with open(key_path, 'r') as file:
    lines = file.readline()
    public_key = [int(line) for line in lines.strip().split()]

# Читаем сообщение
with open(message_path, 'r') as file:
    message = file.read()

# Шифруем сообщение
encrypted_message = encoding_tool.encrypt(message, public_key)

# Записываем зашифрованное сообщение
with open(encrypted_path, 'w') as file:
    file.write(encrypted_message)

# Открываем зашифрованное и перезаписываем (для чистоты эксперимента)
with open(encrypted_path, 'r') as file:
    lines = file.readline()
    encrypted_message = [int(line) for line in lines.strip().split()]

# Рашифровываем
decrypted_message = encoding_tool.decrypt(encrypted_message, public_key)

# Записываем результат расшифровывания
with open(decrypted_path, 'w', encoding='utf-8') as file:
    file.write(decrypted_message)

print("Done!")