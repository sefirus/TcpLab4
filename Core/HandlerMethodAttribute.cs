namespace Core;

[AttributeUsage(AttributeTargets.Method)]
public class HandlerMethodAttribute : Attribute
{
    public string Route { get; }

    public HandlerMethodAttribute(string route)
    {
        Route = route;
    }
}