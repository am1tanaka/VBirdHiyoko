using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ブロックが押され終わった後、切りがよいところまで移動させる状態
    /// </summary>
    public class BlockStateAfterPushMove : AM1StateQueueBase
    {
        MoveBlockBase moveBlock;
        Vector3 targetPosition;
        AM1StateQueueBase fallState;
        bool isMoveDone;

        public BlockStateAfterPushMove(MoveBlockBase block, AM1StateQueueBase fallState) : base()
        {
            this.moveBlock = block;
            this.fallState = fallState;
        }

        /// <summary>
        /// 移動開始
        /// </summary>
        public override void Init()
        {
            base.Init();
            targetPosition = moveBlock.BoxColliderInstance.bounds.center;
            targetPosition.x = Mathf.Round(targetPosition.x);
            targetPosition.z = Mathf.Round(targetPosition.z);
            isMoveDone = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isMoveDone) { return; }

            float step = Time.fixedDeltaTime * moveBlock.PushVelocity.magnitude;
            if (CheckMoveContinue(step))
            {
                // 移動継続
                moveBlock.rb.MovePosition(moveBlock.rb.position + step * Direction.Vector[(int)moveBlock.MoveDirection]);
            }
            else
            {
                // 到着
                targetPosition.y = moveBlock.rb.position.y;
                moveBlock.rb.MovePosition(targetPosition);
                moveBlock.Enqueue(fallState);
                isMoveDone = true;
            }
        }

        /// <summary>
        /// 滑りを継続するか確認
        /// </summary>
        /// <param name="step">速度の更新1回分の移動量</param>
        /// <returns>移動する時、true</returns>
        bool CheckMoveContinue(float step)
        {
            // 移動量を確認
            Vector3 dpos = targetPosition - moveBlock.BoxColliderInstance.bounds.center;
            dpos.y = 0;

            // 到着していなければ移動継続
            if (dpos.magnitude >= step)
            {
                VBirdHiyokoManager.Log($"  移動継続 {dpos.magnitude} >= {step} target={targetPosition} currentpos={moveBlock.BoxColliderInstance.bounds.center}");
                return true;
            }

            // 到着範囲。滑らないなら到着
            if (!moveBlock.CanContinue)
            {
                VBirdHiyokoManager.Log($"  滑らない");
                return false;
            }
            VBirdHiyokoManager.Log($"  滑る");

            // 下を見る
            int count = MoveBlockBase.OverlapSphereBlockAndWall(targetPosition + Vector3.down);
            if (count == 0)
            {
                // 下に何もなければ落下のために移動終了
                return false;
            }

            // 滑った先の衝突確認
            Vector3 moveDir = Direction.Vector[(int)moveBlock.MoveDirection];
            count = MoveBlockBase.OverlapSphereBlockAndWall(targetPosition + moveDir);
            VBirdHiyokoManager.Log($"  滑った先 {targetPosition}+{moveDir} count={count} moveDir={moveDir}");
            if (count > 0)
            {
                float dy = MoveBlockBase.Results[0].bounds.max.y - moveBlock.BoxColliderInstance.bounds.min.y;
                VBirdHiyokoManager.Log($"  dy={dy} StepHeight={MoveBlockBase.StepHeight}");
                if (dy > MoveBlockBase.StepHeight)
                {
                    VBirdHiyokoManager.Log($"  衝突 {count} {MoveBlockBase.Results[0].name} 段差={dy}");

                    // 衝突したので、衝突音をさせて停止
                    moveBlock.PlayCollisionSE();
                    return false;
                }
            }

            // ぶつからないので移動継続
            targetPosition += moveDir;
            VBirdHiyokoManager.Log($"  継続 次のtargetPosition={targetPosition}");
            return true;
        }
    }
}