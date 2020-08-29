using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Algorithm
{
    public abstract class AlgorithmAbstract : IAlgorithm
    {
        /// <summary>
        /// Строка к которому будет применен алгоритм
        /// </summary>
        public GraphRow GraphRow { set; get; }

        /// <summary>
        /// Настройки графика
        /// </summary>
        public Setting GraphSettings { set; get; }
        
        /// <summary>
        /// Расчёт строки графика
        /// </summary>
        public abstract void Calculate();

        public AlgorithmAbstract() { }
        /// <summary>
        /// Абстрактный класс для алгоритма расчёта графика
        /// </summary>
        /// <param name="settings">Настройки графика</param>
        public AlgorithmAbstract(Setting settings)
        {
            this.GraphSettings = settings;
        }

        /// <summary>
        /// Находит проценты на дату данной строки графика
        /// </summary>
        /// <returns>Процент</returns>
        public Percent GetPersent()
        {
            Percent percent = null;
            if (GraphSettings != null && GraphRow != null)
                foreach (Percent p in GraphSettings.Percents)
                {
                    if (p.DateStart.Date <= GraphRow.Date.Date && p.DateEnd.Date >= GraphRow.Date.Date
                        && p.Amount >= GraphRow.RemainderMainSum)
                        percent = p;
                    if (percent == null)
                        percent = GraphSettings.Percents.LastOrDefault();
                }
            return percent;            
        }


        /// <summary>
        /// Расчёт количества месяцев между датой выдачи и датой первого погашения
        /// </summary>
        /// <returns>Количество месяцев между датой выдачи и датой первого погашения</returns>
        public int GetMounthInIssueFirstPay()
        {
            if (GraphSettings != null)
            {
                return ((GraphSettings.IssueDate.Year - GraphSettings.FirstRepayDate.Year) * 12) + GraphSettings.IssueDate.Month - GraphSettings.FirstRepayDate.Month;
            }
            return 0;
        }

        /// <summary>
        /// Является ли данная строка последней
        /// </summary>
        /// <returns></returns>
        public bool IsLastRow()
        {
            return GetWorkDate(GraphSettings.EndDate).Date == GraphRow.Date
                || GetWorkDate(GraphSettings.EndDate).Date == GetWorkDate(GraphRow.Date).Date
                || GetWorkDate(GraphSettings.EndDate).Date == GetWorkDate(GraphRow.Date.AddDays(-2))
                || GetWorkDate(GraphSettings.EndDate).Date == GetWorkDate(GraphRow.Date.AddDays(-1))
                || GetWorkDate(GraphSettings.EndDate).Date == GetWorkDate(GraphRow.Date.AddDays(1))
                || GetWorkDate(GraphSettings.EndDate).Date == GetWorkDate(GraphRow.Date.AddDays(2));
        }
                
        /// <summary>
        /// Возвращает разницу в днях
        /// </summary>
        /// <returns></returns>
        public int GetDaysBetweenUpper()
        {
            if (GraphRow != null && GraphRow.UpperRow != null
                && GraphRow.Date != null && GraphRow.UpperRow.Date != null)
                return DaysBetween(GraphRow.UpperRow.Date, GraphRow.Date);
            else
                return 0;
        }

        /// <summary>
        /// Возврат даты если нет в спике не рабичих дней.
        /// </summary>
        /// <param name="currentDate">Дата проверки</param>
        /// <returns>Рабочий день</returns>
        public DateTime GetWorkDate(DateTime currentDate)
        {
            if(GraphSettings != null)
            {
                if (currentDate != null && GraphSettings.NotWorkDays.Any(d => d.Year == currentDate.Year && d.DayOfYear == currentDate.DayOfYear))
                    return GetWorkDate(currentDate.AddDays(1));
                else
                    return currentDate;
            }
            else return currentDate;
        }

        /// <summary>
        /// Возвращает пердыдущую рабочию дату
        /// </summary>
        /// <param name="currentDate"></param>
        /// <param name="notWorkDays">список не рабочих дней</param>
        /// <returns></returns>
        public DateTime GetPreviousWorkDate(DateTime currentDate)
        {
            if (currentDate != null && GraphSettings.NotWorkDays.Any(d => d.Year == currentDate.Year && d.DayOfYear == currentDate.DayOfYear))
                return GetPreviousWorkDate(currentDate.AddDays(-1));
            else return currentDate;
        }

        /// <summary>
        /// Расчёт рабочих дней между датами
        /// </summary>
        /// <param name="dateStart">Дата начало</param>
        /// <param name="dateEnd">Дата конец</param>
        /// <returns></returns>
        public int GetWorkDaysBetween(DateTime dateStart, DateTime dateEnd)
        {
            int workDays = 0;
            if (dateEnd != null && dateStart != null && GraphSettings != null && GraphSettings.NotWorkDays != null)
            {
                int days = Convert.ToInt32((dateEnd - dateStart).TotalDays);
                DateTime date = dateStart;
                for(int i = 0; i < days; i++)
                {
                    //Если рабочий день (нет в списке не рабочих дней)
                    if(!GraphSettings.NotWorkDays.Any(d=> d.Year == date.Year && d.DayOfYear == date.DayOfYear))
                        workDays++;
                }
            }
            return workDays;
        }

        /// <summary>
        /// Расчёт дней между датами
        /// </summary>
        /// <param name="dateStart">Дата начало</param>
        /// <param name="dateEnd">Дата конец</param>
        /// <returns></returns>
        public int DaysBetween(DateTime dateStart, DateTime dateEnd)
        {
            int days = 0;
            if (dateEnd != null && dateStart != null)
            {
                days = Convert.ToInt32((dateEnd.Date - dateStart.Date).TotalDays);
            }
            return days;
        }

        /// <summary>
        /// Расчёт количества погашений
        /// </summary>
        /// <param name="settings">Настройки графика</param>
        /// <returns>Количество погашений</returns>
        public int GetPaymentCount(Setting settings)
        {
            if(settings != null)
            {
                int paymentCount = settings.RepayType == RepayType.TwoTimeInMonth ? settings.MounthCount * 2 : settings.MounthCount;
                return paymentCount - (settings.PrivelegyMainSum);
            }
            return 0;
        }

        public decimal Days360(DateTime StartDate, DateTime EndDate)
        {
            int StartDay = StartDate.Day;
            int StartMonth = StartDate.Month;
            int StartYear = StartDate.Year;
            int EndDay = EndDate.Day;
            int EndMonth = EndDate.Month;
            int EndYear = EndDate.Year;

            if (StartDay == 31 || IsLastDayOfFebruary(StartDate) || StartDate.Month != GraphUtils.GetNextWorkDate(StartDate.AddDays(1), GraphSettings.NotWorkDays).Month)
            {
                StartDay = 30;
            }

            if (EndDay == 31 || IsLastDayOfFebruary(EndDate) || EndDate.Month != GraphUtils.GetNextWorkDate(EndDate.AddDays(1), GraphSettings.NotWorkDays).Month)
            {
                EndDay = 30;
            }

            return ((EndYear - StartYear) * 360) + ((EndMonth - StartMonth) * 30) + (EndDay - StartDay);
        }

        private bool IsLastDayOfFebruary(DateTime date)
        {
            return date.Month == 2 && date.Day == DateTime.DaysInMonth(date.Year, date.Month);
        }
    }
}
