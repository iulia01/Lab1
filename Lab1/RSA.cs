using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.String;

namespace Lab1
{
    public class RSA
    {
        private BigInt[] _publicKey;
        private BigInt[] _privateKey;
        public void GenerateKeys()
        {
            var numbers = File.ReadAllText("Primes.txt", Encoding.UTF8).Split(' ');
            var rnd = new Random();
            var indexP = rnd.Next(0, numbers.Length);
            var indexQ = rnd.Next(0, numbers.Length);
            var p = new BigInt(numbers[indexP]);
            var q = new BigInt(numbers[indexQ]);
            var n = p * q;
            var fi = (p - BigInt.One()) * (q - BigInt.One());
            var indexE = rnd.Next(0, numbers.Length);
            var e = new BigInt(numbers[indexE]);
            while (BigInt.GetGCD(e, fi, out _, out _) != BigInt.One() || e >= fi)
            {
                indexE = rnd.Next(0, numbers.Length);
                e = new BigInt(numbers[indexE]);
            }

            var d = e.ModInverse(fi);
            _publicKey = new[] {e, n};
            _privateKey = new[] {d, n};
        }
        public string Encrypt(string text, BigInt e, BigInt n)
        {
            var cipher = new List<string>();
            foreach (var elem in text)
            {
                var c = new BigInt(elem).ModPow(e, n);
                cipher.Add(c.ToString());
            }

            return Join(" ", cipher);
        }
        public BigInt[] GetPublicKey()
        {
            return _publicKey;
        }
        public BigInt[] GetPrivateKey()
        {
            return _privateKey;
        }
        public string Decrypt(string cipher, BigInt d, BigInt n)
        {
            var text = new List<char>();
            var ciphers = cipher.Split(' ');
            foreach (var elem in ciphers)
            {
                var c = new BigInt(elem).ModPow(d, n);
                try
                {
                    text.Add((char)Convert.ToInt32(c.ToString()));
                }
                catch
                {
                    throw new ArgumentException(
                        "Какой-то символ невозможно расшифровать. Возможно, введены неправильные ключи при шифровании или дешифровании");
                }
            }
            return Join("", text);
        }
    }
}