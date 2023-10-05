using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// イベントを登録できるようにして、アニメから呼び出させる。
    /// </summary>
    public class AnimEvent : MonoBehaviour
    {
        /// <summary>
        /// アニメが完了した時に実行する処理
        /// </summary>
        public readonly UnityEvent onEvent = new UnityEvent();

        /// <summary>
        /// アニメからイベント登録
        /// </summary>
        public void CallEvent()
        {
            onEvent.Invoke();
        }

        /// <summary>
        /// イベントが呼ばれるのを待つ
        /// </summary>
        public IEnumerator WaitEvent()
        {
            bool isCalled = false;

            onEvent.AddListener(() => isCalled = true);
            while (!isCalled)
            {
                yield return null;
            }
            onEvent.RemoveAllListeners();
        }
    }
}