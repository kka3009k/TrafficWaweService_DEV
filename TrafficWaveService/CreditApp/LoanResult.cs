using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TrafficWaveService;

namespace TrafficWaveService.CreditApp
{
    [DataContract]
    public class LoanResult
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string IDString { get; set; }

        [DataMember]
        public string Base64Str { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public EnumRequestStatus EnumRequestStatus { get; set; }
    }
}