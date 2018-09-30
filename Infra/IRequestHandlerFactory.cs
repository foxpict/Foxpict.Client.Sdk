namespace Foxpict.Client.Sdk.Infra
{
    public interface IRequestHandlerFactory
    {
        IRequestHandler CreateNew(string name);
    }
}
