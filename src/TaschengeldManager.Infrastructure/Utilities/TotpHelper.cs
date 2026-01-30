using System.Security.Cryptography;

namespace TaschengeldManager.Infrastructure.Utilities;

/// <summary>
/// Helper class for TOTP (Time-based One-Time Password) operations.
/// Implements RFC 6238 TOTP algorithm.
/// </summary>
public static class TotpHelper
{
    private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
    private const int TimeStepSeconds = 30;
    private const int CodeDigits = 6;
    private const int ClockSkewSteps = 1; // Allow ±1 time step for clock skew

    /// <summary>
    /// Verifies a TOTP code against a secret.
    /// </summary>
    /// <param name="secret">The Base32-encoded secret.</param>
    /// <param name="code">The TOTP code to verify.</param>
    /// <returns>True if the code is valid, false otherwise.</returns>
    public static bool VerifyCode(string? secret, string code)
    {
        if (string.IsNullOrEmpty(secret))
            return false;

        var key = Base32Decode(secret);
        var timeStep = GetCurrentTimeStep();

        // Check current and adjacent time steps for clock skew tolerance
        for (int i = -ClockSkewSteps; i <= ClockSkewSteps; i++)
        {
            var expectedCode = GenerateCode(key, timeStep + i);
            if (expectedCode == code)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Generates a TOTP code for the current time.
    /// </summary>
    /// <param name="secret">The Base32-encoded secret.</param>
    /// <returns>The generated TOTP code.</returns>
    public static string GenerateCurrentCode(string secret)
    {
        var key = Base32Decode(secret);
        var timeStep = GetCurrentTimeStep();
        return GenerateCode(key, timeStep);
    }

    /// <summary>
    /// Generates a new Base32-encoded secret for TOTP setup.
    /// </summary>
    /// <param name="length">The length of the secret (default 20 characters).</param>
    /// <returns>A Base32-encoded secret string.</returns>
    public static string GenerateSecret(int length = 20)
    {
        var random = RandomNumberGenerator.GetBytes(length);
        var chars = new char[length];

        for (int i = 0; i < length; i++)
        {
            chars[i] = Base32Alphabet[random[i] % 32];
        }

        return new string(chars);
    }

    private static long GetCurrentTimeStep()
    {
        return (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds / TimeStepSeconds;
    }

    private static string GenerateCode(byte[] key, long timeStep)
    {
        var timeBytes = BitConverter.GetBytes(timeStep);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(timeBytes);

        var data = new byte[8];
        Array.Copy(timeBytes, 0, data, 8 - timeBytes.Length, timeBytes.Length);

        using var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(data);

        var offset = hash[^1] & 0x0F;
        var binary = ((hash[offset] & 0x7F) << 24) |
                     ((hash[offset + 1] & 0xFF) << 16) |
                     ((hash[offset + 2] & 0xFF) << 8) |
                     (hash[offset + 3] & 0xFF);

        var modulo = (int)Math.Pow(10, CodeDigits);
        return (binary % modulo).ToString($"D{CodeDigits}");
    }

    private static byte[] Base32Decode(string input)
    {
        input = input.ToUpperInvariant().Replace(" ", "").TrimEnd('=');

        var bits = 0;
        var value = 0;
        var output = new List<byte>();

        foreach (var c in input)
        {
            var index = Base32Alphabet.IndexOf(c);
            if (index < 0) continue;

            value = (value << 5) | index;
            bits += 5;

            if (bits >= 8)
            {
                bits -= 8;
                output.Add((byte)(value >> bits));
            }
        }

        return output.ToArray();
    }
}
