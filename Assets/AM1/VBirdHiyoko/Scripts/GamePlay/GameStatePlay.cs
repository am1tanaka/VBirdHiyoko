// #define DEBUG_KEY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ゲームのプレイ状態
    /// </summary>
    public class GameStatePlay : AM1StateQueueBase
    {
        public override void Init()
        {
            base.Init();
            PiyoBehaviour.Instance.EnqueueState<PiyoStateWaitInput>();
        }

        public override void Update()
        {
#if DEBUG_KEY
            if (Keyboard.current.cKey.isPressed)
            {
                GamePlayStateQueue.Instance.Enqueue(GamePlayStateQueue.Instance.GameStateClearInstance);
            }
#endif
        }

    }
}