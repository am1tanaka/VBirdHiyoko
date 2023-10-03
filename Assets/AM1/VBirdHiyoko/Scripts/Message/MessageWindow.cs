using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.MessageSystem
{
    public class MessageWindow : MonoBehaviour
    {
        public static MessageWindow Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
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