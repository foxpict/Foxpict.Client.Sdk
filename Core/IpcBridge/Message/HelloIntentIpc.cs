using System;
using Foxpict.Client.Sdk.Bridge.Handler;
using Foxpict.Client.Sdk.Infra;

namespace Foxpict.Client.Sd.Core.Bridge.Message
{
    /// <summary>
    /// この名前空間に、IPCメッセージ処理用のプラグインを追加してください。
    /// </summary>
    public class HelloIntentIpc : IIpcExtention
    {
        public string IpcMessageName => "HELLO_INTENT";

        public Type RequestHandler => typeof(PixstockIntentHandler);
    }
}
