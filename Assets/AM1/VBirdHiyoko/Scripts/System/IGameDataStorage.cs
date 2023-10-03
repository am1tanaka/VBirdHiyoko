namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ゲームデータの初期化と保存
    /// </summary>
    public interface IGameDataStorage
    {
        /// <summary>
        /// 初期化と読み込みを実行。初期値はGameDataDefaultsに定義
        /// </summary>
        void InitAndLoad();

        /// <summary>
        /// 指定のデータを保存
        /// </summary>
        /// <param name="newStage">新しい値</param>
        void SaveClearedStage(int newStage);

        /// <summary>
        /// 全ての記録データを削除
        /// </summary>
        void DeleteAll();
    }
}
