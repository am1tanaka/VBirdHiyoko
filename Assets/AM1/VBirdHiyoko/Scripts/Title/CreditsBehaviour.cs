using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class CreditsBehaviour : MonoBehaviour
    {
        public static CreditsBehaviour Instance { get; private set; }

        /// <summary>
        /// 実効モード
        /// </summary>
        public enum Mode
        {
            TitleCredits,
            EndCredits
        }

        /// <summary>
        /// 状態
        /// </summary>
        public enum State
        {
            None = -1,
            LoadScene,    // シーン読み込み中
            Hide,
            ToShow,
            Show,
            ToHide,
            EndRoll,
            WaitToTitle,
            ToTitle,
        }

        /// <summary>
        /// 現在の状態
        /// </summary>
        public static State CurrentState { get; private set; } = State.None;

        /// <summary>
        /// 開始。シーンがなければ読み込んで実行
        /// </summary>
        /// <param name="mode">開始するモード</param>
        public static void Show(Mode mode)
        {
            Debug.Log("未実装");
        }
    }
}