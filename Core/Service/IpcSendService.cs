using Foxpict.Client.Sdk.Core.Intent;
using Foxpict.Client.Sdk.Core.IpcApi;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foxpict.Client.Sdk.Core.Service
{
  public class IpcSendService : IResolveDeclare
  {
    public string ResolveName => ServiceType.FrontendIpc.ToString();

    public Type ResolveType => typeof(Handler);

    public class Handler : PackageResolveHandler
    {
      private readonly Container mContainer;

      private IpcSendResolveHandlerFactory mFactory = null;

      /// <summary>
      /// コンストラクタ
      /// </summary>
      /// <param name="container"></param>
      public Handler(Container container)
      {
        this.mContainer = container;
      }

      public override void Handle(object param)
      {
        var intentParam = (IntentParam)param;

        var handler = GetFactory().CreateNew(intentParam.IntentName); // paramからIntentメッセージ名を取得し、ファクトリー経由でハンドラを取得する
        handler.Handle(new IpcSendServiceParam { Data = intentParam.ExtraData });
      }

      private IpcSendResolveHandlerFactory GetFactory()
      {
        if (mFactory == null)
        {
          mFactory = mContainer.GetInstance<IpcSendResolveHandlerFactory>();
        }

        return mFactory;
      }
    }
  }
}
