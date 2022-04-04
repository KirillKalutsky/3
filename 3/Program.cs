using System;
using System.Collections.Generic;
using System.Linq;


//Определить вид шифротекста при известном ключе и известном открытом тексте(закодировать используя ключ)

//Определить ключ, с помощью которого шифртекст может быть преобразован в некоторый фрагмент текста,
//представляющий собой  один из возможных вариантов прочтения  открытого текста(текст и желаемый текст, находим ключ)

//1.Генерировать случайный ключ заданной длины.(длина равна длине сообщения)
//2. На базе сгенерированного ключа формировать группу из 10 равнозначных ключей.
//Набор ключей должен быть сохранен в файл, для последующей передачи его адресату.
//3. Шифровать и дешифровать вводимые пользователем данные ключом, выбираемым случайно, либо указываемым пользователем.

namespace _3
{
    class Program
    {
        private static readonly string border = "-----------------------";
        private readonly static Cryptographer cryptographer = new();
        private readonly static Dictionary<int,Tuple<Action,string>> funcs= new();
        private static void ShowFunc()
        {
            foreach (var f in funcs)
            {
                Console.WriteLine($"{f.Key}: {f.Value.Item2}");
                Console.WriteLine(border);
            }
        }

        private static Action ChooseFunc(string func)
        {
            if (!int.TryParse(func, out var result))
                return () =>  Console.WriteLine("Введите одно из предложенных чисел");
            if(!funcs.ContainsKey(result))
                return () => Console.WriteLine("По данному индексу нет функций");

            return funcs[result].Item1;
        }

        static void Main(string[] args)
        {
           /* var fK = cryptographer.GenerateKey(5).ToList();
            var sk = cryptographer.GenerateKey(5).ToList();
            var result = cryptographer.XOR(fK.ToList(), sk.ToList());
            var pResult = string.Join(" ", result.Concat(sk));
            Console.WriteLine(pResult);

            var rPResult = pResult.Split(" ");
            Console.WriteLine(string.Join(" ", fK));
            Console.WriteLine(string.Join(" ",cryptographer.DecryptKeyFromGroup(rPResult.ToList())));
*/
            var gettingKeys = new Dictionary<int, Tuple<string, Func<int, IEnumerable<string>>>>
            {
                {
                    1, Tuple.Create<string, Func<int, IEnumerable<string>>>("Сгенерировать случайный ключ",
                        (x) =>
                        {
                            var key = cryptographer.GenerateKey(x).ToList();
                            Console.WriteLine($"Сгенерированный ключ: {string.Join(" ", key)}");
                            return key;
                        })
                },
                {
                    2, Tuple.Create<string, Func<int, IEnumerable<string>>>("Ввести ключ вручную",
                        (x) =>
                        {
                            IEnumerable<string> key;

                            do
                            {
                                Console.WriteLine("Введите ключ:");
                                key = Console.ReadLine().Split(" ");
                                if (key.Count() != x)
                                    Console.WriteLine("Ключ должен быть равен длине шифруемого текста");
                            } while (key.Count() != x);

                            return key;
                        })
                }
            };
            funcs[1] = Tuple.Create<Action, string>(() =>
                {
                    Console.WriteLine("Введите сообщение которое вы хотите зашифровать");
                    var input = Console.ReadLine();

                    int kChoose;
                    do
                    {
                        foreach (var k in gettingKeys)
                        {
                            Console.WriteLine($"{k.Key} {k.Value.Item1}");
                            Console.WriteLine(border);
                        }

                        var isInt = int.TryParse(Console.ReadLine(), out kChoose);
                        if (!(isInt || gettingKeys.ContainsKey(kChoose)))
                            Console.WriteLine("Такого варианта не существует");
                    } while (!gettingKeys.ContainsKey(kChoose));

                    var key = gettingKeys[kChoose].Item2(input.Length);
                    cryptographer.CreateKeyGroup(key.ToList());
                    var output = cryptographer.EncryptString(input, key);
                    Console.WriteLine($"Зашифрованное сообщение: {output}");
                },
                "Закодировать сообщение");
            funcs[2] = Tuple.Create<Action, string>(() =>
                {
                    Console.WriteLine("Введите сообщение которое вы хотите расшифровать");
                    var encode = Console.ReadLine();
                    Console.WriteLine("Введите ключ для расшифровки");
                    var key = Console.ReadLine();
                    string output;
                    try
                    {
                        output = cryptographer.Decrypt(encode, key);
                    }
                    catch (ArgumentException e)
                    {
                        output = e.Message;
                    }

                    Console.WriteLine(output);
                },
                "Декодировать сообщение");
            funcs[3] = Tuple.Create<Action, string>(() =>
                {
                    Console.WriteLine("Введите зашифрованное сообщение");
                    var encode = Console.ReadLine();
                    Console.WriteLine("Введите возможную расшифровку сообщения");
                    var decode = Console.ReadLine();
                    string output;
                    try
                    {
                        output = cryptographer.FindKey(encode, decode);
                    }
                    catch (Exception e)
                    {
                        output = e.Message;
                    }

                    Console.WriteLine(output);
                },
                "Найти ключ");
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                Console.WriteLine("Выберите одну из функций кодеровщика");

                ShowFunc();

                var chooseFunc = Console.ReadLine();

                var func = ChooseFunc(chooseFunc);

                func();
            }
        }
    }
}
