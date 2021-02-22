using System.ServiceModel;
using System.Threading.Tasks;
using TrafficWaveService.Factory.SearchFactory;
using TrafficWaveService.Client;
using TrafficWaveService.Reports;
using TrafficWaveService.ClientSearch;
using TrafficWaveService.CreditApp;
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
        /// Формирование графика погашения
        /// </summary>
        /// <param name="pCreditQuery"></param>
        /// <returns></returns>
        [OperationContract]
        bool FormGraph(CreditAppData pCad);

        /// <summary>
        /// Формирование анкеты клиента
        /// </summary>
        /// <param name="IdClient"></param>
        /// <returns></returns>
        [OperationContract]
        Task<Dictionary<string, object>> GetClientProfile(int IdClient, int IdOtv);

        /// <summary>
        /// Подтверждение выдачи кредита
        /// </summary>
        /// <param name="pCreditQuery"></param>
        /// <returns></returns>
        [OperationContract]
        Task<bool> ConfirmCredit(CreditQuery pCreditQuery);

        /// <summary>
        ///  Отклонение кредита
        /// </summary>
        /// <param name="pCreditQuery">Параметры запроса</param>
        /// <returns></returns>
        [OperationContract]
        Task<bool> RejectCredit(CreditQuery pCreditQuery);


    }
}