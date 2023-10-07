using AM1.BaseFrame;
using AM1.CommandSystem;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// マウスが指しているスクリーン座標を示すイメージ
    /// </summary>
    public class ScreenPointer : MonoBehaviour
    {
        RectTransform rectTransform;
        Animator animator;
        RectTransform canvasRectTransform;

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                rectTransform = GetComponent<RectTransform>();
                canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
                InputActionDetector.Instance.OnMovePoint.AddListener(OnMovePoint);
                CommandQueue.AddChangeListener(CommandInputType.Game, OnChangeActive);
                animator = GetComponentInChildren<Animator>();
                OnChangeActive(false);
            }
        }

        private void OnDestroy()
        {
            if (InputActionDetector.Instance != null)
            {
                InputActionDetector.Instance.OnMovePoint.AddListener(OnMovePoint);
                CommandQueue.RemoveChangeListener(CommandInputType.Game, OnChangeActive);
            }
        }

        void OnMovePoint(Vector2 pos)
        {
            rectTransform.anchoredPosition = pos / canvasRectTransform.localScale.x;
        }

        /// <summary>
        /// 操作の有無の切り替え
        /// </summary>
        /// <param name="flag">操作の有効無効</param>
        void OnChangeActive(bool flag)
        {
            animator.SetBool("Enabled", flag);
        }
    }
}