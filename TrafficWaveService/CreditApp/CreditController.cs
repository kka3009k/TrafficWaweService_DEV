﻿using Newtonsoft.Json;
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
        public async Task<LoanResult> Run()
        {
            return await Task.Run(() => CreditApp());
        }

        private LoanResult CreditApp()
        {
            LoanResult res = new LoanResult();
            try
            {

                //Десериализация строки
                string str = _CreditQuery.RequestStringCreditData.Replace("None", "null").Replace("False", "false").Replace("True", "true");
                _crApp = JsonConvert.DeserializeObject<CreditAppData>(str);
                switch (_crApp.TypeOperation)
                {
                    case 1:
                        //Создание новой заявки
                        res.ID = InsertCreditApp(_crApp);
                        if (res.ID == -1)
                        {
                            res.Message = "Ошибка создания заявки в ОДБ";
                            res.EnumRequestStatus = EnumRequestStatus.ServiceError;
                        }
                        break;
                    case 2:
                        //Создание кредитного договора
                        res.IDString = InsertCreditContract(_crApp);
                        if (res.IDString == "-1")
                        {
                            res.Message = "Ошибка создания договора в ОДБ";
                            res.EnumRequestStatus = EnumRequestStatus.ServiceError;
                        }
                        break;
                    case 3:
                        //Формирование договора 
                        res.Base64Str = GetContractCredit(_crApp);
                        break;
                    case 4:
                        //Формирование договора о залоге 
                        res.Base64Str = GetContractPledge(_crApp);
                        break;
                    default:
                        res.ID = 200;
                        break;
                }
                res.Message = res.Base64Str == null || res.Base64Str == "" ? "Заявка создана":"Документ сформирован";
                res.EnumRequestStatus = EnumRequestStatus.Ok;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.EnumRequestStatus = EnumRequestStatus.ServiceError;
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
                try
                {
                    LoanApplication ln = db.LoanApplication.FirstOrDefault(x => x.ID == pCr.IDLoan);
                    sprotv_k otvK = db.sprotv_k.FirstOrDefault(x => x.OT_NOMOD == pCr.CreatorID);
                    if (ln != null)
                    {
                        return ln.ID;
                    }
                    if (otvK != null)
                    {
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
                            (short)otvK.OT_NOM,
                            (short)otvK.OT_NOM,
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
                catch(Exception ex)
                {
                    new DataBase().WriteLog(ex, "CreateLoanRequest");
                    return -1;
                }
                return -1;
            }
        }

        /// <summary>
        /// Создание нового договора
        /// </summary>
        /// <param name="pCr"></param>
        private string InsertCreditContract(CreditAppData pCr)
        {
            CreditContract cr = new CreditContract(pCr);
            return cr.CreateCreditContract();
        }

        /// <summary>
        /// Формирование кр. договора
        /// </summary>
        /// <param name="pCr"></param>
        /// <returns></returns>
        private string GetContractCredit(CreditAppData pCr)
        {
            CreditContract cr = new CreditContract(pCr);
            return cr.CreateCreditContractDocx();
        }
        private string GetContractPledge(CreditAppData pCr)
        {
            CreditContract cr = new CreditContract(pCr);
            return cr.CreateCreditPledgeDocx();
        }

        public async Task<bool> ConfirmCredit()
        {
            return await Task.Run(() =>
            {
                bool status = false;
                try
                {

                    //Десериализация строки
                    string str = _CreditQuery.RequestStringCreditData.Replace("None", "null").Replace("False", "false").Replace("True", "true");
                    _crApp = JsonConvert.DeserializeObject<CreditAppData>(str);
                    CreditContract cr = new CreditContract(_crApp);
                    status = cr.IssueLoan();
                }
                catch (Exception ex)
                {
                    new DataBase().WriteLog(ex, "Run");

                }
                return status;
            });
        }

        public async Task<bool> RejectCredit()
        {
            return await Task.Run(() =>
            {
                bool status = false;
                try
                {

                    //Десериализация строки
                    string str = _CreditQuery.RequestStringCreditData.Replace("None", "null").Replace("False", "false").Replace("True", "true");
                    _crApp = JsonConvert.DeserializeObject<CreditAppData>(str);
                    CreditContract cr = new CreditContract(_crApp);
                    status = cr.RejectLoanApp();
                }
                catch (Exception ex)
                {
                    new DataBase().WriteLog(ex, "Run");

                }
                return status;
            });
        }
    }
}