using Castle.DynamicProxy;

namespace DataLayer.Telemetry;

public class RepositoryInterceptor : IInterceptor
{
    private static readonly AsyncLocal<string?> OperationNameAsyncLocal = new();

    public static string OperationName => OperationNameAsyncLocal.Value ?? throw new InvalidOperationException();

    public void Intercept(IInvocation invocation)
    {
        var className = invocation.TargetType?.Name;
        var methodName = invocation.Method.Name;
        OperationNameAsyncLocal.Value = $"{className}.{methodName}";

        invocation.Proceed();

        if (invocation.ReturnValue is Task task)
        {
            task.ContinueWith(_ => Stop());
        }
        else
        {
            Stop();
        }
    }

    private static void Stop()
    {
        OperationNameAsyncLocal.Value = null;
    }
}