using System;
using System.Threading;
using System.Threading.Tasks;
using Foxpict.Client.Sdk.Infra;
using NLog;

namespace Foxpict.Client.Sdk.Intent {
  public class IntentManager : IIntentManager {
    private readonly ILogger mLogger;

    readonly IBackgroundTaskQueue mBackgroundTaskQueue;

    readonly IServiceDistoributor mServiceDistoributor;

    /// <summary>
    ///コンストラクタ
    /// </summary>
    /// <param name="queue"></param>
    public IntentManager (IBackgroundTaskQueue queue, IServiceDistoributor distributor) {
      this.mBackgroundTaskQueue = queue;
      this.mServiceDistoributor = distributor;
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    public void AddIntent (ServiceType service, string intentName) {
      AddIntent (service, intentName, null);
    }

    public void AddIntent (ServiceType service, string intentName, object parameter) {
      mBackgroundTaskQueue.QueueBackgroundWorkItem (ExecuteItem);

      async Task ExecuteItem (CancellationToken token) {
        this.mLogger.Debug ("[ExecuteItem] IntentName={IntentName}", intentName);

        //await Task.Delay(TimeSpan.FromSeconds(5), token); //デバッグ用のウェイト

        // Distributorの呼び出し
        mServiceDistoributor.ExecuteService (service, intentName, parameter);
      }
    }
  }
}
