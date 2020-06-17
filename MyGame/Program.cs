using System;
using System.Security.Cryptography;
using System.Text;

namespace MyGame
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                for (int j = i + 1; j < args.Length; j++)
                {
                    if (args[i] == args[j])
                    {
                        Console.WriteLine("Wrong arguments");
                        return;
                    }
                }
            }
            if (args.Length % 2 != 0 && args.Length > 2)
            {
                var key = new byte[32];
                var pc = new byte[4];
                var gen = RandomNumberGenerator.Create();
                gen.GetBytes(key);
                gen.GetBytes(pc);
                var ipc = (int)(((double)BitConverter.ToUInt32(pc, 0) / UInt32.MaxValue) * (double)args.Length);
                var hmac = CreateToken(ipc.ToString(), BitConverter.ToString(key));
                Console.WriteLine("HMAC: " + hmac);
                Console.WriteLine("Available moves:");
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine((i + 1) + " - " + args[i]);
                }
                Console.WriteLine("0 - exit");
                int player = 0;
                if (!int.TryParse(Console.ReadLine(), out player) || player < 0 || player > args.Length)
                {
                    Console.WriteLine("Wrong move");
                    return;
                }
                else if (player == 0) return;
                player -= 1;
                if (player > ipc && player + (args.Length / 2) > ipc || ipc > player && player - (args.Length / 2) >= ipc)
                {
                    Console.WriteLine("You win!");
                }
                else if (ipc == player)
                {
                    Console.WriteLine("Draw!");
                }
                else Console.WriteLine("You lose!");
                StringBuilder keyString = new StringBuilder();
                for (int i = 0; i < key.Length; i++)
                {
                    keyString.Append(key[i].ToString("x2"));
                }
                Console.WriteLine("Computer move: " + args[ipc] + "\nHMAC key: " + keyString);
                Console.ReadLine();
            }
            else Console.WriteLine("Wrong arguments");
        }
        private static string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashmessage.Length; i++)
                {
                    builder.Append(hashmessage[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}

