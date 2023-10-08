using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴を持っているゲームオブジェクトを座標を元に整列してデータを登録する。
    /// </summary>
    public static class HistoryObjectRegistrant
    {
        public static Bounds MapBounds { get; private set; }

        class OrderClass
        {
            /// <summary>
            /// 座標によるソートのための値
            /// </summary>
            public float score;

            /// <summary>
            /// 履歴インスタンス
            /// </summary>
            public HistoryBehaviour historyObject;

            public OrderClass(Vector3 pos, HistoryBehaviour obj, Vector3 mapMin, Vector3 mapMax)
            {
                score = pos.z * ((mapMax.x - mapMin.x + 1) * (mapMax.y - mapMin.y + 1))
                    + pos.y * (mapMax.x - mapMin.x + 1)
                    + pos.x;
                historyObject = obj;
            }
        }

        /// <summary>
        /// シーンに配置されたHistoryBehaviourを検索して、X,Y,Zの小さい順に並び替えて、その順でHistoryRecorderへ登録する。
        /// </summary>
        public static void RegisterObjects()
        {
            var objects = GameObject.FindObjectsOfType<HistoryBehaviour>();

            CalcMapBounds(objects);

            // 並び替えるためのクラス生成
            List<OrderClass> orderObjects = new(objects.Length);
            for (int i = 0; i < objects.Length; i++)
            {
                orderObjects.Add(new OrderClass(objects[i].transform.position, objects[i], MapBounds.min, MapBounds.max));
            }

            // 並び替え
            var sorted = orderObjects.OrderBy((data) => data.score);
            foreach (var obj in sorted)
            {
                obj.historyObject.Register();
            }

            sorted = null;
            orderObjects.Clear();
            orderObjects = null;
            objects = null;
        }

        /// <summary>
        /// マップの範囲を調査する。
        /// </summary>
        static void CalcMapBounds(HistoryBehaviour[] objects)
        {
            if (objects.Length == 0) return;

            // 1つ目を基準にする
            Vector3 min = objects[0].transform.position;
            Vector3 max = objects[0].transform.position;

            // 最大値と最小値を設定していく
            for (int i = 1; i < objects.Length; i++)
            {
                min = Vector3.Min(min, objects[i].transform.position);
                max = Vector3.Max(max, objects[i].transform.position);
            }

            MapBounds = new Bounds(0.5f * (min + max), max - min);
        }
    }
}