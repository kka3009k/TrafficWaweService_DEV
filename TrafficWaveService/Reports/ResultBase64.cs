using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrafficWaveService.Reports
{
    [DataContract]
    public class ResultBase64
    {

        [DataMember]
        public string ResultPdf { get; set; }

    }

}