using System;
using System.Linq;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Foxpict.Client.Sdk.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NLog;

namespace Foxpict.Client.Sdk.Core.IpcApi.Handler {
  public class LabelTreeHandler : IResolveDeclare {
    public string ResolveName => "LabelTree";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly Logger mLogger;

      readonly IMemoryCache mMemoryCache;

      readonly IFrontendIpcMessageBridge mIpcMessageBridge;

      public Handler (IMemoryCache memoryCache, IFrontendIpcMessageBridge ipcMessageBridge) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
        this.mIpcMessageBridge = ipcMessageBridge;
      }

      public override void Handle (object param) {
        mLogger.Debug ("IN");
        IpcSendServiceParam serviceParam = (IpcSendServiceParam) param;

        string cacheKey = "LabelTree";
        var labelId = long.Parse (serviceParam.Data.ToString ());
        cacheKey += labelId;

        // MemCacheから、更新通知を行うカテゴリオブジェクトを取得
        if (this.mMemoryCache.TryGetValue (cacheKey, out Label[] cachedObject)) {
          var ipcMessage = new IpcMessage ();
          object obj = new {
            PropertyName = "LabelTree",
            Hint = labelId,
            Value = JsonConvert.SerializeObject (cachedObject)
          };

          // Viewへ更新通知メッセージを送信する
          ipcMessage.Body = JsonConvert.SerializeObject (obj, Formatting.Indented);
          mIpcMessageBridge.Send ("IPC_UPDATEPROP", ipcMessage);
        } else {
          //this.mLogger.LogWarning(LoggingEvents.Undefine, "[Execute] Failer MemCache (CacheKey={CacheKey})", cacheKey);
        }
      }
    }
  }
}
