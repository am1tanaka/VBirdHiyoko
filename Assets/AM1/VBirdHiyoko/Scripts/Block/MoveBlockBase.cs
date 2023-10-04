using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 移動ブロックの基底クラス
    /// </summary>
    public abstract class MoveBlockBase : AM1StateQueue, IMovableBlock, IBlockInfo
    {
        public static int BlockLayer { get; private set; }

        public void PlayCollisionSE()
        {
            Debug.Log("未実装");
        }

        public void PlayPushSE()
        {
            Debug.Log("未実装");
        }

        public void Push(Vector3 move)
        {
            Debug.Log("未実装");
        }

        public void PushDone()
        {
            Debug.Log("未実装");
        }

        public bool StartPush(Vector3 direction)
        {
            Debug.Log("未実装");
            return false;
        }
    }
}