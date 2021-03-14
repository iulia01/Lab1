using System;
using System.IO;

namespace Lab1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var rsa = new RSA();
            Console.Write("Введите команду: \ngK - сгенерировать ключи \neC - защифровать сообщение \ndC - расшифровать сообщение \nКоманда: ");
            var command = Console.ReadLine();
            var success = "Команда выполнена успешно!";
            switch (command)
            {
                case "gK":
                {
                    rsa.GenerateKeys();
                    var openKey = rsa.GetPublicKey();
                    var e = openKey[0];
                    var n = openKey[1];
                    var closeKey = rsa.GetPrivateKey();
                    var d = closeKey[0];
                    Console.WriteLine("Открытый ключ \n e = {0} n = {1}", e, n);
                    Console.WriteLine("Закрытый ключ \n d = {0} n = {1}", d, n);
                    Console.WriteLine(success);
                    break;
                }
                case "eC":
                {
                    Console.WriteLine("Введите открытый ключ");
                    BigInt e, n;
                    try
                    {
                        Console.Write("e = ");
                        e = new BigInt(Console.ReadLine());
                        Console.Write("n = ");
                        n = new BigInt(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("Ошибка: e и n должны быть числами с или без знака");
                        break;
                    }

                    Console.WriteLine("Введите полный адрес файла для шифрования");
                    var input = Console.ReadLine();
                    StreamReader sr;
                    try
                    {
                        sr = new StreamReader(input ?? string.Empty);
                    }
                    catch
                    {
                        Console.WriteLine("Ошибка: Неправильный адрес файла с сообщением");
                        break;
                    }

                    Console.WriteLine("Введите полный адрес файла для шифра");
                    var output = Console.ReadLine();
                    var text = "";


                    while (!sr.EndOfStream)
                    {
                        text += sr.ReadLine();
                    }

                    sr.Close();

                    var cipher = rsa.Encrypt(text, e, n);
                    var sw = new StreamWriter(output ?? string.Empty);
                    sw.WriteLine(cipher);
                    sw.Close();
                    Console.WriteLine(success);
                    break;
                }
                case "dC":
                {
                    Console.WriteLine("Введите секретный ключ");
                    BigInt d, n;
                    try
                    {
                        Console.Write("d = ");
                        d = new BigInt(Console.ReadLine());
                        Console.Write("n = ");
                        n = new BigInt(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("Ошибка: d и n должны быть числами с или без знака");
                        break;
                    }
                    Console.WriteLine("Введите полный адрес файла c шифром");
                    var input = Console.ReadLine();
                    StreamReader sr;
                    try
                    {
                        sr = new StreamReader(input ?? string.Empty);
                    }
                    catch
                    {
                        Console.WriteLine("Ошибка: Неправильный адрес файла с шифром");
                        break;
                    }
                    Console.WriteLine("Введите полный адрес файла для расшифрованного сообщения");
                    var output = Console.ReadLine();
                    var cipher = "";
                    while (!sr.EndOfStream)
                    {
                        cipher += sr.ReadLine();
                    }
                    sr.Close();
                    string text;
                    try
                    {
                        text = rsa.Decrypt(cipher, d, n);
                    }
                    catch
                    {
                        Console.WriteLine(
                            "Ошибка: Какой-то символ невозможно расшифровать. Возможно, введены неправильные ключи при шифровании или дешифровании");
                        break;
                    }
                    var sw = new StreamWriter(output ?? string.Empty);
                    sw.WriteLine(text);
                    sw.Close();
                    Console.WriteLine(success);
                    break;
                }
                default:
                    Console.WriteLine("Ошибка: Введена неправильная команда");
                    break;
            }

            Console.ReadLine();
        }
    }
}