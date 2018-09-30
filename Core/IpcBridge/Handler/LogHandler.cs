using System;
using Foxpict.Client.Sdk.Infra;

namespace Foxpict.Client.Sdk.Bridge.Handler {
  /// <summary>
  /// ログ出力用のIPCハンドラ
  /// </summary>
  public class LogHandler : IRequestHandler {
    public void Handle (IpcMessage param) {
      Console.WriteLine ("Execute LogHandler.Handle");
    }
  }
}
