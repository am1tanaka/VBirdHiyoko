namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴再生処理のインターフェース。
    /// </summary>
    public interface IHistoryPlayer
    {
        public enum Mode
        {
            None = -1,
            Undo,
            Redo,
            Standby,
        }

        /// <summary>
        /// 指定のデータへの移行を開始する。
        /// </summary>
        /// <param name="data">移動先のデータ</param>
        /// <param name="mode">UndoかRedoか</param>
        /// <param name="sec">切り替えに要する秒数</param>
        /// <param name="isStart">開始時、true。動作中の変更の時、false</param>
        void SetMove(HistoryData data, Mode mode, float sec, bool isStart);

        /// <summary>
        /// 移動完了。目的状態にして、移動を完了させる。
        /// </summary>
        void MoveDone();
    }
}