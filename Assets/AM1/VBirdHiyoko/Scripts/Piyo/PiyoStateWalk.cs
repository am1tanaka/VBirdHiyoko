using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 目的座標へ歩く
    /// </summary>
    public class PiyoStateWalk : AM1StateQueueBase
    {
        /// <summary>
        /// 目的のブロックのインスタンス
        /// </summary>
        BlockRouteData targetBlockData;

        /// <summary>
        /// イベントで中断したら true
        /// </summary>
        bool isPaused;

        /// <summary>
        /// 歩けるなら歩きへ移行
        /// </summary>
        /// <param name="target">目的のブロックデータ</param>
        /// <returns>歩きへ移行する時、true。以降しないなら false</returns>
        public bool SetWalk(BlockRouteData target)
        {
            if ((PiyoBehaviour.Instance.Mover.FootBlock == null)
                || (target == null)
                || (!target.CanWalk))
            {
                targetBlockData = null;
                return false;
            }

            targetBlockData = target;
            PiyoBehaviour.Instance.Enqueue(this);
            PiyoBehaviour.Instance.ShowTargetFlag(target.CenterTop);
            return true;
        }

        public override void Init()
        {
            VBirdHiyokoManager.Log($"State Walk Init {targetBlockData.transform.position}");
            base.Init();
            if (!isPaused)
            {
                WalkCourse.MakeCourse(targetBlockData);
            }
            isPaused = false;
            PiyoBehaviour.Instance.StartCoroutine(Walk());
        }

        /// <summary>
        /// 移動方向を取り出して歩きを指示
        /// </summary>
        IEnumerator Walk()
        {
            VBirdHiyokoManager.Log($"Walk Step={WalkCourse.walkForwards.Count}");
            while (WalkCourse.walkForwards.TryPop(out Direction.Type dir))
            {
                yield return PiyoBehaviour.Instance.Mover.WalkTo(Direction.Vector[(int)dir]);

                // 歩数更新
                PiyoBehaviour.Instance.StepCounterInstance.IncrementAll();

                // シナリオがあれば中断して実行
                if (PiyoBehaviour.Instance.IsAddedScenario)
                {
                    break;
                }
            }

            // 目的地アイコンを非表示にする
            PiyoBehaviour.Instance.HideTargetFlag();

            // シナリオが登録された
            if (PiyoBehaviour.Instance.IsAddedScenario)
            {
                if (WalkCourse.walkForwards.Count > 0)
                {
                    // 歩き続ける
                    isPaused = true;
                    PiyoBehaviour.Instance.afterScenarioState = PiyoBehaviour.Instance.GetInstance<PiyoStateWalk>();
                }
                else
                {
                    // ここで終わるので次は入力待ち
                    PiyoBehaviour.Instance.afterScenarioState = PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>();
                }
                PiyoBehaviour.Instance.InvokeAddedScenarioState();
            }
            else
            {
                // 到着
                VBirdHiyokoManager.Log($"到着");
                PiyoBehaviour.Instance.EnqueueState<PiyoStateWaitInput>();
            }
        }
    }
}