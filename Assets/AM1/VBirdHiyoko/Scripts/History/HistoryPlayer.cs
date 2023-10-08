using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴の再生を管理する static クラス。
    /// </summary>
    public static class HistoryPlayer
    {
        public static IHistoryPlayer.Mode CurrentState = IHistoryPlayer.Mode.None;

        /// <summary>
        /// 指定の状態に一致するかを確認する。
        /// </summary>
        /// <param name="state">確認する状態</param>
        /// <returns>一致する時、true</returns>
        public static bool IsCurrentState(IHistoryPlayer.Mode state) => state == CurrentState;

        /// <summary>
        /// 初期化
        /// </summary>
        public static void Init()
        {
            Debug.Log("未実装");
        }

        public static void Accept()
        {
            Debug.Log("未実装");
        }
    }
}