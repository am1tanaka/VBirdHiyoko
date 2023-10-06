using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 歩行コースを作成するクラス
    /// </summary>
    public static class WalkCourse
    {
        /// <summary>
        /// リストの初期最大数
        /// </summary>
        static float ListMax => 256;

        /// <summary>
        /// プレイヤーの座標から歩く方向を並べたスタック
        /// </summary>
        public static readonly Stack<Direction.Type> walkForwards = new Stack<Direction.Type>((int)ListMax);

        /// <summary>
        /// 受け取ったブロックへ歩くルートを作成して、wayPointsに設定する。
        /// </summary>
        /// <param name="blockRouteData">目的地のブロック</param>
        public static void MakeCourse(BlockRouteData blockRouteData)
        {
            walkForwards.Clear();
            if (blockRouteData == null) return;

            VBirdHiyokoManager.Log($"MakeCourse({blockRouteData.transform.position}) stepCount={blockRouteData.StepCount}");

            var checkBlock = blockRouteData;
            int i = 0;
            while ((checkBlock.StepCount > 0) && (i++ < 100))
            {
                VBirdHiyokoManager.Log($"MakeCourse check ({i}) {checkBlock.transform.position} stepcount={checkBlock.StepCount}");
                // 逆順に追加
                var nearDirection = GetNearDirection(checkBlock);

#if UNITY_EDITOR
                if (nearDirection == Direction.Type.None)
                {
                    Debug.LogError("歩行コースの作成に失敗");
                    return;
                }
#endif

                VBirdHiyokoManager.Log($"  nearDir={nearDirection}");
                walkForwards.Push(Direction.Reverse(nearDirection));
                checkBlock = checkBlock.LinkedBlock[(int)nearDirection];

                VBirdHiyokoManager.Log($"  pos={checkBlock.transform.position} nextStep={checkBlock.StepCount}");
            }
        }

        /// <summary>
        /// 指定のブロックに隣接するブロックから
        /// </summary>
        /// <param name="block">対象のブロック</param>
        /// <returns>近いブロックへの方向。この逆方向をリストへ積む</returns>
        static Direction.Type GetNearDirection(BlockRouteData block)
        {
#if DEBUG_LOG
            GameManager.Log($"  block {block.transform.position} StepCount={block.StepCount}");
            for (int i=0;i<4;i++) {
                if (block.LinkedBlock[i]) {
                    Debug.Log($"  [{i}] {block.LinkedBlock[i].StepCount}");
                }
            }
#endif

            for (int i = 0; i < 4; i++)
            {
                // 指定のブロックより歩数が少ない方向を見つけたらその方向で確定
                if (block.LinkedBlock[i] && (block.LinkedBlock[i].StepCount < block.StepCount))
                {
                    return (Direction.Type)i;
                }
            }

            return Direction.Type.None;
        }
    }
}