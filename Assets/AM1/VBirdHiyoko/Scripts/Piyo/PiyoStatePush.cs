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
        /// 目的のブロックのインスタンス
        /// </summary>
        IMovableBlock targetBlock;

        /// <summary>
        /// 押す方向
        /// </summary>
        Direction.Type pushDir;

        /// <summary>
        /// 押せるなら押す処理へ移行
        /// </summary>
        /// <param name="target">押そうとしているブロックデータ</param>
        /// <param name="dir">押す方向</param>
        /// <returns>押せる時、true。押せないなら false</returns>
        public bool SetPush(BlockRouteData target, Direction.Type dir)
        {
            if ((PiyoBehaviour.Instance.Mover.FootBlock == null)
                || (target == null)
                || !target.CanPush(dir))
            {
                targetBlock = null;
                return false;
            }

            targetBlock = target.GetComponent<IMovableBlock>();
            if (targetBlock == null)
            {
                return false;
            }

            pushDir = dir;
            PiyoBehaviour.Instance.Enqueue(this);
            PiyoBehaviour.Instance.HidePushArrows();
            return true;
        }

        public override void Init()
        {
            base.Init();
            PiyoBehaviour.Instance.StartCoroutine(Push());
        }

        /// <summary>
        /// 押す。
        /// </summary>
        IEnumerator Push()
        {
            // 向きを変える
            yield return PiyoBehaviour.Instance.Mover.TurnTo(pushDir);

            // 押す
            yield return PiyoBehaviour.Instance.Mover.PushTo(
                Direction.Vector[(int)pushDir],
                targetBlock);

            // 歩数更新
            PiyoBehaviour.Instance.StepCounterInstance.IncrementAll();

            // イベントがあったら実行
            PiyoBehaviour.Instance.afterScenarioState = PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>();
            PiyoBehaviour.Instance.InvokeAddedScenarioState();
        }
    }
}