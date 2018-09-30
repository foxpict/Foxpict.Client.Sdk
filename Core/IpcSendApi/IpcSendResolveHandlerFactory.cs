using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foxpict.Client.Sdk.Core.IpcApi
{
  public class IpcSendResolveHandlerFactory : PackageResolveHandlerFactory
  {
    public IpcSendResolveHandlerFactory(Container container) :
      base(container, "Foxpict.Client.Sdk.Core.IpcApi.Handler")
    {

    }
  }
}
