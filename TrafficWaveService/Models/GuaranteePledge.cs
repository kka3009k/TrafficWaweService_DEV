using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficWaveService.Models
{
    public class GuaranteePledge
    {
        public string Name { get; set; }
        public string DocName { get; set; }
        public decimal? AssessedSum { get; set; }
        public string AssessedSumText { get; set; }
    }
}