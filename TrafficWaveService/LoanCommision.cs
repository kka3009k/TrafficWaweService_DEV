//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class LoanCommision
    {
        public int DgPozn { get; set; }
        public int Kodkl { get; set; }
        public Nullable<decimal> AmountManage { get; set; }
        public Nullable<decimal> PercentManage { get; set; }
        public Nullable<decimal> AmountCosts { get; set; }
        public Nullable<decimal> AmountInsurance { get; set; }
        public Nullable<decimal> PercentInsurance { get; set; }
        public Nullable<decimal> AmountRestr { get; set; }
        public Nullable<decimal> PercentRestr { get; set; }
        public Nullable<decimal> AmountPrepayment { get; set; }
        public Nullable<decimal> PercentPrepayment { get; set; }
        public Nullable<decimal> AmountChange { get; set; }
        public Nullable<decimal> PercentChange { get; set; }
        public Nullable<decimal> PercentVBBalance { get; set; }
    
        public virtual clients clients { get; set; }
    }
}
