namespace Core.Interfaces.Infrastructure;

public interface IApplicationBuilder<out TChild, in THandlerBase, TFunc> where TFunc: MulticastDelegate 
{
    TChild AddHandler<TController>() where TController : THandlerBase, new();
    IApplication Build();
}