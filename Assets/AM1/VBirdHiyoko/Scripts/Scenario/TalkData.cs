namespace AM1.ScenarioSystem
{
    [System.Serializable]
    public class TalkData
    {
        /// <summary>
        /// メッセージID
        /// </summary>
        public string messageID;

        /// <summary>
        /// 表示秒数。0の時、ボタン押下待ち
        /// </summary>
        public float showSeconds;

        public TalkData(string mes, float sec)
        {
            messageID = mes;
            showSeconds = sec;
        }
    }
}