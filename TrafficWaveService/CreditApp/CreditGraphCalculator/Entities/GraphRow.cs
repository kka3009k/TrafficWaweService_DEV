using TrafficWaveService.CreditApp.CreditGraphCalculator.Algorithm;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Listeners;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Observers;
using System;
using System.Collections.Generic;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Entities
{
    public class GraphRow : IGraphRowObservable
    {
        private List<IGraphRowListener> listeners = new List<IGraphRowListener>();

        private AlgorithmAbstract algorithm;
        /// <summary>
        /// Алгоритм расчёта графика
        /// </summary>
        public AlgorithmAbstract Algorithm {
            set
            {
                this.algorithm = value;
                if(algorithm != null)
                {
                    algorithm.GraphRow = this;
                }
            }
            get
            {
                return algorithm;
            }
        }

        private GraphRow _upperRow;
        /// <summary>
        /// Верхняя строка
        /// </summary>
        public GraphRow UpperRow { set
            {

                //if(value != null)
                //{
                //    if(_upperRow != null)
                //        _upperRow.BottomRow = value;

                //    value.BottomRow = this;
                //    value.UpperRow = this._upperRow;
                //}
                _upperRow = value;
            }
            get
            {
                return _upperRow;
            }
        }

        private GraphRow _bottonRow;
        /// <summary>
        /// Нижняя строка
        /// </summary>
        public GraphRow BottomRow {
            set{
                _bottonRow = value;
            }
            get
            {
                return _bottonRow;
            }
        }

        /// <summary>
        /// Идентификация
        /// </summary>
        public int Identity { set; get; }
        /// <summary>
        /// Нумеряция
        /// </summary>
        public int number { set; get; }
        
        /// <summary>
        /// Признак округления.
        /// Если больше 0 то данные будут 
        /// возврашаться с округлением
        /// </summary>
        public int Round { set; get; }
        /// <summary>
        /// Является ли выдачой
        /// </summary>
        public bool IsIssue {
            set; get; 
        }

        /// <summary>
        /// Является ли последней строкой
        /// </summary>
        public bool IsLast { set; get; }

        /// <summary>
        /// Является ли льготным по ОС
        /// </summary>
        public bool IsMSPrivilegy { set; get; }

        /// <summary>
        /// Является ли льготным по процентам
        /// </summary>
        public bool IsPercPrivilegy { set; get; }

        /// <summary>
        /// Является ли серединой месяца
        /// </summary>
        public bool IsMonthMiddle { set; get; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { set; get; }

        private decimal issueSum;
        /// <summary>
        /// Выдача
        /// </summary>
        public decimal IssueSum {
            get { return round(issueSum); }
            set { issueSum = value; }
        }

        private decimal remainderMainSum;
        /// <summary>
        /// Остаток основной суммы
        /// </summary>
        public decimal RemainderMainSum {
            set {
                remainderMainSum = value;
            }
            get {
                return round(remainderMainSum);
            }
        }

        private decimal repaymentMainSum;
        /// <summary>
        /// Погашение основной суммы
        /// </summary>
        public decimal RepaymentMainSum {
            set {
                if (repaymentMainSum != value)
                {
                    NotifyRepaymentMSChanged(this, repaymentMainSum, value);
                    repaymentMainSum = value;
                }
            }
            get { return round(repaymentMainSum);
            }
        }

        private decimal repaymentPercent;
        /// <summary>
        /// Погашение процентов
        /// </summary>
        public decimal RepaymentPercent {
            set { repaymentPercent = value; }
            get { return round(repaymentPercent); }
        }

        /// <summary>
        /// Погашение основной суммы + Погашение процентов
        /// </summary>
        public decimal RepaymentMSPercent
        {
            get
            {
                return round(RepaymentMainSum + RepaymentPercent);
            }
        }

        private decimal _repaymentNsp;
        /// <summary>
        /// Погашение НсП
        /// </summary>
        public decimal RepaymentNsp
        {
            set { _repaymentNsp = value; }
            get { return round(_repaymentNsp); }
        }
        
        /// <summary>
        /// Итого к оплате
        /// </summary>
        public decimal TotalPayment
        {
            get { return round(RepaymentMainSum + RepaymentPercent + RepaymentNsp); }
        }

        /// <summary>
        /// Изменен ли в ручную
        /// </summary>
        public bool IsModifyed { set; get; }

        private decimal round(decimal value)
        {
            return Round > 0 ? Decimal.Round(value, Round) : value;
        }

        public void AddListener(IGraphRowListener listener)
        {
            listeners.Add(listener);
        }

        public void RemoveListener(IGraphRowListener listener)
        {
            listeners.Remove(listener);
        }

        public void NotifyRepaymentMSChanged(GraphRow row, decimal oldValue, decimal newValue)
        {
            foreach(IGraphRowListener listener in listeners)
            {
                listener.GraphRowRepaymentMSChanged(row, oldValue, newValue);
            }
        }
        /// <summary>
        /// Дата сохранения
        /// </summary>
        public DateTime SaveDate { set; get; }
        
        public Codes Code { set; get; }

        public string ErrorMessage { set; get; }
        public Exception Exception { set; get; }
    }
}
