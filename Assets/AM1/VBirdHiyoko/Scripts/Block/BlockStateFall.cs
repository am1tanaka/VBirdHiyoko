using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ブロックを落下させる。
    /// </summary>
    public class BlockStateFall : AM1StateQueueBase
    {
        /// <summary>
        /// これ以上の高さ落下しないと効果音は鳴らさない
        /// </summary>
        static float SEHeight => 0.1f;

        static RaycastHit[] results = new RaycastHit[4];
        static int blockAndBaseLayer;

        MoveBlockBase moveBlock;
        Vector3 velocity;
        float fallHeight;

        public BlockStateFall(MoveBlockBase block) : base()
        {
            moveBlock = block;
        }

        /// <summary>
        /// 移動開始
        /// </summary>
        public override void Init()
        {
            base.Init();
            if (blockAndBaseLayer == 0)
            {
                blockAndBaseLayer = LayerMask.GetMask("Block", "StageBase");
            }
            velocity = Vector3.zero;
            fallHeight = 0;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
            float fallDistance = Mathf.Abs(Time.fixedDeltaTime * velocity.y);

            int count = Physics.BoxCastNonAlloc(
                moveBlock.BoxColliderInstance.bounds.center,
                moveBlock.BoxColliderInstance.bounds.extents,
                Vector3.down,
                results,
                Quaternion.identity,
                fallDistance,
                blockAndBaseLayer);

            bool isGrounded = false;
            for (int i = 0; i < count; i++)
            {
                if ((results[i].collider != moveBlock.BoxColliderInstance) && (results[i].distance < fallDistance))
                {
                    fallDistance = results[i].distance;
                    isGrounded = true;
                }
            }

            // 落下
            moveBlock.rb.MovePosition(moveBlock.rb.position + fallDistance * Vector3.down);
            fallHeight += fallDistance;

            if (isGrounded)
            {
                // ここで停止
                velocity = Vector3.zero;
                moveBlock.RequestTerminateCurrentState();
                if (fallHeight > SEHeight)
                {
                    moveBlock.PlayCollisionSE();
                }
            }
        }

        public override void Terminate()
        {
            base.Terminate();
            BlockMoveObserver.Remove(moveBlock);
        }
    }
}