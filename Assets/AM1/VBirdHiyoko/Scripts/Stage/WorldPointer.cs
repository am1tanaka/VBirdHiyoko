using AM1.BaseFrame;
using AM1.CommandSystem;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// マウスが指しているワールド座標の位置を示すオブジェクト
    /// @url https://forpro.unity3d.jp/unity_pro_tips/2021/05/20/1957/
    /// </summary>
    public class WorldPointer : MonoBehaviour
    {
        Animator animator;
        Transform pivotTransform;

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                animator = GetComponent<Animator>();
                pivotTransform = transform.Find("Pivot");
                pivotTransform.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (SceneStateChanger.IsReady)
            {
                InputActionDetector.Instance.OnWorldPoint.AddListener(OnWorldPoint);
                InputActionDetector.Instance.OnWorldPointExit.AddListener(OnWorldPointExit);
                CommandQueue.AddChangeListener(CommandInputType.Game, OnChangeEnabled);
            }
        }

        private void OnDestroy()
        {
            if (InputActionDetector.Instance != null)
            {
                InputActionDetector.Instance.OnWorldPoint.RemoveListener(OnWorldPoint);
                InputActionDetector.Instance.OnWorldPointExit.RemoveListener(OnWorldPointExit);
                CommandQueue.RemoveChangeListener(CommandInputType.Game, OnChangeEnabled);
            }
        }

        void OnWorldPoint(RaycastHit hit)
        {
            transform.position = hit.point;
            pivotTransform.gameObject.SetActive(true);
        }

        void OnWorldPointExit()
        {
            pivotTransform.gameObject.SetActive(false);
        }

        void OnChangeEnabled(bool flag)
        {
            animator.SetBool("Enabled", flag);
        }
    }
}