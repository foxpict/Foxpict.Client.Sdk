using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foxpict.Client.Sdk.Infra;
using Foxpict.Client.Sdk.Intent;
using NLog;

namespace Foxpict.Client.Sdk {
  /// <summary>
  /// DIコンテナに登録する画面遷移状態保持および画面遷移イベント制御用の管理クラス
  /// </summary>
  public class ScreenManager : IScreenManager {
    private readonly ILogger mLogger;

    /// <summary>
    /// 戻る遷移スタック
    /// </summary>
    readonly List<ScreenItem> mTransBackStack = new List<ScreenItem> ();

    /// <summary>
    /// 画面遷移のスタック
    /// フロントエンドの画面スタックと同期すること。
    /// </summary>
    /// <typeparam name="ScreenItem"></typeparam>
    /// <returns></returns>
    readonly Stack<ScreenItem> mScreenStack = new Stack<ScreenItem> ();

    /// <summary>
    /// ワークフローの画面遷移により、発生した画面表示非表示イベントのリスト
    /// このリストは、ビュー層へのみ処理の画面遷移を示している。
    /// ビュー層への画面処理メッセージ発行でリスト内容をクリアする。
    /// </summary>
    /// <typeparam name="ScreenEventItem"></typeparam>
    /// <returns></returns>
    List<ScreenEventItem> mScreenTransitionEventList = new List<ScreenEventItem> ();

    /// <summary>
    /// 戻る遷移マップ
    /// </summary>
    /// <remarks>
    /// Key = 戻る遷移を実行する画面状態名
    /// Value = 戻り先の画面状態名と、遷移イベントの対応表
    /// </remarks>
    Dictionary<string, BackTransitionMapItem[]> mBackTrantionMap = new Dictionary<string, BackTransitionMapItem[]> ();

    readonly IIntentManager mIntentManager;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="intentManager"></param>
    public ScreenManager (IIntentManager intentManager) {
      this.mIntentManager = intentManager;
      this.mLogger = LogManager.GetCurrentClassLogger ();
      this.mBackTrantionMap = Builder.Make (); // 戻る遷移テーブルを取得　
    }

    /// <summary>
    /// 戻る遷移を行います
    /// </summary>
    public void BackScreen () {
      mLogger.Info (string.Format ("[BackScreen] mTransBackStack.Count: {0}", mTransBackStack.Count));

      if (mTransBackStack.Count < 2) {
        mLogger.Debug ("戻るスタックに戻る遷移するための遷移先が足りません");
        return;
      }

      int lastPos = mTransBackStack.Count - 1;
      var lastScreen = mTransBackStack.ElementAt (lastPos);
      var nextScreen = mTransBackStack.ElementAt (lastPos - 1);
      mTransBackStack.RemoveAt (lastPos);

      string currentScreen = lastScreen.mScreenName; // 戻る遷移での遷移先画面状態名(戻るスタックから取得する)
      string transitionScreen = nextScreen.mScreenName; // 戻る遷移での遷移先画面状態名(戻るスタックから取得する)
      mLogger.Info (string.Format ("[BackScreen] currentScreen: {0}  transitionScreen: {1}", currentScreen, transitionScreen));

      if (mBackTrantionMap.ContainsKey (currentScreen)) {
        var backTransitionEventName = (from u in mBackTrantionMap[currentScreen] where u.TransitionScreenName == transitionScreen select u.TransitionEventName).FirstOrDefault ();
        mLogger.Info (string.Format ("[BackScreen] backTransitionEventName: {0}", backTransitionEventName));

        if (!string.IsNullOrEmpty (backTransitionEventName)) {
          mLogger.Warn (string.Format ("[BackScreen] 戻る遷移テーブルから、戻り先「{0}」を見つけることができません", transitionScreen));
          mIntentManager.AddIntent (ServiceType.Workflow, backTransitionEventName, "FACTOR=BACK");
        }
      } else {
        mLogger.Warn (string.Format ("[BackScreen] 戻る遷移テーブルから「{0}」を見つけることができません", currentScreen));
      }
    }

    /// <summary>
    /// スクリーンを表示します
    /// </summary>
    /// <param name="screenName">表示するスクリーン名</param>
    public void ShowScreen (string screenName) {
      mLogger.Info (string.Format ("[ShowScreen] ScreenName={0}", screenName));
      this.RemoveScreenBackStackItem (screenName);
      this.mTransBackStack.Add (new ScreenItem (screenName));

      // スタックに画面情報を追加する
      mScreenTransitionEventList.Add (new ScreenEventItem {
        ScreenName = screenName,
          PushEventFlag = true
      });
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="screenName"></param>
    public void HideScreen (string screenName) {
      mScreenTransitionEventList.Add (new ScreenEventItem {
        ScreenName = screenName,
          PushEventFlag = false
      });
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="param"></param>
    public void UpdateScreenTransitionView (object param) {
      this.mLogger.Debug ("[UpdateScreenTransitionView] - IN");
      this.mLogger.Debug ("[UpdateScreenTransitionView] Count=" + mScreenTransitionEventList.Count);
      var viewEventList = new List<UpdateViewIntentParameter> ();

      var copy = new List<ScreenEventItem> (mScreenTransitionEventList);
      mScreenTransitionEventList.Clear ();

      ScreenEventItem lastItem = null;
      foreach (var item in copy) {
        if (mScreenStack.Count == 1 &&
          !item.PushEventFlag &&
          lastItem == null) {
          lastItem = item;
        } else if (mScreenStack.Count == 1 &&
          lastItem != null &&
          lastItem.ScreenName == item.ScreenName) {
          // set
          viewEventList.Add (new UpdateViewIntentParameter {
            ScreenName = item.ScreenName,
              UpdateType = UpdateType.SET
          });

          lastItem = null;
        } else {
          if (lastItem != null) {
            if (lastItem.PushEventFlag) {
              // push
              viewEventList.Add (new UpdateViewIntentParameter {
                ScreenName = lastItem.ScreenName,
                  UpdateType = UpdateType.PUSH
              });

              mScreenStack.Push (new ScreenItem (item.ScreenName));
            } else {
              // pop
              viewEventList.Add (new UpdateViewIntentParameter {
                ScreenName = lastItem.ScreenName,
                  UpdateType = UpdateType.POP
              });
              mScreenStack.Pop ();
            }
          }

          if (item.PushEventFlag) {
            // push
            var viewEvent = new UpdateViewIntentParameter {
              ScreenName = item.ScreenName,
              UpdateType = UpdateType.PUSH
            };

            if (mScreenStack.Count == 0) {
              viewEvent.UpdateType = UpdateType.SET;
            }

            viewEventList.Add (viewEvent);
            mScreenStack.Push (new ScreenItem (item.ScreenName));
          } else {
            // pop
            viewEventList.Add (new UpdateViewIntentParameter {
              ScreenName = item.ScreenName,
                UpdateType = UpdateType.POP
            });
            mScreenStack.Pop ();
          }

          lastItem = null;
        }
      }

      // 画面表示の切り替えがある場合のみ、UpdateViewメッセージを送信する
      if (viewEventList.Count > 0) {
        var peekItem = mScreenStack.Peek ();

        var nextScreen = "";
        if (peekItem != null) {
          this.mLogger.Debug ("[UpdateScreenTransitionView] - スタック内の次の要素={@ScreenName}", peekItem.mScreenName);
          nextScreen = peekItem.mScreenName;
        }
        this.DumpBackStack();

        mIntentManager.AddIntent (ServiceType.FrontendIpc, "UpdateView", new UpdateViewResponse {
          ViewEventList = viewEventList,
            Parameter = param,
            NextScreenName = nextScreen
        });
      }
    }

    /// <summary>
    /// デバッグ用コマンドで、戻るスタックをクリアします
    /// </summary>
    public void ResetStack () {
      mScreenStack.Clear();
    }

    /// <summary>
    /// 戻る遷移スタックをデバッグ出力します
    /// </summary>
    public void DumpBackStack () {
      var sb = new StringBuilder ();
      foreach (var prop in mTransBackStack) {
        sb.AppendLine (prop.mScreenName);
      }

      mLogger.Debug (sb.ToString ());
    }

    private void RemoveScreenBackStackItem (string screenName) {
      int lastIndex = mTransBackStack.FindLastIndex (finder);
      if (lastIndex != -1) {
        mTransBackStack.RemoveRange (lastIndex, mTransBackStack.Count - lastIndex);
      }

      bool finder (ScreenItem item) {
        return item.mScreenName == screenName;
      }
    }

    private class Builder {
      /// <summary>
      /// 戻る遷移テーブル
      /// </summary>
      /// <remarks>
      /// 戻る機能を使って戻りスタックから戻る処理をする際の戻り先状態名と戻りに使用する状態遷移イベント名の対応表を定義します。
      /// /// </remarks>
      /// <returns></returns>
      internal static Dictionary<string, BackTransitionMapItem[]> Make () {
        var result = new Dictionary<string, BackTransitionMapItem[]> {
            {
            // Preview
            "Preview",
            new BackTransitionMapItem[] {
            new BackTransitionMapItem { TransitionEventName = "TRNS_BACK", TransitionScreenName = "Finder" }
            }
            },
            {
            // ContentListPreview
            "ContentListPreview",
            new BackTransitionMapItem[] {
            new BackTransitionMapItem { TransitionEventName = "TRNS_BACK", TransitionScreenName = "Finder" }
            }
            }
          };

        return result;
      }
    }

    public class ScreenItem {
      internal string mScreenName;

      public ScreenItem (string screenName) {
        this.mScreenName = screenName;
      }
    }

    public class ScreenEventItem {
      public string ScreenName;

      public bool PushEventFlag;
    }

    public class UpdateViewResponse {
      public List<UpdateViewIntentParameter> ViewEventList;

      public object Parameter;

      public string NextScreenName;
    }

    /// <summary>
    /// 戻る遷移定義
    /// </summary>
    public struct BackTransitionMapItem {
      /// <summary>
      /// 戻る遷移先の画面状態名
      /// </summary>
      public string TransitionScreenName;

      /// <summary>
      /// 遷移イベント名
      /// </summary>
      public string TransitionEventName;
    }
  }

}
