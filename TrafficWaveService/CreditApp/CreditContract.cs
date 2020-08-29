using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using TrafficWaveService.Reports;
using WordToPDF;
using System.Net;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Settings;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;
using TrafficWaveService.CreditApp.CreditGraphCalculator;
namespace TrafficWaveService.CreditApp
{
    public class CreditContract
    {
        CreditAppData _Cr;
        bankasiaNSEntities _db;
        public CreditContract()
        {
           
        }
        public CreditContract(CreditAppData pCr)
        {
            _db = new bankasiaNSEntities();
            _Cr = pCr;
        }

        public int CreateCreditContract(CreditAppData pCr)
        {
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                LoanCredits lc = db.LoanCredits.FirstOrDefault(x => x.DgPozn == pCr.IDLoanContract);
                if(lc != null)
                {
                    return lc.DgPozn;
                }
                var contract = db.LoanApplication_createLoanContract(pCr.IDLoan, (short)pCr.CreatorID).ToList();
                if (CreateGuarantee(pCr, db, pCr.IDLoan))
                {
                    return (int)pCr.IDLoan;
                }
                else
                {
                    return -1;
                }
            }
        }

        private bool CreateGuarantee(CreditAppData pCr, bankasiaNSEntities db, int id)
        {
            bool status = false;
            try
            {
                Guarantee gr = new Guarantee
                {
                    DG_POZN = id,
                    CreateDate = DateTime.Now,
                    CreatorID = pCr.CreatorID,
                    GuaranteeTypeID = 3,
                    GuaranteeClientID = pCr.ClientID,
                    GuaranteeName = pCr.BankConnection,
                    BorrowerIsHolder = true
                };
                db.Guarantee.Add(gr);
                db.SaveChanges();
                status = CreateGuaranteeProduct(pCr,db,gr.ID);

            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
            }
            return status;
        }

        private bool CreateGuaranteeProduct(CreditAppData pCr, bankasiaNSEntities db, int id)
        {
            bool status = false;
            try
            {
                Guarantee_ProductsAndEquipment gpre = new Guarantee_ProductsAndEquipment
                {
                    GuaranteeID = id,
                    Name = pCr.BankConnection,
                    DocName = "Акт передачи товара",
                    StateID = 1,
                    MakeDate = pCr.DateCreate,
                    InspectionDate = DateTime.Now,
                    Lowering = (decimal)0.7,
                    MarketValue = (decimal)pCr.Amount,
                    StatusID = 1
                };
                db.Guarantee_ProductsAndEquipment.Add(gpre);
                db.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
            }
            return status;
        }

        public bool CreateLoanGraph()
        {
           // _db.LoanContracts.FirstOrDefault(x=>x.);
            bool status = false;
            vLoanContract vLC = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan && x.DG_KODKL == _Cr.ClientID);
            LoanApplication la = _db.LoanApplication.FirstOrDefault(x => x.ID == _Cr.IDLoan);
            Setting settings = FormSettingGraph(_Cr);
            GraphCalculator calculator = new GraphCalculator();
            AddLoanRows(calculator.createGraph(settings), settings, la, vLC);
            return status;
        }

        private Setting FormSettingGraph(CreditAppData pCr)
        {

           
            Setting settings = new Setting();
            try
            {
                vLoanContract vLC = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == pCr.IDLoan && x.DG_KODKL == pCr.ClientID);
                LoanApplication la = _db.LoanApplication.FirstOrDefault(x => x.ID == pCr.IDLoan);
                //Начальный конфиг
                settings.GraphType = GraphType.Annuity;
                settings.RepayType = RepayType.OneTimeInMonth;
                settings.IssueType = IssueType.OneSum;
                settings.Shema = Shema.SH_360_30;
                settings.Round = 2;
                settings.Sum = la.Amount;
                settings.MounthCount = la.PeriodInMonth;
                settings.IssueDate = la.DateCreate != null ? la.DateCreate : DateTime.Now;
                settings.FirstRepayDate = la.DateCreate != null ? la.DateCreate.AddMonths(1) : DateTime.Now.AddMonths(1);
                settings.IsEmployee = false;
                //Не рабочие дни
                DateTime endDate = settings.IssueDate.AddMonths(settings.MounthCount);
                settings.NotWorkDays.Clear();
                settings.NotWorkDays.AddRange(_db.kalendar.Where(k => k.K_DATE >= settings.IssueDate &&
                k.K_DATE <= endDate && k.K_PRIZ == 1).Select(x => x.K_DATE));
                //Проценты
                settings.Percents.Clear();
                settings.Percents.AddRange(_db.LoanPercent.Where(lp => lp.DgPozn == pCr.IDLoan && lp.KodKl == pCr.ClientID).Select(lp =>
                    new Percent()
                    {
                        DateStart = lp.DateStart,
                        DateEnd = lp.DateEnd,
                        Amount = lp.Amount,
                        FinePercents = lp.FinePercent,
                        Percents = lp.Percents
                    }
                ));
                //Прочий конфиг
                settings.SumFixedPay = 0;
                settings.FixedPaySince = 0;
                settings.FixedPayBefore = 0;
                settings.PrivelegyMainSum = 0;
                settings.PrivelegyPersent = 0;
            }
            catch(Exception ex)
            {

            }

            return settings;
        }

        private void AddLoanRows(GraphResult pGr, Setting settings, LoanApplication pLa, vLoanContract pVlc)
        {
            try
            {
                LoanGraph lg;
                LoanGraphSettings lgs = new LoanGraphSettings
                {
                    DgPozn = pLa.ID,
                    KlKode = pLa.ClientID,
                    DateStart = settings.IssueDate,
                    PrivelegPeriodMS = settings.PrivelegyMainSum,
                    FiveDayPrincip = false,
                    RepayType = (int)settings.RepayType,
                    PrivelegPeriodPercent = settings.PrivelegyPersent,
                    GraphType = (int)settings.GraphType,
                    IssueCondition = false,
                    OtvNum = pLa.CreatorID,
                    ActualGraphNum = 1,
                    MonthCount = settings.MounthCount
                };
                _db.LoanGraphSettings.Add(lgs);
                _db.SaveChanges();
                foreach (GraphRow row in pGr.GraphRows)
                {
                    lg = new LoanGraph
                    {
                        Date = row.Date,
                        Issue = row.IssueSum,
                        RemainderMS = row.RemainderMainSum,
                        RepaymentMSPer = row.RepaymentMSPercent,
                        RepaymentPercent = row.RepaymentPercent,
                        RepaymentMS = row.RepaymentMainSum,
                        RepaymentNsp = row.RepaymentNsp,
                        TotalPayment = row.TotalPayment,
                        SaveDate = row.SaveDate,
                        HistoryNum = 1,
                        DgPozn = pLa.ID,
                        KlKode = pLa.ClientID,
                        OtvNum = pLa.CreatorID
                    };
                    _db.LoanGraph.Add(lg);
                }
                _db.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }


        

        /// <summary>
        /// Формирование шаблона в формате docx
        /// </summary>
        /// <returns></returns>
        public string CreateCreditContractDocx()
        {
            Dictionary<string, object> dict = CreateContractMeta();
            ReportServiceRef.ReportServiceClient client = new ReportServiceRef.ReportServiceClient();

            /*client.ClientCredentials.UserName.UserName = "60k.kargin";
            client.ClientCredentials.UserName.Password = "W0Y0b8FPAhSAtZSiWIhugw==PAcw5B3SToLcFg";*/
            client.ClientCredentials.UserName.UserName = "60s.korostelev";
            client.ClientCredentials.UserName.Password = "L8PY5ID5bsVS9k0PV5S3Kg==FCGgPat3KVIsbh";
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl)=>true);
            ReportServiceRef.XLSDownload xls = client.print_DOG_KREDIT(dict, 0);
            byte [] bytes = xls.XLSFile;
            string path = HostingEnvironment.MapPath("~/CreditApp/") + xls.XLSName;
            return Convert.ToBase64String(bytes);
        }

        private Dictionary<string, object> CreateContractMeta()
        {
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                vLoanContract loanContract = db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoanContract);
                if(loanContract != null)
                {
                    dict.Add("<DG_NOM>", loanContract.DG_NOM);
                    dict.Add("<NAIMGRUP>", loanContract.GroupName);
                    dict.Add("<RV_NPUKT>", "PUNCT");
                    dict.Add("<OT_FIO>", "Каргин Константин");
                    dict.Add("<RV_PODRAZD>", "PODRAZD");
                    dict.Add("<DG_POZN>", loanContract.DG_POZN);
                    //dict.Add("<RV_REKVIZIT>", dicRekvizitsData[eTypeRekvizitData.rv_rekvizit]);
                    //dict.Add("<RV_DOLJN_FIO_RP>", dicRekvizitsData[eTypeRekvizitData.rv_doljn_fio_rp]);
                    //dict.Add("<RV_DOK>", dicRekvizitsData[eTypeRekvizitData.rv_doc]);
                    //dict.Add("<RV_FIO>", dicRekvizitsData[eTypeRekvizitData.rv_fio]);
                    //dict.Add("<RV_DOLJN>", dicRekvizitsData[eTypeRekvizitData.rv_doljn]);
                    dict.Add("LANGUAGE", "RU");
                    //if (vGuarantee != null)
                    //    dicTemplateData.Add("GUARANTEE_ID", vGuarantee.ID);
                    dict.Add("AGREEMENT_TYPE", 110);
                    dict.Add("IS_TYPE_INSURANCE", 0);
                    dict.Add("IS_TYPE_GKS", 0);
                }
                return dict;
            }

        }

        private void FileWriteStream(byte[] pBytes, string pFileName)
        {
            using (FileStream fs = File.Create(pFileName))
            {
                fs.Write(pBytes, 0, pBytes.Length);
                fs.Close();
            }
        }
    }
}