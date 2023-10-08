using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 確認ダイアログ
    /// </summary>
    public class ConfirmDialog : MonoBehaviour
    {
        public static ConfirmDialog Instance { get; private set; }

        [Tooltip("メッセージ"), SerializeField]
        TextMeshProUGUI messageText = default;
        [Tooltip("左ボタンテキスト"), SerializeField]
        TextMeshProUGUI leftText = default;
        [Tooltip("右ボタンテキスト"), SerializeField]
        TextMeshProUGUI rightText = default;

        public enum State
        {
            Hide,
            ToShow,
            Show,
            ToHide,
        }

        public static State CurrentState { get; private set; } = State.Hide;

        /// <summary>
        /// 現在が指定の状態かを確認する。
        /// </summary>
        /// <param name="state">確認したい状態</param>
        /// <returns>一致する時、true</returns>
        public static bool IsCurrentState(State state) => state == CurrentState;

        Animator anim;
        ConfirmDialogData confirmDialogData;

        private void Awake()
        {
            Instance = this;
            anim = GetComponent<Animator>();
            anim.SetBool("Show", false);
        }

        /// <summary>
        /// 表示開始
        /// </summary>
        /// <param name="data">確認ダイアログが実行する情報</param>
        /// <returns>表示できたら、true</returns>
        public bool Show(ConfirmDialogData data)
        {
            if (CurrentState != State.Hide) return false;

            confirmDialogData = data;
            CurrentState = State.ToShow;
            anim.SetBool("Show", true);
            messageText.text = data.Message;
            leftText.text = data.LeftButtonText;
            rightText.text = data.RightButtonText;
            return true;
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public bool Hide()
        {
            if ((CurrentState != State.Show)
                && (CurrentState != State.ToShow)) return false;

            CurrentState = State.ToHide;
            anim.SetBool("Show", false);
            return true;
        }

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        public void HideButton()
        {
            if (Hide())
            {
                SEPlayer.Play(SEPlayer.SE.Cancel);
            }
        }

        /// <summary>
        /// 表示が完了したらアニメから呼び出す。
        /// </summary>
        public void ShowDone()
        {
            if (CurrentState == State.ToHide)
            {
                anim.SetBool("Show", false);
            }
            CurrentState = State.Show;
        }

        /// <summary>
        /// 非表示が完了したらアニメから呼び出す。
        /// </summary>
        public void HideDone()
        {
            CurrentState = State.Hide;
        }

        /// <summary>
        /// 左ボタンを押した処理
        /// </summary>
        public void OnClickLeft()
        {
            if (CurrentState != State.Show) return;

            confirmDialogData?.LeftAction();
        }

        /// <summary>
        /// 右ボタンを押した処理
        /// </summary>
        public void OnClickRight()
        {
            if (CurrentState != State.Show) return;

            confirmDialogData?.RightAction();
        }
    }
}