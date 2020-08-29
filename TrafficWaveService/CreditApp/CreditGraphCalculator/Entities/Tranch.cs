using System;
using System.Net;
using System.Windows;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Entities
{
    public class Tranch
    {
        public int Identity { set; get; }
        public DateTime Date { set; get; }
        public decimal Sum { set; get; }
    }
}
