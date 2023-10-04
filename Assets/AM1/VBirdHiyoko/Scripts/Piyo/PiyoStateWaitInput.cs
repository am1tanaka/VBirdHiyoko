using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 操作待ち状態
    /// </summary>
    public class PiyoStateWaitInput : AM1StateQueueBase
    {
        /// <summary>
        /// 入力待ちになったら true
        /// </summary>
        public bool IsWaitInput =>
            PiyoBehaviour.Instance.StateIs<PiyoStateWaitInput>()
            && CommandQueue.CurrentInputMask.HasFlag(CommandInputType.Game)
            && !CommandQueue.IsSetNextCommand;
    }
}