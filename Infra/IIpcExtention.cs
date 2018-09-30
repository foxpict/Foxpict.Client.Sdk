using System;

namespace Foxpict.Client.Sdk.Infra
{
    public interface IIpcExtention
    {
        string IpcMessageName { get; }

        Type RequestHandler { get; }
    }
}
