using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AM1.MessageSystem
{
    public class MessageWindow : MonoBehaviour
    {
        public static MessageWindow Instance { get; private set; }

        [System.Flags]
        public enum State
        {
            None = 0,
            Hide = 1,
            ToShow = 2,
            Show = 4,
            ToHide = 8,
            CloseAll = 16,
        }

        /// <summary>
        /// 閉じられない秒数
        /// </summary>
        public static float IgnoreCloseSeconds => 0.5f;

        /// <summary>
        /// 現在の状態
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// 次の状態
        /// </summary>
        public State NextState { get; private set; } = State.None;

        /// <summary>
        /// 表示中のメッセージ
        /// </summary>
        public string MessageText => messageText != null ? messageText.text : "";

        /// <summary>
        /// メッセージ表示中フラグ
        /// </summary>
        public bool IsShowing => (CurrentState != State.Hide) || (NextState != State.None);

        TextMeshProUGUI messageText;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// メッセージの表示を登録する。
        /// </summary>
        /// <param name="data">登録するメッセージのデータ</param>
        /// <returns>登録</returns>
        public bool Show(MessageData data)
        {
            return false;
        }

        /// <summary>
        /// 現在のメッセージを1つ閉じて次のメッセージ表示へ。
        /// </summary>
        /// <param name="isForce">時間の経過と関係なく無条件で閉じる時、trueを指定。デフォルトはfalse</param>
        public void Close(bool isForce = false)
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 登録済みのメッセージも含めて全て閉じる。
        /// </summary>
        public void CloseAll()
        {
            Debug.Log("未実装");
        }
    }
}