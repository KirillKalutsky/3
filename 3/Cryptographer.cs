using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3
{
    public class Cryptographer
    {
        private readonly int leftBorder = Convert.ToInt32('a');
        private readonly int rightBorder = Convert.ToInt32('z');
        private readonly string keysFile = "Keys.txt";
        private readonly string splitSymbol = " ";
        private readonly Random rnd = new Random();
        private const int groupKeySize = 10;

        public IEnumerable<string> Encrypt(string input, IEnumerable<string> key)
        {
            var hInput = input.ToHexEnumerable().ToList();
            var keyL = key.ToList();

            if (hInput.Count != keyL.Count)
                throw new ArgumentException("Длина ключа должна соответсвовать длине" +
                                            " кодируемого сообщения");

            return XOR(hInput, keyL);
        }

        public string EncryptString(string input, IEnumerable<string> key)
        {
            var hInput = input.ToHexEnumerable().ToList();
            var keyL = key.ToList();

            if (hInput.Count != keyL.Count)
                throw new ArgumentException("Длина ключа должна соответсвовать длине" +
                                            " кодируемого сообщения");

            return string.Join(splitSymbol, XOR(hInput, keyL));
        }

        public IEnumerable<string> Decrypt(IEnumerable<string> encryptMessage, IEnumerable<string> key)
        {
            var hInput = encryptMessage.ToList();
            var keyL = key.ToList();

            if (hInput.Count != keyL.Count)
                throw new ArgumentException("Длина ключа должна соответсвовать длине" +
                                            " декодируемого сообщения");

            return XOR(hInput, keyL);
        }

        public string Decrypt(string encryptMessage, string key)
        {
            var hInput = encryptMessage.Split(splitSymbol).ToList();
            var keyL = key.Split(splitSymbol).ToList();

            if (hInput.Count != keyL.Count)
                throw new ArgumentException("Длина ключа должна соответсвовать длине" +
                                            " декодируемого сообщения");

            var result = XOR(hInput, keyL);
            return result.FromHexEnumerableToString();
        }

        public IEnumerable<string> FindKey(IEnumerable<string> first, IEnumerable<string> second)
        {
            var hInput = first.ToList();
            var keyL = second.ToList();

            if (hInput.Count != keyL.Count)
                throw new ArgumentException("Длина зашифрованного сообщения должна соответсвовать длине" +
                                            " расшифрованного сообщения");

            return XOR(hInput, keyL);
        }

        public string FindKey(string first, string second)
        {
            var hInput = first.Split(splitSymbol).ToList();
            var keyL = second.ToHexEnumerable().ToList();

            if (hInput.Count != keyL.Count)
                throw new ArgumentException("Длина зашифрованного сообщения должна соответсвовать длине" +
                                            " расшифрованного сообщения");

            return string.Join(splitSymbol, XOR(hInput, keyL));
        }

        public IEnumerable<string> XOR(List<string> first, List<string> second)
        {
            for (var i = 0; i < first.Count; i++)
            {
                yield return (int.Parse(first[i], NumberStyles.HexNumber) ^
                              int.Parse(second[i], NumberStyles.HexNumber)).ToString(format: "X");
            }
        }

        public IEnumerable<string> GenerateKey(int length)
        {
            for (var i = 0; i < length; i++)
                yield return rnd.Next(leftBorder, rightBorder)
                    .ToString(format: "X");
        }

        public IEnumerable<IEnumerable<string>> CreateKeyGroup(List<string> key)
        {
            File.Delete(keysFile);
            using (var tw = new StreamWriter(keysFile, true))
            {
                for (var i = 0; i < groupKeySize; i++)
                {
                    var keymask = GenerateKey(key.Count).ToList();
                    var newKey = XOR(key, keymask).ToList();
                    var lockKey = newKey.Concat(keymask).ToList();
                    yield return lockKey;
                    var line = string.Join("", lockKey);
                    tw.WriteLineAsync(line).Wait();
                }
            }
        }

        public IEnumerable<string> DecryptKeyFromGroup(List<string> encryptKey)
        {
            var center = encryptKey.Count / 2;
            var f = encryptKey.Take(center).ToList();
            var s = encryptKey.Skip(center).ToList();

            var result = XOR(s, f);
            return result;
        }
    }
}
