namespace EmberToolkit.Common.Interfaces.Encryption
{
    public interface IAESController
    {
        string DecryptData(string encryptedData);
        string EncryptData(string jsonData);
    }
}