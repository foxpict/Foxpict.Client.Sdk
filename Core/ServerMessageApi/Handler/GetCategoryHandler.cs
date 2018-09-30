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
using Foxpict.Client.Sdk.Json.ServerMessage;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Foxpict.Client.Sdk.Core.ServerMessageApi.Handler {
  public class GetCategoryHandler : IResolveDeclare {
    public string ResolveName => "GETCATEGORY";

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

        var handlerParam = JsonConvert.DeserializeObject<GetCategoryParam> (serviceParam.Data.ToString ());

        var category = mCategoryDao.LoadCategory (handlerParam.CategoryId, handlerParam.OffsetSubCategory, handlerParam.LimitOffsetSubCategory);

        mMemoryCache.Set ("ResponseCategory", new CategoryDetailResponse {
          Category = category,
            SubCategory = category.LinkSubCategoryList.ToArray (),
            Content = category.LinkContentList.ToArray ()
        });

        //this.mLogger.LogDebug(LoggingEvents.Undefine, "[Execute] Register RESPONSE_GETCATEGORY");
        mIntentManager.AddIntent (ServiceType.Workflow, "RESPONSE_GETCATEGORY", null);
      }
    }
  }
}
