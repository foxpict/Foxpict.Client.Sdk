using Foxpict.Client.App.Models;

namespace Foxpict.Client.Sdk.Json.ServerMessage {
    public class UpdateCategoryPropParam {
        public long CategoryId;

        /// <summary>
        /// 更新するカテゴリ情報のプロパティのみ含めたオブジェクト
        /// </summary>
        public object Category;
    }
}
