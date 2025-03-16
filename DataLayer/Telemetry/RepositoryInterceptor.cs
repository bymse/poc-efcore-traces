using Castle.DynamicProxy;

namespace DataLayer.Telemetry;

public class RepositoryInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var className = invocation.TargetType?.Name;
        var methodName = invocation.Method.Name;
        OperationNameContainer.OperationName = $"{className}.{methodName}";

        invocation.Proceed();

        if (invocation.ReturnValue is Task task)
        {
            task.ContinueWith(_ => OperationNameContainer.OperationName = null);
        }
    }
}