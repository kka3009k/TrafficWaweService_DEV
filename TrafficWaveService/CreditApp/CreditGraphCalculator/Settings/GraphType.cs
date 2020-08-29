using TrafficWaveService.CreditApp.CreditGraphCalculator.Attributes;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Settings
{
    /// <summary>
    /// Тип кредита (Аннуитет, Индивидуальный)
    /// </summary>
    public enum GraphType
    {
        /// <summary>
        /// Аннуитет
        /// </summary>
        [AttrDisplayName("Аннуитет")]
        Annuity = 5,

        /// <summary>
        /// Индивидуальный
        /// </summary>
        [AttrDisplayName("Индивидуальный")]
        Individual = 1
    }
}
