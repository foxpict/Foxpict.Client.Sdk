using System;

namespace Foxpict.Client.Sdk.Json.ServerMessage {
    public class ActDisplayPreviewcurrentlistParam {

        public long ContentListPos;

        /// <summary>
        /// 所属カテゴリの表示情報を更新するかどうかのフラグ
        /// </summary>
        public bool UpdateCategoryDisplayInfo;

        /// <summary>
        /// 表示継続情報を更新するかどうかのフラグ
        /// /// </summary>
        public bool UpdateLastDisplayContent;
    }
}
