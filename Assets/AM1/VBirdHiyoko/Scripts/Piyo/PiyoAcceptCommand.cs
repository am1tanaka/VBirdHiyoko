using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴の確定コマンド
    /// </summary>
    public class PiyoAcceptCommand : ICommandQueueData
    {
        public CommandInputType Type => CommandInputType.Game;
        public int Priority => 10;

        public void Invoke()
        {
            PiyoBehaviour.Instance.StartCoroutine(AcceptWork());
        }

        IEnumerator AcceptWork()
        {
            // 履歴を確定
            HistoryPlayer.Accept();

            // 落下して足場を確保
            yield return PiyoBehaviour.Instance.Mover.Fall();

            // ルートを更新
            PiyoBehaviour.Instance.UpdateRoute();

            // アクションを実行
            var playerActionCommand = PiyoBehaviour.Instance.GetInstance<PiyoActionCommand>();
            playerActionCommand.Invoke();
        }
    }
}