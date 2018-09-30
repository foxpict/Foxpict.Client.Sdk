using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foxpict.Client.Sdk.Cache;
using Foxpict.Client.Sdk.Core.ServerMessageApi.Handler;
using Foxpict.Client.Sdk.Core.Workflow.Param;
using Foxpict.Client.Sdk.Dao;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.IpcApi.Response;
using Foxpict.Client.Sdk.Json.ServerMessage;
using Foxpict.Client.Sdk.Models;
using Foxpict.Client.Sdk.Workflow;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NLog;
using SimpleInjector;

namespace Pixstock.Applus.Foundations.ContentBrowser.Transitions {
  public partial class CategoryTreeTransitionWorkflow {
    private ILogger mLogger;

    readonly Container mContainer;

    int mCurrentPreviewContentListPosition = 0; // < プレビュー画面に表示しているコンテントの、コンテントリスト内での位置

    public CategoryTreeTransitionWorkflow (Container container) {
      this.mContainer = container;
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    async Task OnHomePageBase_Entry () {

    }

    async Task OnHomePageBase_Exit () {

    }

    async Task OnThumbnailListPage_Entry () {

    }

    async Task OnThumbnailListPage_Exit () {

    }

    async Task OnPreviewPage_Entry () {

    }

    async Task OnREQUEST_GetCategory (object param) { }

    async Task OnACT_ContinueCategoryList (object param) { }

    async Task OnRESPONSE_GETCATEGORY (object param) {
      this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnRESPONSE_GETCATEGORY]");
      await Task.Delay (1);

      var intentManager = mContainer.GetInstance<IIntentManager> ();
      var memCache = mContainer.GetInstance<IMemoryCache> ();
      CategoryDetailResponse response;
      if (memCache.TryGetValue ("ResponseCategory", out response)) {
        memCache.Set ("CategoryList", new CategoryListParam {
          Category = response.Category,
            CategoryList = response.SubCategory
        });

        memCache.Set ("ContentList", new ContentListParam {
          Category = response.Category,
            ContentList = response.Content
        });

        intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "CategoryList");
        intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "ContentList");
      } else {
        Console.WriteLine ("    MemCache TryGet Failer");
      }
    }

    async Task OnRESPONSE_GETCATEGORYCONTENT (object param) {
      this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnRESPONSE_GETCATEGORYCONTENT]");
      var intentManager = mContainer.GetInstance<IIntentManager> ();
      var memCache = mContainer.GetInstance<IMemoryCache> ();
      CategoryDetailResponse response;
      if (memCache.TryGetValue ("ResponseCategoryContent", out response)) {
        memCache.Set ("ContentList", new ContentListParam {
          Category = response.Category,
            ContentList = response.Content
        });

        intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "ContentList");
      } else {
        Console.WriteLine ("    MemCache TryGet Failer");
      }
    }

    async Task OnCategorySelectBtnClick (object param) {
      this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnCategorySelectBtnClick]");

      var intentManager = mContainer.GetInstance<IIntentManager> ();
      var memCache = mContainer.GetInstance<IMemoryCache> ();
      await Task.Run (() => {
        CategoryDetailResponse response;
        if (memCache.TryGetValue ("ResponseCategoryContent", out response)) {
          memCache.Set ("ContentList", new ContentListParam {
            Category = response.Category,
              ContentList = response.Content
          });

          intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "ContentList");
        } else {
          Console.WriteLine ("    MemCache TryGet Failer");
        }
      });
    }

    async Task OnACT_DEBUGCOMMAND (object param) {
      this.mLogger.Debug ("IN");
      try {
        var intentManager = mContainer.GetInstance<IIntentManager> ();

        await Task.Run (() => {
          this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnACT_DEBUGCOMMAND] param={0}", param);

          var screenManager = mContainer.GetInstance<IScreenManager> ();

          if (param.ToString () == "RESET_TRANSITION") {
            screenManager.DumpBackStack ();
            intentManager.AddIntent (ServiceType.Workflow, "TRNS_DEBUG_BACK", "");
          }
        });
      } catch (Exception expr) {
        this.mLogger.Error (expr);
      }
      this.mLogger.Debug ("OUT");
    }

    async Task OnACT_DISPLAY_PREVIEWCURRENTLIST (object param) {
      try {
        this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnACT_DISPLAY_PREVIEWCURRENTLIST] param={0}", param);
        var intentManager = mContainer.GetInstance<IIntentManager> ();
        var memCache = mContainer.GetInstance<IMemoryCache> ();

        ContentListParam objContentList;
        if (memCache.TryGetValue ("ContentList", out objContentList)) {
          // コンテント一覧の項目位置(param)にあるコンテント情報を読み込む
          var paramObj = JsonConvert.DeserializeObject<ActDisplayPreviewcurrentlistParam> (param.ToString ());
          var contentPosition = paramObj.ContentListPos;
          var content = objContentList.ContentList[contentPosition];

          if (paramObj.UpdateCategoryDisplayInfo) {
            intentManager.AddIntent (ServiceType.Server, "UpdateReading", content.Id);
          }

          if (paramObj.UpdateLastDisplayContent) {
            this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnACT_DISPLAY_PREVIEWCURRENTLIST] UpdateLastDisplayContent=true, Category={0}", objContentList.Category);
            if (objContentList.Category != null) {

              // 更新したいプロパティを設定
              var updateCategoryProp = new {
              NextDisplayContentId = content.Id
              };

              var updateCategoryPropParam = new UpdateCategoryPropParam {
                CategoryId = objContentList.Category.Id,
                Category = updateCategoryProp
              };

              intentManager.AddIntent (ServiceType.Server, "UpdateCategoryProp", JsonConvert.SerializeObject (updateCategoryPropParam));
            }
          }

          intentManager.AddIntent (ServiceType.Server, "GETCONTENT", content.Id);
        } else {
          throw new ApplicationException ("ContentListプロパティを取得できませんでした");
        }
      } catch (Exception expr) {
        Console.WriteLine (expr.Message);
      }
    }

    async Task OnACT_UpperCategoryList (object param) {
      this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnACT_UpperCategoryList]");

      var intentManager = mContainer.GetInstance<IIntentManager> ();
      var memCache = mContainer.GetInstance<IMemoryCache> ();
      var categoryDao = mContainer.GetInstance<ICategoryDao>();

      try {
        CategoryListParam objCategoryList;
        if (memCache.TryGetValue ("CategoryList", out objCategoryList)) {
          var parentCategory = categoryDao.LoadParentCategory (objCategoryList.Category.Id);
          if (parentCategory == null) {
            throw new ApplicationException ("Not Get");
          }

          var paramJson = JsonConvert.SerializeObject (new GetCategoryParam {
            CategoryId = parentCategory.Id,
              OffsetSubCategory = 0,
              LimitOffsetSubCategory = 10
          });

          intentManager.AddIntent (ServiceType.Server, "GETCATEGORY", paramJson);
        }
      } catch (Exception expr) {
        this.mLogger.Error (expr, "メッセージの処理に失敗しました");
      }
    }

    async Task OnRESPONSE_GETCONTENT (object param) {
      this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnRESPONSE_GETCONTENT]");

      var intentManager = mContainer.GetInstance<IIntentManager> ();
      var memCache = mContainer.GetInstance<IMemoryCache> ();
      ContentDetailResponse response;
      if (memCache.TryGetValue ("ResponsePreviewContent", out response)) {
        memCache.Set ("PreviewContent", new PreviewContentParam () {
          Content = response.Content,
            Category = response.Category
        });

        intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "PreviewContent");
      } else {
        Console.WriteLine ("    MemCache TryGet Failer");
      }
    }

    async Task OnACT_CATEGORYTREE_UPDATE (object param) {
      try {
        this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnACT_CATEGORYTREE_UPDATE] ");

        var intentManager = mContainer.GetInstance<IIntentManager> ();
        var memCache = mContainer.GetInstance<IMemoryCache> ();

        this.mLogger.Debug ("1" + param.ToString ());

        long tgtCategoryId = long.Parse (param.ToString ());

        // TODO: サーバに問い合わせるためのIntentメッセージを発行する
        this.mLogger.Debug ("2");

        // TODOの間は、ハードコードされたダミーデータを取得したことにする
        List<Category> subCategoryList = new List<Category> ();
        subCategoryList.Add (new Category { Id = 2L, Name = "サブカテゴリA" });
        subCategoryList.Add (new Category { Id = 3L, Name = "サブカテゴリB" });
        subCategoryList.Add (new Category { Id = 4L, Name = "サブカテゴリC" });
        this.mLogger.Debug ("3");
        memCache.Set ("CategoryList", new CategoryListParam {
          Category = new Category {
              Id = 1L,
                Name = "サンプルカテゴリ"
            },
            CategoryList = subCategoryList.ToArray ()
        });

        this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnACT_CATEGORYTREE_UPDATE] Execute 'CategoryList' IntentMessage");
        intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "CategoryList");
      } catch (Exception expr) {
        this.mLogger.Debug (expr, "Error");
      }
    }

    async Task OnACT_THUMBNAILLIST_UPDATE (object param) {
      this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnACT_THUMBNAILLIST_UPDATE]");

      // TODO: サーバに問い合わせるためのIntentメッセージを発行する
    }

    async Task OnACT_PREVIEW_UPDATE (object param) {
      this.mLogger.Debug ("[CategoryTreeTransitionWorkflow][OnACT_PREVIEW_UPDATE]");

      // TODO: サーバに問い合わせるためのIntentメッセージを発行する
    }

    /// <summary>
    /// CategoryTree更新要求
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_REQINVALIDATE_CATEGORYTREE (object param) {
      this.mLogger.Debug ("IN - {@Param}", param);
      long tgtCategoryId = long.Parse (param.ToString ());

      // ServerMessageServiceへのIntentメッセージを送信する（CategroyTree更新指示メッセージ）
      var intentManager = mContainer.GetInstance<IIntentManager> ();
      intentManager.AddIntent (ServiceType.Server, "CategoryTreeLoad", tgtCategoryId);
    }

    /// <summary>
    /// ラベル情報取得／子階層ラベル取得
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_REQINVALIDATE_LABELTREE (object param) {
      this.mLogger.Debug ("IN - {@Param}", param);
      long tgtLabelId = long.Parse (param.ToString ());

      // ServerMessageServiceへのIntentメッセージを送信する（CategroyTree更新指示メッセージ）
      var intentManager = mContainer.GetInstance<IIntentManager> ();
      intentManager.AddIntent (ServiceType.Server, "LabelTreeLoad", tgtLabelId);
    }

    /// <summary>
    /// プレビューコンテント更新要求
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_REQINVALIDATE_PREVIEW (object param) {
      this.mLogger.Debug ("IN - {@Param}", param);

      var memCache = mContainer.GetInstance<IMemoryCache> ();
      var intentManager = mContainer.GetInstance<IIntentManager> ();

      ReqInvalidatePreviewParameter paramObject = new ReqInvalidatePreviewParameter ();
      JsonConvert.PopulateObject (param.ToString (), paramObject);

      long? currentPreviewContentId = null; //< プレビューの表示を行うコンテントID
      try {
        switch (paramObject.Operation) {
          case "NavigationPosition":
            {
              ContentListParam objContentList;
              if (memCache.TryGetValue ("ContentList", out objContentList)) {
                mCurrentPreviewContentListPosition = paramObject.Position;

                // コンテント一覧の項目位置(ReqInvalidatePreviewParameter.Position)にあるコンテント情報を読み込む
                var content = objContentList.ContentList[mCurrentPreviewContentListPosition];
                currentPreviewContentId = content.Id;
                intentManager.AddIntent (ServiceType.Server, "ContentPreview", new ContentPreviewHandler.HandlerParameter { ContentId = content.Id });
              } else {
                throw new ApplicationException ("キャッシュからコンテント一覧を取得できませんでした。");
              }
            }
            break;
          case "NavigationNext":
            {
              ContentListParam objContentList;
              if (memCache.TryGetValue ("ContentList", out objContentList)) {
                if (objContentList.ContentList.Length > mCurrentPreviewContentListPosition + 1) {
                  mCurrentPreviewContentListPosition = mCurrentPreviewContentListPosition + 1;
                  var content = objContentList.ContentList[mCurrentPreviewContentListPosition];
                  this.mLogger.Debug ($"現在位置の次のコンテントを読み込みます({@content.Id})");
                  currentPreviewContentId = content.Id;
                  intentManager.AddIntent (ServiceType.Server, "ContentPreview", new ContentPreviewHandler.HandlerParameter { ContentId = content.Id });
                }
              } else {
                throw new ApplicationException ("キャッシュからコンテント一覧を取得できませんでした。");
              }
            }
            break;
          case "NavigationPrev":
            this.mLogger.Debug ("Operation={@OperationName}", paramObject.Operation); {
              ContentListParam objContentList;
              if (memCache.TryGetValue ("ContentList", out objContentList)) {
                if (mCurrentPreviewContentListPosition > 0) {
                  mCurrentPreviewContentListPosition = mCurrentPreviewContentListPosition - 1;
                  var content = objContentList.ContentList[mCurrentPreviewContentListPosition];
                  this.mLogger.Debug ($"現在位置の前のコンテントを読み込みます({@content.Id})");
                  currentPreviewContentId = content.Id;
                  intentManager.AddIntent (ServiceType.Server, "ContentPreview", new ContentPreviewHandler.HandlerParameter { ContentId = content.Id });
                }
              } else {
                throw new ApplicationException ("キャッシュからコンテント一覧を取得できませんでした。");
              }
            }
            break;
          case "Content":
            // TODO: 未実装
          default:
            this.mLogger.Warn ("処理できないオペレーション名({@Operation})です", paramObject.Operation);
            break;
        }

        if (currentPreviewContentId.HasValue) {
          // コンテント情報の更新
          intentManager.AddIntent (ServiceType.Server, "ContentOperationExecution", new ContentOperationExecutionHandler.HandlerParameter (
            new ContentOperationExecutionHandler.HandlerParameter.Operation[] {
              new ContentOperationExecutionHandler.HandlerParameter.Operation { OperationName = "Read" }
            }) {
            ContentId = currentPreviewContentId.Value
          });
        }

      } catch (Exception expr) {
        this.mLogger.Error (expr);
      }

      this.mLogger.Debug ("OUT");
    }

    /// <summary>
    /// CategoryTree更新通知
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_RESINVALIDATE_CATEGORYTREE (object param) {
      this.mLogger.Debug ("IN - {@Param}", param);
      long tgtCategoryId = long.Parse (param.ToString ());

      // IpcSendServiceへのIntentメッセージを送信する(カテゴリツリー更新通知メッセージ)
      var intentManager = mContainer.GetInstance<IIntentManager> ();
      intentManager.AddIntent (ServiceType.FrontendIpc, "CategoryTree", tgtCategoryId);
    }

    /// <summary>
    /// ラベル
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_UPDATED_LABELTREE (object param) {
      this.mLogger.Debug ("IN - {@Param}", param);
      long tgtLabelId = long.Parse (param.ToString ());

      // IpcSendServiceへのIntentメッセージを送信する(カテゴリツリー更新通知メッセージ)
      var intentManager = mContainer.GetInstance<IIntentManager> ();
      intentManager.AddIntent (ServiceType.FrontendIpc, "LabelTree", tgtLabelId);
    }

    /// <summary>
    /// コンテント情報更新通知メッセージ
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_RESINVALIDATE_CONTENT (object param) {
      this.mLogger.Debug ("IN - {@Param}", param);
      var paramObject = (ResInvalidateContentParameter) param;

      var memCache = mContainer.GetInstance<IMemoryCache> ();
      var intentManager = mContainer.GetInstance<IIntentManager> ();

      // レジスタ名の設定／未設定で処理を切り分けます。
      // 1. パラメータのレジスタ名が空文字の場合は、「コンテント情報取得要求」を実施する
      // 2-1. 空文字ではない場合は、メモリキャッシュからデータを取得しメモリキャッシュに「PreviewContent」として値を格納する
      // 2-2. InvalidatePropを実行する

      if (string.IsNullOrEmpty (paramObject.RegisterName)) {
        // 1.
        this.mLogger.Info ("レジスタ名が未設定のため、コンテント情報取得要求を行います。");
        intentManager.AddIntent (ServiceType.Server, "LoadContent", new LoadContentHandler.HandlerParameter {
          ContentId = paramObject.ContentId,
            RegisterName = "RANDOMWORD"
        });
      } else {
        // 2.
        this.mLogger.Info ("レジスタ名({@RegisterName})から値を読み込みます。", paramObject.RegisterName);
        Content content;
        if (memCache.TryGetValue (paramObject.RegisterName, out content)) {
          memCache.Set ("PreviewContent",
            content,
            new MemoryCacheEntryOptions ());
          intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "PreviewContent");
        } else {
          throw new ApplicationException ("レジスタからContentオブジェクトを取得できませんでした。");
        }
      }

      this.mLogger.Debug ("OUT");
    }

    /// <summary>
    /// コンテント一覧更新要求メッセージのハンドラ
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_REQINVALIDATE_CONTENTLIST (object param) {
      mLogger.Debug ("[IN] - {@Param}", param);

      ReqInvalidateContentListParameter paramObject = new ReqInvalidateContentListParameter ();
      JsonConvert.PopulateObject (param.ToString (), paramObject);

      // TODO: ルールの保存／既存ルールの読み込み

      var intentParam = new LoadContentListHandler.HandlerParameter ();
      if (paramObject.ContentId.HasValue) {
        intentParam.CategoryId = paramObject.ContentId.Value;
      } else if (paramObject.LabelId.HasValue) {
        intentParam.LabelId = paramObject.LabelId.Value;
      } else {
        mLogger.Debug ("コンテント一覧を作成したいソースを指定してください。");
        return;
      }

      var intentManager = mContainer.GetInstance<IIntentManager> ();
      intentManager.AddIntent (ServiceType.Server, "LoadContentList", intentParam);
    }

    async Task OnACT_REQINVALIDATE_CATEGORYLIST (object param) {
      mLogger.Debug ("[IN] - {@Param}", param);

      ReqInvalidateCategoryListParameter paramObject = new ReqInvalidateCategoryListParameter ();
      JsonConvert.PopulateObject (param.ToString (), paramObject);

      var intentParam = new LoadCategoryListHandler.HandlerParameter ();
      if (paramObject.LabelId.HasValue) {
        intentParam.LabelId = paramObject.LabelId.Value;
      } else {
        mLogger.Debug ("コンテント一覧を作成したいソースを指定してください。");
        return;
      }

      var intentManager = mContainer.GetInstance<IIntentManager> ();
      intentManager.AddIntent (ServiceType.Server, "LoadCategoryList", intentParam);
    }

    /// <summary>
    ///コンテント一覧更新通知メッセージのハンドラ
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_RESINVALIDATE_CONTENTLIST (object param) {
      this.mLogger.Debug ("IN - {@Param}", param);

      var intentManager = mContainer.GetInstance<IIntentManager> ();
      intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "ContentList");
    }

    /// <summary>
    /// カテゴリ一覧更新通知
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_UPDATED_CATEGORYLIST (object param) {
      this.mLogger.Debug ("IN");

      var intentManager = mContainer.GetInstance<IIntentManager> ();
      intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "CategoryList");
    }

    /// <summary>
    ///コンテント情報永続化メッセージのハンドラ
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    async Task OnACT_STORE_CONTENTPROP (object param) {
      this.mLogger.Debug ("IN - {@Param}", param);
      var intentManager = mContainer.GetInstance<IIntentManager> ();

      StoreContentPropParameter paramObject = new StoreContentPropParameter ();
      paramObject.Content = new Content ();
      JsonConvert.PopulateObject (param.ToString (), paramObject);

      switch (paramObject.Hint) {
        case "Invalidate":
          this.mLogger.Debug ("永続化方法をInvalidateとして処理を実行します");
          this.mLogger.Debug (JsonConvert.SerializeObject (paramObject.Content));
          intentManager.AddIntent (ServiceType.Server, "StoreData", new StoreDataHandler.HandlerParameter {
            Value = paramObject.Content,
              ModelType = "Content",
              UpdateNotificationFlag = true
          });
          break;
        default:
          this.mLogger.Warn ($"不明なオペレーション({@paramObject.Hint})のため実行しませんでした。");
          break;
      }

      this.mLogger.Debug ("OUT");
    }
  }
}
