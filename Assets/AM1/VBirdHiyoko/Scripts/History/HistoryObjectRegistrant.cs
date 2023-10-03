using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴を持っているゲームオブジェクトを座標を元に整列してデータを登録する。
    /// </summary>
    public static class HistoryObjectRegistrant
    {
        /// <summary>
        /// シーンに配置されたHistoryBehaviourを検索して、X,Y,Zの小さい順に並び替えて、その順でHistoryRecorderへ登録する。
        /// </summary>
        public static void RegisterObjects()
        {
            Debug.Log($"未登録");
        }
    }
}