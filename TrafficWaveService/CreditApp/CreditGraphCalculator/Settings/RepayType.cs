namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Settings
{
    /// <summary>
    /// Тип погашения (Один раз в месяц, два раза)
    /// </summary>
    public enum RepayType
    {
        /// <summary>
        /// Один раз в месяц
        /// </summary>
        OneTimeInMonth = 1,
        /// <summary>
        /// Два раза в месяц
        /// </summary>
        TwoTimeInMonth = 2,

        /// <summary>
        /// Раз в квартал
        /// </summary>
        OnceQuarter = 3,

        /// <summary>
        /// Раз в полгода
        /// </summary>
        HalfYearly = 4,

        /// <summary>
        /// Раз в год
        /// </summary>
        Yearly = 5
    }
}
