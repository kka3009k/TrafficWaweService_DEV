using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrafficWaveService.ClientSearch
{
    /// <summary>
    /// Параметры передаваемых метаданных шаблона
    /// </summary>
    [DataContract]
    public class SearchClientQuery
    {
        /// <summary>
        /// Номер шаблона документа
        /// </summary>
        [DataMember]
        public string INN { get; set; }

        [DataMember]
        public Result Error = new Result();
    }
}