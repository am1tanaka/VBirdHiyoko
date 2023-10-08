namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴をメモリで管理する開発用のストレージクラス
    /// </summary>
    public class HistoryMemoryStorage : IHistoryLoader, IHistorySaver
    {
        /// <summary>
        /// データが記録されている時、true
        /// </summary>
        bool isSaved = false;

        /// <summary>
        /// 保存している履歴のステージ番号
        /// </summary>
        int savedStage = 0;

        /// <summary>
        /// 内部ステップ数
        /// </summary>
        ushort savedInnerStep = 0;

        /// <summary>
        /// 履歴配列
        /// </summary>
        sbyte[] historyArray;

        /// <summary>
        /// 最新データへの配列
        /// </summary>
        sbyte[] toLatestArray;

        public bool Load(int stage, out ushort innerStep, out sbyte[] history, out sbyte[] toLatest)
        {
            if (!isSaved || (stage != savedStage))
            {
                innerStep = savedInnerStep;
                history = new sbyte[0];
                toLatest = new sbyte[0];
                return false;
            }

            innerStep = savedInnerStep;
            history = new sbyte[historyArray.Length];
            historyArray.CopyTo(history, 0);
            toLatest = new sbyte[toLatestArray.Length];
            toLatestArray.CopyTo(toLatest, 0);

            VBirdHiyokoManager.Log($"innerStep={innerStep} history={history.Length} toLate={toLatest.Length}");
            return true;
        }

        public void Save(int stage, ushort innterStep, sbyte[] history, sbyte[] toLatest)
        {
            savedStage = stage;
            savedInnerStep = innterStep;

            historyArray = null;
            historyArray = new sbyte[history.Length];
            history.CopyTo(historyArray, 0);

            toLatestArray = null;
            toLatestArray = new sbyte[toLatest.Length];
            toLatest.CopyTo(toLatestArray, 0);
            isSaved = true;

            VBirdHiyokoManager.Log($"history={history.Length} toLatest={toLatest.Length} historyArray={historyArray.Length} toLatestArray={toLatestArray.Length}");
        }
    }
}