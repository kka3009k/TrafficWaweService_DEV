using System;
using System.Threading.Tasks;
using TrafficWaveService.Factory.SearchFactory;
using TrafficWaveService.Client;
using TrafficWaveService.Dictionaries;
using TrafficWaveService.Reports;
using TrafficWaveService.ClientSearch;
using TrafficWaveService.CreditApp;
using System.ServiceModel.Web;
using System.Net;
using System.Collections.Generic;

namespace TrafficWaveService
{
    /// <summary>
    /// Веб сервис кредитного конвеера TrafficWave
    /// </summary>
    public class TrafficWave : ITrafficWave
    {
        WebHeaderCollection _headers;
        public TrafficWave()
        {
           // Connection_State();
        }

        private void Connection_State()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            _headers = request.Headers;
            string head = _headers["pLogin"];
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                db.SetContextInfo(_headers["pLogin"], _headers["pIpAddress"]);
            }
            CheckUser();
        }

        private void CheckUser()
        {
            Auth auth = new Auth(_headers);
            if (auth.CheckUser())
            {
                throw new WebFaultException(HttpStatusCode.Forbidden);
            }
        }

        /// <summary>
        /// Метод поиска
        /// </summary>
        /// <param name="pSearchQuery">Параметры запроса</param>
        /// <param name="pSearchType">Тип объекта</param>
        /// <returns></returns>
        public async Task<SearchQuery> Search(SearchQuery pSearchQuery, SearchType pSearchType)
        {
            pSearchQuery.QueryId = 20;
            pSearchQuery.RequestId = 20;
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
        /// Метод для работы с клиентами
        /// </summary>
        /// <param name="pClientQuery">Параметры запроса</param>
        /// <param name="pAddType">Тип объекта</param>
        /// <returns></returns>
        public async Task<Result> AddClient(ClientQuery pClientQuery)
        {
            ClientMain client = new ClientMain(pClientQuery);
            return await client.Run();
        }

        /// <summary>
        /// Метод для работы c шаблоном документа
        /// </summary>
        /// <param name="pTemplateQuery">Параметры запроса</param>
        /// <returns></returns>
        public async Task<ResultBase64> GetTemplatePdf(TemplateQuery pTemplateQuery)
        {
            TemlateMain client = new TemlateMain(pTemplateQuery);  
            return await client.Run();
        }

        /// <summary>
        /// Метод для поиска клиента по ИНН
        /// </summary>
        /// <param name="pTemplateQuery">Параметры запроса</param>
        /// <returns></returns>
        public async Task<ResultData> SearchClient(SearchClientQuery pTemplateQuery)
        {
            SeachMain search= new SeachMain(pTemplateQuery);
            return await search.Run();
        }

        /// <summary>
        /// Создание кредитной заявки
        /// </summary>
        /// <param name="pCreditQuery">Параметры запроса</param>
        /// <returns></returns>
        public async Task<LoanResult> CreateLoanApp(CreditQuery pCreditQuery)
        {
            CreditController credit = new CreditController(pCreditQuery);
            return await credit.Run();
        }

        /// <summary>
        /// Формирование графика погашения
        /// </summary>
        /// <param name="pSprQuery"></param>
        /// <returns></returns>
        public bool FormGraph(CreditAppData pCad)
        {
            CreditContract cc = new CreditContract(pCad);
            cc.CreateLoanGraph();
            return true;
        }
        
        /// <summary>
        /// Формирование анкеты клиента
        /// </summary>
        /// <param name="IdClient"></param>
        /// <returns></returns>
        public async Task<Dictionary<string,object>> GetClientProfile(int IdClient, int IdOtv)
        {
            ClientReport cr = new ClientReport(IdClient, IdOtv);
            return await cr.GetClientProfile();
        }

        /// <summary>
        /// Подтверждение выдачи кредита
        /// </summary>
        /// <param name="pCreditQuery">Параметры запроса</param>
        /// <returns></returns>
        public async Task<bool> ConfirmCredit(CreditQuery pCreditQuery)
        {
            CreditController credit = new CreditController(pCreditQuery);
            return await credit.ConfirmCredit();
        }

        /// <summary>
        ///  Отклонение кредита
        /// </summary>
        /// <param name="pCreditQuery">Параметры запроса</param>
        /// <returns></returns>
        public async Task<bool> RejectCredit(CreditQuery pCreditQuery)
        {
            CreditController credit = new CreditController(pCreditQuery);
            return await credit.RejectCredit();
        }

    }
}