using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// リトライを実行するコマンド
    /// </summary>

    public class RetryCommand : ICommandQueueData
    {
        public CommandInputType Type => CommandInputType.UI;

        public int Priority => 10;

        ConfirmDialogData retryConfirmData;

        /// <summary>
        /// コマンドを実行する
        /// </summary>
        public void Invoke()
        {
            if (retryConfirmData == null)
            {
                retryConfirmData = new ConfirmDialogData(
                    Messages.GetMessage(Messages.Type.RetryConfirm),
                    Messages.GetMessage(Messages.Type.RetryButton),
                    Messages.GetMessage(Messages.Type.NoButton),
                    Retry,
                    Cancel);
            }

            SEPlayer.Play(SEPlayer.SE.Click);
            ConfirmDialog.Instance.Show(retryConfirmData);
        }

        void Retry()
        {
            SEPlayer.Play(SEPlayer.SE.Click);
            ConfirmDialog.Instance.Hide();
            GameSceneStateChanger.Instance.Request();
        }

        void Cancel()
        {
            SEPlayer.Play(SEPlayer.SE.Cancel);
            ConfirmDialog.Instance.Hide();
        }
    }
}