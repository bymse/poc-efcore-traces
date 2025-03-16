namespace DataLayer.Telemetry;

public static class OperationNameContainer
{
    private static readonly AsyncLocal<string?> AsyncLocalName = new();

    public static string? OperationName
    {
        get => AsyncLocalName.Value;
        set => AsyncLocalName.Value = value;
    }
}