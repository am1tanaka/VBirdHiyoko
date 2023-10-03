namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴を保存するインターフェース
    /// </summary>
    public interface IHistorySaver
    {
        /// <summary>
        /// 履歴を保存する。
        /// </summary>
        /// <param name="stage">保存するステージ数。0=タイトル、1=Stage1</param>
        /// <param name="innterStep">内部ステップ数</param>
        /// <param name="history">履歴データ</param>
        /// <param name="toLatest">最新状態へのデータ</param>
        void Save(int stage, ushort innterStep, sbyte[] history, sbyte[] toLatest);
    }
}