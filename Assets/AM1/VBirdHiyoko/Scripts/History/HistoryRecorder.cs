using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴の記録を統括するクラス。
    /// TODO: Unity2021.3から使えるObjectPoolで代替可能な機能がある。
    /// </summary>
    public static class HistoryRecorder
    {
        /// <summary>
        /// 履歴保存用クラス
        /// </summary>
        public static IHistorySaver historySaver;

        /// <summary>
        /// 履歴読み込み用クラス
        /// </summary>
        public static IHistoryLoader historyLoader;

    }
}