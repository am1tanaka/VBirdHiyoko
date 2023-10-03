namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴をメモリで管理する開発用のストレージクラス
    /// </summary>
    public class HistoryMemoryStorage : IHistoryLoader, IHistorySaver
    {
        public bool Load(int stage, out ushort innerStep, out sbyte[] history, out sbyte[] toLatest)
        {
            throw new System.NotImplementedException();
        }

        public void Save(int stage, ushort innterStep, sbyte[] history, sbyte[] toLatest)
        {
            throw new System.NotImplementedException();
        }
    }
}