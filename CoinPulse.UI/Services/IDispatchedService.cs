using System;

namespace CoinPulse.UI.Services;

public interface IDispatchedService
{
    void Invoke(Action action);
}