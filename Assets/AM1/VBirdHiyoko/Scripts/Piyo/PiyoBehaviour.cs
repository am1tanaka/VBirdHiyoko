using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    public class PiyoBehaviour : AM1StateQueue
    {
        public static PiyoBehaviour Instance { get; private set; }

        void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// 表示、および物理座標を設定する。
        /// </summary>
        /// <param name="pos">設定する座標</param>
        public void SetPosition(Vector3 pos)
        {
            Debug.Log($"未実装");
        }

        /// <summary>
        /// 行動中に発動したイベントを受け取るキュー
        /// </summary>
        /// <param name="scenario">プレイヤーの状態として使えるもの</param>
        public void AddScenario(AM1StateQueueBase scenario)
        {
            Debug.Log("未実装");
        }
    }
}