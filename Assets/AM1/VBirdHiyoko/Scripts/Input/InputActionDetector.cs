using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;
using UnityEngine.Events;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// PlayerActionsから情報を取得して、ゲーム内の指示に変換して呼び出す。
    /// </summary>
    public class InputActionDetector : MonoBehaviour
    {
        public static InputActionDetector Instance { get; private set; }

        /// <summary>
        /// クリックしたゲームオブジェクトを渡す。
        /// カーソルがUI上の時は呼び出さない。
        /// </summary>
        /// <typeparam name="RaycastHit">対象の接触情報</typeparam>
        public UnityEvent<RaycastHit> OnAction { get; private set; } = new UnityEvent<RaycastHit>();

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// ポインターの情報を更新する。
        /// </summary>
        public void UpdatePointer()
        {
            Debug.Log("未実装");
        }
    }
}