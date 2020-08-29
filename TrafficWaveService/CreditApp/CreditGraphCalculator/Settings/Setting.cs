using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Listeners;
using System;
using System.Collections.Generic;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Settings
{
    public class Setting
    {
        //public delegate void OnSettingChanged();

        /// <summary>
        /// Сумма выдачи
        /// </summary>
        public decimal Sum { set; get; }
        /// <summary>
        /// Количество месяцев
        /// </summary>
        public int MounthCount { set; get; }

        private List<Percent> percents = new List<Percent>();
        public List<Percent> Percents {
            get { return percents; }
        }
        /// <summary>
        /// Дата выдачи
        /// </summary>
        public DateTime IssueDate { set; get; }
        /// <summary>
        /// Дата первого погашения
        /// </summary>
        public DateTime FirstRepayDate { set; get; }
        /// <summary>
        /// Льготный периуд по ОС
        /// </summary>
        public int PrivelegyMainSum { set; get; }
        /// <summary>
        /// Льготный периуд по процентам
        /// </summary>
        public int PrivelegyPersent { set; get; }
        /// <summary>
        /// Принцип пяти дней
        /// </summary>
        public bool FiveDayPrincipe { set; get; }
        /// <summary>
        /// Тип графика
        /// </summary>
        public GraphType GraphType { set; get; }
        /// <summary>
        /// Тип погашения
        /// </summary>
        public RepayType RepayType { set; get; }
        /// <summary>
        /// Тип выдачи
        /// </summary>
        public IssueType IssueType { set; get; }

        /// <summary>
        /// Схема начисления
        /// </summary>
        public Shema Shema { set; get; }

        private List<Tranch> tranches = new List<Tranch>();
        /// <summary>
        /// Транши
        /// </summary>
        public List<Tranch> Tranches
        {
            get { return tranches; }
        }
        
        /// <summary>
        /// Сумма всех траншов включая данную дату
        /// </summary>
        public decimal TranchesSum(DateTime? date = null)
        {
            decimal sum = 0;
            tranches.ForEach(t => {
                if(date != null)
                {
                    //t.Date.Year <= date.Value.Year && t.Date.DayOfYear <= date.Value.DayOfYear
                    if (DateTime.Compare(t.Date, date.Value) <= 0)
                    {
                        sum += t.Sum;
                    }
                }else
                {
                    sum += t.Sum;
                }
            });
            return sum;
        }

        private List<DateTime> notWorkDays = new List<DateTime>();
        /// <summary>
        /// Список не рабочих дней
        /// </summary>
        public List<DateTime> NotWorkDays
        {
            get { return notWorkDays; }
        }

        private int round = 2;
        /// <summary>
        /// Округление до указанной точки
        /// по умодлчанию 2 (пример: 3.14)
        /// </summary>
        public int Round {
            set { round = value; }
            get { return round; }
        } 

        /// <summary>
        /// Количество всех дней начиная с даты выдачи и до конца кредита
        /// </summary>
        /// <returns></returns>
        public double TotalDays()
        {
            return ((IssueDate.AddMonths(MounthCount)) - IssueDate).TotalDays;
        }

        public DateTime EndDate
        {
            get
            {
                return IssueDate.AddMonths(MounthCount);
            }
        }

        private List<GraphRow> modifyed = new List<GraphRow>();
        /// <summary>
        /// Cтроки исправленные при индивидуальном графике
        /// </summary>
        public List<GraphRow> ModifyedRows {
            get { return modifyed; }
        }

        /// <summary>
        /// Слушатель изменений в гравике
        /// </summary>
        public IGraphRowListener GraphRowListener { set; get; }

        /// <summary>
        /// Признак является ли клиент сотрудником
        /// </summary>
        public bool IsEmployee = false;

        /// <summary>
        /// Номер платежа с которого начинаются фиксированные платежи 
        /// </summary>
        public int FixedPaySince { set; get; }

        /// <summary>
        /// Номер платежа до которого действуют фиксированные платежи
        /// </summary>
        public int FixedPayBefore { set; get; }

        /// <summary>
        /// Сумма фиксированного платежа
        /// </summary>
        public decimal SumFixedPay { set; get; }
    }
}
