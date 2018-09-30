using System;
using System.Collections.Generic;
using System.Linq;
using Foxpict.Client.Sdk.Core.Intent;
using Foxpict.Client.Sdk.Infra;
using NLog;
using SimpleInjector;

namespace Foxpict.Client.Sdk.Intent {
  public class ServiceDistoributionManager : IServiceDistoributor {
    private ILogger mLogger;

    private readonly ServiceDistributionResolveHandlerFactory mFactory;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ServiceDistoributionManager (ServiceDistributionResolveHandlerFactory factory) {
      this.mFactory = factory;
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    public void ExecuteService (ServiceType service, string intentName, object parameter) {
      this.mLogger.Debug ("[ExecuteService] ServiceType={ServiceType} IntentName={IntentName}", service, intentName);
      try {
        // 各サービスへは、Intentパラメータとして処理を呼び出す
        var serviceObj = mFactory.CreateNew (service.ToString ());
        serviceObj.Handle (new IntentParam (intentName) { ExtraData = parameter });
      } catch (Exception expr) {
        mLogger.Error (expr, "[ExecuteService] Failer Service. {@IntentName}の処理でエラーが発生しました。", intentName);
        mLogger.Error (expr.StackTrace);
      }
    }

  }
}
