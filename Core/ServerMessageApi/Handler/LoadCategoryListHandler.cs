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
  /// カテゴリ一覧取得要求メッセージの処理するハンドラ
  /// </summary>
  public class LoadCategoryListHandler : IResolveDeclare {
    public string ResolveName => "LoadCategoryList";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      const string cacheKey = "CategoryList";

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
        if (paramHandler.LabelId != null && paramHandler.LabelId.Length > 0) {

          var categories = mCategoryDao.FindCategory(true,paramHandler.LabelId);

          var cacheEntryOptions = new MemoryCacheEntryOptions ();
          mMemoryCache.Set (cacheKey,
            new CategoryListParam () { CategoryList = categories.ToArray () },
            cacheEntryOptions);
        } else {
          mLogger.Warn ("カテゴリ一覧の取得ソースを指定してください。");
          return;
        }

        mIntentManager.AddIntent (ServiceType.Workflow, "ACT_UPDATED_CATEGORYLIST");
        mLogger.Debug ("OUT");
      }
    }

    public class HandlerParameter {

      public long[] LabelId;

      public int PageNo;
    }
  }
}
