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
    
    public partial class Guarantee_ProductsAndEquipment
    {
        public int ID { get; set; }
        public int GuaranteeID { get; set; }
        public string Name { get; set; }
        public string DocName { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<System.DateTime> MakeDate { get; set; }
        public Nullable<System.DateTime> InspectionDate { get; set; }
        public Nullable<decimal> Lowering { get; set; }
        public Nullable<decimal> MarketValue { get; set; }
        public Nullable<decimal> AssessedValue { get; set; }
        public Nullable<decimal> CurrentMarketValue { get; set; }
        public Nullable<decimal> CurrentAssessedValue { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string Description { get; set; }
    
        public virtual Guarantee Guarantee { get; set; }
    }
}