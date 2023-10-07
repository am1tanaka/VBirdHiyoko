using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 目的地の旗を制御するクラス
    /// </summary>
    public class TargetFlag : MonoBehaviour
    {
        Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// 指定のワールド座標に旗を表示
        /// </summary>
        /// <param name="pos">移動先の座標</param>
        public void Show(Vector3 pos)
        {
            transform.position = pos;
            animator.SetBool("Show", true);
        }

        /// <summary>
        /// フラグを下す
        /// </summary>
        public void Hide()
        {
            animator.SetBool("Show", false);
        }
    }
}