using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foxpict.Client.Sdk.Core.Intent
{
  public class ServiceDistributionResolveHandlerFactory : PackageResolveHandlerFactory
  {
    public ServiceDistributionResolveHandlerFactory(Container container) :
      base(container, "Foxpict.Client.Sdk.Core.Service", Lifestyle.Singleton)
    {

    }
  }
}
