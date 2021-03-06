using System;
using System.Collections.Generic;
using System.Linq;
using Foxpict.Client.App.Models;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Models;
using Foxpict.Common.Core;
using Foxpict.Common.Model;
using Newtonsoft.Json;
using NLog;
using RestSharp;

namespace Foxpict.Client.Sdk.Dao {
  /// <summary>
  ///
  /// </summary>
  public class CategoryDao : DaoBase, ICategoryDao {
    public const int MAXLIMIT = 1000000;

    readonly Logger mLogger;

    public CategoryDao (AppSettings appSettings) : base (appSettings) {
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    /// <summary>
    /// カテゴリ情報を読み込みます
    /// </summary>
    /// <param name="categoryId">カテゴリID</param>
    /// <param name="offsetSubCategory">子階層カテゴリリストの取得開始位置</param>
    /// <param name="limitSubCategory">子階層カテゴリリストの取得最大数</param>
    /// <returns>カテゴリ情報</returns>
    public Category LoadCategory (long categoryId, int offsetSubCategory = 0, int limitSubCategory = Constant.MAXLIMIT, int offsetContent = 0) {
      if (limitSubCategory == -1) limitSubCategory = Constant.MAXLIMIT;

      try {
        var request = new RestRequest ("category/{id}", Method.GET);
        request.AddUrlSegment ("id", categoryId);
        request.AddQueryParameter ("lla_order", "NAME_ASC");

        var response = mClient.Execute<PixstockResponseAapi<Category>> (request);
        if (!response.IsSuccessful) {
          this.mLogger.Warn ("ErrorCode=" + response.StatusCode +
            "  ErrorException=" + response.ErrorException +
            "  ErrorMessage=" + response.ErrorMessage +
            "  ContentError=" + response.Data.Error);
          return null;
        }

        var category = response.Data.Value;
        category.LinkSubCategoryList = LinkGetSubCategory (categoryId, offsetSubCategory, limitSubCategory, response);
        category.LinkContentList = LinkGetContentList (categoryId, offsetContent, response);
        return category;
      } catch (Exception expr) {
        this.mLogger.Error (expr, "APIの実行に失敗しました");
      }
      return new Category ();
    }

    /// <summary>
    /// 親カテゴリ情報を取得する
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public Category LoadParentCategory (long categoryId) {
      var request = new RestRequest ("category/{id}/pc", Method.GET);
      request.AddUrlSegment ("id", categoryId);
      var response = mClient.Execute<PixstockResponseAapi<List<Category>>> (request);
      if (!response.IsSuccessful) {
        this.mLogger.Warn ("ErrorCode=" + response.StatusCode +
          "  ErrorException=" + response.ErrorException +
          "  ErrorMessage=" + response.ErrorMessage +
          "  ContentError=" + response.Data.Error);
        return null;
      }

      return response.Data.Value.FirstOrDefault ();
    }

    private List<Content> LinkGetContentList (long categoryId, long offset, IRestResponse<PixstockResponseAapi<Category>> response) {
      // リンク情報から、コンテント情報を取得する
      var contentList = new List<Content> ();

      var request_link_la = new RestRequest ("category/{id}/la", Method.GET);
      request_link_la.AddUrlSegment ("id", categoryId);

      var response_link_la = mClient.Execute<ResponseAapi<List<Content>>> (request_link_la);
      if (response_link_la.IsSuccessful) {
        foreach (var content in response_link_la.Data.Value) {
          // サムネイルが存在する場合は、サムネイルのURLを設定
          if (!string.IsNullOrEmpty (content.ThumbnailKey)) {
            content.ThumbnailImageSrcUrl = mServiceServerUrl + "/thumbnail/" + content.ThumbnailKey;
          }

          // コンテントのURLを設定
          content.PreviewFileUrl = mServiceServerUrl + "/artifact/" + content.Id + "/preview";

          contentList.Add (content);
        }
      }

      return contentList;
    }

    private List<Category> LinkGetSubCategory (long categoryId, int offset, int limit, IRestResponse<PixstockResponseAapi<Category>> response) {
      // リンク情報から、カテゴリ情報を取得する
      List<Category> categoryList = new List<Category> ();
      var link_la = response.Data.Link["cc"] as List<object>;
      foreach (var linkedCategoryId in link_la.Skip (offset).Select (p => (long) p).Take (limit)) {
        categoryList.Add (LoadLinkedCategory (categoryId, linkedCategoryId));
      }

      return categoryList;
    }

    /// <summary>
    /// カテゴリ情報更新API
    /// </summary>
    /// <param name="categoryId">更新対象のカテゴリのキー</param>
    /// <param name="category">更新データ(更新するプロパティのみ含んだオブジェクト)</param>
    internal void Update (long categoryId, object category) {
      var request = new RestRequest ("category/{id}", Method.PUT);
      request.AddUrlSegment ("id", categoryId);
      //var s = JsonConvert.SerializeObject (category);
      request.AddJsonBody (JsonConvert.SerializeObject (category));

      var response = mClient.Execute (request);
      if (!response.IsSuccessful) {
        throw new ApplicationException ("DAOの実行に失敗しました");
      }
    }

    /// <summary>
    /// カテゴリ表示情報更新API
    /// </summary>
    /// <param name="categoryId"></param>
    internal void UpdateReading (long categoryId) {
      var request = new RestRequest ("category/{id}/read", Method.PUT);
      request.AddUrlSegment ("id", categoryId);

      var response = mClient.Execute (request);
      if (!response.IsSuccessful) {
        throw new ApplicationException ("DAOの実行に失敗しました");
      }
    }

    /// <summary>
    /// 任意のカテゴリのサブカテゴリ情報を取得します
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="linkedCategoryId"></param>
    /// <returns></returns>
    public Category LoadLinkedCategory (long categoryId, long linkedCategoryId) {
      var request_link_la = new RestRequest ("category/{id}/cc/{category_id}", Method.GET);
      request_link_la.AddUrlSegment ("id", categoryId);
      request_link_la.AddUrlSegment ("category_id", linkedCategoryId);
      //request_link_la.AddQueryParameter("offset", offset.ToString());

      var response_link_la = mClient.Execute<PixstockResponseAapi<Category>> (request_link_la);
      if (!response_link_la.IsSuccessful)
        throw new ApplicationException ("DAOの実行に失敗しました");

      var linked_category = response_link_la.Data.Value;

      if (response_link_la.Data.Link.ContainsKey ("cc_available")) {
        var ccAvailable = response_link_la.Data.Link["cc_available"];
        if (Boolean.TrueString == ccAvailable.ToString ()) {
          linked_category.HasLinkSubCategoryFlag = true;
        }
      }

      return linked_category;
    }

    public List<Category> FindCategory (bool? albumCategory, long[] labelId) {
      if (!albumCategory.HasValue) // TODO: 現在はアルバムカテゴリのみ取得可能とする
        throw new NotSupportedException ("アルバムカテゴリのみ条件に含めることができます。");
      else if (albumCategory.Value != true) // TODO: 現在はアルバムカテゴリは必須とする
        throw new NotSupportedException ("アルバムカテゴリのみ条件に含めることができます。");
      if (labelId == null) // TODO: 現在はラベルの条件指定は必須です。
        throw new NotSupportedException ("ラベル一覧の指定は必須です。");

      var request = new RestRequest ("category/w_album/{labels_cond}", Method.GET);
      request.AddUrlSegment ("labels_cond", "+" + string.Join (",", labelId)); // ANDのみサポート

      var response = mClient.Execute<PixstockResponseAapi<List<Category>>> (request);
      if (!response.IsSuccessful)
        throw new ApplicationException ("DAOの実行に失敗しました");

      return response.Data.Value;
    }
  }
}
