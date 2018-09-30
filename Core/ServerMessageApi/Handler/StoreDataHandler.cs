using System;
using NLog;
using Foxpict.Common.Model;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Foxpict.Client.Sdk.Dao;
using Foxpict.Client.App.Models;
using Foxpict.Client.Sdk.Models;
using Foxpict.Client.Sdk.Workflow;

namespace Foxpict.Client.Sdk.Core.ServerMessageApi.Handler {
  /// <summary>
  /// データの永続化を実行する
  /// </summary>
  public class StoreDataHandler : IResolveDeclare {
    public string ResolveName => "StoreData";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
      readonly Logger mLogger;

      readonly IIntentManager mIntentManager;

      readonly IContentDao mConentDao;

      public Handler (IIntentManager intentManager, IContentDao contentDao) {
        this.mLogger = LogManager.GetCurrentClassLogger ();
        this.mIntentManager = intentManager;
        this.mConentDao = contentDao;
      }

      public override void Handle (object param) {
        this.mLogger.Debug ("IN - {@Param}", param);
        var paramObj = (ServerMessageServiceParam) param;
        var paramHandler = paramObj.Data as HandlerParameter;

        switch (paramHandler.ModelType) {
          case "Content":
            this.mLogger.Debug ("データモデルタイプをContentとして処理を実行します");
            var content = (Content) paramHandler.Value;
            mConentDao.Update (content);
            if (paramHandler.UpdateNotificationFlag) {
              mIntentManager.AddIntent (ServiceType.Workflow, "ACT_RESINVALIDATE_CONTENT", new ResInvalidateContentParameter { ContentId = content.Id });
            }
            break;
          default:
            this.mLogger.Warn ($"不明なオペレーション({@paramHandler.ModelType})のため実行しませんでした。");
            break;
        }
        this.mLogger.Debug ("OUT");
      }
    }

    public class HandlerParameter {
      public object Value; //< 更新対象のオブジェクト

      public string ModelType; //< 更新対象オブジェクトのデータモデルタイプ

      public bool UpdateNotificationFlag = false;
    }
  }
}
