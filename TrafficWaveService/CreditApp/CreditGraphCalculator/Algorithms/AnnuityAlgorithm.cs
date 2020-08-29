using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;
using System;
using System.Linq;
using System.Net;
using System.Windows;


namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Algorithm
{
    /// <summary>
    /// Алгоритм расчёта Аннуитетного графика
    /// </summary>
    public class AnnuityAlgorithm : AlgorithmAbstract
    {
        public AnnuityAlgorithm(Settings.Setting settings) : base(settings)
        {
        }

        public override void Calculate()
        {
            if (GraphRow != null)
            {
                GraphRow.number = GraphRow.UpperRow.number + 1;

                if (!GraphRow.IsIssue)
                {
                    Percent percent = GetPersent();
                    if (percent != null)
                    {
                        decimal yearPercent = percent.Percents * 100;
                        DateTime lastTranchDate = GraphSettings.Tranches.Count == 0 ? new DateTime(1,1,1) : GraphSettings.Tranches.Where(w => w.Date < GraphRow.Date).Max(m => m.Date);
                        GraphRow lastGraphRow;
                        decimal sumForAnnuit = 0;
                        int mounthCountForAnnuit = 0;
                        if (lastTranchDate != new DateTime(1,1,1))
                        {
                            lastGraphRow = GraphRow.UpperRow;                            
                            while (lastGraphRow != null)
                            {
                                if (lastGraphRow.Date.Date == GetWorkDate(lastTranchDate).Date)
                                {
                                    sumForAnnuit = lastGraphRow.RemainderMainSum;
                                    while (lastGraphRow != null && lastGraphRow.IsIssue)
                                    {
                                        mounthCountForAnnuit = Math.Abs((lastGraphRow.Date.Year * 12 + lastGraphRow.Date.Month) - (GraphSettings.EndDate.Year * 12 + GraphSettings.EndDate.Month));
                                        lastGraphRow = lastGraphRow.UpperRow;
                                    }
                                    break;
                                }

                                lastGraphRow = lastGraphRow.UpperRow;                                
                            }
                        }
                        if (GraphSettings.IssueType == Settings.IssueType.OneSum)
                        {
                            sumForAnnuit = GraphSettings.Sum;
                            mounthCountForAnnuit = GraphSettings.MounthCount;
                        }

                        decimal annuityMonth = GetAnnuitySum(sumForAnnuit, yearPercent, mounthCountForAnnuit); 

                        GraphRow.Date = GetWorkDate(GraphRow.Date);

                        decimal repaymentPercentDop = 0;
                        if(GraphSettings.IssueType == Settings.IssueType.ManyTranches)
                        {
                            lastGraphRow = GraphRow.UpperRow;
                            while (lastGraphRow != null)
                            {
                                if (lastGraphRow.IssueSum > 0 && (lastGraphRow).UpperRow != null)
                                {
                                    repaymentPercentDop = repaymentPercentDop + ((lastGraphRow).UpperRow.RemainderMainSum * yearPercent / 360 * Days360((lastGraphRow).UpperRow.Date, lastGraphRow.Date)) / 100;
                                    lastGraphRow = lastGraphRow.UpperRow;
                                }
                                else break;
                            }
                        }
                        decimal repaymentPercent = (GraphRow.UpperRow.RemainderMainSum * yearPercent / 360 * Days360(GraphRow.UpperRow.Date, GraphRow.Date)) / 100 + repaymentPercentDop;


                        int daysBetweenUpper = GetDaysBetweenUpper();                        

                        decimal repaymentMS = annuityMonth - repaymentPercent;
                        
                        if(GraphRow.IsLast)
                        {
                            repaymentMS = GraphRow.UpperRow.RemainderMainSum;
                        }

                        
                        GraphRow.RepaymentPercent = repaymentPercent;
                        GraphRow.RepaymentMainSum = repaymentMS;
                        GraphRow.RemainderMainSum = GraphRow.UpperRow.RemainderMainSum - repaymentMS;
                        GraphRow.Round = GraphSettings.Round;
                    }
                    else
                    {
                        GraphRow.Code = Codes.ERROR_NO_PERCENTS_FOUND;
                        GraphRow.ErrorMessage = "Не найдена процентная ставка на период или на сумму";
                    }
                }else
                {

                }
            }
        }

        public decimal GetPercentsForDay(decimal sum, decimal percents, decimal days)
        {
            return (sum * percents * days) / (100 * GetYearDays());
        }

        public int GetYearDays()
        {
            int yearDays = 360;

            if (GraphSettings != null)
            {

                switch (GraphSettings.Shema)
                {
                    case Shema.SH_360_30:
                    case Shema.SH_360_FACTUALLY:
                        break;
                    case Shema.SH_FACTUALLY_30:
                    case Shema.SH_FACTUALLY_FACTUALLY:
                        yearDays = new DateTime(DateTime.Now.Year, 12, 31).DayOfYear;
                        break;
                }
            }

            return yearDays;
        }

        /// <summary>
        /// Вычисляет ануитет для данной суммы
        /// </summary>
        /// <param name="sum">Сумма</param>
        /// <param name="percents">Проценты</param>
        /// <param name="mounthCount">Количество месяцев</param>
        /// <returns>Аннуитет</returns>
        public static double GetAnnuitySum(double sum, double percents, double mounthCount)
        {
            double i = (percents / 12) / 100;
            double a = Math.Pow((1 + i), mounthCount);
            return ((i * a) / (a - 1)) * sum;
        }
        /// <summary>
        /// Разсчитывает сумму аннуитета
        /// </summary>
        /// <param name="sum">на сумму</param>
        /// <param name="percents">проценты</param>
        /// <param name="mounthCount">количество месяцев</param>
        /// <returns>сумма аннуитета в месяц</returns>
        public static decimal GetAnnuitySum(decimal sum, decimal percents, decimal mounthCount)
        {
            decimal i = (percents / 12) / 100;
            double dA = Math.Pow(Convert.ToDouble((1 + i)), Convert.ToDouble(mounthCount));
            decimal a = Convert.ToDecimal(dA);
            return ((i * a) / (a - 1)) * sum;
        }

        public static decimal GetAnnuitySumDay(decimal sum, decimal percents, decimal mounthCount)
        {
            decimal dayPercent = (percents / 360) / 100;
            double dCoefficient = Math.Pow(Convert.ToDouble((1 + dayPercent)), Convert.ToDouble(mounthCount * 30));
            decimal coefficient = Convert.ToDecimal(dCoefficient);
            return ((dayPercent * coefficient) / (coefficient - 1)) * sum;
        }
       
    }
}