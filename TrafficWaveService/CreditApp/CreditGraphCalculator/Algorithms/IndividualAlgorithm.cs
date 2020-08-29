using TrafficWaveService.CreditApp.CreditGraphCalculator.Algorithm;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Settings;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Algorithm
{
    public class IndividualAlgorithm : AlgorithmAbstract
    {
        
        public IndividualAlgorithm() { }

        public IndividualAlgorithm(Settings.Setting settings):base(settings) { }

        /// <summary>
        /// Расчёт строки индивидуального графика
        /// </summary>
        public override void Calculate()
        {
            if(GraphRow != null)
            {
                GraphRow.number = GraphRow.UpperRow.number + 1;
                
                if (GraphSettings.RepayType == RepayType.TwoTimeInMonth) GraphRow.Date = GetPreviousWorkDate(GraphRow.Date);
                else GraphRow.Date = GetWorkDate(GraphRow.Date);

                if (!GraphRow.IsIssue)
                {
                    Percent percent = GetPersent();
                    if (percent != null)
                    {
                        if (!GraphRow.IsModifyed)
                        {
                            decimal yearPercent = percent.Percents * 100;

                            GraphRow lastGraphRow;
                            decimal sumForIndiv = 0;
                            
                            DateTime lastTranchDate = GraphSettings.Tranches.Count == 0 ? new DateTime(1, 1, 1) : GraphSettings.Tranches.Where(w => w.Date < GraphRow.Date).Max(m => m.Date);
                            if(lastTranchDate != new DateTime(1,1,1))
                            {
                                lastGraphRow = GraphRow.UpperRow;
                                if(lastGraphRow != null && lastGraphRow.Date.Date == GetWorkDate(lastTranchDate).Date)
                                {
                                    sumForIndiv = lastGraphRow.RemainderMainSum;
                                }
                            }

                            decimal repaymentPercent = (GraphSettings.PrivelegyPersent > 0 && GraphSettings.PrivelegyPersent + 1 == GraphRow.number) ? GetRepaymentPercents(GraphRow, GraphSettings.PrivelegyPersent, percent) :
                                (GraphRow.UpperRow.RemainderMainSum * ((percent.Percents * 100) / 360 * Days360(GraphRow.UpperRow.Date, GraphRow.Date))) / 100;
                            decimal repaymentMS = GraphSettings.Sum / (GetPaymentCount(GraphSettings) - (Math.Abs(GetMounthInIssueFirstPay())>1 ? Math.Abs(GetMounthInIssueFirstPay())-1 : 0));

                            if (GraphSettings.RepayType == RepayType.TwoTimeInMonth)
                                repaymentMS = GraphSettings.Sum / (GetPaymentCount(GraphSettings) - ((GraphSettings.FirstRepayDate.Date - GraphSettings.IssueDate.Date).TotalDays > 15 ?
                                    Convert.ToInt32((GraphSettings.FirstRepayDate.Date - GraphSettings.IssueDate.Date).TotalDays / 15 - 1) : 0));

                            if (GraphSettings.RepayType == RepayType.OnceQuarter && GraphRow.IsMSPrivilegy == false && GraphRow.number != 0)
                                repaymentMS = (GraphRow.number % 3) == 0 ? repaymentMS * 3 : repaymentMS * (GraphRow.number % 3);
                            if (GraphSettings.RepayType == RepayType.HalfYearly && GraphRow.IsMSPrivilegy == false && GraphRow.number != 0)
                                repaymentMS = (GraphRow.number % 6) == 0 ? repaymentMS * 6 : repaymentMS * (GraphRow.number % 6);
                            if (GraphSettings.RepayType == RepayType.Yearly && GraphRow.IsMSPrivilegy == false && GraphRow.number != 0)
                                repaymentMS = (GraphRow.number % 12) == 0 ? repaymentMS * 12 : repaymentMS * (GraphRow.number % 12);

                            //Акыл
                            if (GraphRow.number >= GraphSettings.FixedPaySince && GraphRow.number <= GraphSettings.FixedPayBefore)
                            {
                                if (GraphSettings.SumFixedPay <= repaymentPercent)
                                {
                                    repaymentMS = 0;
                                }
                                else
                                {
                                    repaymentMS = GraphRow.UpperRow.RemainderMainSum >= (GraphSettings.SumFixedPay - repaymentPercent) ? (GraphSettings.SumFixedPay - repaymentPercent) : GraphRow.UpperRow.RemainderMainSum; 
                                }
                            }

                            if (GraphSettings.FixedPayBefore > 0 && GraphRow.number > GraphSettings.FixedPayBefore)
                            {
                                GraphRow gr = GraphRow.UpperRow;
                                while (gr.number > GraphSettings.FixedPayBefore)
                                {
                                    gr = gr.UpperRow;
                                }
                                repaymentMS = gr.RemainderMainSum / ((GetPaymentCount(GraphSettings) + GraphSettings.PrivelegyMainSum - (Math.Abs(GetMounthInIssueFirstPay()) > 1 ? Math.Abs(GetMounthInIssueFirstPay()) - 1 : 0)) - GraphSettings.FixedPayBefore);
                                if (GraphSettings.RepayType == RepayType.TwoTimeInMonth)
                                {
                                    repaymentMS = gr.RemainderMainSum / ((GetPaymentCount(GraphSettings) + GraphSettings.PrivelegyMainSum - ((GraphSettings.FirstRepayDate.Date - GraphSettings.IssueDate.Date).TotalDays > 15 ?
                                        Convert.ToInt32((GraphSettings.FirstRepayDate.Date - GraphSettings.IssueDate.Date).TotalDays / 15 - 1) : 0)) - GraphSettings.FixedPayBefore);
                                }
                            }
                            //

                            if (GraphRow.IsLast)
                            {
                                repaymentMS = GraphRow.UpperRow.RemainderMainSum;
                            }

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

                            GraphRow.RepaymentPercent = GraphRow.IsPercPrivilegy ? 0 : repaymentPercent + repaymentPercentDop;
                            GraphRow.RepaymentMainSum = GraphRow.IsMSPrivilegy ? 0 : repaymentMS;
                            if (GraphRow.IsMSPrivilegy)
                            {
                                GraphRow.RepaymentMainSum = 0;
                                GraphRow.RemainderMainSum = GraphRow.UpperRow.RemainderMainSum;
                            }
                            else
                            {
                                GraphRow.RemainderMainSum = GraphRow.UpperRow.RemainderMainSum - repaymentMS;
                                GraphRow.RepaymentMainSum = repaymentMS;
                            }
                            GraphRow.Round = GraphSettings.Round;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        GraphRow.Code = Codes.ERROR_NO_PERCENTS_FOUND;
                        GraphRow.ErrorMessage = "Не найдена процентная ставка на период или на сумму";
                    }
                }
            }
        }

        public decimal GetRepaymentPercents(GraphRow gr, int periodPrivelPerc, Percent perc)
        {
            decimal res = 0;
            for(int i=0; i<=periodPrivelPerc; i++)
            {
                res = res + (gr.UpperRow.RemainderMainSum * ((perc.Percents * 100) / 360 * Days360(gr.UpperRow.Date, gr.Date))) / 100;
                gr = gr.UpperRow;
            }
            return res;
        }
    }
}