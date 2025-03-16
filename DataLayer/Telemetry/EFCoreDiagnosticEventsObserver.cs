namespace DataLayer.Telemetry;

internal class EFCoreDiagnosticEventsObserver : IObserver<KeyValuePair<string, object?>>
{
    public void OnNext(KeyValuePair<string, object?> value)
    {
        var name = value.Key;
        var payload = value.Value;
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }
}