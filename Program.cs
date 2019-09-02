using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCache
{
    class Program
    {
        static void Main(string[] args)
        {
            var cacheLRUfreq = new CacheLRUfreq<int, string>(2);
            for (; ; )
            {
                Console.WriteLine("Введите строку ('q' - выход):");
                string inputStr = Console.ReadLine();
                if (inputStr.ToLower() == "q")
                    break;           // !!!

                bool hasCache = cacheLRUfreq.SearchValue(inputStr.GetHashCode());
                if (!hasCache)
                {
                    cacheLRUfreq.Put(inputStr, inputStr.GetHashCode());
                    Console.WriteLine($"В кеше не было. Добавили");
                }
                else
                    Console.WriteLine($"В кеше присутствует.");
                Console.WriteLine();
            }
        }
    }
}
