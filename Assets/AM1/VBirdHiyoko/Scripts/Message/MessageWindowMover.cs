using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// メッセージウィンドウを移動させるクラス
    /// </summary>
    public class MessageWindowMover : MonoBehaviour
    {
        [Tooltip("ゲームで利用しているカメラ"), SerializeField]
        Camera gameCamera = default;

        Transform targetTransform;
        /// <summary>
        /// 表示位置の影響を考慮する相手のオブジェクトの Transform。
        /// 通常は Player の transform を設定する。
        /// </summary>
        Transform TargetTransform
        {
            get
            {
                if (targetTransform == null)
                {
                    if (PiyoBehaviour.Instance != null)
                    {
                        targetTransform = PiyoBehaviour.Instance.transform;
                    }
                    else
                    {
                        targetTransform = null;
                    }
                }
                return targetTransform;
            }
        }

        /// <summary>
        /// 画面端からのマージン
        /// </summary>
        static float Margin = 20;

        RectTransform rectTransform;
        Animator topBottomAnimator;
        float startY;

        private void Awake()
        {
            rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
            topBottomAnimator = GetComponent<Animator>();
            startY = rectTransform.position.y + rectTransform.sizeDelta.y * 0.5f;
        }

        private void LateUpdate()
        {
            var target = TargetTransform;
            if (target == null) return;

            var targetScreenPos = gameCamera.WorldToScreenPoint(target.position);
            bool current = topBottomAnimator.GetBool("Top");
            bool isTop = targetScreenPos.y < startY + Margin;
            if (current != isTop)
            {
                topBottomAnimator.SetBool("Top", isTop);
            }
        }

    }
}