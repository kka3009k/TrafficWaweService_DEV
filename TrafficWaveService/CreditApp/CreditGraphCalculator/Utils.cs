using TrafficWaveService.CreditApp.CreditGraphCalculator.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator
{
    public class GraphUtils
    {
        /// <summary>
        /// Возвращает следующую рабочию дату
        /// </summary>
        /// <param name="currentDate"></param>
        /// <param name="notWorkDays">список не рабочих дней</param>
        /// <returns></returns>
        public static DateTime GetNextWorkDate(DateTime currentDate, List<DateTime> notWorkDays)
        {
            if (currentDate != null && notWorkDays.Any(d => d.Year == currentDate.Year && d.DayOfYear == currentDate.DayOfYear))
                return GetNextWorkDate(currentDate.AddDays(1), notWorkDays);
            else return currentDate;
        }

        /// <summary>
        /// Возвращает следующую дату в зависимости от типа погашения
        /// - если тип два раза в месяц то 15, если оно прошло то конец месяца
        /// - если тип один раз в месяц то конец месяца
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DateTime GetNextDate(DateTime date, RepayType type)
        {
            DateTime resDate = date;

            if (date != null)
                switch (type)
                {
                    case RepayType.OneTimeInMonth:
                        resDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                        break;
                    case RepayType.TwoTimeInMonth:
                        if(date.Day <= 15)
                        {
                            resDate = new DateTime(date.Year, date.Month, 15);
                        }else
                        {
                            resDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                        }
                        break;
                }

            return resDate;
        }
    }
}
