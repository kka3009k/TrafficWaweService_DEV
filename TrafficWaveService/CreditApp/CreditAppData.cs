using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficWaveService.CreditApp
{
    public class CreditAppData
    {
       public string ApplicationNumber { get; set; }
        public int BranchID { get; set; }
        public int OfficeID { get; set; }
        public int CurrencyID { get; set; }
        public int ClientID { get; set; }
        public float Amount { get; set; }
        public float LoanPercent { get; set; }
        public int PeriodInMonth { get; set; }

        public float FirstPaymentInPercent { get; set; }
        public int AttractionCanalID { get; set; }
        public int ProductID { get; set; }
        public int ClientClassID { get; set; }
        public int TypeClientID { get; set; }
        public int AuctionSourceID { get; set; }
        public int PaymentSourceID { get; set; }
        public int GuaranteeID { get; set; }
        public int StatusID { get; set; }
        public int TypeCreditID { get; set; }

        public int EnterpriseID { get; set; }
        public int LoanSubTypeID { get; set; }
        public int AimsID { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime FirstContactDate { get; set; }
        public DateTime AnalysisDate { get; set; }
        public int PersonID { get; set; }
        public int CreatorID { get; set; }
        public string BankConnection { get; set; }
        public string GroupName { get; set; }
        public int PurposeID { get; set; }
        public string AimsComment { get; set; }
        public int PercentType { get; set; }

        public int IDLoan { get; set; }
        public int IDLoanContract { get; set; }

        //Тип операция 1 создание заявки 2 создание договора 3 формирование договора
        public int TypeOperation { get; set; }

    }
}