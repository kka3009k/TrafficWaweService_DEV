using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrafficWaveService.CreditApp
{
    /// <summary>
    /// Параметры передаваемых метаданных кредитной заявки
    /// </summary>
    [DataContract]
    public class CreditQuery
    {
        /// <summary>
        /// Данные кредитной заявки
        /// </summary>
        [DataMember]
        public string RequestStringCreditData { get; set; }

    
    }
}