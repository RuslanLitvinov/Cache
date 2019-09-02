using System;
using UserCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    // План реализации функционала задачи.
    // Вызов стандартного метода поиска ключа в кеше SearchValue, нет вставить.
    // Вид кеша LRU, MRU, другого-самодельного это будет реализация этого интерфейса. 
    // Для этого нужно вызвать конструктор, в котором укажем тип ключа, тип значения и размер кеша.
    public class TestLRUFreq
    {
        [TestMethod]
        public void GetValue()
        {

            // Инициализируем Кеш размера 3 с учетом частоты запроса значений
            var cacheLRUfreq = new CacheLRUfreq<int, string>(3);
            string expected = "Значение 1";
            cacheLRUfreq.Put(expected, expected.GetHashCode());

            string actual = cacheLRUfreq.GetValue(expected.GetHashCode());

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void WipeOldVal()
        {

            // Инициализируем Кеш размера 3 с учетом частоты запроса значений
            var cacheLRUfreq = new CacheLRUfreq<int, string>(3);

            string val1 = "Значение 1";
            string val2 = "Значение 2";
            string val3 = "Значение 3";
            string val4 = "Значение 4";

            cacheLRUfreq.Put(val1, val1.GetHashCode());
            cacheLRUfreq.Put(val2, val2.GetHashCode());
            cacheLRUfreq.Put(val3, val3.GetHashCode());
            cacheLRUfreq.Put(val4, val4.GetHashCode());

            Assert.AreEqual(false, cacheLRUfreq.SearchValue(val1.GetHashCode()));
        }
        [TestMethod]
        public void ReSearch_Key_Appeared_in_Main_Queue()
        {

            // Инициализируем Кеш размера 3 с учетом частоты запроса значений
            var cacheLRUfreq = new CacheLRUfreq<int, string>(3);

            string val1 = "Значение 1";
            cacheLRUfreq.Put(val1, val1.GetHashCode());

            bool hasKey = cacheLRUfreq.SearchValue(val1.GetHashCode());  // В этот момент ключ перемещается в главную очередь, так как повторное обращение по нему
            int mainQIndex = cacheLRUfreq.MainQueueIndexOf(val1.GetHashCode());

            Assert.AreEqual(true, mainQIndex > -1);
        }
        [TestMethod]
        public void When_Search_Again_Key_Went_Up_in_Main_Queue()
        {
            // Инициализируем Кеш размера 3 с учетом частоты запроса значений
            var cacheLRUfreq = new CacheLRUfreq<int, string>(3);

            string val1 = "Значение 1";
            cacheLRUfreq.Put(val1, val1.GetHashCode());
            string val2 = "Значение 2";
            cacheLRUfreq.Put(val2, val2.GetHashCode());

            cacheLRUfreq.SearchValue(val1.GetHashCode());
            cacheLRUfreq.SearchValue(val2.GetHashCode());
            // Еще раз ищем ключ 1
            cacheLRUfreq.SearchValue(val1.GetHashCode());
            int mainQIndexActual = cacheLRUfreq.MainQueueIndexOf(val1.GetHashCode());
            int indexExpected = 0;

            Assert.AreEqual(indexExpected, mainQIndexActual);
        }
    }
}
