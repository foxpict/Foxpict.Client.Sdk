using Foxpict.Client.Sdk.Models;

namespace Foxpict.Client.Sdk.Cache
{
    /// <summary>
    /// MemCacheのContentListパラメータ
    /// </summary>
    public class ContentListParam
    {
        /// <summary>
        /// ContentListを生成したカテゴリ情報（ある場合のみ）
        /// </summary>
        /// <remarks>
        /// コンテント一覧をカテゴリから生成した場合のみ、このフィールドに生成元のカテゴリ情報を格納する。
        /// それ以外はNULL。
        /// </remarks>
        public Category Category = null;

        public Content[] ContentList;
    }
}
