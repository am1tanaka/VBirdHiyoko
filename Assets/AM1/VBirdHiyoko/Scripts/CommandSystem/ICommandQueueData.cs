namespace AM1.CommandSystem
{
    public interface ICommandQueueData
    {
        /// <summary>
        /// 種類を返す
        /// </summary>
        CommandInputType Type => CommandInputType.None;

        /// <summary>
        /// 優先度。大きいほど優先
        /// </summary>
        int Priority => 0;

        /// <summary>
        /// コマンドを実行する
        /// </summary>
        void Invoke();
    }
}