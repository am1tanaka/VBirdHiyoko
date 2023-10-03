using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.CommandSystem
{
    /// <summary>
    /// 操作コマンドを扱う static クラス
    /// 操作できるものをマスクで指定したり、操作可能な設定が変化したら
    /// 変更処理を呼び出す。
    /// マスクや優先度から最適なコマンドを nextState に記録して、
    /// 次のフレームで実行する。
    /// 実効は1フレームで完了させる。複数にまたがる場合はPlayerStateScenarioを使う。
    /// </summary>
    public static class CommandQueue
    {
        /// <summary>
        /// 指定の入力を有効にする。
        /// </summary>
        /// <param name="flag">有効にする時、true</param>
        public static void ChangeInputMask(CommandInputType type)
        {
            Debug.Log("未実装");
        }
    }
}