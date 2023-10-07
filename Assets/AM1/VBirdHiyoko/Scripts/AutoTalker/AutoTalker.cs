using AM1.CommandSystem;
using AM1.MessageSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 自動発話コンポーネント。
    /// 有効化されたら時間をカウントダウンして、
    /// 設定に従って発話シナリオをPlayerStateScenarioとして登録する。
    /// </summary>
    public class AutoTalker : MonoBehaviour
    {
        public enum State
        {
            None = -1,
            CountDown,
            Talk,
            Stop
        }

        /// <summary>
        /// 現在の状態
        /// </summary>
        public State CurrentState { get; private set; } = State.None;

        /// <summary>
        /// 自動発話用のデータ
        /// </summary>
        AutoTalkerData autoTalkerData;

        /// <summary>
        /// 発話シナリオプレイヤー
        /// </summary>
        AutoTalkCommand autoTalkCommand = new();

        /// <summary>
        /// 次の発話までの時間
        /// </summary>
        float waitTime = 0f;

        /// <summary>
        /// 次に発話する、あるいは発話中の発話データのインデックス
        /// </summary>
        int messageIndex = 0;

        /// <summary>
        /// 発話順のインデックス配列
        /// </summary>
        int[] messageIndexArray;

        private void Update()
        {
            CountDown();
        }

        /// <summary>
        /// 自動発話カウントダウン
        /// </summary>
        void CountDown()
        {
            if (!autoTalkerData || (CurrentState != State.CountDown)) { return; }
            if (MessageWindow.Instance.IsShowing || !PiyoBehaviour.Instance.IsState<PiyoStateWaitInput>()) { return; }

            waitTime -= Time.deltaTime;

            // 時間まだ
            if (waitTime > 0) { return; }

            // 時間になった
            CurrentState = State.Talk;
            autoTalkCommand.SetData(this, autoTalkerData.messageData[messageIndexArray[messageIndex]]);
            CommandQueue.EntryCommand(autoTalkCommand);
        }

        /// <summary>
        /// 指定の自動発話用のデータをセット
        /// </summary>
        /// <param name="data">設定する発話データ</param>
        public void SetTalkData(AutoTalkerData data)
        {
            autoTalkerData = data;
            if ((messageIndexArray == null) || (messageIndexArray.Length != data.messageData.Length))
            {
                messageIndexArray = null;
                messageIndexArray = new int[data.messageData.Length];
            }
            messageIndex = 0;

            if (autoTalkerData.isRandom)
            {
                RandomIndex();
            }
            else
            {
                for (int i = 0; i < messageIndexArray.Length; i++)
                {
                    messageIndexArray[i] = i;
                }
            }
            waitTime = data.firstTalkSeconds;
        }

        /// <summary>
        /// インデックス配列をランダムに並び替える。
        /// </summary>
        void RandomIndex()
        {
            int[] indices = new int[messageIndexArray.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }
            for (int i = 0; i < indices.Length; i++)
            {
                int r = Random.Range(i, indices.Length);
                messageIndexArray[i] = indices[r];
                indices[r] = indices[i];
            }
        }

        /// <summary>
        /// 自動発話をするかどうかを設定する。
        /// </summary>
        /// <param name="flag">自動発話を行う時、true</param>
        public void SetTimerActive(bool flag)
        {
            CurrentState = flag ? State.CountDown : State.Stop;
            if (autoTalkerData != null)
            {
                waitTime = autoTalkerData.firstTalkSeconds;
            }
        }

        /// <summary>
        /// 発話が終わった時に呼び出して、次の発話を設定する。
        /// </summary>
        public void TalkDone()
        {
            messageIndex++;
            if (messageIndex >= autoTalkerData.messageData.Length)
            {
                // ループ
                if (autoTalkerData.isRandom)
                {
                    RandomIndex();
                }
                messageIndex = 0;
            }
            waitTime = autoTalkerData.waitTalkSeconds;

            if (CurrentState == State.Talk)
            {
                CurrentState = State.CountDown;
            }
        }

        /// <summary>
        /// 発話がキャンセルされた時に呼び出される。少し時間を置いてから再発話。
        /// </summary>
        public void DeniedTalk()
        {
            // 最初の時間に巻き戻す
            // 発話内容はそのまま
            if (autoTalkerData != null)
            {
                waitTime = autoTalkerData.firstTalkSeconds;
            }

            if (CurrentState == State.Talk)
            {
                CurrentState = State.CountDown;
            }
        }
    }
}