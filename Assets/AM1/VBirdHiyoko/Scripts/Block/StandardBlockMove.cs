using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class StandardBlockMove : MoveBlockBase, IMovableBlock, IBlockInfo
    {
        BlockStateAfterPushMove stateAfterPushMove;
        BlockStateFall stateFall;

        public override bool StartPush(Vector3 direction)
        {
            if (!CanPush(direction))
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
        /// プレイヤーから受け取ったベクトルを移動に反映する
        /// </summary>
        /// <param name="move">移動距離</param>
        public override void Push(Vector3 move)
        {
            PushMove(move);
        }

        /// <summary>
        /// 押し終えたらプレイヤーから呼び出す。
        /// ここからきりがよいところまで移動して落下まで自律して実行。
        /// </summary>
        public override void PushDone()
        {
            AdjustXZ();
            Enqueue(stateAfterPushMove);
        }

        /// <summary>
        /// 足元が氷かを確認する。
        /// </summary>
        public override bool CanContinue => IsOnIce();
    }
}