using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 3Dボタンのアニメーション
    /// </summary>
    public class Button3DAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        /// <summary>
        /// 有効、無効を切り替える時に呼び出す処理を登録。
        /// </summary>
        static UnityEvent<bool> EnabledChanged { get; set; } = new UnityEvent<bool>();

        Animator _animator;
        Animator animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }
                return _animator;
            }
        }

        /// <summary>
        /// 全ての3Dボタンの有効、無効を設定する。
        /// </summary>
        /// <param name="flag">有効にするなら、true</param>
        public static void SetEnabledAll(bool flag)
        {
            EnabledChanged.Invoke(flag);
        }

        private void Start()
        {
            // 無効で開始
            SetEnabled(false);

            EnabledChanged.AddListener(SetEnabled);
        }

        void OnDestroy()
        {
            EnabledChanged.RemoveListener(SetEnabled);
        }

        /// <summary>
        /// 有効、無効を設定する
        /// </summary>
        /// <param name="enabled">有効にする時、true</param>
        public void SetEnabled(bool enabled)
        {
            animator.SetBool("Enabled", enabled);
            if (!enabled)
            {
                animator.ResetTrigger("Push");
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (animator.GetBool("Enabled"))
            {
                animator.SetTrigger("Push");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            animator.SetBool("Active", true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            animator.SetBool("Active", false);
        }
    }
}