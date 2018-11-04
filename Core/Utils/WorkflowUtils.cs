using System;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Foxpict.Client.Sdk.Core.Utils.Workflow {
  public class WorkflowUtils {
    /// <summary>
    /// ナビゲーションリストを元にプレビュー画面を表示している場合の、
    /// プレビュー画面の情報をビューに送信します。
    /// </summary>
    /// <param name="intentManager"></param>
    /// <param name="memoryCache"></param>
    /// <param name="totalNum">ナビゲーションリスト内の総項目数</param>
    /// <param name="currentPos">ナビゲーションリスト内での現在の位置</param>
    internal static void ExecuteInvalidatePreviewInfo (
      IIntentManager intentManager,
      IMemoryCache memoryCache,
      long totalNum,
      long currentPos) {
      var cacheEntryOptions = new MemoryCacheEntryOptions ()
        .SetSlidingExpiration (TimeSpan.FromSeconds (30));
      memoryCache.Set ("PreviewInfo",
        new PreviewInfo (totalNum, currentPos),
        cacheEntryOptions);
      intentManager.AddIntent (ServiceType.FrontendIpc, "UpdateProp", "PreviewInfo");
    }
  }
}
