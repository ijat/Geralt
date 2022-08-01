using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Geralt.Tests;

[TestClass]
public class XChaCha20Tests
{
    // draft-irtf-cfrg-xchacha-01 Section A.3.2.1: https://tools.ietf.org/id/draft-irtf-cfrg-xchacha-01.html#block-counter--0
    private static readonly byte[] Plaintext = Convert.FromHexString("5468652064686f6c65202870726f6e6f756e6365642022646f6c65222920697320616c736f206b6e6f776e2061732074686520417369617469632077696c6420646f672c2072656420646f672c20616e642077686973746c696e6720646f672e2049742069732061626f7574207468652073697a65206f662061204765726d616e20736865706865726420627574206c6f6f6b73206d6f7265206c696b652061206c6f6e672d6c656767656420666f782e205468697320686967686c7920656c757369766520616e6420736b696c6c6564206a756d70657220697320636c6173736966696564207769746820776f6c7665732c20636f796f7465732c206a61636b616c732c20616e6420666f78657320696e20746865207461786f6e6f6d69632066616d696c792043616e696461652e");
    private static readonly byte[] Ciphertext = Convert.FromHexString("4559abba4e48c16102e8bb2c05e6947f50a786de162f9b0b7e592a9b53d0d4e98d8d6410d540a1a6375b26d80dace4fab52384c731acbf16a5923c0c48d3575d4d0d2c673b666faa731061277701093a6bf7a158a8864292a41c48e3a9b4c0daece0f8d98d0d7e05b37a307bbb66333164ec9e1b24ea0d6c3ffddcec4f68e7443056193a03c810e11344ca06d8ed8a2bfb1e8d48cfa6bc0eb4e2464b748142407c9f431aee769960e15ba8b96890466ef2457599852385c661f752ce20f9da0c09ab6b19df74e76a95967446f8d0fd415e7bee2a12a114c20eb5292ae7a349ae577820d5520a1f3fb62a17ce6a7e68fa7c79111d8860920bc048ef43fe84486ccb87c25f0ae045f0cce1e7989a9aa220a28bdd4827e751a24a6d5c62d790a66393b93111c1a55dd7421a10184974c7c5");
    private static readonly byte[] Nonce = Convert.FromHexString("404142434445464748494a4b4c4d4e4f5051525354555658");
    private static readonly byte[] Key = Convert.FromHexString("808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9f");
    
    [TestMethod]
    public void Encrypt_ValidInputs()
    {
        Span<byte> ciphertext = stackalloc byte[Plaintext.Length];
        XChaCha20.Encrypt(ciphertext, Plaintext, Nonce, Key);
        Assert.IsTrue(ciphertext.SequenceEqual(Ciphertext));
    }
    
    [TestMethod]
    public void Encrypt_DifferentPlaintext()
    {
        Span<byte> ciphertext = stackalloc byte[Plaintext.Length];
        Span<byte> plaintext = Plaintext.ToArray();
        plaintext[0]++;
        XChaCha20.Encrypt(ciphertext, plaintext, Nonce, Key);
        Assert.IsFalse(ciphertext.SequenceEqual(Ciphertext));
    }
    
    [TestMethod]
    public void Encrypt_DifferentNonce()
    {
        Span<byte> ciphertext = stackalloc byte[Plaintext.Length];
        Span<byte> nonce = Nonce.ToArray();
        nonce[0]++;
        XChaCha20.Encrypt(ciphertext, Plaintext, nonce, Key);
        Assert.IsFalse(ciphertext.SequenceEqual(Ciphertext));
    }
    
    [TestMethod]
    public void Encrypt_DifferentKey()
    {
        Span<byte> ciphertext = stackalloc byte[Plaintext.Length];
        Span<byte> key = Key.ToArray();
        key[0]++;
        XChaCha20.Encrypt(ciphertext, Plaintext, Nonce, key);
        Assert.IsFalse(ciphertext.SequenceEqual(Ciphertext));
    }
    
    [TestMethod]
    public void Encrypt_InvalidCiphertext()
    {
        var ciphertext = Array.Empty<byte>();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Encrypt(ciphertext, Plaintext, Nonce, Key));
    }
    
    [TestMethod]
    public void Encrypt_InvalidPlaintext()
    {
        var ciphertext = new byte[Plaintext.Length];
        var plaintext = Array.Empty<byte>();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Encrypt(ciphertext, plaintext, Nonce, Key));
    }
    
    [TestMethod]
    public void Encrypt_InvalidNonce()
    {
        var ciphertext = new byte[Plaintext.Length];
        var nonce = new byte[XChaCha20.NonceSize - 1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Encrypt(ciphertext, Plaintext, nonce, Key));
        nonce = new byte[XChaCha20.NonceSize + 1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Encrypt(ciphertext, Plaintext, nonce, Key));
    }
    
    [TestMethod]
    public void Encrypt_InvalidKey()
    {
        var ciphertext = new byte[Plaintext.Length];
        var key = new byte[XChaCha20.KeySize - 1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Encrypt(ciphertext, Plaintext, Nonce, key));
        key = new byte[XChaCha20.KeySize + 1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Encrypt(ciphertext, Plaintext, Nonce, key));
    }
    
    [TestMethod]
    public void Decrypt_ValidInputs()
    {
        Span<byte> plaintext = stackalloc byte[Ciphertext.Length];
        XChaCha20.Decrypt(plaintext, Ciphertext, Nonce, Key);
        Assert.IsTrue(plaintext.SequenceEqual(Plaintext));
    }
    
    [TestMethod]
    public void Decrypt_DifferentCiphertext()
    {
        Span<byte> plaintext = stackalloc byte[Ciphertext.Length];
        Span<byte> ciphertext = Ciphertext.ToArray();
        ciphertext[0]++;
        XChaCha20.Decrypt(plaintext, ciphertext, Nonce, Key);
        Assert.IsFalse(plaintext.SequenceEqual(Plaintext));
    }
    
    [TestMethod]
    public void Decrypt_DifferentNonce()
    {
        Span<byte> plaintext = stackalloc byte[Ciphertext.Length];
        Span<byte> nonce = Nonce.ToArray();
        nonce[0]++;
        XChaCha20.Decrypt(plaintext, Ciphertext, nonce, Key);
        Assert.IsFalse(plaintext.SequenceEqual(Plaintext));
    }
    
    [TestMethod]
    public void Decrypt_DifferentKey()
    {
        Span<byte> plaintext = stackalloc byte[Ciphertext.Length];
        Span<byte> key = Key.ToArray();
        key[0]++;
        XChaCha20.Decrypt(plaintext, Ciphertext, Nonce, key);
        Assert.IsFalse(plaintext.SequenceEqual(Plaintext));
    }

    [TestMethod]
    public void Decrypt_InvalidPlaintext()
    {
        var plaintext = Array.Empty<byte>();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Decrypt(plaintext, Ciphertext, Nonce, Key));
    }
    
    [TestMethod]
    public void Decrypt_InvalidCiphertext()
    {
        var plaintext = new byte[Ciphertext.Length];
        var ciphertext = Array.Empty<byte>();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Decrypt(plaintext, ciphertext, Nonce, Key));
    }
    
    [TestMethod]
    public void Decrypt_InvalidNonce()
    {
        var plaintext = new byte[Ciphertext.Length];
        var nonce = new byte[XChaCha20.NonceSize - 1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Decrypt(plaintext, Ciphertext, nonce, Key));
        nonce = new byte[XChaCha20.NonceSize + 1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Decrypt(plaintext, Ciphertext, nonce, Key));
    }
    
    [TestMethod]
    public void Decrypt_InvalidKey()
    {
        var plaintext = new byte[Ciphertext.Length];
        var key = new byte[XChaCha20.KeySize - 1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Decrypt(plaintext, Ciphertext, Nonce, key));
        key = new byte[XChaCha20.KeySize + 1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => XChaCha20.Decrypt(plaintext, Ciphertext, Nonce, key));
    }
}