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
    
    public partial class LoanIncomeKodv
    {
        public int DgPozn { get; set; }
        public int KodKl { get; set; }
        public System.DateTime Date { get; set; }
        public short Kodv1 { get; set; }
        public decimal Percent1 { get; set; }
        public Nullable<short> Kodv2 { get; set; }
        public Nullable<decimal> Percent2 { get; set; }
        public Nullable<short> Kodv3 { get; set; }
        public Nullable<decimal> Percent3 { get; set; }
        public Nullable<short> Kodv4 { get; set; }
        public Nullable<decimal> Percent4 { get; set; }
        public Nullable<short> Kodv5 { get; set; }
        public Nullable<decimal> Percent5 { get; set; }
    
        public virtual clients clients { get; set; }
    }
}