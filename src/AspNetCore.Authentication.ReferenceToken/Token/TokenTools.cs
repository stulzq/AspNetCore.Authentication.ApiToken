using System;
using System.Security.Cryptography;
using System.Text;

namespace AspNetCore.Authentication.ReferenceToken
{
    /// <summary>
    /// Token Generating Algorithm
    /// <para></para>
    /// UserId + DateTime.Now(MillSecond) + RandBytes(Length 8)
    /// <para></para>
    /// SHA256 + HEX
    /// </summary>
    public class TokenTools
    {
        public static string CreateToken(string userId)
        {
            var bytes = new Span<byte>(new byte[8]);
            var random = new Random(DateTime.Now.Millisecond);
            random.NextBytes(bytes);
            return CreateToken(userId, DateTime.Now, bytes);
        }

        public static string CreateToken(string userId, DateTimeOffset now, Span<byte> randBytes)
        {
            var data = Encoding.UTF8.GetBytes($"{userId}{now.ToUnixTimeMilliseconds()}").AsSpan();
            var resultBytes = new Span<byte>(new byte[data.Length + randBytes.Length]);
            data.CopyTo(resultBytes);
            randBytes.CopyTo(resultBytes.Slice(data.Length));

            using var sha = SHA256.Create();
            var result = sha.ComputeHash(resultBytes.ToArray());

            var hex = new StringBuilder();
            foreach (byte b in result)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString().ToUpper();
        }
    }
}