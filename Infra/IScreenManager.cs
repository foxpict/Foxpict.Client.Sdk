namespace Foxpict.Client.Sdk.Infra {
  public interface IScreenManager {
    void ShowScreen (string screenName);

    void HideScreen (string screenName);

    /// <summary>
    /// 戻る遷移を行います
    /// </summary>
    void BackScreen ();

    /// <summary>
    /// ビュー層に画面遷移メッセージを送信する
    /// </summary>
    void UpdateScreenTransitionView (object param);

    void ResetStack ();

    /// <summary>
    /// 戻る遷移スタックをダンプ出力する
    /// </summary>
    void DumpBackStack ();
  }
}
