using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Listeners
{
    /// <summary>
    /// Слушатель изменений в графике.
    /// <see cref="Observers.IGraphRowObservable"/>
    /// </summary>
    public interface IGraphRowListener
    {
        /// <summary>
        /// Вызывается при изменнеи поля
        /// <see cref="GraphRow.RemainderMainSum"/>
        /// </summary>
        /// <param name="row"><see cref="GraphRow"/> Измененная строка</param>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
        void GraphRowRepaymentMSChanged(GraphRow row, decimal oldValue, decimal newValue);
    }
}
