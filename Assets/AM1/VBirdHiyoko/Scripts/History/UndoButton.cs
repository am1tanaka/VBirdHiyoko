using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// Undoボタン
    /// </summary>
    public class UndoButton : MonoBehaviour, ICommandQueueData
    {
        public void Invoke()
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// クリック動作
        /// </summary>
        /// <returns>登録できたとき、true</returns>
        public bool OnClick()
        {
            return CommandQueue.EntryCommand(this);
        }
    }
}