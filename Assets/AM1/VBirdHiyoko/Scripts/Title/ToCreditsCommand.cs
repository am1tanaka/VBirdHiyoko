using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// クレジット表示への遷移コマンド
    /// </summary>
    public class ToCreditsCommand : ICommandQueueData
    {
        public int Priority => 10;
        public CommandInputType Type => CommandInputType.UI;
        public void Invoke()
        {
            SEPlayer.Play(SEPlayer.SE.Click);
            TitleBehaviour.Instance.ChangeState(TitleBehaviour.State.Credits);
        }
    }
}