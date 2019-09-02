using System;
using System.Collections.Generic;

namespace UserCache
{
    /// <summary>
    /// Класс LRU-кеша с учетом частоты поиска ключа.
    /// Организован в виде двух очередей: 
    /// inQueue - вх.очередь - поиск ключа.
    /// mainQueue - повторный поиск ключа. 
    /// При вытеснении из mainQueue, ключи попадают в inQueue.
    /// При вытеснении из inQueue - забывается.
    /// Сложность такого кеша - всегда константа.
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    /// <typeparam name="Tvalue"></typeparam>
    public class CacheLRUfreq<Tkey, Tvalue> : ICache<Tkey, Tvalue>
    {
        public int CachValue { get; set; }
        /// <summary>
        /// Входная очередь устареваний поиска ключа
        /// </summary>
        private List<Tkey> inQueue;
        /// <summary>
        /// Возвращает индекс ключа во входной очереди.
        /// Если не найдено возвращает -1
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int InQueueIndexOf(Tkey key)
        {
            return inQueue.IndexOf(key);
        }
        /// <summary>
        /// Главная очередь повторений поиска ключа
        /// </summary>
        private List<Tkey> mainQueue;
        /// Возвращает индекс ключа в главной очереди повторений.
        /// Если не найдено возвращает -1
        public int MainQueueIndexOf(Tkey key)
        {
            return mainQueue.IndexOf(key);
        }

        private Dictionary<Tkey, Tvalue> repository;
        public bool SearchValue(Tkey key)
        {
            // Посмотрим во главной очереди
            if (MainQueueIndexOf(key) > -1)
            {
                MainQueueMoveToTop(key);
                return true;        // !!!
            }

            {   // Посмотрим во входной очереди
                int inQueueIndex = InQueueIndexOf(key);

                if (inQueueIndex > -1)
                {  // То, что во входной очереди нашелся ключ, означает, что в кеш уже обращались с этим ключем
                   // и значит поск повторился.
                    inQueue.RemoveAt(inQueueIndex);    // Сначала удалить, потому, что после вставки в главную очередь может произойти вытеснение во входную очередь и на месте inQueueIndex - будет уже другой ключ
                    MainQueueAdd(key);

                    return true;           // !!!
                }
            }

            // Не нашлось в кеше
            return false;        // !!!
        }
        public Tvalue GetValue(Tkey key)
        {
            return repository[key];        // !!!
        }
        /// <summary>
        /// Инкапсулирует хранилище для последующего рефакторинга, 
        /// например для перенесения хранилища (ключ, значение) в кучу
        /// </summary>
        /// <param name="key"></param>
        private void RemoveFromRepository(Tkey key)
        {
            repository.Remove(key);
        }
        private void RemoveLastFromInQueue()
        {
            int indexForRemove = inQueue.Count - 1;
            var keyForRemove = inQueue[indexForRemove];

            // Из репозитория значение удаляется только если нет в очереди частоты поиска.
            // Потому, что при удалении во входящей очереди, ключ в очереди частоты должен остаться.
            if (MainQueueIndexOf(keyForRemove) == -1)
                RemoveFromRepository(keyForRemove);

            inQueue.RemoveAt(indexForRemove);
        }
        public void Put(Tvalue value, Tkey key)
        {
            if (InQueueIndexOf(key) != -1)
                throw new InvalidOperationException($"[CacheLRUfreq.Put] Ключ уже есть во входной очереди. Объект <{value.ToString()}>");

            if (MainQueueIndexOf(key) != -1)
                throw new InvalidOperationException($"[CacheLRUfreq.Put] Ключ уже есть во главной очереди. Объект <{value.ToString()}>");

            repository.Add(key, value);
            InQueueAdd(key);
        }
        /// <summary>
        /// Вытеснение нижнего значения из очереди частоты поиска
        /// </summary>
        private void MainQueueDisplacement()
        {
            int dispIndex = mainQueue.Count - 1;
            Tkey dispKey = mainQueue[dispIndex];

            mainQueue.RemoveAt(dispIndex);
            InQueueAdd(dispKey);
        }
        /// <summary>
        /// Перемещает наверх. Если ключа не нашлось, то ничего не делает
        /// </summary>
        /// <param name="key"></param>
        private bool MainQueueMoveToTop(Tkey key)
        {
            int findIndex = mainQueue.IndexOf(key);
            if (findIndex == -1)
            {
                return false;          // !!!
            }

            mainQueue.RemoveAt(findIndex);
            mainQueue.Insert(0, key);

            return true;              // !!!
        }
        /// <summary>
        /// Добавляет ключ в очередь частоты поиска.
        /// </summary>
        /// <param name="key"></param>
        private void MainQueueAdd(Tkey key)
        {
            if (MainQueueMoveToTop(key))
                return;       // !!!

            // Добавить
            mainQueue.Insert(0, key);

            if (mainQueue.Count > this.CachValue)
                MainQueueDisplacement();            // Вытеснить нижнее значение 
        }
        /// <summary>
        /// Добавляет ключ в очередь частоты поиска
        /// </summary>
        /// <param name="key"></param>
        private void InQueueAdd(Tkey key)
        {
            inQueue.Insert(0, key);

            if (inQueue.Count > CachValue)
                // Последнее значение входной очереди забывается
                RemoveLastFromInQueue();
        }
        public CacheLRUfreq(int cachValue)
        {
            CachValue = cachValue;
            inQueue = new List<Tkey>();
            mainQueue = new List<Tkey>();
            repository = new Dictionary<Tkey, Tvalue>();
        }
    }
}