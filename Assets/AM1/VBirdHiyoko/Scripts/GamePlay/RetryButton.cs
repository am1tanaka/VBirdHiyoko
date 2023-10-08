using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;
using UnityEngine.EventSystems;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// リトライボタン
    /// </summary>
    public class RetryButton : MonoBehaviour, IPointerClickHandler
    {
        RetryCommand retryCommand = new RetryCommand();

        /// <summary>
        /// クリック動作
        /// </summary>
        public void OnClick()
        {
            CommandQueue.EntryCommand(retryCommand);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
    }
}