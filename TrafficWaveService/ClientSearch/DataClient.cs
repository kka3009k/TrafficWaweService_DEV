using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace TrafficWaveService.ClientSearch
{
    public class DataClient
    {
        public clients clients { get; set; }
        public List<client_adress> client_adress { get; set; }
        public client_paspdata client_paspdata { get; set; }

        public string check_result { get; set; }
      
    }
}