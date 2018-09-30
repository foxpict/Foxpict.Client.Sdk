namespace Foxpict.Client.Sdk.Infra
{
    public interface IRequestHandler
    {
        void Handle(IpcMessage param);
    }
}
