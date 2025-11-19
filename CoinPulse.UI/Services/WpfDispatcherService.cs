using System;
using System.Windows;

namespace CoinPulse.UI.Services;

public class WpfDispatcherService : IDispatchedService
{
    public void Invoke(Action action)
    {
        if (Application.Current?.Dispatcher != null)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
        else
        {
            action();
        }
    }
}