using System;
using System.Collections.Generic;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Entities
{
    /// <summary>
    /// Результат постройки графика
    /// </summary>
    public class GraphResult
    {
        public bool IsSuccess { set; get; }
        /// <summary>
        /// Код ответа
        /// </summary>
        public Codes Code { set; get; }
        /// <summary>
        /// Сообщение ответа
        /// </summary>
        public string Message { set; get; }

        /// <summary>
        /// Строки графика
        /// </summary>
        private GraphRowList graphRows = new GraphRowList();
        /// <summary>
        /// Test
        /// </summary>
        public GraphRowList GraphRows {
            get{
                return graphRows;
            }
        }
        
        public Exception Exception { set; get; }

        public static GraphResult Build(Codes code, string message, Exception exception = null)
        {
            return new GraphResult()
            {
                IsSuccess = code == Codes.OK,
                Code = code,
                Message = message,
                Exception = exception
            };
        }
    }
}
