using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SecureFileDelivery.Application.Interfaces;

namespace SecureFileDelivery.Infrastructure.Services;

/// Service for encrypting and decrypting files using AES-256
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _encryptionKey;

    public EncryptionService(IConfiguration configuration)
    {
        var keyString = configuration["Security:EncryptionKey"]
            ?? throw new InvalidOperationException("Encryption key not configured");

        // Convert key to 32 bytes (256 bits) for AES-256
        _encryptionKey = Encoding.UTF8.GetBytes(keyString.PadRight(32).Substring(0, 32));
    }

    public async Task<Stream> EncryptAsync(Stream inputStream, string keyId)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.GenerateIV(); // Generate random IV for each encryption

        var outputStream = new MemoryStream();

        // Write IV to the beginning of the stream (needed for decryption)
        await outputStream.WriteAsync(aes.IV, 0, aes.IV.Length);

        // Encrypt the data
        using var cryptoStream = new CryptoStream(
            outputStream,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write,
            leaveOpen: true);

        await inputStream.CopyToAsync(cryptoStream);
        await cryptoStream.FlushFinalBlockAsync();

        outputStream.Position = 0;
        return outputStream;
    }

    public async Task<Stream> DecryptAsync(Stream inputStream, string keyId)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;

        // Read IV from the beginning of the stream
        var iv = new byte[aes.IV.Length];
        await inputStream.ReadAsync(iv, 0, iv.Length);
        aes.IV = iv;

        var outputStream = new MemoryStream();

        // Decrypt the data
        using var cryptoStream = new CryptoStream(
            inputStream,
            aes.CreateDecryptor(),
            CryptoStreamMode.Read);

        await cryptoStream.CopyToAsync(outputStream);

        outputStream.Position = 0;
        return outputStream;
    }

    public string GenerateKeyId()
    {
        // Generate a unique key identifier
        return $"key_{Guid.NewGuid():N}";
    }
}