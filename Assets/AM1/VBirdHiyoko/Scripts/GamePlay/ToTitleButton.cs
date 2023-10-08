using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;
using UnityEngine.EventSystems;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// タイトルヘボタン
    /// </summary>
    public class ToTitleButton : MonoBehaviour, IPointerClickHandler
    {
        ToTitleCommand toTitleCommand = new ToTitleCommand();

        /// <summary>
        /// クリック動作
        /// </summary>
        public void OnClick()
        {
            CommandQueue.EntryCommand(toTitleCommand);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
    }
}