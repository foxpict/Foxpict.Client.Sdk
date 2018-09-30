using Foxpict.Client.Sdk.Models;

namespace Foxpict.Client.Sdk.Cache
{
    /// <summary>
    /// MemCacheのPreviewContentパラメータ
    /// </summary>
    public class PreviewContentParam
    {
        /// <summary>
        ///
        /// </summary>
        public Content Content;

        /// <summary>
        /// Contentプロパティが示すコンテント情報が所属しているカテゴリ情報の詳細
        /// </summary>
        public Category Category;
    }
}
