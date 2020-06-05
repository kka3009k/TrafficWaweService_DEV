using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TrafficWaveService.CreditApp
{
    public class CreditController
    {
        /// <summary>
        /// Объект для передачи параметров запроса
        /// </summary>
        private CreditQuery _CreditQuery { get; set; }

        private CreditAppData _crApp { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public CreditController() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pClientQuery">Параметры запроса</param>
        public CreditController(CreditQuery pCreditQuery)
        {
            _CreditQuery = pCreditQuery;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Result> Run()
        {
            return await Task.Run(() => CreateCreditApp());
        }

        private Result CreateCreditApp()
        {
            Result res = new Result();
            try
            {
              
                //Десериализация строки
                string str = _CreditQuery.RequestStringCreditData.Replace("None", "null").Replace("False", "false").Replace("True", "true");
                _crApp = JsonConvert.DeserializeObject<CreditAppData>(str);
                InsertCreditApp(_crApp);
                res.Code = 200;
                res.Message = "Заявка создана";
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                new DataBase().WriteLog(ex, "Run");

            }
            return res;
        }

        /// <summary>
        /// Создание новой заявки
        /// </summary>
        /// <param name="pCr"></param>
        private void InsertCreditApp(CreditAppData pCr)
        {
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
               var creditId  =  db.LoanApplication_create(
                    (short)pCr.BranchID,
                    (short)pCr.OfficeID,
                    (short)pCr.CurrencyID,
                    pCr.ClientID,
                    (decimal)pCr.Amount,
                    (decimal)pCr.LoanPercent,
                    pCr.PeriodInMonth,
                    pCr.GroupName,
                    (decimal)pCr.FirstPaymentInPercent,
                    (byte)pCr.AttractionCanalID,
                    pCr.ProductID,
                    (byte)pCr.ClientClassID,
                    (byte)pCr.TypeClientID,
                    pCr.AuctionSourceID,
                    (byte)pCr.PurposeID,
                    (byte)pCr.PaymentSourceID,
                    pCr.GuaranteeID,
                    (byte)pCr.StatusID,
                    (short)pCr.TypeCreditID,
                    pCr.EnterpriseID,
                    (byte)pCr.LoanSubTypeID,
                    pCr.AimsID,
                    pCr.AimsComment,
                    //Дата первого контакта
                    DateTime.Now,
                    //Дата анализа
                    DateTime.Now,
                    DateTime.Now,
                    (short)pCr.PersonID,
                    (short)pCr.CreatorID,
                    pCr.BankConnection,
                    //Комментарий
                    " ",
                    //Коментарий клиента
                    " ",
                    (byte)pCr.PercentType
                    
                    );
                db.SaveChanges();

            }
        }
    }
}