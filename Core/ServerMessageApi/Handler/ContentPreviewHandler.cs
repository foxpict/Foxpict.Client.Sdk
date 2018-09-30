using System;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Dao;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace Foxpict.Client.Sdk.Core.ServerMessageApi.Handler {
  /// <summary>
  /// プレビュー取得要求メッセージを処理するハンドラ
  /// </summary>
  public class ContentPreviewHandler : IResolveDeclare {
    public string ResolveName => "ContentPreview";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      const string CACHE_KEY = "PreviewUrl";

      readonly Logger mLogger;

      readonly IMemoryCache mMemoryCache;

      readonly IIntentManager mIntentManager;

      readonly IContentDao mContentDao;

      public Handler (IMemoryCache memoryCache, IIntentManager intentManager, IContentDao contentDao) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
        this.mIntentManager = intentManager;
        this.mContentDao = contentDao;
      }

      public override void Handle (object param) {
        mLogger.Debug ("IN - {@Param}", param);
        ServerMessageServiceParam paramObj = (ServerMessageServiceParam) param;
        var paramHandler = paramObj.Data as HandlerParameter;
        var content = mContentDao.LoadContent (paramHandler.ContentId);

        // 取得したURLをキャッシュに格納
        var cacheEntryOptions = new MemoryCacheEntryOptions ()
          .SetSlidingExpiration (TimeSpan.FromSeconds (30));
        mMemoryCache.Set (CACHE_KEY,
          content.PreviewFileUrl,
          cacheEntryOptions);

        mIntentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", CACHE_KEY);
        mLogger.Debug ("OUT");
      }
    }

    public class HandlerParameter {
      public long ContentId;
    }
  }
}
