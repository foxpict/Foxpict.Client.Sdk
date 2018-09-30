using System;
using System.Collections.Generic;
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
  /// 「ラベルツリー更新指示」メッセージを処理するハンドラを実装したクラスです
  /// /// </summary>
  public class LabelTreeLoadHandler : IResolveDeclare {
    public string ResolveName =>
      "LabelTreeLoad";

    public Type ResolveType =>
      typeof (Handler);

    public class Handler : PackageResolveHandler {

      readonly IMemoryCache mMemoryCache;

      readonly IIntentManager mIntentManager;

      private readonly Logger mLogger;

      readonly ILabelDao mLabelDao;

      public Handler (IMemoryCache memoryCache, IIntentManager intentManager, ILabelDao labelDao) {
        mLogger = LogManager.GetCurrentClassLogger ();
        this.mMemoryCache = memoryCache;
        this.mIntentManager = intentManager;
        this.mLabelDao = labelDao;
      }

      public override void Handle (object param) {
        this.mLogger.Debug ("IN - {@param}", param);

        ServerMessageServiceParam serviceParam = (ServerMessageServiceParam) param;
        string cacheKey = "LabelTree";

        var labelId = long.Parse (serviceParam.Data.ToString ());
        cacheKey += labelId;
        if (!mMemoryCache.TryGetValue (cacheKey, out Label[] s)) {
          if (labelId > 0) {
            var label = mLabelDao.LoadLabel (labelId);
            if (label == null) {
              throw new ApplicationException ($"カテゴリID({labelId})の読み込みに失敗しました。");
            }
            s = label.LinkSubLabelList.ToArray ();

            var cacheEntryOptions = new MemoryCacheEntryOptions ()
              .SetSlidingExpiration (TimeSpan.FromSeconds (3));

            this.mLogger.Debug ("Push MemCache (CacheKey={CacheKey})", cacheKey);
            mMemoryCache.Set (cacheKey, s, cacheEntryOptions);
          } else if (labelId == 0) {
            var labelList = mLabelDao.LoadRoot ();

            s = (new List<Label> (labelList)).ToArray ();

            var cacheEntryOptions = new MemoryCacheEntryOptions ()
              .SetSlidingExpiration (TimeSpan.FromSeconds (3));

            this.mLogger.Debug ("Push MemCache (CacheKey={CacheKey})", cacheKey);
            mMemoryCache.Set (cacheKey, s, cacheEntryOptions);
          }

          mIntentManager.AddIntent (ServiceType.Workflow, "ACT_UPDATED_LABELTREE", labelId);
        }
      }
    }
  }
}
