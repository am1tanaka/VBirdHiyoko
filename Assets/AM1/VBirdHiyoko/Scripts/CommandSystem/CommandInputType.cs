namespace AM1.CommandSystem
{
    /// <summary>
    /// コマンドキューの入力の種類
    /// </summary>
    [System.Flags]
    public enum CommandInputType
    {
        None = 0,
        UI = 1 << 0,
        Game = 1 << 1,
        Everything = -1
    }
}