using System;
using System.Net;
using System.Windows;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator
{
    public enum Codes
    {
        OK = 100,
        SETTINGS_IS_NULL = 101,
        UNKNOWN_GRAPH_TYPE = 102,

        //******************************************************
        // ОШИБКИ ( 4000 - 4999)
        //******************************************************

        /// <summary>
        /// Не найден процент для некоторых дней
        /// </summary>
        ERROR_NO_PERCENTS_FOUND = 4001,


        //*******************************************************
        // КОДЫ ПРЕДУПРЕЖДЕНИЙ ДИАПАЗОН 5000 - 5100
        //*******************************************************

        /// <summary>
        /// Основная сумма ушла ниже 0
        /// </summary>
        MAINSUM_IS_NEGATIV = 5001
    }
}
