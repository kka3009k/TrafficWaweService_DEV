using System.ServiceModel;
using System.Threading.Tasks;
using TrafficWaveService.Factory.SearchFactory;
using TrafficWaveService.Client;
using TrafficWaveService.Reports;
using TrafficWaveService.ClientSearch;
using TrafficWaveService.CreditApp;
using TrafficWaveService.Sprs;
using System.Collections.Generic;

namespace TrafficWaveService
{
    [ServiceContract]
    public interface ITrafficWave
    {
        [OperationContract]
        Task<SearchQuery> Search(SearchQuery pSearchQuery, SearchType pSearchType);

        [OperationContract]
        Task<ResultBase64> GetTemplatePdf(TemplateQuery pTemlateQuery);

        [OperationContract]
        Task<Result> AddClient(ClientQuery pClientQuery);


        [OperationContract]
        Task<Result> CheckCriminalStatus(long pQueryId);

        [OperationContract]
        Task<ResultData> SearchClient(SearchClientQuery pTemplateQuery);

        /// <summary>
        /// Создание кредитной заявки
        /// </summary>
        /// <param name="pCreditQuery"></param>
        /// <returns></returns>
        [OperationContract]
        Task<LoanResult> CreateLoanApp(CreditQuery pCreditQuery);

        /// <summary>
        /// Загрузка справочника
        /// </summary>
        /// <param name="pCreditQuery"></param>
        /// <returns></returns>
        [OperationContract]
        Task<string> GetSprs(SprQuery pSprQuery);

    }
}