using System;
using Foxpict.Client.Sdk.Bridge.Handler;
using Foxpict.Client.Sdk.Infra;

namespace Foxpict.Client.Sdk.Core.Bridge.Message {
  /// <summary>
  /// この名前空間に、IPCメッセージ処理用のプラグインを追加してください。
  /// </summary>
  public class HelloIpc : IIpcExtention {
    public string IpcMessageName => "HELLO";

    public Type RequestHandler => typeof (LogHandler);
  }
}
