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
        /// 指定の自動発話用のデータをセット
        /// </summary>
        /// <param name="data">設定する発話データ</param>
        public void SetTalkData(AutoTalkerData data)
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 自動発話をするかどうかを設定する。
        /// </summary>
        /// <param name="flag">自動発話を行う時、true</param>
        public void SetTimerActive(bool flag)
        {
            Debug.Log("未実装");
        }
    }
}