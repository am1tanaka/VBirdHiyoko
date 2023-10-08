using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴の再生を管理する static クラス。
    /// </summary>
    public static class HistoryPlayer
    {
        public static IHistoryPlayer.Mode CurrentState = IHistoryPlayer.Mode.None;

        /// <summary>
        /// 1手の秒数
        /// </summary>
        static float StepSeconds => 0.2f;

        /// <summary>
        /// 最大秒数。これを越える場合はStepSecondsに関わらず、この秒数で動作を終えるように再生する。
        /// </summary>
        static float MaxSeconds => 1f;

        /// <summary>
        /// 指定の状態に一致するかを確認する。
        /// </summary>
        /// <param name="state">確認する状態</param>
        /// <returns>一致する時、true</returns>
        public static bool IsCurrentState(IHistoryPlayer.Mode state) => state == CurrentState;

        /// <summary>
        /// 今回処理した最後の履歴のインデックス。継続する場合は一つ隣から。逆転の場合はその場所から。
        /// </summary>
        static int currentIndex;

        /// <summary>
        /// この後に更に移動させる相対ステップ数。0なら移動が終わったら完了。1ならRedo。-1ならUndo。
        /// </summary>
        static int relativeTargetStep;

        static IHistoryPlayer.Mode lastMove = IHistoryPlayer.Mode.None;

        static PiyoStateHistory stateHistory;

        /// <summary>
        /// 現在の経過秒数
        /// </summary>
        static float currentTime;

        /// <summary>
        /// 次の状態への残り秒数
        /// </summary>
        static float nextTime;

        /// <summary>
        /// アニメ秒数を算出するための移動回数の最大値。
        /// </summary>
        static float targetIndexMax;

        static HistoryData[] historyArray;

        /// <summary>
        /// 初期化
        /// </summary>
        public static void Init()
        {
            if (PiyoBehaviour.Instance != null)
            {
                stateHistory = PiyoBehaviour.Instance.GetInstance<PiyoStateHistory>();
            }
            CurrentState = IHistoryPlayer.Mode.None;
            lastMove = IHistoryPlayer.Mode.None;
        }

        /// <summary>
        /// 更新処理。1フレームに1回呼び出す。
        /// </summary>
        public static void Update()
        {
            switch (CurrentState)
            {
                case IHistoryPlayer.Mode.Undo:
                case IHistoryPlayer.Mode.Redo:
                    UpdateTime();
                    break;
            }
        }

        /// <summary>
        /// 時間を経過させる。
        /// </summary>
        static void UpdateTime()
        {
            currentTime += Time.deltaTime;
            if (currentTime < nextTime) return;

            VBirdHiyokoManager.Log($"current={currentTime} next={nextTime} indexMax={targetIndexMax} / relTargetStep={relativeTargetStep}");

            // 完了確認
            if (relativeTargetStep == 0)
            {
                VBirdHiyokoManager.Log($"Standby");
                PlayDoneAll();
                CurrentState = IHistoryPlayer.Mode.Standby;
                CommandQueue.ChangeInputMask(CommandInputType.Everything);
                return;
            }

            // 次の移動を開始する
            NextPlay();
        }

        /// <summary>
        /// 全てのオブジェクトの履歴再生を終了する。
        /// </summary>
        static void PlayDoneAll()
        {
            // プレイヤーの位置を更新してルートを再計算して、押せる方向を表示する
            PiyoBehaviour.Instance.Mover.UpdateFootBlockAndLand();
            PiyoBehaviour.Instance.UpdateRoute();
            PiyoBehaviour.Instance.ShowPushArrows();

            for (int i = 0; i < HistoryObjectList.Count; i++)
            {
                HistoryObjectList.objectList[i].HistoryPlayerInstance.MoveDone();
            }
        }

        /// <summary>
        /// 次の履歴移動を開始する。
        /// </summary>
        static void NextPlay()
        {
            // 移動継続
            if (lastMove == IHistoryPlayer.Mode.Undo)
            {
                if (relativeTargetStep < 0)
                {
                    // 継続してUndo
                    UndoNext();
                }
                else
                {
                    // UndoからRedoに切り替え
                    UndoToRedo();
                }
            }
            else
            {
                if (relativeTargetStep > 0)
                {
                    // 継続してRedo
                    RedoNext();
                }
                else
                {
                    // RedoからUndoへ切り替え
                    RedoToUndo();
                }
            }
        }

        /// <summary>
        /// Undoを継続する。
        /// </summary>
        static void UndoNext()
        {
            if (currentIndex <= 0)
            {
                // これ以上戻れない
                relativeTargetStep = 0;
                CurrentState = IHistoryPlayer.Mode.Standby;
                return;
            }

            // 一手戻す
            currentIndex--;
            SetNextTime();
            relativeTargetStep++;   // 一手分消化
            currentIndex = StartPlay(currentIndex, IHistoryPlayer.Mode.Undo, nextTime - currentTime);
        }

        /// <summary>
        /// Redoを継続する。
        /// </summary>
        static void RedoNext()
        {
            if (currentIndex >= historyArray.Length - 1)
            {
                // これ以上進めない
                relativeTargetStep = 0;
                CurrentState = IHistoryPlayer.Mode.Standby;
                return;
            }

            // 一手進める
            currentIndex++;
            SetNextTime();
            relativeTargetStep--;   // 一手分消化
            currentIndex = StartPlay(currentIndex, IHistoryPlayer.Mode.Redo, nextTime - currentTime);
        }

        /// <summary>
        /// UndoからRedoへ切り替え。
        /// </summary>
        static void UndoToRedo()
        {
            if (currentIndex >= historyArray.Length - 1)
            {
                // これ以上進められない
                relativeTargetStep = 0;
                CurrentState = IHistoryPlayer.Mode.Standby;
                return;
            }

            // その場から反転
            targetIndexMax = Mathf.Abs(relativeTargetStep);
            SetNextTime();
            relativeTargetStep--;   // 1手分消化
            currentIndex = StartPlay(currentIndex, IHistoryPlayer.Mode.Redo, nextTime - currentTime);
        }

        /// <summary>
        /// RedoからUndoへ切り替え。
        /// </summary>
        static void RedoToUndo()
        {
            if (currentIndex < 0)
            {
                // これ以上進められない
                relativeTargetStep = 0;
                CurrentState = IHistoryPlayer.Mode.Standby;
                return;
            }

            // その場から反転
            targetIndexMax = Mathf.Abs(relativeTargetStep);
            SetNextTime();
            relativeTargetStep++;   // 1手分消化
            currentIndex = StartPlay(currentIndex, IHistoryPlayer.Mode.Undo, nextTime - currentTime);
        }

        /// <summary>
        /// 再生秒数を更新する。
        /// </summary>
        static void SetNextTime()
        {
            float maxSec = MaxSeconds / (targetIndexMax + 1);
            float time = Mathf.Min(maxSec, StepSeconds);
            nextTime += time;
        }

        /// <summary>
        /// この状態で履歴を確定する。
        /// </summary>
        public static void Accept()
        {
            VBirdHiyokoManager.Log($"Accept CurrentState={CurrentState}");

            // 処理完了まで待つ
            if ((CurrentState != IHistoryPlayer.Mode.Standby)
                && (CurrentState != IHistoryPlayer.Mode.None))
            {
                return;
            }

            // 実行していなければそのまま戻す
            if (lastMove == IHistoryPlayer.Mode.None) return;

            // 状態をリセット
            var last = lastMove;
            CurrentState = IHistoryPlayer.Mode.None;
            lastMove = IHistoryPlayer.Mode.None;

            // currentIndexの調整
            if (last == IHistoryPlayer.Mode.Redo)
            {
                // Redoで最後のデータなら元に戻っているので処理なし
                if (currentIndex == historyArray.Length - 1) return;

                // Redoの時、削除範囲は次のデータから
                currentIndex++;
            }

            // currentIndex以降を削除する
            HistoryRecorder.UndoAccept(currentIndex);
        }

        /// <summary>
        /// 指定の履歴インデックスから再生を設定する。
        /// </summary>
        /// <param name="fromIndex">開始する履歴インデックス</param>
        /// <param name="mode">IHistoryPlayer.ModeのUndoかRedo</param>
        /// <param name="sec">現在の位置から目的座標への移動秒数</param>
        /// <returns>最後に設定した履歴データのインデックス</returns>
        public static int StartPlay(int fromIndex, IHistoryPlayer.Mode mode, float sec)
        {
            var historyArray = HistoryRecorder.HistoryArray;
            if ((fromIndex < 0) || (fromIndex >= historyArray.Length))
            {
                return fromIndex;
            }

            // 押せる矢印を消す
            PiyoBehaviour.Instance.HidePushArrows();

            int currentStep = historyArray[fromIndex].step;
            int step = mode == IHistoryPlayer.Mode.Undo ? -1 : 1;
            lastMove = mode;

            int i = fromIndex;
            while ((i >= 0) && (i < historyArray.Length) && (historyArray[i].step == currentStep))
            {
                int objIndex = historyArray[i].ObjectId;
                VBirdHiyokoManager.Log($"SetMove({i}, {mode}, {sec})");
                HistoryObjectList.objectList[objIndex].HistoryPlayerInstance.SetMove(
                    historyArray[i], mode, sec, true);
                i += step;
            }

            return i - step;
        }
    }
}