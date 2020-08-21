using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using TrafficWaveService.Reports;
using WordToPDF;
using System.Net;
namespace TrafficWaveService.CreditApp
{
    public class CreditContract
    {
        CreditAppData _Cr;
        public CreditContract()
        {
           
        }
        public CreditContract(CreditAppData pCr)
        {
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

                return (int)pCr.IDLoan;
            }
        }

        private bool CreateGuarantee(CreditAppData pCr)
        {
            bool status = false;
            try
            {

            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
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