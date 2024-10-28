using ITSecLab2;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        DES encr = new DES();

        string message = "hello world";
        string key = "alekos";

        string encryptedMessage = encr.EncryptMessage(message, key);
        Console.WriteLine(encryptedMessage);

        Console.WriteLine(encr.DecryptMessage(encryptedMessage, key));
    }
}
