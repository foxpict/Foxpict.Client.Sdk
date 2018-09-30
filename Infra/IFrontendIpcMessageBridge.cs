using System;
using Foxpict.Client.Sdk.Infra;

namespace Foxpict.Client.Sdk.Infra {
  public interface IFrontendIpcMessageBridge {
    /// <summary>
    /// IPCメッセージ受信時に呼び出すコールバック関数
    /// /// </summary>
    /// <param name="ipcEventName"></param>
    /// <param name="receiveHandler">パラメータを受け取る任意の関数</param>
    void RegisterEventHandler (string ipcEventName, Action<object> receiveHandler);

    /// <summary>
    ///
    /// /// </summary>
    /// <param name="ipcEventName"></param>
    /// <param name="param"></param>
    void Send (string ipcEventName, IpcMessage param);
  }
}
