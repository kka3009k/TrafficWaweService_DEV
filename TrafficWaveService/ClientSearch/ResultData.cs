using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrafficWaveService.ClientSearch
{
    [DataContract]
    public class ResultData
    {
        /// <summary>
        /// Персональные данные клиента
        /// </summary>
        [DataMember]
        //public DataClient DataClient { get; set; }
        public string DataClient { get; set; }

        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}