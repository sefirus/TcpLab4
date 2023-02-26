namespace Core.Interfaces.Infrastructure;

public interface IApplicationBuilder<out TChild, in THandlerBase, TFuncArg, TFuncReturn> 
{
    TChild AddHandler<TController>() where TController : THandlerBase, new();
    TChild AddConfiguration(string filePath);
    IApplication Build();
}