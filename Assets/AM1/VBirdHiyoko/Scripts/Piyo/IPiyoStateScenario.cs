namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// PiyoStateScenarioに設定するシナリオデータ用のインターフェース
    /// </summary>
    public interface IPiyoStateScenario
    {
        /// <summary>
        /// シナリオのテキストを返す。
        /// </summary>
        string ScenarioText { get; }

        /// <summary>
        /// 処理完了時に呼び出す処理
        /// </summary>
        public void Done();
    }
}