using System.Security.Cryptography;

namespace DotNetAppSettingEncryptor;

/// <summary>
/// 
/// </summary>
/// <param name="base64Key"></param>
/// <param name="base64Iv"></param>
public class AesEncryption(string base64Key, string base64Iv)
{
    private readonly byte[] _key = Convert.FromBase64String(base64Key);
    private readonly byte[] _iv = Convert.FromBase64String(base64Iv);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public byte[] Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(plainText);
        }

        return ms.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public string Decrypt(byte[] cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);
        return reader.ReadToEnd();
    }
}

/*
 *
 * Developed by Ahmadreza Bahramian (http://ahmadrezadev.ir | 09039818200)
 *
 */