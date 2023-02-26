using Core.Helpers;

namespace Core.Infrastructure;

public abstract class BuilderBase<TChild, THandlerBase, TFuncArg, TFuncReturn> 
{
    protected TChild Child;

    protected readonly Dictionary<string, (string, THandlerBase, Func<THandlerBase, TFuncArg, TFuncReturn>)> Endpoints =
        new();
    protected readonly Dictionary<string, string> Configuration = new();
    protected int Port { get; private set; }

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
            if (methodInfo.ReturnType != typeof(TFuncReturn)
                || parameters.FirstOrDefault()?.ParameterType != typeof(TFuncArg)
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

        return Child;
    }
    
    public TChild AddConfiguration(string filePath)
    {
        var readConfiguration = JsonHelper.ReadObject<Dictionary<string, string>>(filePath)
                                ?? throw new Exception("Cant read settings!");
        foreach (var keyValue in readConfiguration)
        {
            Configuration.Add(keyValue.Key, keyValue.Value);
        }
        if (!int.TryParse(Configuration["Port"], out var port))
        {
            throw new ArgumentNullException($"Port", "Port you provided is not in correct format!");
        }

        Port = port;
        return Child;
    }
}