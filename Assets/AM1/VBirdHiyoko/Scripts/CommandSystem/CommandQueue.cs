using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        /// 登録可能な入力の種類。
        /// 登録済みのコマンドには影響しない。
        /// </summary>
        public static CommandInputType CurrentInputMask { get; private set; }

        /// <summary>
        /// 次に実行予定のコマンド
        /// </summary>
        static ICommandQueueData nextCommand;

        /// <summary>
        /// 次のコマンドが設定済みのとき、true
        /// </summary>
        public static bool IsSetNextCommand => nextCommand != null;

        /// <summary>
        /// 起動時などのシステムの開始時に呼び出す。テストなどで利用。
        /// </summary>
        public static void Init()
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 指定の入力を有効にする。
        /// </summary>
        /// <param name="flag">有効にする時、true</param>
        public static void ChangeInputMask(CommandInputType type)
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 変更時のイベントを登録
        /// </summary>
        /// <param name="type">登録先の種類</param>
        /// <param name="action">登録する処理</param>
        public static void AddChangeListener(CommandInputType type, UnityAction<bool> action)
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 変更時の処理の登録を解除
        /// </summary>
        /// <param name="type">解除する種類</param>
        /// <param name="action">解除するアクション</param>
        public static void RemoveChangeListener(CommandInputType type, UnityAction<bool> action)
        {
            Debug.Log("未実装");
        }

    }
}