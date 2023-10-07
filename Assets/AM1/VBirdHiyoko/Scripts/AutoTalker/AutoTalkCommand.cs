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
        AutoTalker autoTalker;
        MessageData autoTalkData;

        /// <summary>
        /// 発話するメッセージデータを受け取る。
        /// </summary>
        /// <param name="messageData">発話するデータ</param>
        public void SetAutoTalkData(MessageData messageData)
        {
            autoTalkData = messageData;
        }

        /// <summary>
        /// メッセージウィンドウ用のデータをセット
        /// </summary>
        /// <param name="talker">AutoTalkerのインスタンス</param>
        /// <param name="data">表示するメッセージ</param>
        public void SetData(AutoTalker talker, MessageData data)
        {
            autoTalker = talker;
            autoTalkData = data;
        }

        public void Invoke()
        {
            // メッセージ発話中の時は発話キャンセル
            if (MessageWindow.Instance.IsShowing)
            {
                autoTalker.DeniedTalk();
            }
            else
            {
                // メッセージに登録できたら自動発話成功
                bool result = MessageWindow.Instance.Show(autoTalkData);
                if (result)
                {
                    autoTalker.TalkDone();
                }
            }
        }
    }
}