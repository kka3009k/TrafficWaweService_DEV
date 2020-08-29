using System;
using System.Net;
using System.Windows;


namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Entities
{
    /// <summary>
    /// Схема начисления
    /// 360/30 |
    /// 360/Фактически |
    /// Фактически/30 |
    /// Фактически/Фактически
    /// </summary>
    public enum Shema
    {
        /// <summary>
        /// 360/30
        /// </summary>
        SH_360_30,
        /// <summary>
        /// 360/Фактически
        /// </summary>
        SH_360_FACTUALLY,
        /// <summary>
        /// Фактически/30 
        /// </summary>
        SH_FACTUALLY_30,
        /// <summary>
        /// Фактически/Фактически
        /// </summary>
        SH_FACTUALLY_FACTUALLY
    }
}
