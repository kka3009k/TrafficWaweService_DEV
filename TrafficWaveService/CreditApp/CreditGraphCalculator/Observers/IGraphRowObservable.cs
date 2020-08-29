using TrafficWaveService.CreditApp.CreditGraphCalculator.Entities;
using TrafficWaveService.CreditApp.CreditGraphCalculator.Listeners;
using System;
using System.Net;
using System.Windows;


namespace TrafficWaveService.CreditApp.CreditGraphCalculator.Observers
{
    public interface IGraphRowObservable
    {
        void AddListener(IGraphRowListener listener);
        void RemoveListener(IGraphRowListener listener);
        void NotifyRepaymentMSChanged(GraphRow row, decimal oldValue, decimal newValue);
    }
}
