using System;
using Foxpict.Client.Sdk.Infra;
using Hyperion.Pf.Workflow;
using Hyperion.Pf.Workflow.StateMachine.Eventsas;
using NLog;
using Pixstock.Applus.Foundations.ContentBrowser.Transitions;
using Pixstock.Core;
using SimpleInjector;

namespace Foxpict.Client.Sdk.Core.Workflow
{
  public class PixstockMainContent : Hyperion.Pf.Workflow.Content, IPixstockContent
  {
    private readonly ILogger mLogger;

    readonly CategoryTreeTransitionWorkflow mWorkflow;

    readonly Container mContainer;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <returns></returns>
    public PixstockMainContent(Container container) : base("PixstockMainContent")
    {
      this.mLogger = LogManager.GetCurrentClassLogger();
      this.mContainer = container;
      this.mWorkflow = new CategoryTreeTransitionWorkflow(container);
      mWorkflow.InvokeShowFrame += OnInvokeShowFrame;
      mWorkflow.InvokeHideFrame += OnInvokeHideFrame;
    }

    /// <summary>
    /// イベント名（文字列）からイベントを実行する
    /// </summary>
    /// <param name="workflowEvent"></param>
    public void FireWorkflowEvent(Container context, string workflowEvent, object param)
    {
      try
      {
        var events = Events.ForName(workflowEvent);
        mWorkflow.Fire(events, param);
      }
      catch (Exception expr)
      {
        mLogger.Error(expr, "[FireWorkflowEvent] Event Not Found (EventName={EventName}).", workflowEvent);
      }
    }

    /// <summary>
    /// ワークフローのUIパーツ表示指示イベントのハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnInvokeShowFrame(object sender, InvokeShowFrameEventArgs args)
    {
      try
      {
        // this.mLogger.LogDebug(LoggingEvents.Undefine, "[PixstockMainContent][OnInvokeShowFrame] - IN");
        // this.mLogger.LogDebug(LoggingEvents.Undefine, "   mContainer=" + this.mContainer);
        // this.mLogger.LogDebug(LoggingEvents.Undefine, "   ScreenManager=" + this.ScreenManager);
        // ScreenManagerを呼び出して、表示処理を行う
        this.ScreenManager.ShowScreen(args.FrameName);
      }
      catch (Exception expr)
      {
        mLogger.Error(expr, "[OnInvokeShowFrame] Exception.");
      }
    }

    /// <summary>
    /// ワークフローのUIパーツ非表示指示イベントのハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnInvokeHideFrame(object sender, InvokeHideFrameEventArgs args)
    {
      try
      {
        // ScreenManagerを呼び出して、非表示処理を行う
        this.ScreenManager.HideScreen(args.FrameName);
      }
      catch (Exception expr)
      {
        mLogger.Error(expr, "[OnInvokeHideFrame] Exception.");
      }
    }

    IScreenManager ScreenManager => this.mContainer.GetInstance<IScreenManager>();

    public override void OnDestroy()
    {
      //throw new System.NotImplementedException();
    }

    public override void OnDiscontinue()
    {
      //throw new System.NotImplementedException();
    }

    public override void OnEnd()
    {
      //throw new System.NotImplementedException();
    }

    public override void OnIdle()
    {
      //throw new System.NotImplementedException();
    }

    public override void OnInitialize()
    {
      //throw new System.NotImplementedException();
      mWorkflow.Setup();
    }

    public override bool OnPreResume()
    {
      //throw new System.NotImplementedException();
      return true;
    }

    public override bool OnPreStop()
    {
      //throw new System.NotImplementedException();
      return true;
    }

    public override void OnRestart()
    {
      //throw new System.NotImplementedException();
    }

    public override void OnResume()
    {
      mWorkflow.Start();
      CompleteStart();
    }

    public override void OnRun()
    {
      //throw new System.NotImplementedException();
    }

    public override void OnStop()
    {
      mWorkflow.Stop();
      CompleteStop();
    }

    public override void OnSuspend()
    {
      //throw new System.NotImplementedException();
    }
  }

}
