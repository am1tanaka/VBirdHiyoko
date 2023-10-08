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
            objectList.Clear();
        }

        /// <summary>
        /// 指定の履歴オブジェクトを登録する。
        /// </summary>
        /// <param name="behaviour">登録したいインスタンス</param>
        /// <returns>登録した時のId。登録済みの時は -1</returns>
        public static int Register(HistoryBehaviour behaviour)
        {
            // 登録ずみ
            if (objectList.Contains(behaviour))
            {
                return -1;
            }

            objectList.Add(behaviour);
            return objectList.Count - 1;
        }

        /// <summary>
        /// オブジェクトがあったら指定のデータの位置に
        /// </summary>
        /// <param name="data">設定する履歴データ</param>
        public static void LatestTransform(HistoryData data)
        {
            for (int i = 0; i < Count; i++)
            {
                if (objectList[i].SetHistoryData(data))
                {
                    return;
                }
            }
        }
    }
}