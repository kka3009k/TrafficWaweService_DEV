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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TrafficWaveService.Reports.TempController;
using TrafficWaveService.Reports.Utils;
using TrafficWaveService.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

namespace TrafficWaveService.CreditApp
{
    public class CreditContract
    {
        CreditAppData _Cr;
        LoanApplication _ln;
        Dictionary<string, object> _dict;
        bankasiaNSEntities _db;      
        public CreditContract()
        {
           
        }
        public CreditContract(CreditAppData pCr)
        {
            _db = new bankasiaNSEntities();
            _Cr = pCr;
        }
        public CreditContract(CreditAppData pCr, Dictionary<string, object> pDict)
        {
            _db = new bankasiaNSEntities();
            _Cr = pCr;
            _dict = pDict;
        }

        #region CreateCreditContract
        public string CreateCreditContract()
        {
            _ln = _db.LoanApplication.FirstOrDefault(x => x.ID == _Cr.IDLoan);
            string id = "-1";
            vLoanContract lc = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan);
            if (lc != null)
            {
                id = lc.DG_NOM;
            }
            else
            {
                var contract = _db.LoanApplication_createLoanContract(_Cr.IDLoan, (short)_Cr.CreatorID).ToList();
                id = contract.First().MQ_RESULT.Value.ToString();
            }
            if (CreateGuarantee(_Cr.IDLoan) && _ln != null && id != "-1")
            {
                lc = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan);
                return lc.DG_NOM;
            }
            else
            {
                return "-1";
            }
        }

        private bool CreateGuarantee(int id)
        {
            bool status = false;
            try
            {
                List<Guarantee> grList = _db.Guarantee.Where(x => x.DG_POZN == id).ToList();
                if (grList.Count == 0)
                {
                    List<Reports.Data.Product> products = JsonConvert.DeserializeObject<List<Reports.Data.Product>>(_Cr.Products);
                    //clients client = _db.clients.FirstOrDefault(x => x.kl_kod == _ln.ClientID);
                    foreach (var x in products)
                    {
                        Guarantee gr = new Guarantee
                        {
                            DG_POZN = id,
                            CreateDate = DateTime.Now,
                            CreatorID = _ln.CreatorID,
                            GuaranteeTypeID = 3,
                            GuaranteeClientID = _ln.ClientID,
                            GuaranteeName = $"{x.name_product}, {x.description_product}",
                            BorrowerIsHolder = true,
                            GuaranteeOwnerType = 1,
                            
                        };
                        _db.Guarantee.Add(gr);
                        _db.SaveChanges();
                        _db.Guarantee_ProductsAndEquipment.Add(new Guarantee_ProductsAndEquipment
                        {
                            GuaranteeID = gr.ID,
                            Name = x.name_product,
                            DocName = "Акт передачи товара",
                            StateID = 1,
                            MakeDate = DateTime.Parse($"{x.date_production.Trim()}-{DateTime.Now.Month}-{DateTime.Now.Day}"),
                            InspectionDate = DateTime.Now,
                            Lowering = (decimal)70,
                            AssessedValue = (decimal.Parse(x.price_product) / 100) * 70,
                            MarketValue = decimal.Parse(x.price_product),
                            StatusID = 1,
                            Description = x.description_product
                        });
                        _db.SaveChanges();
                    }
                }
                status = CreateLoanClassification();
            }
            catch(Exception ex)
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
                LoanClassification lc = _db.LoanClassification.FirstOrDefault(x => x.DgPozn == _Cr.IDLoan);
                if (lc == null)
                {
                    lc = new LoanClassification
                    {
                        DgPozn = _Cr.IDLoan,
                        Date = DateTime.Now,
                        Kod = 410,
                        KodKl = _ln.ClientID
                    };
                    _db.LoanClassification.Add(lc);
                    _db.SaveChanges();
                }
                status = CreateLoanIncomeKodv();
            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
            }
            return status;
              
        }

        private bool CreateLoanIncomeKodv()
        {
            bool status = false;
            try
            {
                LoanIncomeKodv lk = _db.LoanIncomeKodv.FirstOrDefault(x => x.DgPozn == _Cr.IDLoan);
                if (lk == null)
                {
                    lk = new LoanIncomeKodv
                    {
                        DgPozn = _Cr.IDLoan,
                        Date = DateTime.Now,
                        Percent1 = (decimal)1,
                        Kodv1 = 417,
                        KodKl = _ln.ClientID
                    };
                    _db.LoanIncomeKodv.Add(lk);
                    _db.SaveChanges();
                }
                status = CreateLoanGraph();
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
            try
            {
                LoanApplication la = _db.LoanApplication.FirstOrDefault(x => x.ID == _Cr.IDLoan);
                vLoanContract vLC = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan && x.DG_KODKL == _ln.ClientID);
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
                vLoanContract vLC = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan && x.DG_KODKL == _ln.ClientID);
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
                settings.FirstRepayDate = _Cr.FirstRepayDate != null ? DateTime.Parse(_Cr.FirstRepayDate) : DateTime.Now.AddMonths(1);
                settings.IsEmployee = false;
                
                //Не рабочие дни
                DateTime endDate = settings.IssueDate.AddMonths(settings.MounthCount);
                settings.NotWorkDays.Clear();
                settings.NotWorkDays.AddRange(_db.kalendar.Where(k => k.K_DATE >= settings.IssueDate &&
                k.K_DATE <= endDate && k.K_PRIZ == 1).Select(x => x.K_DATE));
                //Проценты
                settings.Percents.Clear();
                settings.Percents.AddRange(_db.LoanPercent.Where(lp => lp.DgPozn == _Cr.IDLoan && lp.KodKl == la.ClientID).Select(lp =>
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
                if (countRow < 1)
                {
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
                }
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
                DateStart = settings.FirstRepayDate,
                PrivelegPeriodMS = settings.PrivelegyMainSum,
                FiveDayPrincip = false,
                RepayType = (int)settings.RepayType,
                PrivelegPeriodPercent = settings.PrivelegyPersent,
                GraphType = (int)settings.GraphType,
                IssueCondition = false,
                OtvNum = pLa.CreatorID,
                ActualGraphNum = 1,
                MonthCount = settings.MounthCount,
                ActualGraphDate = DateTime.Now,
                SaveDate = DateTime.Now
            };
            try
            {
                if (lgs == null)
                {
                    _db.LoanGraphSettings.Add(lgsNew);
                    _db.SaveChanges();
                }
                else
                {
                    return lgs;
                    /*lgsNew.ActualGraphNum = 1;//lgs.ActualGraphNum + 1;
                    _db.LoanGraphSettings.Remove(lgs);
                    _db.LoanGraphSettings.Add(lgsNew);*/
                }
            }
            catch { lgsNew = null; }
            return lgsNew;
        }
        #endregion

        #region Confirm credit
        public bool IssueLoan()
        {
            bool status = false;
            try
            {
                _ln = _db.LoanApplication.FirstOrDefault(x => x.ID == _Cr.IDLoan);
                vLoanContract vlC = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _ln.ID);
                LoanAccountNumber lan = _db.LoanAccountNumber.FirstOrDefault(x => x.DgPozn == _ln.ID);
                LoanCredits lc = _db.LoanCredits.FirstOrDefault(x => x.DgPozn == _Cr.IDLoan);
                if (_ln != null)
                {
                    if(_db.LoanIssue.FirstOrDefault(x=>x.DgPozn == _Cr.IDLoan) != null)
                    {
                        return false;
                    }
                    lc.CheckCredits = 2;
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
                    _db.LoanIssue.Add(loanIssue);
                    _db.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                new DataBase().WriteLog(ex, "IssueLoan");
            }
            return status;
        }
        #endregion

        #region CreateCreditContractDocx
        /// <summary>
        /// Формирование шаблона в формате docx
        /// </summary>
        /// <returns></returns>
        public string CreateCreditContractDocx()
        {
            Dictionary<string, object> dict = CreateContractMeta();
            ReportServiceRef.ReportServiceClient client = new ReportServiceRef.ReportServiceClient();
            client.ClientCredentials.UserName.UserName = AuthData.UserNameService;
            client.ClientCredentials.UserName.Password = AuthData.PasswordService;
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl)=>true);
            ReportServiceRef.XLSDownload xls = client.print_DOG_KREDIT(dict, 0);
            byte [] bytes = xls.XLSFile;
            string path = HostingEnvironment.MapPath("~/CreditApp/") + xls.XLSName;
            return Convert.ToBase64String(bytes);
        }

        private Dictionary<string, object> CreateContractMeta()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            vLoanContract loanContract = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan);
            sprotv otv = _db.sprotv.FirstOrDefault(x => x.OT_NOM == loanContract.DG_OTV);
            rekvizit rek = _db.rekvizit.FirstOrDefault(x => x.KODB == loanContract.kodb && x.KODC == loanContract.kodc);
            if (loanContract != null)
            {
                dict.Add("<DG_NOM>", loanContract.DG_NOM);
                dict.Add("<NAIMGRUP>", loanContract.GroupName);
                dict.Add("<OT_FIO>", otv.OT_FIO);
                dict.Add("<RV_PODRAZD>", rek.rv_podrazd);
                dict.Add("<DG_POZN>", loanContract.DG_POZN);
                dict.Add("<RV_REKVIZIT>", rek.rv_rekvizit);
                dict.Add("<RV_NPUKT>", rek.rv_npunkt);
                dict.Add("<RV_DOLJN_FIO_RP>", rek.rv_doljn_fio_rp);
                dict.Add("<RV_DOK>", rek.rv_dok);
                dict.Add("<RV_FIO>", rek.rv_fio);
                dict.Add("<RV_DOLJN>", rek.rv_doljn);
                dict.Add("LANGUAGE", "RU");    
                dict.Add("AGREEMENT_TYPE", 110);
                dict.Add("IS_TYPE_INSURANCE", 0);
                dict.Add("IS_TYPE_GKS", 0);
            }
            return dict;
        }
        #endregion

        #region CreateCreditPledgeDocx
        /// <summary>
        /// Формирование шаблона в формате docx
        /// </summary>
        /// <returns></returns>
        public string CreateCreditPledgeDocx()
        {
            Dictionary<string, object> dict = CreateContractPledgeMeta();
            MainController mainController = new MainController();
            Guarantee gr = _db.Guarantee.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan);
            return mainController.createReport(dict, CreateListGuarantee(dict), false);
           
        }

        private Dictionary<string, object> CreateContractPledgeMeta()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            vLoanContract loanContract = _db.vLoanContract.FirstOrDefault(x => x.DG_POZN == _Cr.IDLoan);
            clients client = _db.clients.FirstOrDefault(x => x.kl_kod == loanContract.DG_KODKL);
            sprotv otv = _db.sprotv.FirstOrDefault(x => x.OT_NOM == loanContract.DG_OTV);
            rekvizit rek = _db.rekvizit.FirstOrDefault(x => x.KODB == loanContract.kodb && x.KODC == loanContract.kodc);
            dict.Add("<DG_NOM>", loanContract.DG_NOM);
            dict.Add("<NAIMGRUP>", loanContract.GroupName);
            dict.Add("<RV_NPUKT>", rek.rv_npunkt);
            dict.Add("<RV_PODRAZD>", rek.rv_podrazd);
            dict.Add("<RV_REKVIZIT>", rek.rv_rekvizit);
            dict.Add("<OT_FIO>", otv.OT_FIO);
            dict.Add("<OT_DOLJN>", "Торговый агент");
            dict.Add("<DG_DATE>", DateTime.Now.ToString().Remove(10));
            dict.Add("<RV_DOK>", DateTime.Now);
            dict.Add("<DG_POZN>", loanContract.DG_POZN);
            dict.Add("<DG_SUM>", loanContract.DG_SUM);
            dict.Add("<DG_KODV>", "сом");
            dict.Add("<PROCENT>", 0.0);
            dict.Add("<RV_FIO>", client.kl_nam);
            dict.Add("<DG_SUM_RECIPE>", $"{loanContract.DG_SUM} {Util.toRecipe(loanContract.DG_SUM, 0)}");
            dict.Add("TEMPLATE_FILE_NAME", "credit_contarct_pledge.docx");
            IQueryable<SharedProperty> qSharProp = _db.SharedProperty.Where(shp => shp.Key.Contains("kr800PAT_Data"));
            List<SharedProperty> sharedProperties = qSharProp != null ? qSharProp.ToList() : new List<SharedProperty>();
            Dictionary<string, string> dataCl = GetClientData(client);
            dict["<Данные 1>"] = CreateReplaceString(sharedProperties, "kr800PAT_Data1_2",
                                       "<B_NAM>", client.kl_nam,
                                       "<B_PASP>", dataCl["passData"]);

            dict["<Данные 2>"] = CreateReplaceString(sharedProperties, "kr800PAT_Data2_2",
                "<B_NAM>", client.kl_nam,
                "<B_PASP>", dataCl["passData"],
                "<B_ADR_J>", dataCl["addressU"],
                "<B_ADR_F>", dataCl["addressF"]);

            dict["<Данные 3>"] = " ";
            return dict;
        }


        private Dictionary<string,string> GetClientData(clients pCl)
        {
            Dictionary<string, string> dataCl = new Dictionary<string, string>();
            string klientAdrJ = "";
            string klientAdrF = "";
            //Адреса клиента
            if (pCl != null)
            {
                List<client_adress> clAddresses = _db.client_adress.Where(gha => gha.kl_kod == pCl.kl_kod).ToList();
                client_adress clAddressU = clAddresses.Where(gha => gha.a_typeadress == "U").FirstOrDefault();
                client_adress clAddressF = clAddresses.Where(gha => gha.a_typeadress == "F").FirstOrDefault();

                klientAdrF = clAddressF != null
                    ? (clAddressF.a_obl != ""
                          ? clAddressF.a_obl + (clAddressF.a_obl.Contains("обл") ? ", " : " обл., ")
                          : "")
                      + (clAddressF.a_raion != "" ? clAddressF.a_raion + ", " : "")
                      + clAddressF.a_np + ", "
                      + (clAddressF.a_topfx != "" ? clAddressF.a_topfx + " " : "")
                      + (clAddressF.a_to != "" ? clAddressF.a_to + ", " : "")
                      + clAddressF.a_ulpfx
                      + clAddressF.a_ul + ", "
                      + (clAddressF.a_dom != "" ? "д." + clAddressF.a_dom : "")
                      + (clAddressF.a_korpuc != "" ? ", корпус " + clAddressF.a_korpuc : "")
                      + (clAddressF.a_ctroen != "" ? ", строение " + clAddressF.a_ctroen : "")
                      + (clAddressF.a_kvart != "" ? ", кв." + clAddressF.a_kvart : "")
                    : "";
                klientAdrJ = clAddressU != null
                    ? (clAddressU.a_obl != ""
                          ? clAddressU.a_obl + (clAddressU.a_obl.Contains("обл") ? ", " : " обл., ")
                          : "")
                      + (clAddressU.a_raion != "" ? clAddressU.a_raion + ", " : "")
                      + clAddressU.a_np + ", "
                      + (clAddressU.a_topfx != "" ? clAddressU.a_topfx + " " : "")
                      + (clAddressU.a_to != "" ? clAddressU.a_to + ", " : "")
                      + clAddressU.a_ulpfx
                      + clAddressU.a_ul + ", "
                      + (clAddressU.a_dom != "" ? "д." + clAddressU.a_dom : "")
                      + (clAddressU.a_korpuc != "" ? ", корпус " + clAddressU.a_korpuc : "")
                      + (clAddressU.a_ctroen != "" ? ", строение " + clAddressU.a_ctroen : "")
                      + (clAddressU.a_kvart != "" ? ", кв." + clAddressU.a_kvart : "")
                    : "";
            }
            dataCl["addressU"] = klientAdrJ;
            dataCl["addressF"] = klientAdrF;
            //Паспортные данные
            client_paspdata client_pass = _db.client_paspdata.FirstOrDefault(f => f.kl_kod == pCl.kl_kod);
            string clientPassData = " ";
            if (client_pass != null)
            {
                if (0 == 0)
                    clientPassData = client_pass.p_viddok + ": " + client_pass.p_srdok + client_pass.p_ndok + ", выдан " + client_pass.p_mvd
                                     + " от " + parseDateTime("dd.MM.yyyy", client_pass.p_datev);
                else
                    clientPassData = client_pass.p_viddok + ": " + client_pass.p_srdok + client_pass.p_ndok + ", документти берген мекеме " + client_pass.p_mvd
                                     + " берилген күнү " + parseDateTime("dd.MM.yyyy", client_pass.p_datev);
            }
            dataCl["passData"] = clientPassData;
            return dataCl;
        }


        private string CreateReplaceString(List<SharedProperty> listShProp, string propKey, params string[] datas)
        {
            if (listShProp == null) return null;
            SharedProperty sharedProp = listShProp.FirstOrDefault(shP => shP.Key.Equals(propKey));

            if (sharedProp == null) return null;

            string dataString = sharedProp.Value;
            if (datas != null && (datas.Length % 2 == 0))
            {
                for (int i = 0; i < datas.Length - 1; i += 2)
                {
                    if (datas[i] != null && datas[i] != "")
                        dataString = dataString.Replace(datas[i], datas[i + 1]);
                }
            }

            return dataString;
        }

        private  string parseDateTime(string pattern, DateTime? dTime)
        {
            string txtDate = dTime == null || dTime.Value == null ? "" : dTime.Value.ToString(pattern);
            return txtDate;
        }

        private List<DocumentCommand> CreateListGuarantee(Dictionary<string, object> data)
        {
            List<DocumentCommand> commands = new List<DocumentCommand>();
            List<vGuarantee> vGr = _db.vGuarantee.Where(x => x.DG_POZN == _Cr.IDLoan).ToList();
            List<GuaranteePledge> grPl = new List<GuaranteePledge>();
            Guarantee_ProductsAndEquipment grProduct;
            decimal summ = 0;
            decimal assessed;
            foreach (var i in vGr)
            {
                grProduct = _db.Guarantee_ProductsAndEquipment.FirstOrDefault(x => x.GuaranteeID == i.ID);
                assessed = grProduct.AssessedValue != null ? (decimal)grProduct.AssessedValue : 0;
                grPl.Add(new GuaranteePledge
                {
                    Name = $"{grProduct.Name}, {grProduct.Description}",
                    DocName = grProduct.DocName,
                    AssessedSum = assessed,
                    AssessedSumText = $"{assessed} ({Util.toRecipe(assessed, 0)})"
                });
                summ += assessed;
            }
            data.Add("<SUMVALUE>", $"{summ} ({Util.toRecipe(summ, 0)})");
                TableData<GuaranteePledge> tabData1 = new TableData<GuaranteePledge>
                {
                    caption = "Таблица 1",
                    data = grPl,
                    fields = new string[] { "Name", "DocName", "AssessedSumText" },
                    isNumbered = true
                };

            CopyReplaceCommand cCommand2 = new CopyReplaceCommand();
            cCommand2.replaceData = data;
            cCommand2.afterBookMark = "GrPg";
            cCommand2.sourceFileName = "pledges_tab.docx";

            FillTablesCommand fCommand = new FillTablesCommand();
            fCommand.afterRow = 1;
            fCommand.addTableData(tabData1);
            fCommand.addParagraphProperties(new ParagraphProperties
            {
                Justification = new Justification { Val = JustificationValues.Center },
            });

            cCommand2.addInnerCoomand(fCommand);
            commands.Add(cCommand2);

            return commands;
        }
        #endregion

        #region RejectLoanApp
        public bool RejectLoanApp()
        {
            bool status = false;
            LoanApplication la = _db.LoanApplication.FirstOrDefault(x=>x.ID == _Cr.IDLoan);
            if(la != null)
            {
                int result = _db.LoanApplication_deleteLoanContract(la.ID, la.ClientID, 4).ToList().First().MQ_RESULT.Value;
                if (result != 0)
                    return false;
                la.Comment = _Cr.Comment;
                la.ClientComment = _Cr.Comment;
                _db.SaveChanges();
                status = true;
            }
            return status;
        }
        #endregion
    }
}