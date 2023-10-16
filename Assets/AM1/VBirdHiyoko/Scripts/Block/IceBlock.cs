using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 氷ブロックの制御クラス
    /// </summary>
    public class IceBlock : MoveBlockBase, IMovableBlock, IBlockInfo
    {
        /// <summary>
        /// 氷は常に滑る
        /// </summary>
        public override bool CanContinue => true;

        public bool IsSlippery => true;

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
    }
}