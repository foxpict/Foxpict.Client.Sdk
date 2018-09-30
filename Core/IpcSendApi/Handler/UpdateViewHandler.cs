using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Newtonsoft.Json;
using NLog;
using static Foxpict.Client.Sdk.ScreenManager;

namespace Foxpict.Client.Sdk.Core.IpcApi.Handler {
  public class UpdateViewHandler : IResolveDeclare {
    readonly ILogger mLogger;

    public string ResolveName => "UpdateView";

    public Type ResolveType => typeof (Handler);

    public UpdateViewHandler () {
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    public class Handler : PackageResolveHandler {
      readonly ILogger mLogger;

      readonly IFrontendIpcMessageBridge mIpcMessageBridge;

      public Handler (IFrontendIpcMessageBridge ipcMessageBridge) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mIpcMessageBridge = ipcMessageBridge;
      }

      public override void Handle (object param) {
        IpcSendServiceParam serviceParam = (IpcSendServiceParam) param;

        var messageparameter = serviceParam.Data as UpdateViewResponse;

        var ipcMessage = new IpcMessage ();
        object obj = new {
          UpdateList = messageparameter.ViewEventList,
          Parameter = messageparameter.Parameter,
          NextScreenName = messageparameter.NextScreenName
        };
        ipcMessage.Body = JsonConvert.SerializeObject (obj, Formatting.Indented);
        mLogger.Info ("Body(JSON)=" + ipcMessage.Body);

        mIpcMessageBridge.Send ("IPC_UPDATEVIEW", ipcMessage);
      }
    }
  }
}
