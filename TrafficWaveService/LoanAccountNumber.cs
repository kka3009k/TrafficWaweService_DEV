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
    
    public partial class LoanAccountNumber
    {
        public int DgPozn { get; set; }
        public int Kodkl { get; set; }
        public System.DateTime DateLC { get; set; }
        public Nullable<int> LC_Loan { get; set; }
        public Nullable<int> LC_Percent { get; set; }
        public Nullable<int> LC_FuturePeriod { get; set; }
        public Nullable<int> LC_Discount { get; set; }
        public Nullable<int> LC_LoanVN { get; set; }
        public Nullable<int> LC_PercentVN { get; set; }
        public Nullable<int> LC_Returns { get; set; }
        public Nullable<int> LC_OffBalance { get; set; }
        public Nullable<int> LC_OffBalanceK { get; set; }
        public Nullable<int> LC_DocVN { get; set; }
        public Nullable<int> LC_VNFineOnMS { get; set; }
        public Nullable<int> LC_VNFineOnPercents { get; set; }
        public Nullable<int> LcNsp { get; set; }
        public Nullable<int> LcVnNsp { get; set; }
    
        public virtual clients clients { get; set; }
    }
}
