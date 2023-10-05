using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    public class TitleBehaviour : MonoBehaviour
    {
        public static TitleBehaviour Instance { get; private set; }

        public enum State
        {
            None = -1,
            Play,       // 操作受付
            Credits,    // クレジット表示中
            GameStart,  // ゲーム開始へ。処理なし
            Dialog,     // ダイアログ表示中
        }

        /// <summary>
        /// 現在の状態
        /// </summary>
        public State CurrentState { get; private set; } = State.None;
        State nextState = State.None;
        /// <summary>
        /// 状態の切り替えができるかを確認
        /// </summary>
        public bool CanChangeState =>
            ((nextState == State.None) && (CurrentState != State.GameStart));

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// 状態変更。すでに他の状態が設定済みの時は受け入れない
        /// </summary>
        /// <param name="state">切り替えたい状態</param>
        /// <returns>true=受け入れ</returns>
        public bool ChangeState(State state)
        {
            if (!CanChangeState)
            {
                return false;
            }

            nextState = state;
            return true;
        }

        /// <summary>
        /// クレジットリンクをクリックした時の処理
        /// </summary>
        public void OnClickCredits()
        {
            Debug.Log("未実装");
        }
    }
}