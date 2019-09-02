namespace UserCache
{
    /// <summary>
    /// Использование пользовательского кеша
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    /// <typeparam name="Tvalue"></typeparam>
    public interface ICache<Tkey, Tvalue>
    {
        /// <summary>
        /// Выполняет поиск в кеше.
        /// Если ключ нашелся учитывается частота обращения по этому ключу.
        /// Не может сразу возвращать значение по ключу, так как некоторые типы (структуры, целые типы) не могуть принимать NULL
        /// Если ключ есть, то объект по ключу получать методом GetValue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool SearchValue(Tkey key);

        /// <summary>
        /// Метод использовать только если в кеше точно есть значение этого ключа.
        /// Для ответа на вопрос есть или нет, использовать метод bool SearchValue.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Tvalue GetValue(Tkey key);
        void Put(Tvalue value, Tkey key);
    }
}
