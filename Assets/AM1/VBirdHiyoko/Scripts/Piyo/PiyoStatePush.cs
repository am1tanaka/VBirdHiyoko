using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 押す状態
    /// </summary>
    public class PiyoStatePush : AM1StateQueueBase
    {
        /// <summary>
        /// 押せるなら押す処理へ移行
        /// </summary>
        /// <param name="target">押そうとしているブロックデータ</param>
        /// <param name="dir">押す方向</param>
        /// <returns>押せる時、true。押せないなら false</returns>
        public bool SetPush(BlockRouteData target, Direction.Type dir)
        {
            Debug.Log("未実装");
            return false;
        }
    }
}