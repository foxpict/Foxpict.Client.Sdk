using System;
using System.Collections.Generic;
using Foxpict.Client.Sdk.Infra;
using SimpleInjector;

namespace Foxpict.Client.Sdk.Bridge
{
    public class IpcRequestHandlerFactory : Dictionary<string, Type>, IRequestHandlerFactory
    {
        private readonly Container container;

        public IpcRequestHandlerFactory(Container container)
        {
            this.container = container;
        }

        public IRequestHandler CreateNew(string name) =>
            (IRequestHandler)this.container.GetInstance(this[name]);
    }
}
