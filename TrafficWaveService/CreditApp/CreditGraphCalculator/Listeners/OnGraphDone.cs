using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;

namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Listeners
{
    public interface OnGraphDone
    {
        void OnDone(GraphResult Response);
    }
}
