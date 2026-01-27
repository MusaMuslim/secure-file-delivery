namespace SecureFileDelivery.Application.Interfaces;

// Service for encrypting and decrypting files
public interface IEncryptionService
{
    // Encrypts a stream and returns the encrypted stream
    Task<Stream> EncryptAsync(Stream inputStream, string keyId);

    // Decrypts a stream and returns the decrypted stream
    Task<Stream> DecryptAsync(Stream inputStream, string keyId);

    // Generates a new encryption key ID
    string GenerateKeyId();
}