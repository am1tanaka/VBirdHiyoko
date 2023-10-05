using AM1.CommandSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.MessageSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 自動発話を登録するコマンド。
    /// </summary>
    public class AutoTalkCommand : ICommandQueueData
    {
        MessageData autoTalkData;

        /// <summary>
        /// 発話するメッセージデータを受け取る。
        /// </summary>
        /// <param name="messageData">発話するデータ</param>
        public void SetAutoTalkData(MessageData messageData)
        {
            autoTalkData = messageData;
        }

        public void Invoke()
        {
            Debug.Log($"未実装 {autoTalkData.Message}を設定");

            // メッセージ発話中の時は発話キャンセル

            // メッセージに登録できたら自動発話成功
        }
    }
}