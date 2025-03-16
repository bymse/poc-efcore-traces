using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Telemetry;

public class EFCoreDiagnosticSourceObserver : IDisposable, IObserver<DiagnosticListener>
{
    private readonly Lock subscriptionLock = new();
    private IDisposable? subscription;
    private IDisposable? allSourcesSubscription;
    private long disposed;

    public void Subscribe()
    {
        allSourcesSubscription ??= DiagnosticListener.AllListeners.Subscribe(this);
    }

    public void OnNext(DiagnosticListener value)
    {
        if (Interlocked.Read(ref disposed) == 0 && value.Name == DbLoggerCategory.Name)
        {
            lock (subscriptionLock)
            {
                subscription?.Dispose();
                subscription = value.Subscribe(new EFCoreDiagnosticEventsObserver());
            }
        }
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref disposed, 1, 0) == 1)
        {
            return;
        }

        lock (subscriptionLock)
        {
            subscription?.Dispose();
        }


        allSourcesSubscription?.Dispose();
        GC.SuppressFinalize(this);
    }
}