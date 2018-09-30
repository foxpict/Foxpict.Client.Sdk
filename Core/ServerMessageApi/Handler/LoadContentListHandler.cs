using System;
using Foxpict.Client.Sdk.Cache;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Dao;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace Foxpict.Client.Sdk.Core.ServerMessageApi.Handler {
  /// <summary>
  /// コンテント一覧取得要求メッセージの処理するハンドラ
  /// </summary>
  public class LoadContentListHandler : IResolveDeclare {
    public string ResolveName => "LoadContentList";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      const string cacheKey = "ContentList";

      readonly Logger mLogger;

      readonly IMemoryCache mMemoryCache;

      readonly IIntentManager mIntentManager;

      readonly ICategoryDao mCategoryDao;

      readonly ILabelDao mLabelDao;

      public Handler (IMemoryCache memoryCache, IIntentManager intentManager, ICategoryDao categoryDao, ILabelDao labelDao) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
        this.mIntentManager = intentManager;
        this.mCategoryDao = categoryDao;
        this.mLabelDao = labelDao;
      }

      public override void Handle (object param) {
        mLogger.Debug ("IN - {@Param}", param);
        ServerMessageServiceParam paramObj = (ServerMessageServiceParam) param;
        var paramHandler = paramObj.Data as HandlerParameter;

        if (paramHandler.CategoryId.HasValue) {
          // カテゴリから関連するコンテント一覧を作成する場合
          var category = mCategoryDao.LoadCategory (categoryId: paramHandler.CategoryId.Value, offsetContent: paramHandler.PageNo);

          var cacheEntryOptions = new MemoryCacheEntryOptions ();
          mMemoryCache.Set (cacheKey,
            new ContentListParam () { Category = category, ContentList = category.LinkContentList.ToArray () },
            cacheEntryOptions);
        } else if (paramHandler.LabelId.HasValue) {
          // ラベルから関連するコンテント一覧を生成する場合
          var label = mLabelDao.LoadLabel (paramHandler.LabelId.Value);
          var cacheEntryOptions = new MemoryCacheEntryOptions ();
          mMemoryCache.Set (cacheKey,
            new ContentListParam () { ContentList = label.LinkContentList.ToArray () },
            cacheEntryOptions);
        } else {
          mLogger.Warn ("コンテント一覧の取得ソースを指定してください。");
          return;
        }

        mIntentManager.AddIntent (ServiceType.Workflow, "ACT_RESINVALIDATE_CONTENTLIST");
        mLogger.Debug ("OUT");
      }
    }

    public class HandlerParameter {
      public long? CategoryId;

      public long? LabelId;

      public int PageNo;
    }
  }
}
