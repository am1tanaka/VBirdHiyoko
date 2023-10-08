using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// タイトルへボタンを押した時の処理
    /// </summary>
    public class ToTitleCommand : ICommandQueueData
    {
        public CommandInputType Type => CommandInputType.UI;

        public int Priority => 10;

        ConfirmDialogData toTitleConfirmData;

        /// <summary>
        /// コマンドを実行する
        /// </summary>
        public void Invoke()
        {
            if (toTitleConfirmData == null)
            {
                toTitleConfirmData = new ConfirmDialogData(
                    Messages.GetMessage(Messages.Type.ToTitleConfirm),
                    Messages.GetMessage(Messages.Type.ToTitleButton),
                    Messages.GetMessage(Messages.Type.NoButton),
                    ToTitle,
                    Cancel);
            }

            SEPlayer.Play(SEPlayer.SE.Click);
            ConfirmDialog.Instance.Show(toTitleConfirmData);
        }

        void ToTitle()
        {
            if (TitleSceneStateChanger.Instance.RequestFrom(TitleSceneStateChanger.FromState.Game, false))
            {
                SEPlayer.Play(SEPlayer.SE.Click);
                HistoryPlayer.Accept();
                HistoryRecorder.Save(VBirdHiyokoManager.CurrentStage.Current,
                    PiyoBehaviour.Instance.StepCounterInstance.CurrentInnerStep);
            }
            else
            {
                SEPlayer.Play(SEPlayer.SE.Cancel);
            }
            ConfirmDialog.Instance.Hide();
        }

        void Cancel()
        {
            SEPlayer.Play(SEPlayer.SE.Cancel);
            ConfirmDialog.Instance.Hide();
        }
    }
}