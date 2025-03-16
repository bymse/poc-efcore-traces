using Castle.DynamicProxy;

namespace DataLayer.Telemetry;

public class RepositoryInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var className = invocation.TargetType?.Name;
        var methodName = invocation.Method.Name;
        EFCoreDiagnosticEventsObserver.StartActivity($"{className}.{methodName}");

        invocation.Proceed();

        if (invocation.ReturnValue is Task task)
        {
            task.ContinueWith(_ => EFCoreDiagnosticEventsObserver.StopActivity());
        }
        else
        {
            EFCoreDiagnosticEventsObserver.StopActivity();
        }
    }
}