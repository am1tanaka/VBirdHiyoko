using AM1.MessageSystem;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    [CreateAssetMenu(menuName = "AM1/ScenarioSystem/AutoTalkerData")]
    /// <summary>
    /// 自動発話用のデータ
    /// </summary>
    public class AutoTalkerData : ScriptableObject
    {
        [Tooltip("ランダム発話にする時、チェック。デフォルトでは順番通りループ")]
        public bool isRandom = false;

        [Tooltip("最初の発話までの秒数")]
        public float firstTalkSeconds = 2;

        [Tooltip("2回目以降の発話までの秒数")]
        public float waitTalkSeconds = 4;

        [Tooltip("メッセージデータのリスト")]
        public MessageData[] messageData = default;

        public void Set(bool isRand, float firstTalkSec, float waitTalkSec, MessageData[] data)
        {
            isRandom = isRand;
            firstTalkSeconds = firstTalkSec;
            waitTalkSeconds = waitTalkSec;
            messageData = data;
        }
    }
}