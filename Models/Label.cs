using System.Collections.Generic;
using Newtonsoft.Json;
using Foxpict.Common.Model;

namespace Foxpict.Client.Sdk.Models {
  public class Label : ILabel {
    public string Name { get; set; }
    public string MetaType { get; set; }
    public string Comment { get; set; }
    public long Id { get; set; }

    /**
     * リンクしているサブカテゴリ情報一覧
     * このプロパティは、フロントエンドへのシリアライズ対象外です。
     */
    [JsonIgnore]
    public List<Label> LinkSubLabelList { get; set; }

    /**
     * リンクしているカテゴリ情報一覧
     * このプロパティは、フロントエンドへのシリアライズ対象外です。
     */
    [JsonIgnore]
    public List<Category> LinkCategoryList { get; set; }

    /**
     * リンクしているコンテント情報一覧
     * このプロパティは、フロントエンドへのシリアライズ対象外です。
     */
    [JsonIgnore]
    public List<Content> LinkContentList { get; set; }

    /**
     * リンクしているサブカテゴリが存在するか示すフラグです。
     */
    public bool HasLinkSubCategoryFlag { get; set; }
  }
}
