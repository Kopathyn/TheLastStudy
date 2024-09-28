using ITSecLab1;
class Progam
{
    static void Main(string[] args)
    {
        string inputPath = @"..\..\..\inoutput.txt";
        string outputPath = @"..\..\..\output.txt";


        string messageToEncrypt = File.ReadAllText(inputPath);
        if (messageToEncrypt == null)
        {
            Console.WriteLine("Error to read file!");
            Environment.Exit(-1);
        }

        TabulaRectaEncryption encryptionTool = new();

        string encryptedMessage = encryptionTool.EncryptMessage(messageToEncrypt);

        if (encryptedMessage == null)
        {
            Console.WriteLine("Encryption failed!");
            Environment.Exit(-1);
        }

        Console.WriteLine("Encryption successfully worked out!");

        string decryptedMessage = encryptionTool.DecryptMessage(encryptedMessage);
        if (decryptedMessage == null)
        {
            Console.WriteLine("Decryption failed!");
            Environment.Exit(-1);
        }
        Console.WriteLine("Decryption successfully worked out!");

        File.WriteAllText(outputPath, $"Message was: {messageToEncrypt}\nEncrypted Message: {encryptedMessage}\nDecrypted Message: {decryptedMessage}");
    }
}