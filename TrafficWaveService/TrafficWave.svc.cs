using System;
using System.Threading.Tasks;
using TrafficWaveService.Factory.SearchFactory;
using TrafficWaveService.Client;
using TrafficWaveService.Dictionaries;
namespace TrafficWaveService
{
    /// <summary>
    /// Веб сервис кредитного конвеера TrafficWave
    /// </summary>
    public class TrafficWave : ITrafficWave
    {
        /// <summary>
        /// Метод поиска
        /// </summary>
        /// <param name="pSearchQuery">Параметры запроса</param>
        /// <param name="pSearchType">Тип объекта</param>
        /// <returns></returns>
        public async Task<SearchQuery> Search(SearchQuery pSearchQuery, SearchType pSearchType)
        {
            SearchFactory searchFactory = new SearchFactory(pSearchQuery);
            return await searchFactory.Search(pSearchType).Run();
        }

        /// <summary>
        /// Проверка статус клиента принят или не ринят
        /// </summary>
        /// <param name="pQueryId">Id запроса</param>
        /// <returns></returns>
        public async Task<Result> CheckCriminalStatus(long pQueryId)
        {
            return await new SearchCriminals().Check(pQueryId);
        }


        /// <summary>
        /// Метод поиска
        /// </summary>
        /// <param name="pClientQuery">Параметры запроса</param>
        /// <param name="pAddType">Тип объекта</param>
        /// <returns></returns>
        public async Task<Result> AddClient(ClientQuery pClientQuery)
        {
            ClientMain client = new ClientMain(pClientQuery);
            RuToLatin trans =new RuToLatin();
            string res = trans.Front("Султан Джунушалиев Аскенович");
            return await client.Run();
        }


    }
}