using ITSecLab3;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        string messagePath = @"..\..\..\Message.txt";
        string keyPath = @"..\..\..\key.txt";
        string encryptedPath = @"..\..\..\EncryptedMessage.txt";

        ElGamal EncodingTool = new ElGamal();

        long[] publicKey = EncodingTool.GeneratePublicKey();

        string message = File.ReadAllText(messagePath);

        File.WriteAllText(encryptedPath, Convert.ToString(EncodingTool.Encrypt(message, publicKey)));

        Process.Start("explorer.exe", encryptedPath);
    }
}