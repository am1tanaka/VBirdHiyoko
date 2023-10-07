using System.Collections;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 文字列シナリオのコマンドインターフェース
    /// </summary>
    public interface IScenarioTextCommand
    {
        /// <summary>
        /// 行をスペースで分離した文字列配列を受け取って、コマンドが自分を表しているかどうかを返す。
        /// </summary>
        /// <param name="words">0にコマンド。1にコンマ区切りのパラメーター</param>
        /// <returns>自分のコマンドの時、true。</returns>
        bool IsCommand(string[] words);

        /// <summary>
        /// コマンドを実行する。
        /// </summary>
        IEnumerator Invoke();
    }
}