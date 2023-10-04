using AM1.CommandSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// Undoボタン
    /// </summary>
    public class RedoButton : MonoBehaviour, ICommandQueueData
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
            Debug.Log("未実装");
            return false;
        }
    }
}