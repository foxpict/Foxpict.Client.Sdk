using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NLog;

namespace Foxpict.Client.Sdk.Core.IpcApi.Handler {
  public class UpdatePropHandler : IResolveDeclare {
    public string ResolveName => "UpdateProp";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly Logger mLogger;

      readonly IMemoryCache mMemoryCache;

      readonly IFrontendIpcMessageBridge mIpcMessageBridge;

      /// <summary>
      /// コンストラクタ
      /// </summary>
      /// <param name="memoryCache"></param>
      public Handler (IMemoryCache memoryCache, IFrontendIpcMessageBridge ipcMessageBridge) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
        this.mIpcMessageBridge = ipcMessageBridge;
      }

      public override void Handle (object param) {
        IpcSendServiceParam serviceParam = (IpcSendServiceParam) param;

        var propertyName = serviceParam.Data.ToString ();
        object cachedObject;
        if (mMemoryCache.TryGetValue (propertyName, out cachedObject)) {
          var ipcMessage = new IpcMessage ();
          object obj = new {
            PropertyName = propertyName,
            Value = JsonConvert.SerializeObject (cachedObject)
          };

          ipcMessage.Body = JsonConvert.SerializeObject (obj, Formatting.Indented);
          this.mLogger.Debug ("UpdateProp 送信本文={0}", ipcMessage.Body);

          mIpcMessageBridge.Send ("IPC_UPDATEPROP", ipcMessage);
        } else {
          this.mLogger.Warn ("[Execute] Faile MemCache");
        }
      }
    }
  }
}
