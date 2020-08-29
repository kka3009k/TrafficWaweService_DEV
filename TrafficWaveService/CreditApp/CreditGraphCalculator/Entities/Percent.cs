using System;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Entities
{
    public class Percent
    {
        /// <summary>
        /// Дата начала действия процента
        /// </summary>
        public DateTime DateStart { set; get; }
        /// <summary>
        /// Дата окончания действия процента
        /// </summary>
        public DateTime DateEnd { set; get; }
        /// <summary>
        /// Начисляемый процент
        /// </summary>
        public decimal Percents { set; get; }
        /// <summary>
        /// Сумма до которого будет действовать процент
        /// </summary>
        public decimal Amount { set; get; }
        /// <summary>
        /// Штрафные проценты
        /// </summary>
        public decimal FinePercents { set; get; }
    }
}
