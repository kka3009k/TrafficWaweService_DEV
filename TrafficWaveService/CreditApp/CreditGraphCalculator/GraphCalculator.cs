using TrafficWaveService.CreditApp.CreditGraphCalculator.Algorithm;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Listeners;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator
{
    public class GraphCalculator
    {
        public delegate void OnGraphCreated(GraphResult result);

        /// <summary>
        /// Пострoить график асинхронно
        /// при завершении вызывается метод из <see cref="OnGraphDone"/>
        /// </summary>
        /// <param name="creditSettings"></param>
        /// <param name="calback"></param>
        public static void ExecuteAsync(Setting creditSettings, OnGraphCreated calback)
        {
            GraphResult result = new GraphResult();
            var worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                result = new GraphCalculator().createGraph(creditSettings);
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                calback?.Invoke(result);
            };

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Пострoить график асинхронно
        /// при завершении вызывается метод из <see cref="OnGraphDone"/>
        /// </summary>
        /// <param name="creditSettings"></param>
        /// <param name="calback"></param>
        public static void ExecuteAsync(Setting creditSettings, OnGraphDone calback)
        {
            GraphResult result = new GraphResult();
            var worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                result = new GraphCalculator().createGraph(creditSettings);
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                calback?.OnDone(result);
            };

            worker.RunWorkerAsync();
        }
        /// <summary>
        /// Создает график на основе заданных настроек
        /// </summary>
        /// <param name="settings"></param>
        /// <returns><see cref="GraphResult"/></returns>
        public GraphResult createGraph(Setting settings)
        {
            GraphResult result = new GraphResult();
            if (settings != null)
            {
                switch (settings.GraphType)
                {
                    case GraphType.Annuity:
                        result = createAnnuity(settings);
                        break;
                    case GraphType.Individual:
                        result = createIndividual(settings);
                        break;
                    default:
                        result.IsSuccess = false;
                        result.Message = "Не известный тип графика";
                        result.Code = Codes.UNKNOWN_GRAPH_TYPE;
                        result.Exception = new Exception("Unknown graph type");
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// Расчет суммы ежемесячного платежа в случае если 
        /// выдача несколькими траншами
        /// </summary>
        /// <param name="result">результат графика</param>
        /// <param name="settings">настройки графика</param>
        /// <param name="remainderMS">Остаток основной суммы</param>
        /// <param name="tranch">Транш до которого должно расчитываться сумма</param>
        /// <param name="date">Дата для которой должна расчитыватся сумма</param>
        /// <param name="leftMonthCount">Количество оставшихся месяцев</param>
        /// <returns>Ежемесячный платёж</returns>
        private decimal getMontlyPaymentForTranch(GraphResult result, Setting settings, ref decimal remainderMS, Tranch tranch, DateTime date, decimal leftMonthCount = 0)
        {
            decimal remainderMSIfModifyed = settings.TranchesSum(tranch.Date);
            decimal monthlyPayment = 0;

            Tranch nextTranch = null;
            if (settings != null && tranch != null)
                //tr.Date.Year >= tranch.Date.Year && tr.Date.DayOfYear > tranch.Date.DayOfYear
                nextTranch = settings.Tranches.FirstOrDefault(tr => DateTime.Compare(tr.Date, tranch.Date) > 0);

            if (settings.IssueType == IssueType.ManyTranches)
            {
                foreach (GraphRow m in settings.ModifyedRows)
                {
                    /* Является ли дата этой измененнной строки меньше чем дата транша
                     * и нет ли еще одного транша после него
                     */

                    if (nextTranch != null)
                    {
                        if (DateTime.Compare(m.Date, tranch.Date) >= 0
                            && DateTime.Compare(m.Date, nextTranch.Date) < 0)
                            remainderMSIfModifyed -= m.RepaymentMainSum;
                    }
                    else
                    {
                        if (DateTime.Compare(m.Date, tranch.Date) >= 0)
                            remainderMSIfModifyed -= m.RepaymentMainSum;
                    }
                }
            }
            // Если есть строки измененные в ручную
            if (settings.ModifyedRows.Count > 0)
            {
                decimal beforeRepayedSum = 0;

                foreach (GraphRow r in result.GraphRows)
                {
                    if (!r.IsIssue && DateTime.Compare(r.Date, tranch.Date) < 0)
                        beforeRepayedSum += r.RepaymentMainSum;
                }

                if (nextTranch != null)
                {
                    monthlyPayment = (remainderMSIfModifyed - beforeRepayedSum) / (leftMonthCount - settings.ModifyedRows
                            .Where(m => DateTime.Compare(m.Date, tranch.Date) >= 0 && DateTime.Compare(m.Date, nextTranch.Date) < 0)
                            .Count());
                }
                else
                {
                    monthlyPayment = (remainderMSIfModifyed - beforeRepayedSum) / (leftMonthCount - settings.ModifyedRows
                            .Where(m => DateTime.Compare(m.Date, tranch.Date) >= 0)
                            .Count());
                }
            }
            else
                monthlyPayment = remainderMS / leftMonthCount;
            return monthlyPayment;
        }

        /// <summary>
        /// Расчёт ежемесячного платежа
        /// если дата не null то расчет 
        /// ежемесячного платежа на дату
        /// </summary>
        /// <param name="settings">Настройки кредита</param>
        /// <returns>Ежемесячный платёж</returns>
        private decimal getMonthlyPaymentForOneSum(Setting settings, ref decimal remainderMS, decimal leftMonthCount = 0, DateTime? date = null)
        {
            decimal remainderMSForModifyed = settings.Sum;
            decimal monthlyPayment = settings.Sum / settings.MounthCount;

            if (settings.IssueType == IssueType.OneSum)
            {
                remainderMSForModifyed = settings.Sum;
                settings.ModifyedRows.ForEach(
                    m =>
                    {
                        remainderMSForModifyed -= m.RepaymentMainSum;
                    });
            }

            if (settings.ModifyedRows.Count > 0)
            {
                monthlyPayment = remainderMSForModifyed / (settings.MounthCount - settings.ModifyedRows.Count);
            }
            else
                monthlyPayment = remainderMS / leftMonthCount;

            return monthlyPayment;
        }

        /// <summary>
        /// Добавить транши в результат
        /// </summary>
        /// <param name="result"></param>
        /// <param name="settings"></param>
        /// <param name="date"></param>
        /// <param name="numberation"></param>
        /// <param name="remainderMS"></param>
        /// <param name="monthlyPayment"></param>
        /// <param name="mountsCounter"></param>
        private void addTranch(GraphResult result, Setting settings, DateTime date,
            ref int numberation, ref decimal remainderMS,
            ref decimal monthlyPayment, decimal mountsCounter)
        {
            Func<Tranch, bool> isTranch = new Func<Tranch, bool>(tr => tr.Date.Year == date.Year && tr.Date.DayOfYear == date.DayOfYear);

            Tranch tranch = settings.Tranches.FirstOrDefault(isTranch);
            if (settings.IssueType == IssueType.ManyTranches && tranch != null)
            {
                result.GraphRows.Add(new GraphRow()
                {
                    number = numberation,
                    IsIssue = true,
                    Date = GraphUtils.GetNextWorkDate(date, settings.NotWorkDays),
                    IssueSum = tranch.Sum,
                    Round = settings.Round
                });
                remainderMS += tranch.Sum;
                monthlyPayment = getMontlyPaymentForTranch(result, settings, ref remainderMS, tranch, GraphUtils.GetNextWorkDate(date, settings.NotWorkDays), mountsCounter);
                numberation++;
            }
        }

        /// <summary>
        /// Добавить строку в график. Для графика "один раз в месяц"
        /// </summary>
        /// <param name="result"></param>
        /// <param name="settings"></param>
        /// <param name="date"></param>
        /// <param name="repaymentPercent"></param>
        /// <param name="numberation"></param>
        /// <param name="remainderMS"></param>
        /// <param name="monthlyPayment"></param>
        /// <param name="mountsCounter"></param>
        private void addOneTimeGraphInMonth(GraphResult result, Setting settings, DateTime date,
            decimal repaymentPercent,
            ref int numberation, ref decimal remainderMS,
            ref decimal monthlyPayment, ref decimal mountsCounter)
        {
            if (date.Day == DateTime.DaysInMonth(date.Year, date.Month))
            {
                DateTime workDate = GraphUtils.GetNextWorkDate(date, settings.NotWorkDays);

                if (settings.ModifyedRows.Any(mr => mr.Date == workDate && mr.IsModifyed))
                {
                    GraphRow row = settings.ModifyedRows.FirstOrDefault(mr => mr.Date == workDate && mr.IsModifyed);
                    remainderMS -= row.RepaymentMainSum;

                    row.number = numberation;
                    row.Round = settings.Round;
                    row.RepaymentPercent = repaymentPercent;
                    row.RemainderMainSum = remainderMS;
                    result.GraphRows.Add(row);
                }
                else
                {
                    remainderMS -= monthlyPayment;

                    GraphRow row = new GraphRow()
                    {
                        number = numberation,
                        Round = settings.Round,
                        Date = workDate,
                        RemainderMainSum = remainderMS,
                        RepaymentMainSum = monthlyPayment,
                        RepaymentPercent = repaymentPercent,

                    };
                    result.GraphRows.Add(row);
                }
                numberation++;
                mountsCounter--;
            }
        }
        /// <summary>
        /// Добавить в график две сроки 
        /// разделив месячный платеж на два
        /// </summary>
        /// <param name="result"></param>
        /// <param name="settings"></param>
        /// <param name="date"></param>
        /// <param name="repaymentPercent"></param>
        /// <param name="numberation"></param>
        /// <param name="remainderMS"></param>
        /// <param name="monthlyPayment"></param>
        /// <param name="mountsCounter"></param>
        private void addTwoTimeInMonth(GraphResult result, Setting settings, DateTime date,
            decimal repaymentPercent,
            ref int numberation, ref decimal remainderMS,
            decimal monthlyPayment, ref decimal mountsCounter)
        {
            // Существует ли уже в исправленных в ручную
            GraphRow mRow = settings.ModifyedRows.FirstOrDefault(mr => mr.Date.Date == date.Date);
            if (mRow != null)
            {
                remainderMS -= mRow.RepaymentMainSum;
                mRow.RemainderMainSum = remainderMS;
                mRow.number = numberation;
                mRow.RepaymentPercent = repaymentPercent;

                result.GraphRows.Add(mRow);
                numberation++;
                if (!mRow.IsMonthMiddle)
                    mountsCounter--;
            }
            else
            {
                if (date.Day == 15)
                {
                    remainderMS -= monthlyPayment / 2;
                    result.GraphRows.Add(new GraphRow()
                    {
                        number = numberation,
                        Round = settings.Round,
                        IsMonthMiddle = true,
                        Date = GraphUtils.GetNextWorkDate(date, settings.NotWorkDays),
                        RemainderMainSum = remainderMS,
                        RepaymentMainSum = monthlyPayment / 2,
                        RepaymentPercent = repaymentPercent / 2
                    });
                    numberation++;
                }
                if (date.Day == DateTime.DaysInMonth(date.Year, date.Month))
                {
                    remainderMS -= monthlyPayment / 2;
                    result.GraphRows.Add(new GraphRow()
                    {
                        number = numberation,
                        Round = settings.Round,
                        Date = GraphUtils.GetNextWorkDate(date, settings.NotWorkDays),
                        RemainderMainSum = remainderMS,
                        RepaymentMainSum = monthlyPayment / 2,
                        RepaymentPercent = repaymentPercent / 2
                    });
                    numberation++;
                    mountsCounter--;
                }
            }
        }

        private GraphResult createIndividual(Setting settings)
        {
            GraphResult result = new GraphResult();

            GraphRow issueRow = new GraphRow();

            issueRow.number = 0;
            issueRow.Date = GraphUtils.GetNextWorkDate(settings.IssueDate, settings.NotWorkDays);
            issueRow.IssueSum = settings.Sum;
            issueRow.RemainderMainSum = settings.Sum;
            issueRow.Round = settings.Round;

            Func<Tranch, bool> isTranch = new Func<Tranch, bool>(tr => GraphUtils.GetNextWorkDate(tr.Date.Date, settings.NotWorkDays) == GraphUtils.GetNextWorkDate(issueRow.Date.Date, settings.NotWorkDays));
            Tranch tranch = settings.Tranches.FirstOrDefault(isTranch);
            if(settings.IssueType == IssueType.ManyTranches && tranch != null)
            {
                issueRow.IssueSum = tranch.Sum;
                issueRow.RemainderMainSum = tranch.Sum;
            }

            result.GraphRows.Add(issueRow);

            int monthCount = settings.MounthCount - ((Math.Abs(MounthIssueFirstPay(settings)) > 1 && settings.RepayType != RepayType.TwoTimeInMonth) ? Math.Abs(MounthIssueFirstPay(settings))-1 : 0);

            if (settings.RepayType == RepayType.TwoTimeInMonth)
            {
                monthCount += monthCount;
                monthCount = monthCount - ((settings.FirstRepayDate.Date - settings.IssueDate.Date).TotalDays > 15 ? 
                    Convert.ToInt32((settings.FirstRepayDate.Date - settings.IssueDate.Date).TotalDays / 15 - 1) : 0);
            }

            int privilegyMS = settings.PrivelegyMainSum;
            int privilegyPerc = settings.PrivelegyPersent;

            for (int i = 0; i < monthCount; i++)
            {
                GraphRow row = new GraphRow();
                row.Algorithm = new IndividualAlgorithm(settings);

                DateTime nextMonth;

                if (i == 0 || settings.RepayType != RepayType.TwoTimeInMonth) nextMonth = settings.FirstRepayDate.AddMonths(i);
                else nextMonth = result.GraphRows.Max(m => m.Date);

                if (i == 0)
                {
                    row.Date = nextMonth;
                }else
                {

                    if (settings.RepayType == RepayType.TwoTimeInMonth)
                    {                       
                        if (result.GraphRows.Where(w => w.Date.Year == nextMonth.Year && w.Date.Month == nextMonth.Month).Count() == 2 || 
                            nextMonth.Month != GraphUtils.GetNextWorkDate(nextMonth.AddDays(1), settings.NotWorkDays).Month)
                            nextMonth = nextMonth.AddDays(15);

                        if (result.GraphRows.Where(w => w.Date.Year == nextMonth.Year && w.Date.Month == nextMonth.Month).Count() == 0)
                            nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 15);
                        if (result.GraphRows.Where(w => w.Date.Year == nextMonth.Year && w.Date.Month == nextMonth.Month).Count() == 1)
                            nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
                    }
                    row.Date = nextMonth;
                }

                if (i == monthCount - 1)
                {
                    if(settings.RepayType != RepayType.TwoTimeInMonth) row.Date = settings.EndDate;
                    row.IsLast = true;
                }

                if(settings.Tranches.FirstOrDefault(tr => tr.Date.Date > result.GraphRows.LastOrDefault().Date.Date && 
                    GraphUtils.GetNextWorkDate(tr.Date.Date, settings.NotWorkDays) == GraphUtils.GetNextWorkDate(row.Date.Date, settings.NotWorkDays)) != null)
                {
                    result.Code = Codes.SETTINGS_IS_NULL;
                    result.Message = "Дата выдачи транша не должна совпадать с датой погашения!";
                    result.Exception = new ArgumentException("Неправильная дата транша!");
                    result.IsSuccess = false;
                    return result;
                }

                Func<Tranch, bool> isTranch2 = new Func<Tranch, bool>(tr => tr.Date.Date > result.GraphRows.LastOrDefault().Date.Date && tr.Date.Date < row.Date.Date);
                Tranch tranch2 = settings.Tranches.FirstOrDefault(isTranch2);

                if(settings.IssueType == IssueType.ManyTranches && tranch2 != null)
                {
                    result.GraphRows.Add(new GraphRow()
                    {
                        IsIssue = true,
                        Date = GraphUtils.GetNextWorkDate(tranch2.Date, settings.NotWorkDays),
                        IssueSum = tranch2.Sum,
                        Round = settings.Round,
                        RemainderMainSum = result.GraphRows.LastOrDefault() == null ? tranch2.Sum : result.GraphRows.LastOrDefault().RemainderMainSum + tranch2.Sum,
                        Algorithm = null
                    });
                    row.Algorithm = null;
                    i--;
                }

                row.IsMSPrivilegy = privilegyMS > 0;
                row.IsPercPrivilegy = privilegyPerc > 0;

                privilegyMS--;
                privilegyPerc--;

                if (settings.RepayType == RepayType.OnceQuarter && settings.MounthCount >= 3 && (i + 1) % 3 != 0 && i != (monthCount - 1))
                    row.IsMSPrivilegy = true;
                if (settings.RepayType == RepayType.OnceQuarter && settings.MounthCount >= 3 && (i + 1) % 3 == 0)
                    row.IsMSPrivilegy = false;
                if (settings.RepayType == RepayType.HalfYearly && settings.MounthCount >= 6 && (i + 1) % 6 != 0 && i != (monthCount - 1))
                    row.IsMSPrivilegy = true;
                if (settings.RepayType == RepayType.HalfYearly && settings.MounthCount >= 6 && (i + 1) % 6 == 0)
                    row.IsMSPrivilegy = false;
                if (settings.RepayType == RepayType.Yearly && settings.MounthCount >= 12 && (i + 1) % 12 != 0 && i != (monthCount - 1))
                    row.IsMSPrivilegy = true;
                if (settings.RepayType == RepayType.Yearly && settings.MounthCount >= 12 && (i + 1) % 12 == 0)
                    row.IsMSPrivilegy = false;

                if(tranch2 == null)
                result.GraphRows.Add(row);

                if (row.Algorithm != null)
                    row.Algorithm.Calculate();

                if (row.Code != Codes.OK)
                {
                    result.Code = row.Code;
                    result.Message = row.ErrorMessage;
                }
            }

            result.IsSuccess = true;

            return result;
        }

        /// <summary>
        /// Создать индивидуальный график
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <returns>Резултат графика</returns>
        private GraphResult createIndividualOld(Setting settings)
        {
            GraphResult result = new GraphResult();
            if (settings == null)
                return GraphResult.Build(Codes.SETTINGS_IS_NULL, "Настройки графика пусты", new ArgumentNullException("Settings is null"));

            int numberation = 1;
            decimal remainderMS = 0;

            if (settings.IssueType == IssueType.OneSum)
            {
                remainderMS = settings.Sum;
                result.GraphRows.Add(new GraphRow()
                {
                    number = numberation,
                    Date = settings.IssueDate,
                    IssueSum = settings.Sum,
                    Round = settings.Round
                });
                numberation++;
            }

            decimal mountsCounter = settings.MounthCount;
            decimal monthlyPayment = getMonthlyPaymentForOneSum(settings, ref remainderMS, mountsCounter);


            for (int i = 0; i < settings.TotalDays(); i++)
            {
                DateTime date = settings.IssueDate.AddDays(i);
                Percent percent = settings.Percents.FirstOrDefault(p => p.DateStart <= date && p.DateEnd >= date);

                if (percent != null)
                {
                    addTranch(result, settings, date, ref numberation, ref remainderMS, ref monthlyPayment, mountsCounter);

                    if (Decimal.Round(remainderMS, 1) >= 1)
                    {
                        decimal repaymentPercent = (remainderMS * ((percent.Percents * 100) / 12)) / 100;

                        if (settings.RepayType == RepayType.OneTimeInMonth)
                        {
                            addOneTimeGraphInMonth(result, settings, date, repaymentPercent, ref numberation, ref remainderMS, ref monthlyPayment, ref mountsCounter);
                        }
                        else
                        {
                            addTwoTimeInMonth(result, settings, date, repaymentPercent, ref numberation, ref remainderMS, monthlyPayment, ref mountsCounter);
                        }
                    }
                    else if (Convert.ToInt32(remainderMS) == 0)
                    {
                        result.Code = Codes.OK;
                        result.IsSuccess = true;
                        return result;
                    }
                    else if (Convert.ToInt32(remainderMS) < 0)
                    {
                        result.Code = Codes.MAINSUM_IS_NEGATIV;
                        result.Message = "Оснавная сумма ушла ниже нуля. Проверьте введенные данные";
                        return result;
                    }
                }
                else
                {
                    result.Code = Codes.ERROR_NO_PERCENTS_FOUND;
                    result.Message = "Не были были установлены проценты на дату: " + date.ToShortDateString();
                    result.Exception = new ArgumentException("Percent not found");
                    return result;
                }
            }
            return result;
        }

        private GraphResult createAnnuity(Setting settings)
        {
            GraphResult result = new GraphResult();

            int numbering = 0;
            GraphRow issueRow = createIssueRow(numbering, settings);
            result.GraphRows.Add(issueRow);
            
            for (int i = 0; i < settings.MounthCount; i++)
            {
                GraphRow row = new GraphRow();
                row.Date = settings.FirstRepayDate.AddMonths(i);
                row.Algorithm = new AnnuityAlgorithm(settings);


                // Если последняя строка указать дату окончания
                if (i == settings.MounthCount - 1)
                {
                    row.Date = settings.EndDate;
                    row.IsLast = true;
                }

                if(settings.Tranches.FirstOrDefault(tr => tr.Date.Date > result.GraphRows.LastOrDefault().Date.Date && 
                        GraphUtils.GetNextWorkDate(tr.Date.Date, settings.NotWorkDays) == GraphUtils.GetNextWorkDate(row.Date.Date, settings.NotWorkDays)) != null)
                {                    
                    result.Code = Codes.SETTINGS_IS_NULL;
                    result.Message = "Дата выдачи транша не должна совпадать с датой погашения!";
                    result.Exception = new ArgumentException("Неправильная дата транша!");
                    result.IsSuccess = false;
                    return result;
                }

                Func<Tranch, bool> isTranch = new Func<Tranch, bool>(tr => 
                                    tr.Date.Date > result.GraphRows.LastOrDefault().Date.Date && tr.Date.Date < row.Date.Date);

                Tranch tranch = settings.Tranches.FirstOrDefault(isTranch);

                if (settings.IssueType == IssueType.ManyTranches && tranch != null)
                {
                    result.GraphRows.Add(new GraphRow()
                    {
                        number = numbering,
                        IsIssue = true,
                        Date = GraphUtils.GetNextWorkDate(tranch.Date, settings.NotWorkDays),
                        IssueSum = tranch.Sum, 
                        Round = settings.Round,
                        RemainderMainSum = result.GraphRows.LastOrDefault() == null ? tranch.Sum : result.GraphRows.LastOrDefault().RemainderMainSum + tranch.Sum,
                        Algorithm = null                   
                    });
                    
                    numbering++;
                    row.Algorithm = null;
                    i--;
                }
                
                if(tranch == null) result.GraphRows.Add(row);                

                if (row.Algorithm != null)
                    row.Algorithm.Calculate();

                if (row.Code != Codes.OK)
                {
                    result.Code = row.Code;
                    result.Message = row.ErrorMessage;
                }
            numbering++;
            }

            result.IsSuccess = true;
            return result;
        }

        private GraphRow createIssueRow(int number, Setting settings)
        {
            GraphRow issueRow = new GraphRow();

            issueRow.number = number;
            issueRow.Date = GraphUtils.GetNextWorkDate(settings.IssueDate, settings.NotWorkDays);
            issueRow.IssueSum = settings.Sum;
            issueRow.RemainderMainSum = settings.Sum;
            issueRow.Round = settings.Round;

            Func<Tranch, bool> isTranch = new Func<Tranch, bool>(tr => tr.Date.Year == issueRow.Date.Year && tr.Date.DayOfYear == issueRow.Date.DayOfYear);

            Tranch tranch = settings.Tranches.FirstOrDefault(isTranch);

            if (settings.IssueType == IssueType.ManyTranches && tranch != null)
            {
                issueRow = (new GraphRow()
                {
                    number = 0,
                    IsIssue = true,
                    Date = GraphUtils.GetNextWorkDate(issueRow.Date, settings.NotWorkDays),
                    IssueSum = tranch.Sum,
                    Round = settings.Round,
                    RemainderMainSum = tranch.Sum,
                    Algorithm = new AnnuityAlgorithm(settings)
                });                
            }

            return issueRow;
        }

        private int MounthIssueFirstPay(Setting settings)
        {
            if (settings != null)
            {
                return ((settings.IssueDate.Year - settings.FirstRepayDate.Year) * 12) + settings.IssueDate.Month - settings.FirstRepayDate.Month;
            }
            return 0;
        }


        /// <summary>
        /// Построить аннуитетный график
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private GraphResult createAnnuityOld(Setting settings)
        {
            GraphResult result = new GraphResult();
            if (settings == null)
                return GraphResult.Build(Codes.SETTINGS_IS_NULL, "Настройки графика пусты", new ArgumentNullException("Settings is null"));

            decimal remainderMS = settings.Sum;

            int numberation = 1;

            if (settings.IssueType == IssueType.OneSum)
            {
                result.GraphRows.Add(new GraphRow()
                {
                    number = numberation,
                    IsIssue = true,
                    Date = settings.IssueDate,
                    IssueSum = settings.Sum
                });
                numberation++;
            }

            for (int i = 0; i < settings.TotalDays(); i++)
            {
                DateTime date = settings.IssueDate.AddDays(i).Date;     //Акылбек
                int monthDays = DateTime.DaysInMonth(date.Year, date.Month);

                Percent percent = settings.Percents.FirstOrDefault(p => p.DateStart <= date && p.DateEnd >= date);
                // TODO: check is new Percent
                if (percent != null)
                {
                    Func<Tranch, bool> isTranch = new Func<Tranch, bool>(tr => tr.Date.Year == date.Year && tr.Date.DayOfYear == date.DayOfYear);

                    Tranch tranch = settings.Tranches.FirstOrDefault(isTranch);

                    if (settings.IssueType == IssueType.ManyTranches && tranch != null)
                    {
                        result.GraphRows.Add(new GraphRow()
                        {
                            number = numberation,
                            IsIssue = true,
                            Date = GraphUtils.GetNextWorkDate(date, settings.NotWorkDays),
                            IssueSum = tranch.Sum,
                            Round = settings.Round
                        });
                        remainderMS += tranch.Sum;
                        numberation++;
                    }

                    decimal yearPercent = percent.Percents * 100;

                    decimal annuityMonth = GetAnnuitySum(settings.Sum, yearPercent, settings.MounthCount);

                    if (Decimal.Round(remainderMS, 1) > 0)
                    {
                        decimal repaymentPercent = (remainderMS * yearPercent / 12) / 100;
                        if (date.Day == monthDays)
                        {
                            remainderMS -= annuityMonth - repaymentPercent;
                            result.GraphRows.Add(new GraphRow()
                            {
                                number = numberation,
                                Round = settings.Round,
                                Date = GraphUtils.GetNextWorkDate(date, settings.NotWorkDays),
                                RemainderMainSum = remainderMS,
                                RepaymentMainSum = annuityMonth - repaymentPercent,
                                RepaymentPercent = repaymentPercent
                            });
                            numberation++;
                        }
                    }
                    else if (Convert.ToInt32(remainderMS) == 0)
                    {
                        result.IsSuccess = true;
                        result.Code = Codes.OK;
                        return result;
                    }
                    else if (Convert.ToInt32(remainderMS) < 0)
                    {
                        result.Code = Codes.MAINSUM_IS_NEGATIV;
                        result.Message = "Осн0вная сумма ушла ниже нуля. Проверьте введенные данные";
                    }
                }
                else
                {
                    result.Code = Codes.ERROR_NO_PERCENTS_FOUND;
                    result.Message = "Не были установлены проценты на дату: " + date.ToShortDateString();
                    result.Exception = new ArgumentException("Percent not found");
                    result.IsSuccess = false;

                    return result;
                }
            }
            return result;
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
    }
}
