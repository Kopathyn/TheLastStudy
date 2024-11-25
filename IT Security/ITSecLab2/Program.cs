using ITSecLab2;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        DESX encrDESX = new DESX();

        string messagePath = @"..\..\..\Message.txt";
        string encryptedMessagePath = @"..\..\..\EncrytpedMessage.txt";
        string decryptedMessagePath = @"..\..\..\DecrytpedMessage.txt";
        string key1Path = @"..\..\..\Key1.txt";
        string key2Path = @"..\..\..\Key2.txt";
        string key3Path = @"..\..\..\Key2.txt";

        byte[] message = File.ReadAllBytes(messagePath);
        byte[] key1 = File.ReadAllBytes(key1Path);
        byte[] key2 = File.ReadAllBytes(key2Path);
        byte[] key3 = File.ReadAllBytes(key3Path);

        File.WriteAllText(encryptedMessagePath, encrDESX.Encryption(message, key1, key2, key3));

        Process.Start("explorer.exe", encryptedMessagePath);

        string encryptedMessage = File.ReadAllText(encryptedMessagePath);

        File.WriteAllText(decryptedMessagePath, encrDESX.Decryption(encryptedMessage, key1, key2, key3));

        Process.Start("explorer.exe", decryptedMessagePath);
    }
}
