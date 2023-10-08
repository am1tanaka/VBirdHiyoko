using AM1.CommandSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;
using UnityEngine.EventSystems;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// Redoボタン
    /// </summary>
    public class RedoButton : MonoBehaviour, IPointerClickHandler, ICommandQueueData
    {
        /// <summary>
        /// Undo操作はプレイヤー操作の一環
        /// </summary>
        public CommandInputType Type => CommandInputType.Game;

        Button3DAnimator button3DAnimator;

        void Start()
        {
            if (!SceneStateChanger.IsReady) { return; }

            button3DAnimator = GetComponent<Button3DAnimator>();
            button3DAnimator.SetEnabled(false);
            CommandQueue.AddChangeListener(Type, ChangingMask);
            HistoryPlayer.ClickedHistoryButton.AddListener(ClickedHistoryButton);
        }

        void OnDestroy()
        {
            if (!SceneStateChanger.IsReady) { return; }

            CommandQueue.RemoveChangeListener(Type, ChangingMask);
            HistoryPlayer.ClickedHistoryButton.RemoveListener(ClickedHistoryButton);
        }

        public void Invoke()
        {
            HistoryPlayer.Redo();
        }

        /// <summary>
        /// クリック動作
        /// </summary>
        /// <returns>登録できたとき、true</returns>
        public bool OnClick()
        {
            return CommandQueue.EntryCommand(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

        /// <summary>
        /// 操作マスクの変更時に呼び出してもらう。
        /// </summary>
        /// <param name="flag">操作可能になる時、true</param>
        void ChangingMask(bool flag)
        {
            button3DAnimator.SetEnabled(flag && HistoryPlayer.CanRedo());
        }

        /// <summary>
        /// 履歴ボタンが押された時に呼び出してもらう。
        /// </summary>
        void ClickedHistoryButton()
        {
            ChangingMask(true);
        }
    }
}