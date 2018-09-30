using Foxpict.Client.Sdk.Models;

namespace Foxpict.Client.Sdk.IpcApi.Response
{
    public class ContentDetailResponse
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Content Content { get; set; }

        /// <summary>
        /// コンテント情報が所属しているカテゴリを取得します
        /// </summary>
        /// <returns></returns>
        public Category Category { get; set; }
    }
}
