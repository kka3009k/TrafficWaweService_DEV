using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                return (int)contract.First().MQ_RESULT;
            }
        }

        public string CreateCreditContractPdf()
        {
            Dictionary<string, object> dict = CreateContractMeta();
            ReportServiceRef.ReportServiceClient client = new ReportServiceRef.ReportServiceClient();
            client.print_DOG_KREDIT(dict, 0);
            return "";
        }

        private Dictionary<string, object> CreateContractMeta()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            return dict;

        }
    }
}