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
    
    public partial class LoanContracts
    {
        public int DgPozn { get; set; }
        public int KodKl { get; set; }
        public System.DateTime Date { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public string Decision { get; set; }
    
        public virtual clients clients { get; set; }
    }
}