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
    
    public partial class sprotv_k
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sprotv_k()
        {
            this.LoanApplication = new HashSet<LoanApplication>();
        }
    
        public decimal OT_NOM { get; set; }
        public string OT_FIO { get; set; }
        public Nullable<decimal> OT_NOMOD { get; set; }
        public string OT_PASSPORT { get; set; }
        public string OT_ADRESS { get; set; }
        public Nullable<decimal> kodb { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LoanApplication> LoanApplication { get; set; }
    }
}
