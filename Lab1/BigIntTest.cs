using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Lab1
{
    [TestFixture]
    public class BigIntTest
    {
        [Test]
        public void TestOnEmptyConstructor()
        {
            var bigInt = new BigInt();
            Assert.AreEqual(Sign.Plus, bigInt.GetSign());
            Assert.AreEqual(new List<int>{0}, bigInt.GetDigits());
        }
        [TestCase("134", Sign.Plus, new[] {1, 3, 4})]
        [TestCase("+134", Sign.Plus, new[] {1, 3, 4})]
        [TestCase("-134", Sign.Minus, new[] {1, 3, 4})]
        public void TestOnStringConstructor(string num, Sign sign, int[] digits)
        {
            var bigInt = new BigInt(num);
            Assert.AreEqual(sign, bigInt.GetSign());
            Assert.AreEqual(digits, bigInt.GetDigits());
            Assert.AreEqual(digits, bigInt.GetDigits());
        }
        [TestCase("3123a")]
        [TestCase("a232")]
        [TestCase("-a232")]
        [TestCase("+a232")]
        [TestCase("+23a232")]
        public void TestOnStringConstructorWithWrongString(string num)
        {
            var exception = Assert.Throws<ArgumentException>(() => { new BigInt(num); });
            if (exception != null)
                Assert.AreEqual("Строка должна состоять из числа с или без знака в начале", exception.Message);
        }
        [Test]
        public void TestOnBigIntConstructor()
        {
            BigInt expected = new BigInt("123");
            BigInt actual = new BigInt(expected);
            Assert.AreEqual(actual.GetSign(), expected.GetSign());
            Assert.AreEqual(actual.GetDigits(), expected.GetDigits());
        }
        [TestCase(Sign.Minus, new[] {1, 2})]
        [TestCase(Sign.Plus, new[] {1, 2})]
        public void TestOnSignAndDigitsConstructor(Sign sign, int[] digits)
        {
            BigInt bigInt = new BigInt(sign, digits.ToList());
            Assert.AreEqual(sign, bigInt.GetSign());
            Assert.AreEqual(digits, bigInt.GetDigits());
        }
        [Test]
        public void TestOnSignAndDigitsConstructorWithNonDigits()
        {
            var sign = Sign.Minus;
            var digits = new List<int> {1, 432, 6};
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                var bigInt = new BigInt(sign, digits);
            });
            if (exception != null) Assert.AreEqual("Лист должен состоять из цифр", exception.Message);
        }
        [TestCase(12)]
        [TestCase(-12)]
        [TestCase(0)]
        public void TestOnIntegerConstructor(long num)
        {
            var bigInt = new BigInt(num);
            Assert.AreEqual(new BigInt(num.ToString()), bigInt);
        }
        [TestCase("123456789", "987654321", "1111111110")]
        [TestCase("-123456789", "-987654321", "-1111111110")]
        [TestCase("-123456789", "987654321", "864197532")]
        [TestCase("123456789", "-123456789", "0")]
        [TestCase("123456789", "0", "123456789")]
        public void TestOnAdd(string num1, string num2, string res)
        {
            var bigInt1 = new BigInt(num1);
            var bigInt2 = new BigInt(num2);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt1 + bigInt2);
        }
        [TestCase("12", "-12")]
        [TestCase("-12", "12")]
        [TestCase("0", "0")]
        public void TestOnUnaryMinus(string num, string res)
        {
            var bigInt1 = new BigInt(num);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, -bigInt1);
        }
        [TestCase("123", "98", "25")]
        [TestCase("-123", "-98", "-25")]
        [TestCase("-123", "98", "-221")]
        [TestCase("123", "-98", "221")]
        [TestCase("123", "123", "0")]
        [TestCase("100", "95", "5")]
        public void TestOnSubtraction(string num1, string num2, string res)
        {
            var bigInt1 = new BigInt(num1);
            var bigInt2 = new BigInt(num2);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt1 - bigInt2);
        }
        [TestCase("134", 0, "0")]
        [TestCase("134", 1, "134")]
        [TestCase("134", 4, "536")]
        [TestCase("-134", 4, "-536")]
        public void TestOnMultOnDigit(string num, int digit, string res)
        {
            var bigInt = new BigInt(num);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt.MultOnDigit(digit));
        }
        [TestCase("134", -1)]
        [TestCase("134", 11)]
        public void TestOmMultOnNonDigit(string num, int digit)
        {
            var exception = Assert.Throws<ArgumentException>(() => { new BigInt(num).MultOnDigit(digit); });
            if (exception != null) Assert.AreEqual("n должна быть цифрой", exception.Message);
        }
        [TestCase("12", 1, "120")]
        [TestCase("-12", 1, "-120")]
        [TestCase("12", 6, "12000000")]
        [TestCase("12", 0, "12")]
        public void TestOnMultOn10(string num, int pow, string res)
        {
            var bigInt = new BigInt(num);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt.MultOn10(pow));
        }
        [TestCase("123", "145", "17835")]
        [TestCase("-123", "145", "-17835")]
        [TestCase("-123", "-145", "17835")]
        [TestCase("-123", "0", "0")]
        [TestCase("123", "0", "0")]
        public void TestOnMultOnBigInt(string num1, string num2, string res)
        {
            var bigInt1 = new BigInt(num1);
            var bigInt2 = new BigInt(num2);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt1 * bigInt2);
        }
        [TestCase("1231234567123456789123456891234567812345123457123456",
            "1231234567123456789123456891234567812345123457123456", true)]
        [TestCase("-1231234567123456789123456891234567812345123457123456",
            "-1231234567123456789123456891234567812345123457123456", true)]
        [TestCase("1231234567123456789123456891234567812345123457123456",
            "-1231234567123456789123456891234567812345123457123456", false)]
        [TestCase("23134543655", "32556456", false)]
        [TestCase("23134543655", "0", false)]
        [TestCase("0", "0", true)]
        public void TestOnEquals(string num1, string num2, bool res)
        {
            var bigInt1 = new BigInt(num1);
            var bigInt2 = new BigInt(num2);
            Assert.AreEqual(res, bigInt1 == bigInt2);
            Assert.AreEqual(!res, bigInt1 != bigInt2);
        }
        [TestCase("1231234567123456789123456891234567812345123457123456",
            "123123456712345678912345689123456781234512345712", true)]
        [TestCase("-1231234567123456789123456891234567812345123457123456",
            "-1231234567123456789123456891234567812345123457123", false)]
        [TestCase("1231234567123456789123456891234567812345123457123456",
            "-1231234567123456789123456891234567812345123457123456", true)]
        [TestCase("23134543655", "32556456", true)]
        [TestCase("23134543655", "0", true)]
        [TestCase("0", "32324224", false)]
        [TestCase("-12", "-5", false)]
        public void TestOnCompare(string num1, string num2, bool res)
        {
            var bigInt1 = new BigInt(num1);
            var bigInt2 = new BigInt(num2);
            Assert.AreEqual(res, bigInt1 > bigInt2);
            Assert.AreEqual(!res, bigInt1 < bigInt2);
        }
        [TestCase("1243567", 4, "310891")]
        [TestCase("-1243567", 4, "-310891")]
        [TestCase("12", 4, "3")]
        [TestCase("7", 8, "0")]
        public void TestOnDivOnDigit(string num, int v, string res)
        {
            var bigInt = new BigInt(num);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt.DivOnDigit(v));
        }
        [Test]
        public void TestOnDivOnZero()
        {
            var exception = Assert.Throws<DivideByZeroException>(() => { new BigInt("321").DivOnDigit(0); });
            if (exception != null) Assert.AreEqual("Делить на 0 нельзя", exception.Message);
        }
        [TestCase(-1)]
        [TestCase(11)]
        public void TestOnDivOnNonDigit(int v)
        {
            var exception = Assert.Throws<ArgumentException>(() => { new BigInt("321").DivOnDigit(v); });
            if (exception != null) Assert.AreEqual("v должно быть цифрой", exception.Message);
        }
        [TestCase("1213", 3, "1")]
        [TestCase("450", 5, "0")]
        public void TestOnModOnDigit(string num, int n, string res)
        {
            var bigInt = new BigInt(num);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt.ModOnDigit(n));
        }
        [TestCase(-1)]
        [TestCase(11)]
        public void TestOnModOnNonDigit(int v)
        {
            var exception = Assert.Throws<ArgumentException>(() => { new BigInt("321").ModOnDigit(v); });
            if (exception != null) Assert.AreEqual("v должно быть цифрой", exception.Message);
        }
        [Test]
        public void TestOnModOnZero()
        {
            var exception = Assert.Throws<DivideByZeroException>(() => { new BigInt("321").ModOnDigit(0); });
            if (exception != null) Assert.AreEqual("Делить на 0 нельзя", exception.Message);
        }
        [TestCase("200", "28", "7")]
        [TestCase("-200", "28", "-7")]
        [TestCase("0", "2", "0")]
        [TestCase("-200", "-28", "7")]
        [TestCase("-200", "3", "-66")]
        [TestCase("-200", "5", "-40")]
        [TestCase("123456789123456789", "23", "5367686483628556")]
        public void TestOnDivOnBigInt(string num1, string num2, string res)
        {
            var a = new BigInt(num1);
            var b = new BigInt(num2);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, a / b);
        }
        [Test]
        public void TestOnDivOnBigIntAsZero()
        {
            var zero = BigInt.Zero();
            var num = new BigInt("12");
            var exception = Assert.Throws<DivideByZeroException>(() =>
            {
                var res = num / zero;
            });
            if (exception != null) Assert.AreEqual("Делить на 0 нельзя", exception.Message);
        }
        [TestCase("200", "28", "4")]
        [TestCase("-200", "28", "-4")]
        [TestCase("0", "2", "0")]
        [TestCase("-200", "-28", "4")]
        [TestCase("123456789123456789", "23", "1")]
        public void TestOnModOnBigInt(string num1, string num2, string res)
        {
            var a = new BigInt(num1);
            var b = new BigInt(num2);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, a % b);
        }
        [TestCase("15", "2", "10", "5")]
        [TestCase("-15", "2", "10", "5")]
        [TestCase("-15", "3", "10", "-5")]
        [TestCase("21312", "32434", "5456", "64")]
        [TestCase("21312", "5", "5456", "5424")]
        public void TestOnModPow(string num1, string num2, string n, string res)
        {
            var bigInt = new BigInt(num1);
            var pow = new BigInt(num2);
            var mod = new BigInt(n);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt.ModPow(pow, mod));
        }
        
        [TestCase("15", "2", "225")]
        [TestCase("-15", "2", "225")]
        [TestCase("-15", "3", "-3375")]
        [TestCase("21312", "2", "454201344")]
        [TestCase("2", "27", "134217728")]
        public void TestOnPow(string num1, string num2, string res)
        {
            var bigInt = new BigInt(num1);
            var pow = new BigInt(num2);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt.Pow(pow));
        }
        [TestCase("11", "15", "11")]
        [TestCase("2423", "24321", "2138")]
        public void TestOnModInverse(string num, string n, string res)
        {
            var bigInt = new BigInt(num);
            var mod = new BigInt(n);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, bigInt.ModInverse(mod));
        }
        [TestCase("111", "15")]
        [TestCase("120", "5")]
        public void TestOnNotExistModInverse(string num, string n)
        {
            var bigInt = new BigInt(num);
            var mod = new BigInt(n);
            var exception = Assert.Throws<ArgumentException>(() => { bigInt.ModInverse(mod); });
            if (exception != null) 
                Assert.AreEqual("Нельзя найти обратный элемент", exception.Message);
        }
        [TestCase("213", "31242", "3")]
        [TestCase("12345", "67890", "15")]
        public void TestOnGCD(string num1, string num2, string res)
        {
            var a = new BigInt(num1);
            var b = new BigInt(num2);
            var expected = new BigInt(res);
            Assert.AreEqual(expected, BigInt.GetGCD(a, b, out _, out _));
        }
    }
}