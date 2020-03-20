using System;

namespace TrafficWaveService.Factory.SearchFactory
{
    /// <summary>
    /// Класс шаблон фабрика поиска
    /// </summary>
    public class SearchFactory
    {
        /// <summary>
        /// Параметры запроса
        /// </summary>
        private SearchQuery _SearchQuery { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pSearchQuery">Параметры запроса</param>
        public SearchFactory(SearchQuery pSearchQuery)
        {
            _SearchQuery = pSearchQuery;
        }

        /// <summary>
        /// Запуск поиска
        /// </summary>
        /// <param name="pSearchType">Тип поиска объекта</param>
        /// <returns></returns>
        public ISearch Search(SearchType pSearchType)
        {
            if (pSearchType == SearchType.CRIMINALS)
                return new SearchCriminals(_SearchQuery);

            throw new NotImplementedException("Class Not Found");
        }
    }

    /// <summary>
    /// Типы классов поиска
    /// </summary>
    public enum SearchType
    {
        CRIMINALS
    }
}