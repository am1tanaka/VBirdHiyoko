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
        /// 現在の状態
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// 表示中のメッセージ
        /// </summary>
        public string MessageText => messageText != null ? messageText.text : "";

        TextMeshProUGUI messageText;

        private void Awake()
        {
            Instance = this;
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