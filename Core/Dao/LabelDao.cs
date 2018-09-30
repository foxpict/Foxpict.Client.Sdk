using System;
using System.Collections.Generic;
using System.Linq;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Models;
using Foxpict.Common.Core;
using NLog;
using RestSharp;

namespace Foxpict.Client.Sdk.Dao {
  public class LabelDao : DaoBase, ILabelDao {

    readonly Logger mLogger;

    public LabelDao (AppSettings appSettings) : base (appSettings) {
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    /// <summary>
    /// ラベル情報を読み込みます
    /// </summary>
    /// <returns></returns>
    public ICollection<Label> LoadLabel () {
      var request = new RestRequest ("label", Method.GET);
      var response = mClient.Execute<PixstockResponseAapi<List<Label>>> (request);
      return response.Data.Value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="labelId"></param>
    /// <returns></returns>
    public Label LoadLabel (long labelId) {
      var request = new RestRequest ("label/{id}", Method.GET);
      request.AddUrlSegment ("id", labelId);

      var response = mClient.Execute<PixstockResponseAapi<Label>> (request);
      var label = response.Data.Value;

      label.LinkSubLabelList = LinkGetSubLabel (labelId, 0, 100000, response);
      label.LinkCategoryList = LinkGetCategory (labelId);
      label.LinkContentList = LinkGetContentList (labelId, 0, response);

      return label;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public ICollection<Label> LoadRoot () {
      var request = new RestRequest ("label/{id}/l/children/", Method.GET);
      request.AddUrlSegment ("id", 0);
      var response = mClient.Execute<PixstockResponseAapi<List<Label>>> (request);
      mLogger.Info ($"ErrorMessage={@response.ErrorMessage}");
      return response.Data.Value;
    }

    /// <summary>
    /// サービスへラベルリンクデータ情報取得API(カテゴリ情報)を要求します。
    /// </summary>
    /// <param name="query">現時点ではラベル情報を示すキー</param>
    /// <param name="offset">未実装</param>
    /// <param name="limit">未実装</param>
    /// <returns></returns>
    public ICollection<Category> LoadLabelLinkCategory (string query, int offset, int limit) {
      var request = new RestRequest ("label/{query}/category", Method.GET);
      request.AddUrlSegment ("query", query);
      //request.AddQueryParameter("offset", offset.ToString()); 実装したら使用する

      //_logger.Info("Execute Request");
      var response = mClient.Execute<PixstockResponseAapi<List<Category>>> (request);

      //_logger.Info("Execute Respose");
      return response.Data.Value;
    }

    private List<Category> LinkGetCategory (long labelId) {
      var categoryList = new List<Category> ();
      var request_link_category_list = new RestRequest ("label/{id}/l/category-list", Method.GET);
      request_link_category_list.AddUrlSegment ("id", labelId);

      var response_link_category_list = mClient.Execute<ResponseAapi<List<Category>>> (request_link_category_list);
      if (response_link_category_list.IsSuccessful) {
        foreach (var category in response_link_category_list.Data.Value) {
          categoryList.Add (category);
        }
      }

      return categoryList;
    }

    private List<Content> LinkGetContentList (long labelId, long offset, IRestResponse<PixstockResponseAapi<Label>> response) {
      var contentList = new List<Content> ();
      var request_link_contentList = new RestRequest ("label/{id}/l/content-list", Method.GET);
      request_link_contentList.AddUrlSegment ("id", labelId);

      var response_link_contentList = mClient.Execute<ResponseAapi<List<Content>>> (request_link_contentList);
      if (response_link_contentList.IsSuccessful) {
        foreach (var content in response_link_contentList.Data.Value) {
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

    private List<Label> LinkGetSubLabel (long labelId, int offset, int limit, IRestResponse<PixstockResponseAapi<Label>> response) {
      // リンク情報から、カテゴリ情報を取得する
      List<Label> labelList = new List<Label> ();
      var link_la = response.Data.Link["children"] as List<object>;
      foreach (var linkedCategoryId in link_la.Skip (offset).Select (p => (long) p).Take (limit)) {
        labelList.Add (LoadLinkedLabel (labelId, linkedCategoryId));
      }

      return labelList;
    }

    /// <summary>
    /// 任意ラベルのサブラベル情報を取得します
    /// </summary>
    /// <param name="labelId"></param>
    /// <param name="linkedLabelId"></param>
    /// <returns></returns>
    public Label LoadLinkedLabel (long labelId, long linkedLabelId) {
      var request_link_la = new RestRequest ("label/{id}/children/{label_id}", Method.GET);
      request_link_la.AddUrlSegment ("id", labelId);
      request_link_la.AddUrlSegment ("label_id", linkedLabelId);
      //request_link_la.AddQueryParameter("offset", offset.ToString());

      var response_link_la = mClient.Execute<PixstockResponseAapi<Label>> (request_link_la);
      if (!response_link_la.IsSuccessful)
        throw new ApplicationException ("DAOの実行に失敗しました");

      var linked_label = response_link_la.Data.Value;

      if (response_link_la.Data.Link.ContainsKey ("cc_available")) {
        var ccAvailable = response_link_la.Data.Link["cc_available"];
        if (Boolean.TrueString == ccAvailable.ToString ()) {
          linked_label.HasLinkSubCategoryFlag = true;
        }
      }

      return linked_label;
    }
  }
}
