using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ステージにある履歴オブジェクトのリスト
    /// </summary>
    public static class HistoryObjectList
    {
        /// <summary>
        /// 登録可能なオブジェクトの上限。
        /// </summary>
        public static int RegisterMax => 64;
        public static readonly List<HistoryBehaviour> objectList = new List<HistoryBehaviour>(RegisterMax);

        /// <summary>
        /// 登録されている履歴オブジェクトの数。
        /// </summary>
        public static int Count { get { return objectList.Count; } }

        public static void Init()
        {
            Debug.Log("未実装");
        }
    }
}