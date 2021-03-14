using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;

namespace Lab1
{
    public enum Sign
    {
        Plus = 1,
        Minus = -1
    }
    public class BigInt
    {
        private Sign _sign = Sign.Plus;
        private List<int> _digits;
        public List<int> GetDigits()
        {
            return _digits;
        }
        public Sign GetSign()
        {
            return _sign;
        }
        public BigInt()
        {
            _digits = new List<int> {0};
        }
        public BigInt(long num)
        {
            if (num == 0)
            {
                _sign = Sign.Plus;
                _digits = new List<int> {0};
            }
            else
            {
                _sign = num < 0 ? Sign.Minus : Sign.Plus;
                _digits = new List<int>();
                num = num < 0 ? -num : num;
                while (num > 0)
                {
                    _digits.Add((int) num % 10);
                    num /= 10;
                }

                _digits.Reverse();
            }
        }
        public BigInt(string num)
        {
            _digits = new List<int>();
            var startIndex = 0;
            var first = num[0];
            if (first == '-')
            {
                _sign = Sign.Minus;
                startIndex = 1;
            }
            else if (first == '+')
            {
                startIndex = 1;
            }
            for (var i = startIndex; i < num.Length; i++)
            {
                var digit = num[i].ToString();
                try
                {
                    _digits.Add(Int32.Parse(digit));
                }
                catch
                {
                    throw new ArgumentException("Строка должна состоять из числа с или без знака в начале");
                }
            }
        }
        public BigInt(BigInt num)
        {
            _sign = num._sign;
            _digits = num._digits;
        }
        public BigInt(Sign sign, List<int> digits)
        {
            if (digits.Any(digit => digit < 0 || digit > 9))
            {
                throw new ArgumentException("Лист должен состоять из цифр");
            }
            _sign = sign;
            _digits = digits.ToList();
        }
        private static int Mod(int a, int b)
        {
            return a - b * Div(a, b);	
        }
        private static int Div(int a, int b)
        {
            if (a >= 0)
                return a / b;
            if (-a % b != 0)					
                return -(-a / b + 1);
            return -(-a / b);
        }
        public BigInt MultOnDigit(int n)
        {
            var buf = Zero();
            if (n < 0 || n > 9)
                throw new ArgumentException("n должна быть цифрой");
            if (n == 0)
                return buf;
            if (n == 1)
                return this;
            buf = new BigInt(this);
            for (var i = 1; i < n; i++)
                buf += this;
            return buf;
        }
        public BigInt MultOn10(int n)
        {
            var buf = new BigInt(_sign, _digits);
            for (var i = 0; i < n; i++)
                buf._digits.Add(0);
            return buf;
        }
        private static BigInt Add(BigInt a, BigInt b)
        {
            var sum = new BigInt(Sign.Plus, new List<int>());
            var aSize = a.Size();
            var bSize = b.Size();
            if (aSize < bSize)
            {
                a.AddNulls(bSize);
            }
            if (bSize < aSize)
            {
                b.AddNulls(aSize);
            }
            var size = a.Size();
            if ((int) a._sign * (int) b._sign == 1)
            {
                sum._sign = a._sign;
                int k = 0;
                int w;
                for (var i = size - 1; i >= 0; i--)
                {
                    w = Mod(a._digits[i] + b._digits[i] + k, 10);
                    k = Div(a._digits[i] + b._digits[i] + k, 10);
                    sum._digits.Add(w);
                }
                if (k > 0)
                    sum._digits.Add(1);
            }
            else
            {
                if (a.CompareToWithoutSign(b) > 0)
                {
                    int k = 0, w;
                    for (var i = size - 1; i >= 0; i--)
                    {
                        w = Mod(a._digits[i] - b._digits[i] + k, 10);
                        k = Div(a._digits[i] - b._digits[i] + k, 10);
                        sum._digits.Add(w);
                    }

                    sum._sign = a._sign;
                }
                else if (a.CompareToWithoutSign(b) < 0)
                {
                    int k = 0, w;
                    for (var i = size - 1; i >= 0; i--)
                    {
                        w = Mod(b._digits[i] - a._digits[i] + k, 10);
                        k = Div(b._digits[i] - a._digits[i] + k, 10);
                        sum._digits.Add(w);
                    }

                    sum._sign = a._sign == Sign.Plus ? Sign.Minus : Sign.Plus;
                }
                else
                {
                    sum._sign = Sign.Plus;
                    sum._digits = new List<int> {0};
                }
            }
            sum._digits.Reverse();
            a.RemoveNulls();
            b.RemoveNulls();
            sum.RemoveNulls();
            return sum;
        }
        private static BigInt Mult(BigInt a, BigInt b)
        {
            var sum = Zero();
            for (int i = 0; i < b.Size(); i++)
            {
                BigInt buf = a.MultOnDigit(b._digits[i]);
                buf = buf.MultOn10(b.Size() - i - 1);
                sum += buf;
            }

            if (sum.CompareToWithoutSign(Zero()) == 0)
            {
                return Zero();
            }

            sum._sign = (Sign) ((int) a._sign * (int) b._sign);
            return sum;
                
        }
        private static BigInt UnaryMinus(BigInt a)
        {
            if (a == Zero())
            {
                return a;
            }

            Sign sign = a._sign == Sign.Plus ? Sign.Minus : Sign.Plus;

            var buf = new BigInt(sign, a._digits);
            return buf;
        }
        private static BigInt Subtraction(BigInt a, BigInt b)
        {
            var buf = -b;
            buf = a + buf;
            return buf;
        }
        public BigInt DivOnDigit(int v)
        {
            if (v == 0)
            {
                throw new DivideByZeroException("Делить на 0 нельзя");
            }
            if (v < 0 || v > 9)
            {
                throw new ArgumentException("v должно быть цифрой");
            }
            var res = new BigInt(Sign.Plus, new List<int>());
            var r = 0;
            for (var i = 0; i < Size(); i++)
            {
                res._digits.Add(Div(r * 10 + _digits[i], v));
                r = Mod(r * 10 + _digits[i], v);
            }
            res.RemoveNulls();
            res._sign = _sign;
            return res;
        }
        public BigInt ModOnDigit(int v)
        {
            var buf = new BigInt(this);
            return buf - buf.DivOnDigit(v).MultOnDigit(v);
        }
        private static BigInt DivOnBigInt(BigInt a, BigInt b)
        {
            if (b == Zero())
            {
                throw new DivideByZeroException("Делить на 0 нельзя");
            }
            if (a == b)
            {
                return One();
            }
            if (a.CompareToWithoutSign(b) < 0)
            {
                return Zero();
            }
            var n = a.Size();
            var t = b.Size();
            BigInt buf;
            var q = new BigInt(Sign.Plus, new List<int>());
            var sign = (Sign) ((int) a._sign * (int) b._sign);
            a._sign = Sign.Plus;
            b._sign = Sign.Plus;
            var current = 0;
            for (var i = n; i >= t; i--)
            {
                buf = b.MultOn10(i - t);
                while (a >= buf)
                {
                    current++;
                    a -= buf;
                }

                q._digits.Add(current);
                current = 0;
            }
            q.RemoveNulls();
            q._sign = sign;
            return q;
        }
        private static BigInt ModOnBigInt(BigInt a, BigInt b)
        {
            if (a == b)
            {
                return Zero();
            }
            if (a.CompareToWithoutSign(b) < 0)
            {
                return a;
            }
            var sign = (Sign) ((int) a._sign * (int) b._sign);
            var u = new BigInt(Sign.Plus, a._digits);
            var v = new BigInt(Sign.Plus, b._digits);
            var buf = u - (u / v) * v;
            buf._sign = sign;
            return buf;
        }
        public BigInt ModPow(BigInt pow, BigInt n)
        {
            if (pow._sign == Sign.Minus)
            {
                return Zero();
            }
            if (pow == Zero())
            {
                return One();
            }
            var a = new BigInt(this);
            var b = One();
            while (pow != Zero())
            {
                if (pow.ModOnDigit(2) == Zero())
                {
                    pow = pow.DivOnDigit(2);
                    a = (a * a) % n;
                }
                else
                {
                    pow -= One();
                    b = (b * a) % n;
                }
            }
            return b;
        }
        public BigInt Pow(BigInt pow)
        {
            if (pow._sign == Sign.Minus)
            {
                return Zero();
            }
            if (pow == Zero())
            {
                return One();
            }
            var a = new BigInt(this);
            var b = One();
            while (pow != Zero())
            {
                if (pow.ModOnDigit(2) == Zero())
                {
                    pow = pow.DivOnDigit(2);
                    a *= a;
                }
                else
                {
                    pow -= One();
                    b *= a;
                }
            }
            return b;
        }
        public static BigInt GetGCD(BigInt a, BigInt b, out BigInt x, out BigInt y)
        {
            if (a == Zero())
            {
                x = Zero();
                y = One();
                return b;
            }
            var d = GetGCD(b % a, a, out var x1, out var y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }

        public BigInt ModInverse(BigInt m)
        {
            var a = new BigInt(this);
            BigInt x;
            var gcd = GetGCD(a, m, out x, out _) == One();
            if (gcd)
            {
                x = (x % m + m) % m;
            }
            else
            {
                throw new ArgumentException("Нельзя найти обратный элемент");
            }
            return x;
        }
        public override bool Equals(object obj)
        {
            var bigInt = (BigInt) obj;
            return !(bigInt is null) && GetHashCode() == bigInt.GetHashCode();
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int) 2166136261;
                hash = (hash * 16777619) ^ _sign.GetHashCode();
                return _digits.Aggregate(hash, (current, digit) => (current * 16777619) ^ digit.GetHashCode());
            }
        }
        private int CompareTo(object obj)
        {
            var bigInt = (BigInt) obj;
            if (_sign == Sign.Plus && bigInt._sign == Sign.Minus)
            {
                return 1;
            }
            if (_sign == Sign.Minus && bigInt._sign == Sign.Plus)
            {
                return -1;
            }
            var res = CompareToWithoutSign(obj);
            return _sign == Sign.Minus && bigInt._sign == Sign.Minus ? -res : res;
        }

        private int CompareToWithoutSign(object obj)
        {
            var bigInt = (BigInt) obj;
            if (Size() > bigInt.Size())
            {
                return 1;
            }
            if (Size() < bigInt.Size())
            {
                return -1;
            }
            for (var i = 0; i < Size(); i++)
            {
                if (_digits[i] > bigInt._digits[i])
                {
                    return 1;
                }
                if (_digits[i] < bigInt._digits[i])
                {
                    return -1;
                }
            }
            return 0;
        }
        private void AddNulls(int length)
        {
            var res = new List<int>();
            var count = length - Size();
            for (var i = 0; i < count; i++)
            {
                res.Add(0);
            }
            res.AddRange(_digits);
            _digits = res;
        }
        private void RemoveNulls()
        {
            for (var i = 0; i < Size(); i++)
            {
                if (_digits[i] != 0)
                {
                    _digits.RemoveRange(0, i);
                    break;
                }
            }
        }
        public int Size()
        {
            return _digits.Count;
        }
        public override string ToString()
        {
            var res = new List<string>();
            if (_sign == Sign.Minus)
            {
                res.Add("-");
            }
            res.AddRange(_digits.Select(digit => digit.ToString()));
            return Join("", res);
        }
        public static BigInt Zero() => new BigInt();
        public static BigInt One() => new BigInt(Sign.Plus, new List<int> {1});
        public static BigInt operator +(BigInt a, BigInt b) => Add(a, b);
        public static BigInt operator -(BigInt a, BigInt b) => Subtraction(a, b);
        public static BigInt operator *(BigInt a, BigInt b) => Mult(a, b);
        public static BigInt operator -(BigInt a) => UnaryMinus(a);
        public static bool operator >(BigInt a, BigInt b) => a.CompareTo(b) > 0;
        public static bool operator <(BigInt a, BigInt b) => a.CompareTo(b) < 0;
        public static bool operator >=(BigInt a, BigInt b) => a.CompareTo(b) >= 0;
        public static bool operator <=(BigInt a, BigInt b) => a.CompareTo(b) <= 0;
        public static bool operator ==(BigInt a, BigInt b) => !(a is null) && a.Equals(b);
        public static bool operator !=(BigInt a, BigInt b) => !(a is null) && !a.Equals(b);
        public static BigInt operator /(BigInt a, BigInt b) => DivOnBigInt(a, b);
        public static BigInt operator %(BigInt a, BigInt b) => ModOnBigInt(a, b);
    }
}