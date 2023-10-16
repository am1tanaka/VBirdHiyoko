using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class StandardBlockMove : MoveBlockBase, IMovableBlock, IBlockInfo
    {
        public override bool StartPush(Vector3 direction)
        {
            if (!TryPush(direction))
            {
                return false;
            }

            // 押し終わったあとの自律動作用の状態
            if (stateAfterPushMove == null)
            {
                stateFall = new BlockStateFall(this);
                stateAfterPushMove = new BlockStateAfterPushMove(this, stateFall);
            }

            HistoryStartMove();
            return true;
        }

        /// <summary>
        /// 足元が氷かを確認する。
        /// </summary>
        public override bool CanContinue => IsOnIce();
    }
}