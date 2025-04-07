using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace DataLayer.Telemetry;

internal class EFCoreDiagnosticEventsObserver : IObserver<KeyValuePair<string, object?>>
{
    private const string ACTIVITY_NAME = "EFCoreDiagnosticActivity";
    private const string DB_SYSTEM_NAME = "db.system.name";
    private const string DB_OPERATION_NAME = "db.operation.name";
    private const string DB_STATUS_CODE = "db.response.status_code";
    private const string ERROR_TYPE = "error.type";
    private const string SERVER_PORT = "server.port";
    private const string SERVER_ADDRESS = "server.address";
    public const string ACTIVITY_SOURCE_NAME = "DataLayer.Telemetry.EFCoreDiagnostics";

    private static readonly ActivitySource ActivitySource = new(ACTIVITY_SOURCE_NAME);

    public void OnNext(KeyValuePair<string, object?> value)
    {
        var name = value.Key;
        var payload = value.Value;

        var activity = Activity.Current;

        if (name == RelationalEventId.CommandCreated.Name)
        {
            activity = ActivitySource.CreateActivity(ACTIVITY_NAME, ActivityKind.Client);
            if (activity == null) return;

            var command = (CommandEventData)payload!;

            activity.DisplayName = RepositoryInterceptor.OperationName;
            activity.SetTag(DB_OPERATION_NAME, RepositoryInterceptor.OperationName);
            activity.Start();

            var providerName = command.Context?.Database.ProviderName;
            if (providerName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                activity.SetTag(DB_SYSTEM_NAME, "postgresql");
            }

            var connectionString = new NpgsqlConnectionStringBuilder(command.Connection?.ConnectionString);
            if (connectionString.Host != null)
            {
                activity.SetTag(SERVER_ADDRESS, connectionString.Host);
            }

            if (connectionString.Port != 0)
            {
                activity.SetTag(SERVER_PORT, connectionString.Port);
            }
        }
        else if (name == RelationalEventId.CommandError.Name)
        {
            if (activity == null || activity.Source != ActivitySource)
            {
                return;
            }

            var error = (CommandErrorEventData)payload!;
            activity.SetTag(ERROR_TYPE, error.Exception.GetType().Name);
            activity.SetTag(DB_STATUS_CODE, error.Exception.HResult);
            activity.SetStatus(ActivityStatusCode.Error, error.Exception.Message);
            activity.Stop();
        }
        else if (name == RelationalEventId.CommandCanceled.Name)
        {
            if (activity == null || activity.Source != ActivitySource)
            {
                return;
            }

            activity.SetStatus(ActivityStatusCode.Error, "Command was canceled");
            activity.Stop();
        }
        else if (name == RelationalEventId.CommandExecuted.Name)
        {
            if (activity == null || activity.Source != ActivitySource)
            {
                return;
            }

            activity.SetStatus(ActivityStatusCode.Ok);
            activity.Stop();
        }
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }
}