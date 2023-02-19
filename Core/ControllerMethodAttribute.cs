namespace Core;

[AttributeUsage(AttributeTargets.Method)]
public class ControllerMethodAttribute : Attribute
{
    public string Route { get; }

    public ControllerMethodAttribute(string route)
    {
        Route = route;
    }
}