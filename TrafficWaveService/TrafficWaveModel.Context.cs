﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TrafficWaveService
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class bankasiaNSEntities : DbContext
    {
        public bankasiaNSEntities()
            : base("name=bankasiaNSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LOG_APP> LOG_APP { get; set; }
        public virtual DbSet<ow> ow { get; set; }
        public virtual DbSet<client_adress> client_adress { get; set; }
        public virtual DbSet<client_comp_details> client_comp_details { get; set; }
        public virtual DbSet<client_dl> client_dl { get; set; }
        public virtual DbSet<client_licence> client_licence { get; set; }
        public virtual DbSet<client_oborot> client_oborot { get; set; }
        public virtual DbSet<client_paspdata> client_paspdata { get; set; }
        public virtual DbSet<client_podpic> client_podpic { get; set; }
        public virtual DbSet<client_risk> client_risk { get; set; }
        public virtual DbSet<ClientOrg> ClientOrg { get; set; }
        public virtual DbSet<clientPhoto> clientPhoto { get; set; }
        public virtual DbSet<ClientProxy> ClientProxy { get; set; }
        public virtual DbSet<clients> clients { get; set; }
        public virtual DbSet<ENUMERATOR_SPR> ENUMERATOR_SPR { get; set; }
        public virtual DbSet<LoanApplication> LoanApplication { get; set; }
        public virtual DbSet<LoanContracts> LoanContracts { get; set; }
        public virtual DbSet<LoanCredits> LoanCredits { get; set; }
        public virtual DbSet<vLoanContract> vLoanContract { get; set; }
        public virtual DbSet<spr> spr { get; set; }
        public virtual DbSet<klient_p482_spr> klient_p482_spr { get; set; }
        public virtual DbSet<Guarantee> Guarantee { get; set; }
        public virtual DbSet<Guarantee_ProductsAndEquipment> Guarantee_ProductsAndEquipment { get; set; }
        public virtual DbSet<LoanClassification> LoanClassification { get; set; }
        public virtual DbSet<LoanGraph> LoanGraph { get; set; }
        public virtual DbSet<LoanGraphSettings> LoanGraphSettings { get; set; }
        public virtual DbSet<kalendar> kalendar { get; set; }
        public virtual DbSet<LoanPercent> LoanPercent { get; set; }
        public virtual DbSet<sprotv> sprotv { get; set; }
        public virtual DbSet<LoanAccountNumber> LoanAccountNumber { get; set; }
        public virtual DbSet<LoanCommision> LoanCommision { get; set; }
        public virtual DbSet<LoanIssue> LoanIssue { get; set; }
        public virtual DbSet<sprotv_k> sprotv_k { get; set; }
        public virtual DbSet<vGuarantee> vGuarantee { get; set; }
        public virtual DbSet<rekvizit> rekvizit { get; set; }
        public virtual DbSet<LoanIncomeKodv> LoanIncomeKodv { get; set; }
        public virtual DbSet<SharedProperty> SharedProperty { get; set; }
        public virtual DbSet<dogkr> dogkr { get; set; }
    
        public virtual ObjectResult<SearchReg_Result> SearchReg(string pFam, string pName, string pOtch, Nullable<byte> pStatus)
        {
            var pFamParameter = pFam != null ?
                new ObjectParameter("pFam", pFam) :
                new ObjectParameter("pFam", typeof(string));
    
            var pNameParameter = pName != null ?
                new ObjectParameter("pName", pName) :
                new ObjectParameter("pName", typeof(string));
    
            var pOtchParameter = pOtch != null ?
                new ObjectParameter("pOtch", pOtch) :
                new ObjectParameter("pOtch", typeof(string));
    
            var pStatusParameter = pStatus.HasValue ?
                new ObjectParameter("pStatus", pStatus) :
                new ObjectParameter("pStatus", typeof(byte));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SearchReg_Result>("SearchReg", pFamParameter, pNameParameter, pOtchParameter, pStatusParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> LoanApplication_create(Nullable<short> branchID, Nullable<short> officeID, Nullable<short> currencyID, Nullable<int> clientID, Nullable<decimal> amount, Nullable<decimal> loanPercent, Nullable<int> periodInMonth, string groupName, Nullable<decimal> firstPaymentInPercent, Nullable<byte> attractionCanalID, Nullable<int> productID, Nullable<byte> clientClassID, Nullable<byte> typeClientID, Nullable<int> auctionSourceID, Nullable<byte> purposeID, Nullable<byte> paymentSourceID, Nullable<int> guaranteeID, Nullable<byte> statusID, Nullable<short> typeCreditID, Nullable<int> enterpriseID, Nullable<byte> loanSubTypeID, Nullable<int> aimsID, string aimsComment, Nullable<System.DateTime> firstContactDate, Nullable<System.DateTime> analysisDate, Nullable<System.DateTime> committeeDate, Nullable<short> personID, Nullable<short> creatorID, string bankConnection, string comment, string clientComment, Nullable<byte> percentType)
        {
            var branchIDParameter = branchID.HasValue ?
                new ObjectParameter("BranchID", branchID) :
                new ObjectParameter("BranchID", typeof(short));
    
            var officeIDParameter = officeID.HasValue ?
                new ObjectParameter("OfficeID", officeID) :
                new ObjectParameter("OfficeID", typeof(short));
    
            var currencyIDParameter = currencyID.HasValue ?
                new ObjectParameter("CurrencyID", currencyID) :
                new ObjectParameter("CurrencyID", typeof(short));
    
            var clientIDParameter = clientID.HasValue ?
                new ObjectParameter("ClientID", clientID) :
                new ObjectParameter("ClientID", typeof(int));
    
            var amountParameter = amount.HasValue ?
                new ObjectParameter("Amount", amount) :
                new ObjectParameter("Amount", typeof(decimal));
    
            var loanPercentParameter = loanPercent.HasValue ?
                new ObjectParameter("LoanPercent", loanPercent) :
                new ObjectParameter("LoanPercent", typeof(decimal));
    
            var periodInMonthParameter = periodInMonth.HasValue ?
                new ObjectParameter("PeriodInMonth", periodInMonth) :
                new ObjectParameter("PeriodInMonth", typeof(int));
    
            var groupNameParameter = groupName != null ?
                new ObjectParameter("GroupName", groupName) :
                new ObjectParameter("GroupName", typeof(string));
    
            var firstPaymentInPercentParameter = firstPaymentInPercent.HasValue ?
                new ObjectParameter("FirstPaymentInPercent", firstPaymentInPercent) :
                new ObjectParameter("FirstPaymentInPercent", typeof(decimal));
    
            var attractionCanalIDParameter = attractionCanalID.HasValue ?
                new ObjectParameter("AttractionCanalID", attractionCanalID) :
                new ObjectParameter("AttractionCanalID", typeof(byte));
    
            var productIDParameter = productID.HasValue ?
                new ObjectParameter("ProductID", productID) :
                new ObjectParameter("ProductID", typeof(int));
    
            var clientClassIDParameter = clientClassID.HasValue ?
                new ObjectParameter("ClientClassID", clientClassID) :
                new ObjectParameter("ClientClassID", typeof(byte));
    
            var typeClientIDParameter = typeClientID.HasValue ?
                new ObjectParameter("TypeClientID", typeClientID) :
                new ObjectParameter("TypeClientID", typeof(byte));
    
            var auctionSourceIDParameter = auctionSourceID.HasValue ?
                new ObjectParameter("AuctionSourceID", auctionSourceID) :
                new ObjectParameter("AuctionSourceID", typeof(int));
    
            var purposeIDParameter = purposeID.HasValue ?
                new ObjectParameter("PurposeID", purposeID) :
                new ObjectParameter("PurposeID", typeof(byte));
    
            var paymentSourceIDParameter = paymentSourceID.HasValue ?
                new ObjectParameter("PaymentSourceID", paymentSourceID) :
                new ObjectParameter("PaymentSourceID", typeof(byte));
    
            var guaranteeIDParameter = guaranteeID.HasValue ?
                new ObjectParameter("GuaranteeID", guaranteeID) :
                new ObjectParameter("GuaranteeID", typeof(int));
    
            var statusIDParameter = statusID.HasValue ?
                new ObjectParameter("StatusID", statusID) :
                new ObjectParameter("StatusID", typeof(byte));
    
            var typeCreditIDParameter = typeCreditID.HasValue ?
                new ObjectParameter("TypeCreditID", typeCreditID) :
                new ObjectParameter("TypeCreditID", typeof(short));
    
            var enterpriseIDParameter = enterpriseID.HasValue ?
                new ObjectParameter("EnterpriseID", enterpriseID) :
                new ObjectParameter("EnterpriseID", typeof(int));
    
            var loanSubTypeIDParameter = loanSubTypeID.HasValue ?
                new ObjectParameter("LoanSubTypeID", loanSubTypeID) :
                new ObjectParameter("LoanSubTypeID", typeof(byte));
    
            var aimsIDParameter = aimsID.HasValue ?
                new ObjectParameter("AimsID", aimsID) :
                new ObjectParameter("AimsID", typeof(int));
    
            var aimsCommentParameter = aimsComment != null ?
                new ObjectParameter("AimsComment", aimsComment) :
                new ObjectParameter("AimsComment", typeof(string));
    
            var firstContactDateParameter = firstContactDate.HasValue ?
                new ObjectParameter("FirstContactDate", firstContactDate) :
                new ObjectParameter("FirstContactDate", typeof(System.DateTime));
    
            var analysisDateParameter = analysisDate.HasValue ?
                new ObjectParameter("AnalysisDate", analysisDate) :
                new ObjectParameter("AnalysisDate", typeof(System.DateTime));
    
            var committeeDateParameter = committeeDate.HasValue ?
                new ObjectParameter("CommitteeDate", committeeDate) :
                new ObjectParameter("CommitteeDate", typeof(System.DateTime));
    
            var personIDParameter = personID.HasValue ?
                new ObjectParameter("PersonID", personID) :
                new ObjectParameter("PersonID", typeof(short));
    
            var creatorIDParameter = creatorID.HasValue ?
                new ObjectParameter("CreatorID", creatorID) :
                new ObjectParameter("CreatorID", typeof(short));
    
            var bankConnectionParameter = bankConnection != null ?
                new ObjectParameter("BankConnection", bankConnection) :
                new ObjectParameter("BankConnection", typeof(string));
    
            var commentParameter = comment != null ?
                new ObjectParameter("Comment", comment) :
                new ObjectParameter("Comment", typeof(string));
    
            var clientCommentParameter = clientComment != null ?
                new ObjectParameter("ClientComment", clientComment) :
                new ObjectParameter("ClientComment", typeof(string));
    
            var percentTypeParameter = percentType.HasValue ?
                new ObjectParameter("PercentType", percentType) :
                new ObjectParameter("PercentType", typeof(byte));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("LoanApplication_create", branchIDParameter, officeIDParameter, currencyIDParameter, clientIDParameter, amountParameter, loanPercentParameter, periodInMonthParameter, groupNameParameter, firstPaymentInPercentParameter, attractionCanalIDParameter, productIDParameter, clientClassIDParameter, typeClientIDParameter, auctionSourceIDParameter, purposeIDParameter, paymentSourceIDParameter, guaranteeIDParameter, statusIDParameter, typeCreditIDParameter, enterpriseIDParameter, loanSubTypeIDParameter, aimsIDParameter, aimsCommentParameter, firstContactDateParameter, analysisDateParameter, committeeDateParameter, personIDParameter, creatorIDParameter, bankConnectionParameter, commentParameter, clientCommentParameter, percentTypeParameter);
        }
    
        public virtual ObjectResult<LoanApplication_update_Result> LoanApplication_update(Nullable<int> iD, string applicationNumber, Nullable<short> branchID, Nullable<short> officeID, Nullable<short> currencyID, Nullable<int> clientID, Nullable<decimal> amount, Nullable<decimal> loanPercent, Nullable<int> periodInMonth, string groupName, Nullable<decimal> firstPaymentInPercent, Nullable<byte> attractionCanalID, Nullable<int> productID, Nullable<byte> clientClassID, Nullable<byte> typeClientID, Nullable<int> auctionSourceID, Nullable<byte> purposeID, Nullable<byte> paymentSourceID, Nullable<int> guaranteeID, Nullable<byte> statusID, Nullable<short> typeCreditID, Nullable<int> enterpriseID, Nullable<byte> loanSubTypeID, Nullable<int> aimsID, string aimsComment, Nullable<System.DateTime> firstContactDate, Nullable<System.DateTime> analysisDate, Nullable<System.DateTime> committeeDate, Nullable<short> personID, Nullable<short> creatorID, string bankConnection, string comment, string clientComment, Nullable<byte> percentType)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            var applicationNumberParameter = applicationNumber != null ?
                new ObjectParameter("ApplicationNumber", applicationNumber) :
                new ObjectParameter("ApplicationNumber", typeof(string));
    
            var branchIDParameter = branchID.HasValue ?
                new ObjectParameter("BranchID", branchID) :
                new ObjectParameter("BranchID", typeof(short));
    
            var officeIDParameter = officeID.HasValue ?
                new ObjectParameter("OfficeID", officeID) :
                new ObjectParameter("OfficeID", typeof(short));
    
            var currencyIDParameter = currencyID.HasValue ?
                new ObjectParameter("CurrencyID", currencyID) :
                new ObjectParameter("CurrencyID", typeof(short));
    
            var clientIDParameter = clientID.HasValue ?
                new ObjectParameter("ClientID", clientID) :
                new ObjectParameter("ClientID", typeof(int));
    
            var amountParameter = amount.HasValue ?
                new ObjectParameter("Amount", amount) :
                new ObjectParameter("Amount", typeof(decimal));
    
            var loanPercentParameter = loanPercent.HasValue ?
                new ObjectParameter("LoanPercent", loanPercent) :
                new ObjectParameter("LoanPercent", typeof(decimal));
    
            var periodInMonthParameter = periodInMonth.HasValue ?
                new ObjectParameter("PeriodInMonth", periodInMonth) :
                new ObjectParameter("PeriodInMonth", typeof(int));
    
            var groupNameParameter = groupName != null ?
                new ObjectParameter("GroupName", groupName) :
                new ObjectParameter("GroupName", typeof(string));
    
            var firstPaymentInPercentParameter = firstPaymentInPercent.HasValue ?
                new ObjectParameter("FirstPaymentInPercent", firstPaymentInPercent) :
                new ObjectParameter("FirstPaymentInPercent", typeof(decimal));
    
            var attractionCanalIDParameter = attractionCanalID.HasValue ?
                new ObjectParameter("AttractionCanalID", attractionCanalID) :
                new ObjectParameter("AttractionCanalID", typeof(byte));
    
            var productIDParameter = productID.HasValue ?
                new ObjectParameter("ProductID", productID) :
                new ObjectParameter("ProductID", typeof(int));
    
            var clientClassIDParameter = clientClassID.HasValue ?
                new ObjectParameter("ClientClassID", clientClassID) :
                new ObjectParameter("ClientClassID", typeof(byte));
    
            var typeClientIDParameter = typeClientID.HasValue ?
                new ObjectParameter("TypeClientID", typeClientID) :
                new ObjectParameter("TypeClientID", typeof(byte));
    
            var auctionSourceIDParameter = auctionSourceID.HasValue ?
                new ObjectParameter("AuctionSourceID", auctionSourceID) :
                new ObjectParameter("AuctionSourceID", typeof(int));
    
            var purposeIDParameter = purposeID.HasValue ?
                new ObjectParameter("PurposeID", purposeID) :
                new ObjectParameter("PurposeID", typeof(byte));
    
            var paymentSourceIDParameter = paymentSourceID.HasValue ?
                new ObjectParameter("PaymentSourceID", paymentSourceID) :
                new ObjectParameter("PaymentSourceID", typeof(byte));
    
            var guaranteeIDParameter = guaranteeID.HasValue ?
                new ObjectParameter("GuaranteeID", guaranteeID) :
                new ObjectParameter("GuaranteeID", typeof(int));
    
            var statusIDParameter = statusID.HasValue ?
                new ObjectParameter("StatusID", statusID) :
                new ObjectParameter("StatusID", typeof(byte));
    
            var typeCreditIDParameter = typeCreditID.HasValue ?
                new ObjectParameter("TypeCreditID", typeCreditID) :
                new ObjectParameter("TypeCreditID", typeof(short));
    
            var enterpriseIDParameter = enterpriseID.HasValue ?
                new ObjectParameter("EnterpriseID", enterpriseID) :
                new ObjectParameter("EnterpriseID", typeof(int));
    
            var loanSubTypeIDParameter = loanSubTypeID.HasValue ?
                new ObjectParameter("LoanSubTypeID", loanSubTypeID) :
                new ObjectParameter("LoanSubTypeID", typeof(byte));
    
            var aimsIDParameter = aimsID.HasValue ?
                new ObjectParameter("AimsID", aimsID) :
                new ObjectParameter("AimsID", typeof(int));
    
            var aimsCommentParameter = aimsComment != null ?
                new ObjectParameter("AimsComment", aimsComment) :
                new ObjectParameter("AimsComment", typeof(string));
    
            var firstContactDateParameter = firstContactDate.HasValue ?
                new ObjectParameter("FirstContactDate", firstContactDate) :
                new ObjectParameter("FirstContactDate", typeof(System.DateTime));
    
            var analysisDateParameter = analysisDate.HasValue ?
                new ObjectParameter("AnalysisDate", analysisDate) :
                new ObjectParameter("AnalysisDate", typeof(System.DateTime));
    
            var committeeDateParameter = committeeDate.HasValue ?
                new ObjectParameter("CommitteeDate", committeeDate) :
                new ObjectParameter("CommitteeDate", typeof(System.DateTime));
    
            var personIDParameter = personID.HasValue ?
                new ObjectParameter("PersonID", personID) :
                new ObjectParameter("PersonID", typeof(short));
    
            var creatorIDParameter = creatorID.HasValue ?
                new ObjectParameter("CreatorID", creatorID) :
                new ObjectParameter("CreatorID", typeof(short));
    
            var bankConnectionParameter = bankConnection != null ?
                new ObjectParameter("BankConnection", bankConnection) :
                new ObjectParameter("BankConnection", typeof(string));
    
            var commentParameter = comment != null ?
                new ObjectParameter("Comment", comment) :
                new ObjectParameter("Comment", typeof(string));
    
            var clientCommentParameter = clientComment != null ?
                new ObjectParameter("ClientComment", clientComment) :
                new ObjectParameter("ClientComment", typeof(string));
    
            var percentTypeParameter = percentType.HasValue ?
                new ObjectParameter("PercentType", percentType) :
                new ObjectParameter("PercentType", typeof(byte));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoanApplication_update_Result>("LoanApplication_update", iDParameter, applicationNumberParameter, branchIDParameter, officeIDParameter, currencyIDParameter, clientIDParameter, amountParameter, loanPercentParameter, periodInMonthParameter, groupNameParameter, firstPaymentInPercentParameter, attractionCanalIDParameter, productIDParameter, clientClassIDParameter, typeClientIDParameter, auctionSourceIDParameter, purposeIDParameter, paymentSourceIDParameter, guaranteeIDParameter, statusIDParameter, typeCreditIDParameter, enterpriseIDParameter, loanSubTypeIDParameter, aimsIDParameter, aimsCommentParameter, firstContactDateParameter, analysisDateParameter, committeeDateParameter, personIDParameter, creatorIDParameter, bankConnectionParameter, commentParameter, clientCommentParameter, percentTypeParameter);
        }
    
        public virtual ObjectResult<LoanApplication_createLoanContract_Result> LoanApplication_createLoanContract(Nullable<int> applicationID, Nullable<short> creatorID)
        {
            var applicationIDParameter = applicationID.HasValue ?
                new ObjectParameter("applicationID", applicationID) :
                new ObjectParameter("applicationID", typeof(int));
    
            var creatorIDParameter = creatorID.HasValue ?
                new ObjectParameter("CreatorID", creatorID) :
                new ObjectParameter("CreatorID", typeof(short));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LoanApplication_createLoanContract_Result>("LoanApplication_createLoanContract", applicationIDParameter, creatorIDParameter);
        }
    
        public virtual int SetContextInfo(string pLogin, string pIpAdress)
        {
            var pLoginParameter = pLogin != null ?
                new ObjectParameter("pLogin", pLogin) :
                new ObjectParameter("pLogin", typeof(string));
    
            var pIpAdressParameter = pIpAdress != null ?
                new ObjectParameter("pIpAdress", pIpAdress) :
                new ObjectParameter("pIpAdress", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SetContextInfo", pLoginParameter, pIpAdressParameter);
        }
    }
}
