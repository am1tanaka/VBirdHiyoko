namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴の読み込みインターフェース
    /// </summary>
    public interface IHistoryLoader
    {
        /// <summary>
        /// データを読み込んで引数に返す。
        /// </summary>
        /// <param name="stage">読み込むステージ番号。不一致なら読み込まない</param>
        /// <param name="innerStep">最終歩数</param>
        /// <param name="history">履歴データ</param>
        /// <param name="toLatest">最新状態へのデータ</param>
        /// <returns>読み込みが完了したら true。データがなければfalse</returns>
        bool Load(int stage, out ushort innerStep, out sbyte[] history, out sbyte[] toLatest);
    }
}