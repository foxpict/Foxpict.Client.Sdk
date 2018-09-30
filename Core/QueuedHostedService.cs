using System;
using System.Threading;
using System.Threading.Tasks;
using Foxpict.Client.Sdk.Infra;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Foxpict.Client.Sdk {

  public class QueuedHostedService : IHostedService {
    private readonly ILogger mLogger;

    private CancellationTokenSource _shutdown = new CancellationTokenSource ();

    readonly IBackgroundTaskQueue mBackgroundTaskQueue;

    private Task _backgroundTask;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="backgroundTaskQueue"></param>
    public QueuedHostedService (IBackgroundTaskQueue backgroundTaskQueue) {
      this.mBackgroundTaskQueue = backgroundTaskQueue;

      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    public Task StartAsync (CancellationToken cancellationToken) {
      _backgroundTask = Task.Run (BackgroundProceessing);
      return Task.CompletedTask;
    }

    public Task StopAsync (CancellationToken cancellationToken) {
      _shutdown.Cancel ();
      return Task.WhenAny (_backgroundTask, Task.Delay (Timeout.Infinite, cancellationToken));
    }

    private async Task BackgroundProceessing () {
      while (!_shutdown.IsCancellationRequested) {
        var workItem = await mBackgroundTaskQueue.DequeueAsync (_shutdown.Token);
        try {
          await workItem (_shutdown.Token);
        } catch (Exception expr) {
          mLogger.Error (expr, $"Error occurred executing {nameof(workItem)}.");
        }
      }
    }
  }
}
