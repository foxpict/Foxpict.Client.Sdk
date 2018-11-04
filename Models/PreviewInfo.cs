namespace Foxpict.Client.Sdk.Models {
  /// <summary>
  ///
  /// </summary>
  public class PreviewInfo {
    long totalNum;
    long currentPos;

    /// <summary>
    ///
    /// </summary>
    /// <param name="totalNum"></param>
    /// <param name="currentPos"></param>
    public PreviewInfo (long totalNum, long currentPos) {
      this.totalNum = totalNum;
      this.currentPos = currentPos;
    }

    public long TotalNum { get { return this.totalNum; } }

    public long CurrentPos { get { return this.currentPos; } }
  }
}
