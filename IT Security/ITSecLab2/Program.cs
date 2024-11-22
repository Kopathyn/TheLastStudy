using ITSecLab2;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        DES encr = new DES();

        string message = "eternity";
        string key = "alekos";

        StreamWriter streamWriter = new StreamWriter(@"..\..\..\text.txt");

        string encryptedMessage = encr.EncryptMessage(message, key);
        streamWriter.Write(encryptedMessage);
        streamWriter.Write('\n');
        streamWriter.Write(encr.DecryptMessage(encryptedMessage, key));
        streamWriter.Close();
        
    }
}
