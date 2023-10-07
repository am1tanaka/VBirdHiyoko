using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.MessageSystem
{
    [System.Serializable]
    /// <summary>
    /// メッセージ１つ分のデータ
    /// </summary>
    public class MessageData
    {
        public string Message = default;
        /// <summary>
        /// 自動で閉じる秒数。0の時、自動で閉じない。
        /// </summary>
        public float Seconds = default;
        public bool AutoClose => !Mathf.Approximately(Seconds, 0f);
        public MessageData(string mes, float sec = 0)
        {
            Message = mes;
            Seconds = sec;
        }

        /// <summary>
        /// データを設定
        /// </summary>
        /// <param name="mes"></param>
        /// <param name="sec"></param>
        public void Set(string mes, float sec = 0)
        {
            Message = mes;
            Seconds = sec;
        }
    }
}