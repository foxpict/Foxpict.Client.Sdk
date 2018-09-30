using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;

namespace Foxpict.Client.Sdk.Core.ServerMessageApi.Handler
{
  public class DebugCommandHandler : IResolveDeclare
  {
    public string ResolveName => "DebugCommand";

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler {
      public override void Handle(object param)
      {
        ServerMessageServiceParam serviceParam = (ServerMessageServiceParam)param;
        Console.WriteLine("[DEBUG][DebugCommandHandler] Handle - " + serviceParam.Data);
      }
    }
  }
}
