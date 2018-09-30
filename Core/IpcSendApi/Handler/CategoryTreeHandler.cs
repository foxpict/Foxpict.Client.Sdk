using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Foxpict.Client.Sdk.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;


namespace Foxpict.Client.Sdk.Core.IpcApi.Handler {
  public class CategoryTreeHandler : IResolveDeclare {
    public string ResolveName => "CategoryTree";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly IMemoryCache mMemoryCache;

      readonly IFrontendIpcMessageBridge mIpcMessageBridge;

      public Handler (IMemoryCache memoryCache, IFrontendIpcMessageBridge ipcMessageBridge) {
        this.mMemoryCache = memoryCache;
        this.mIpcMessageBridge = ipcMessageBridge;
      }

      public override void Handle (object param) {
        IpcSendServiceParam serviceParam = (IpcSendServiceParam) param;

        string cacheKey = "CategoryTree";
        var categoryId = long.Parse (serviceParam.Data.ToString ());
        cacheKey += categoryId;

        // MemCacheから、更新通知を行うカテゴリオブジェクトを取得
        if (this.mMemoryCache.TryGetValue (cacheKey, out Category[] cachedObject)) {
          var ipcMessage = new IpcMessage ();
          object obj = new {
            PropertyName = "CategoryTree",
            Hint = categoryId,
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
