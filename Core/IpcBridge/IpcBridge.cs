using System;
using System.Collections.Generic;
using System.Linq;
using Foxpict.Client.Sdk.Infra;
using Newtonsoft.Json.Linq;
using NLog;
using SimpleInjector;

namespace Foxpict.Client.Sdk.Bridge {
  /// <summary>
  /// フロントエンドからのIPC通信によるメッセージを受信するブリッジ
  /// </summary>
  public class IpcBridge {
    const string IPCEXTENTION_NAMESPACE = "Foxpict.Client.Sdk.Core.Bridge.Message";

    readonly ILogger mLogger;

    readonly Container mContainer;

    readonly IFrontendIpcMessageBridge mFrontendIpcMessageBridge;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="container"></param>
    public IpcBridge (Container container, IFrontendIpcMessageBridge frontendIpcMessageBridge) {
      this.mLogger = LogManager.GetCurrentClassLogger ();
      this.mContainer = container;
      this.mFrontendIpcMessageBridge = frontendIpcMessageBridge;
    }

    /// <summary>
    /// Ipcハンドラの初期化
    /// </summary>
    public IpcRequestHandlerFactory Initialize () {
      IpcRequestHandlerFactory requestHandlerFactory = new IpcRequestHandlerFactory (mContainer);
      Container localContainer = new Container ();

      var repositoryAssembly = typeof (IpcBridge).Assembly;

      // IPCメッセージ処理プラグイン登録
      // IIpcExtentionインターフェースを実装するクラスのみ登録可能とする
      var registrations =
        from type in repositoryAssembly.GetExportedTypes ()
      where type.Namespace == IPCEXTENTION_NAMESPACE
      where type.GetInterfaces ().Contains (typeof (IIpcExtention))
      select new { Service = type.GetInterfaces ().Single (), Implementation = type };

      List<Type> implementationList = new List<Type> ();
      foreach (var reg in registrations) {
        mLogger.Info ("[Initialize] Register");
        implementationList.Add (reg.Implementation);
      }
      localContainer.RegisterCollection<IIpcExtention> (implementationList);
      localContainer.Verify ();
      mLogger.Info ("[Initialize] Register Complete");

      // 受信したIPCメッセージを処理するハンドラを登録
      foreach (var ext in localContainer.GetAllInstances<IIpcExtention> ()) {
        mLogger.Info ("[Initialize] " + ext.IpcMessageName);

        requestHandlerFactory.Add (ext.IpcMessageName, ext.RequestHandler);

        // IPCメッセージ受信時の処理
        this.mFrontendIpcMessageBridge.RegisterEventHandler (ext.IpcMessageName, (param) => {
          mLogger.Info (string.Format ("[IPC][Receive] {0}", ext.IpcMessageName));

          var factory = mContainer.GetInstance<IRequestHandlerFactory> ();
          var handler = factory.CreateNew (ext.IpcMessageName);
          var requestParam = ((JObject) param).ToObject<IpcMessage> ();
          handler.Handle (requestParam);
        });
      }

      return requestHandlerFactory;
    }
  }
}
