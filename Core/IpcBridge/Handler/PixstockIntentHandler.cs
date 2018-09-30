using System;
using Foxpict.Client.Sdk.Infra;
using Newtonsoft.Json;
using NLog;

namespace Foxpict.Client.Sdk.Bridge.Handler {
  /// <summary>
  /// IPCメッセージを、Intentメッセージとして登録するIPCハンドラ
  /// </summary>
  public class PixstockIntentHandler : IRequestHandler {
    readonly IIntentManager mIntentManager;

    readonly ILogger mLogger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="intentManager"></param>
    /// <param name="loggerFactory"></param>
    public PixstockIntentHandler (IIntentManager intentManager) {
      this.mIntentManager = intentManager;
      mLogger = LogManager.GetCurrentClassLogger ();
    }

    public void Handle (IpcMessage param) {
      mLogger.Debug ("[Handle(IpcMessage)] - IN " + param);

      try {
        // IPCメッセージから、PixstockのIntentメッセージを取り出す処理
        var message = JsonConvert.DeserializeObject<IntentMessage> (param.Body.ToString ());
        mIntentManager.AddIntent (message.ServiceType, message.MessageName, message.Parameter);
      } catch (Exception expr) {
        mLogger.Error (expr, "[Handle(IpcMessage)] Faile Deserialize");
      }
    }
  }
}
