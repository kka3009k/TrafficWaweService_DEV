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
            return await Task.Run(() => CreditApp());
        }

        private Result CreditApp()
        {
            Result res = new Result();
            try
            {

                //Десериализация строки
                string str = _CreditQuery.RequestStringCreditData.Replace("None", "null").Replace("False", "false").Replace("True", "true");
                _crApp = JsonConvert.DeserializeObject<CreditAppData>(str);
                switch (_crApp.TypeOperation)
                {
                    case 1:
                        //Создание новой заявки
                        res.Code = InsertCreditApp(_crApp);
                        break;
                    case 2:
                        //Создание кредитнго договора
                        res.Code = InsertCreditContract(_crApp);
                        break;
                    case 3:
                        //Формирование договора в формате PDF
                        res.Message = GetContractCredit(_crApp);
                        break;
                    default:
                        res.Code = 200;
                        break;
                }
                res.Message = res.Message == null || res.Message == "" ? "Заявка создана" : res.Message;
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
        private int InsertCreditApp(CreditAppData pCr)
        {
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                LoanApplication ln = db.LoanApplication.FirstOrDefault(x => x.ID == pCr.IDLoan);
                if (ln != null)
                {
                    return ln.ID;
                }
                var creditId = db.LoanApplication_create(
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

                    ).ToList();
                db.SaveChanges();
                return creditId.First().Value;
            }
        }

        /// <summary>
        /// Создание нового договора
        /// </summary>
        /// <param name="pCr"></param>
        private int InsertCreditContract(CreditAppData pCr)
        {
            CreditContract cr = new CreditContract();
            return cr.CreateCreditContract(pCr);
        }

        /// <summary>
        /// Формирование кр. договора
        /// </summary>
        /// <param name="pCr"></param>
        /// <returns></returns>
        private string GetContractCredit(CreditAppData pCr)
        {
            CreditContract cr = new CreditContract(pCr);
            return cr.CreateCreditContractPdf();
        }
    }
}