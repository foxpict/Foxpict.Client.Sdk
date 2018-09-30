namespace Foxpict.Client.Sdk.Infra {
  public interface IIntentManager {
    void AddIntent (ServiceType service, string intentName);

    /// <summary>
    /// Intent処理キューに、インテントを追加する
    /// </summary>
    /// <param name="intentName">インテント名</param>
    void AddIntent (ServiceType service, string intentName, object parameter);
  }
}
