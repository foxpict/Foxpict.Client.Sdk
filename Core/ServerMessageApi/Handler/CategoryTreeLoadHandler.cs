using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Dao;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Foxpict.Client.Sdk.Models;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace Foxpict.Client.Sdk.Core.ServerMessageApi.Handler {
  /// <summary>
  /// CategroyTree更新指示メッセージを処理するハンドラのクラスです
  /// </summary>
  public class CategoryTreeLoadHandler : IResolveDeclare {
    public string ResolveName => "CategoryTreeLoad";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly IMemoryCache mMemoryCache;

      readonly IIntentManager mIntentManager;

      readonly ICategoryDao mCategoryDao;

      private readonly Logger mLogger;

      public Handler (IMemoryCache memoryCache, IIntentManager intentManager, ICategoryDao categoryDao) {
        mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
        this.mIntentManager = intentManager;
        this.mCategoryDao = categoryDao;
      }

      public override void Handle (object param) {
        this.mLogger.Debug ("IN - {@param}", param);

        ServerMessageServiceParam serviceParam = (ServerMessageServiceParam) param;
        string cacheKey = "CategoryTree";

        var categoryId = long.Parse (serviceParam.Data.ToString ());
        cacheKey += categoryId;

        if (!mMemoryCache.TryGetValue (cacheKey, out Category[] s)) {
          var category = mCategoryDao.LoadCategory (categoryId);
          if (category == null) {
            throw new ApplicationException ($"カテゴリID({categoryId})の読み込みに失敗しました。");
          }
          if (category.LinkSubCategoryList != null) {
            s = category.LinkSubCategoryList.ToArray ();
          } else {
            s = new Category[0];
          }

          var cacheEntryOptions = new MemoryCacheEntryOptions ()
            .SetSlidingExpiration (TimeSpan.FromSeconds (3));

          this.mLogger.Debug ("[OnCategoryTreeLoad] Push MemCache (CacheKey={CacheKey})", cacheKey);
          mMemoryCache.Set (cacheKey, s, cacheEntryOptions);
        }

        mIntentManager.AddIntent (ServiceType.Workflow, "ACT_RESINVALIDATE_CATEGORYTREE", categoryId);
      }
    }
  }
}
