using System;
using System.Collections.Generic;
using Foxpict.Client.Sdk.Core.Service;
using Foxpict.Client.Sdk.Dao;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Infra.Resolver;
using Foxpict.Client.Sdk.Infra.Resolver.Impl;
using Foxpict.Client.Sdk.Workflow;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace Foxpict.Client.Sdk.Core.ServerMessageApi.Handler {

  /// <summary>
  /// コンテントに関するサーバサイドへのロジックを実行する
  /// </summary>
  public class ContentOperationExecutionHandler : IResolveDeclare {
    public string ResolveName => "ContentOperationExecution";

    public Type ResolveType => typeof (Handler);

    public class Handler : PackageResolveHandler {
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
        this.mLogger.Debug ("IN - {@Param}", param);
        var paramObj = (ServerMessageServiceParam) param;
        var paramHandler = paramObj.Data as HandlerParameter;

        var content = mContentDao.LoadContent (paramHandler.ContentId);
        if (content == null) {
          // Guard
          // TODO: 例外をスローする
        }

        // すべてのオペレーションを処理する
        // オペレーションを追加したい場合は、下記に実装してください。
        foreach (var op in paramHandler.Operations) {
          switch (op.OperationName) {
            case "Read":
            this.mLogger.Debug ("Readオペレーションを実行します");
              mContentDao.UpdateRead (paramHandler.ContentId);
              var workflowParam = new ResInvalidateContentParameter () {
                ContentId = paramHandler.ContentId,
                RegisterName = ""
              };
              mIntentManager.AddIntent (ServiceType.Workflow, "ACT_RESINVALIDATE_CONTENT", workflowParam);
              break;
            default:
              this.mLogger.Warn ($"不明なオペレーション({@op.OperationName})のため実行しませんでした。");
              break;
          }
        }
        this.mLogger.Debug ("OUT");
      }
    }

    public class HandlerParameter {

      public HandlerParameter () {
        operations = new List<Operation> ();
      }

      public HandlerParameter (Operation[] operations) {
        this.operations = new List<Operation> (operations);
      }

      public long ContentId;

      public List<Operation> Operations {
        get { return operations; }
      }

      readonly List<Operation> operations;

      public class Operation {
        public string OperationName;

        public object Value;
      }
    }
  }
}
