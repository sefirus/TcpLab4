namespace Core.Infrastructure;

public abstract class BuilderBase<TChild, THandlerBase, TFuncArg, TFuncReturn> 
{
    protected TChild? _child;

    protected readonly Dictionary<string, (string, THandlerBase, Func<THandlerBase, TFuncArg, TFuncReturn>)> Endpoints =
        new();
    
    
    public TChild AddHandler<TController>() where TController : THandlerBase, new()
    {
        var controllerType = typeof(TController);
        var attributeType = typeof(HandlerMethodAttribute);
        var isControllerAdded = Endpoints
            .Any(c => c.Value.Item1 == controllerType.ToString());
        if (isControllerAdded)
        {
            throw new InvalidOperationException($"Controller {controllerType} is already added!");
        }

        var controllerMethods = controllerType
            .GetMethods()
            .Where(m => m.GetCustomAttributes(attributeType, false).Length > 0);
        var newController = new TController();
        foreach (var methodInfo in controllerMethods)
        {
            var parameters = methodInfo.GetParameters();
            if (methodInfo.ReturnType != typeof(Message)
                || parameters.FirstOrDefault()?.ParameterType != typeof(Message)
                || parameters.Length != 1)
            {
                continue;
            }

            var genericFunc = (Func<TController, TFuncArg, TFuncReturn>)Delegate
                .CreateDelegate(typeof(Func<TController, TFuncArg, TFuncReturn>), null, methodInfo);
            var func = new Func<THandlerBase, TFuncArg, TFuncReturn>((a, b) => genericFunc((TController)a, b));
            var address = methodInfo.CustomAttributes
                .FirstOrDefault(a => a.AttributeType == attributeType)?
                .ConstructorArguments?
                .FirstOrDefault()
                .Value?.ToString() ?? throw new InvalidOperationException("Controller contains invalid attributes!");
            Endpoints.Add(address, (controllerType.ToString(), newController, func));
        }

        return _child;
    }
}