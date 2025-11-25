using AM1.BaseFrame;
using AM1.CommandSystem;
using UnityEngine;
using UnityEngine.UI;

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
        Image image;

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                rectTransform = GetComponent<RectTransform>();
                canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
                InputActionDetector.Instance.OnMovePoint.AddListener(OnMovePoint);
                CommandQueue.AddChangeListener(CommandInputType.Game, OnChangeActive);
                animator = GetComponentInChildren<Animator>();
                image = GetComponent<Image>();
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
            if (InputActionDetector.Instance.IsPointer) {
                image.enabled = false;
            }
            else{
                image.enabled = true;
                rectTransform.anchoredPosition = pos / canvasRectTransform.localScale.x;
            }
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