using System.Threading.Tasks;

namespace TrafficWaveService.Factory.SearchFactory
{
    /// <summary>
    /// Интерфейс поиска для шаблона фабрики поиска
    /// </summary>
    public interface ISearch
    {
        /// <summary>
        /// Запуск поиска
        /// </summary>
        /// <returns></returns>
        Task<SearchQuery> Run();
    }
}