using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foxpict.Client.Sdk.Core.ServerMessageApi
{
  public class ServiceMessageResolveHandlerFactory : PackageResolveHandlerFactory
  {
    public ServiceMessageResolveHandlerFactory(Container container) :
      base(container, "Foxpict.Client.Sdk.Core.ServerMessageApi.Handler")
    {

    }
  }
}
