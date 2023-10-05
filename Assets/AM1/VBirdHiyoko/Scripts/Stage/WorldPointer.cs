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

        private void OnDestroy()
        {
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