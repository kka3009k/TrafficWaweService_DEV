using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrafficWaveService.Reports
{
    /// <summary>
    /// Параметры передаваемых метаданных шаблона
    /// </summary>
    [DataContract]
    public class TemplateQuery
    {
            /// <summary>
            /// Номер шаблона документа
            /// </summary>
            [DataMember]
            public string DocNumber { get; set; }

            /// <summary>
            /// Строка метаданных шаблона
            /// </summary>
            [DataMember]
            public string RequestStringMetaData { get; set; }

            
            [DataMember]
            public Result Error = new Result();
        }
    }