using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 確認ダイアログ
    /// </summary>
    public class ConfirmDialog : MonoBehaviour
    {
        public static ConfirmDialog Instance { get; private set; }

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

        private void Awake()
        {
            Instance = this;
            anim = GetComponent<Animator>();
            anim.SetBool("Show", false);
        }

        /// <summary>
        /// 表示開始
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Show(ConfirmDialogData data)
        {
            Debug.Log("未実装");
            return false;
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public bool Hide()
        {
            Debug.Log("未実装");
            return false;
        }
    }
}