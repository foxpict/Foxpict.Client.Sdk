using System;
using Foxpict.Client.Sdk.Bridge.Handler;
using Foxpict.Client.Sdk.Infra;

namespace Foxpict.Client.Sdk.Core.Bridge.Message
{
    /// <summary>
    /// この名前空間に、IPCメッセージ処理用のプラグインを追加してください。
    /// </summary>
    public class PixstockIntentMessageIpc : IIpcExtention
    {
        public string IpcMessageName => "PIXS_INTENT_MESSAGE";
        public Type RequestHandler => typeof(PixstockIntentHandler);
    }
}
