using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class PiyoStateHistory : AM1StateQueueBase
    {
        /// <summary>
        /// クリックを受け取る
        /// </summary>
        /// <param name="hit">クリックしたオブジェクトの当たり判定</param>
        public void OnAction(RaycastHit hit)
        {
            OnAction(hit.collider.GetComponent<BlockRouteData>());
        }

        /// <summary>
        /// 指定のブロックのクリックを受け取る。
        /// </summary>
        /// <param name="blockRouteData">クリックしたオブジェクト</param>
        public void OnAction(BlockRouteData blockRouteData)
        {
            Debug.Log("未実装");
        }
    }
}