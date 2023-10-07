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
        /// <summary>
        /// 表示するメッセージのID
        /// </summary>
        public string MessageID = default;
        /// <summary>
        /// 自動で閉じる秒数。0の時、自動で閉じない。
        /// </summary>
        public float Seconds = default;
        public bool AutoClose => !Mathf.Approximately(Seconds, 0f);

        public MessageData(string mesID, float sec = 0)
        {
            MessageID = mesID;
            Seconds = sec;
        }

        /// <summary>
        /// データを設定
        /// </summary>
        /// <param name="mesID">設定するメッセージID</param>
        /// <param name="sec">秒数。省略すると0</param>
        public void Set(string mesID, float sec = 0)
        {
            MessageID = mesID;
            Seconds = sec;
        }
    }
}