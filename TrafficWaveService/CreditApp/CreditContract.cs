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
        LoanApplication _ln;
        bankasiaNSEntities _db;
        public CreditContract()
        {
           
        }
        public CreditContract(CreditAppData pCr)
        {
            _db = new bankasiaNSEntities();
            _Cr = pCr;
        }

        public int CreateCreditContract()
        {
            _ln = _db.LoanApplication.FirstOrDefault(x => x.ID == _Cr.IDLoan);
            LoanCredits lc = _db.LoanCredits.FirstOrDefault(x => x.DgPozn == _Cr.IDLoanContract);
            if (lc != null)
            {
                return lc.DgPozn;
            }
            var contract = _db.LoanApplication_createLoanContract(_Cr.IDLoan, (short)_Cr.CreatorID).ToList();
            if (CreateGuarantee(_Cr.IDLoan) && _ln != null)
            {
                return (int)_Cr.IDLoan;
            }
            else
            {
                return -1;
            }
        }

        private bool CreateGuarantee(int id)
        {
            bool status = false;
            try
            {
                Guarantee gr = _db.Guarantee.FirstOrDefault(x => x.DG_POZN == id);
                if (gr == null)
                {
                    gr = new Guarantee
                    {
                        DG_POZN = id,
                        CreateDate = DateTime.Now,
                        CreatorID = _ln.CreatorID,
                        GuaranteeTypeID = 3,
                        GuaranteeClientID = _ln.ClientID,
                        GuaranteeName = _ln.BankConnection,
                        BorrowerIsHolder = true
                    };
                    _db.Guarantee.Add(gr);
                    _db.SaveChanges();
                }
                status = CreateGuaranteeProduct(gr.ID);

            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
            }
            return status;
        }

        private bool CreateGuaranteeProduct(int id)
        {
            bool status = false;
            try
            {
                Guarantee_ProductsAndEquipment gpre = _db.Guarantee_ProductsAndEquipment.FirstOrDefault(x => x.GuaranteeID == id);
                if (gpre == null)
                {
                    gpre = new Guarantee_ProductsAndEquipment
                    {
                        GuaranteeID = id,
                        Name = _ln.BankConnection,
                        DocName = "Акт передачи товара",
                        StateID = 1,
                        MakeDate = _ln.DateCreate,
                        InspectionDate = DateTime.Now,
                        Lowering = (decimal)0.7,
                        MarketValue = (decimal)_ln.Amount,
                        StatusID = 1
                    };
                    _db.Guarantee_ProductsAndEquipment.Add(gpre);
                    _db.SaveChanges();
                }
                status = CreateLoanClassification();
            }
            catch (Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
            }
            return status;
        }

        private bool CreateLoanClassification()
        {
            bool status = false;
            try
            {
                LoanClassification lc = new LoanClassification
                {
                    DgPozn = _Cr.IDLoan,
                    Date = DateTime.Now,
                    Kod = 450,
                    KodKl = _ln.ClientID
                };
                _db.LoanClassification.Add(lc);
                _db.SaveChanges();
                status = true;
            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
            }
            return status;
              
        }

        public bool CreateLoanGraph()
        {
           // _db.LoanContracts.FirstOrDefault(x=>x.);
            bool status = false;
            try
            {
                vLoanContract vLC = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan && x.DG_KODKL == _Cr.ClientID);
                LoanApplication la = _db.LoanApplication.FirstOrDefault(x => x.ID == _Cr.IDLoan);
                Setting settings = FormSettingGraph();
                status = AddLoanRowsWithSettings(settings, la, vLC);
            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
            }
            return status;
        }

        private Setting FormSettingGraph()
        {
            Setting settings = new Setting();
            try
            {
                vLoanContract vLC = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan && x.DG_KODKL == _Cr.ClientID);
                LoanApplication la = _db.LoanApplication.FirstOrDefault(x => x.ID == _Cr.IDLoan);
                //Начальный конфиг
                settings.GraphType = GraphType.Individual;
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
                settings.Percents.AddRange(_db.LoanPercent.Where(lp => lp.DgPozn == _Cr.IDLoan && lp.KodKl == _Cr.ClientID).Select(lp =>
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
            catch (Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
            }

            return settings;
        }

        private bool AddLoanRowsWithSettings(Setting settings, LoanApplication pLa, vLoanContract pVlc)
        {
            bool status = false;
            try
            {
                LoanGraph lg;
                //Расчет графика
                GraphCalculator calculator = new GraphCalculator();
                GraphResult pGr = calculator.createGraph(settings);
                //Сохранение настроек
                LoanGraphSettings lgsNew = SaveSettings(settings, pLa);
                if(lgsNew == null){ return false; }
                //Сохранение графика
                int countRow = _db.LoanGraph.Count(x => x.DgPozn == pLa.ID && x.KlKode == pLa.ClientID);
                int index = 1;
                foreach (GraphRow row in pGr.GraphRows)
                {
                    int hisNum = 1;
                    LoanGraph toUpdate = _db.LoanGraph.FirstOrDefault(x => x.DgPozn == pLa.ID && x.KlKode == pLa.ClientID && x.Date == row.Date.Date);
                    if (toUpdate != null)
                    {
                        hisNum = lgsNew.ActualGraphNum;
                    }
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
                        SaveDate = DateTime.Now,
                        HistoryNum = hisNum,
                        DgPozn = pLa.ID,
                        KlKode = pLa.ClientID,
                        OtvNum = pLa.CreatorID,
                        PayoutNumber = index
                    };
                    _db.LoanGraph.Add(lg);              
                    index++;
                }
                _db.SaveChanges();
                status = true;
            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
            }
            return status;
        }



        private LoanGraphSettings SaveSettings(Setting settings, LoanApplication pLa)
        {
            LoanGraphSettings lgs = _db.LoanGraphSettings.FirstOrDefault(x => x.DgPozn == pLa.ID && x.KlKode == pLa.ClientID);
            LoanGraphSettings lgsNew = new LoanGraphSettings
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
            try
            {
                if (lgs == null)
                {
                    _db.LoanGraphSettings.Add(lgsNew);
                }
                else
                {
                    lgsNew.ActualGraphNum = lgs.ActualGraphNum + 1;
                    _db.LoanGraphSettings.Remove(lgs);
                    _db.LoanGraphSettings.Add(lgsNew);
                }
                _db.SaveChanges();
            }
            catch { lgsNew = null; }
            return lgsNew;
        }


        public bool IssueLoan()
        {
            bool status = false;
            _ln = _db.LoanApplication.FirstOrDefault(x => x.ID == _Cr.IDLoanContract);
            vLoanContract vlC = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _ln.ID);
            LoanAccountNumber lan = _db.LoanAccountNumber.FirstOrDefault(x => x.DgPozn == _ln.ID);
            if (_ln != null)
            {
                LoanIssue loanIssue = new LoanIssue
                {
                    DgPozn = _ln.ID,
                    KodKl = _ln.ClientID,
                    Date = DateTime.Now,
                    Amount = _ln.Amount,
                    Description = $"Выдача кредита «Товар в рассрочку» № «{vlC.DG_NOM}» от «{vlC.DG_DATE}",
                    Status = 2,
                    Kodb = _ln.BranchID,
                    Kodc = vlC.kodc,
                    LC_Loan = lan.LC_Loan,
                    //LC_Delivery = lan.
                    AmountCommision = 0,
                    AmountInsurance = 0,
                    Otv = _ln.CreatorID,
                    Cash = false,
                    HisOffice = true,
                    DeliveryKodb = _ln.BranchID,
                    DeliveryKodc = vlC.kodc
                };
            }
            try
            {
                _db.SaveChanges();
            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "IssueLoan");
            }
            return status;
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
            Dictionary<string, object> dict = new Dictionary<string, object>();
            vLoanContract loanContract = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoanContract);
            if (loanContract != null)
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