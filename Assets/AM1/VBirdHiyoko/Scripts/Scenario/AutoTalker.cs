using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.ScenarioSystem
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
        /// 自動発話用のクラスのインスタンスを設定
        /// </summary>
        /// <param name="player">再生用クラスのインスタンス</param>
        public void SetAutoTalkPlayer(AutoTalkScenario player)
        {
            autoTalkPlayer = player;
        }

    }
}