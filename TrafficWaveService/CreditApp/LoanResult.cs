using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrafficWaveService.CreditApp
{
    [DataContract]
    public class LoanResult
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Base64Str { get; set; }

    }
}