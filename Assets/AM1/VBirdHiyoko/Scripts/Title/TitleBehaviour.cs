using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;
using AM1.CommandSystem;
using UnityEngine.UI;

namespace AM1.VBirdHiyoko
{
    public class TitleBehaviour : MonoBehaviour
    {
        public static TitleBehaviour Instance { get; private set; }

        [Tooltip("タイトルのCanvasGroup"), SerializeField]
        CanvasGroup titleCanvasGroup = default;

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

        AutoTalker autoTalker;
        AutoTalker AutoTalkerInstance
        {
            get
            {
                if (autoTalker == null)
                {
                    autoTalker = GameObject.FindObjectOfType<AutoTalker>();
                }
                return autoTalker;
            }
        }

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
            }
        }

        private void Update()
        {
            if (SceneStateChanger.IsChanging) return;

            CommandQueue.Update();
            ChangeState();
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
        /// 状態切り替え処理
        /// </summary>
        void ChangeState()
        {
            if (nextState == State.None) return;

            CurrentState = nextState;
            nextState = State.None;

            switch (CurrentState)
            {
                case State.Play:
                    titleCanvasGroup.interactable = true;
                    AutoTalkerInstance.SetTimerActive(true);
                    PiyoBehaviour.Instance.EnqueueState<PiyoStateWaitInput>();
                    break;
                case State.Credits:
                    titleCanvasGroup.interactable = false;
                    AutoTalkerInstance.SetTimerActive(false);
                    CreditsBehaviour.Show(CreditsBehaviour.Mode.TitleCredits);
                    break;
                case State.GameStart:
                    titleCanvasGroup.interactable = false;
                    AutoTalkerInstance.SetTimerActive(false);
                    GameSceneStateChanger.Instance.Request(false);
                    break;
                case State.Dialog:
                    AutoTalkerInstance.SetTimerActive(false);
                    titleCanvasGroup.interactable = false;
                    break;
            }
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