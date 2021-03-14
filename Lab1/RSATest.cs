using System;
using NUnit.Framework;

namespace Lab1
{
    [TestFixture]
    public class RSATest
    {
        [TestCase(@",.-'""!?:;'/\|@#$%^&*()=_{}[]<>~`—№")]
        [TestCase("0123456789")]
        [TestCase("АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ")]
        [TestCase("абвгдеёжзийклмнопрстуфхцчшщъыьэюя")]
        [TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [TestCase("abcdefghijklmnopqrstuvwxyz")]
        [TestCase(@"К нам весна шагает
Быстрыми шагами,
И сугробы тают под её ногами.
Чёрные проталины
На полях видны.
Видно очень тёплые ноги у весны.")]
        public void TestOnRightArg(string text)
        {
            var rsa = new RSA();
            rsa.GenerateKeys();
            var openKey = rsa.GetPublicKey();
            var e = openKey[0];
            var n = openKey[1];
            var cipher = rsa.Encrypt(text, e, n);
            var closeKey = rsa.GetPrivateKey();
            var d = closeKey[0];
            var res = rsa.Decrypt(cipher, d, n);
            Assert.AreEqual(text, res);
        }
        [TestCase(@",.-'""!?:;'/\|@#$%^&*()=_{}[]<>~`—№")]
        [TestCase("0123456789")]
        [TestCase("АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ")]
        [TestCase("абвгдеёжзийклмнопрстуфхцчшщъыьэюя")]
        [TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [TestCase("abcdefghijklmnopqrstuvwxyz")]
        public void TestOnWrongСloseKey(string text)
        {
            var rsa = new RSA();
            rsa.GenerateKeys();
            var openKey = rsa.GetPublicKey();
            var e = openKey[0];
            var n = openKey[1];
            var cipher = rsa.Encrypt(text, e, n);
            var res = rsa.Decrypt(cipher, new BigInt("21231"), new BigInt("212354"));
            Assert.AreNotEqual(text, res);
        }
        [TestCase(@",.-'""!?:;'/\|@#$%^&*()=_{}[]<>~`—№")]
        [TestCase("0123456789")]
        [TestCase("АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ")]
        [TestCase("абвгдеёжзийклмнопрстуфхцчшщъыьэюя")]
        [TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [TestCase("abcdefghijklmnopqrstuvwxyz")]
        public void TestOnWrongOpenKey(string text)
        {
            var rsa = new RSA();
            rsa.GenerateKeys();
            var openKey = rsa.GetPublicKey();
            var cipher = rsa.Encrypt(text, new BigInt(223), new BigInt(23123));
            var closeKey = rsa.GetPrivateKey();
            var d = closeKey[0];
            var n = openKey[1];
            var res = rsa.Decrypt(cipher, d, n);
            Assert.AreNotEqual(text, res);
        }
    }
}