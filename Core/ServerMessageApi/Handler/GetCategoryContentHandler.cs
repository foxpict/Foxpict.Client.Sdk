using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Dao;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Foxpict.Client.Sdk.IpcApi.Response;
using Microsoft.Extensions.Caching.Memory;

namespace Foxpict.Client.Sdk.Core.ServerMessageApi.Handler {
  public class GetCategoryContentHandler : IResolveDeclare {
    public string ResolveName => "GETCATEGORYCONTENT";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly IMemoryCache mMemoryCache;

      readonly IIntentManager mIntentManager;

      readonly ICategoryDao mCategoryDao;

      public Handler (IMemoryCache memoryCache, IIntentManager intentManager, ICategoryDao categoryDao) {
        this.mMemoryCache = memoryCache;
        this.mIntentManager = intentManager;
        this.mCategoryDao = categoryDao;
      }

      public override void Handle (object param) {
        ServerMessageServiceParam serviceParam = (ServerMessageServiceParam) param;
        var category = mCategoryDao.LoadCategory (long.Parse (serviceParam.Data.ToString ()), 0, CategoryDao.MAXLIMIT);
        mMemoryCache.Set ("ResponseCategoryContent", new CategoryDetailResponse {
          Category = category,
            Content = category.LinkContentList.ToArray ()
        });

        mIntentManager.AddIntent (ServiceType.Workflow, "RESPONSE_GETCATEGORYCONTENT", null);
      }
    }
  }
}
