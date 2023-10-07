using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 押せる方向矢印を管理するクラス
    /// </summary>
    public class PushArrows : MonoBehaviour
    {
        [Tooltip("矢印オブジェクトのアニメーターインスタンス"), SerializeField]
        Animator[] arrows = new Animator[4];

        static int blockLayer;
        static Collider[] results = new Collider[4];
        static float CheckRadius => 0.2f;

        private void Awake()
        {
            blockLayer = LayerMask.GetMask("Block");
        }

        /// <summary>
        /// 受け取ったBoxColliderの周囲のブロックを探索して、必要に応じて矢印を表示する
        /// </summary>
        /// <param name="boxCollider">プレイヤーのBoxColliderのインスタンス</param>
        public void Show(BoxCollider boxCollider)
        {
            transform.position = boxCollider.transform.position;
            CheckAndShow(boxCollider, Direction.Type.Forward);
            CheckAndShow(boxCollider, Direction.Type.Right);
            CheckAndShow(boxCollider, Direction.Type.Back);
            CheckAndShow(boxCollider, Direction.Type.Left);
        }

        /// <summary>
        /// 指定方向に押せるブロックがあるかを調べて、あれば矢印を表示する
        /// </summary>
        /// <param name="boxCollider">プレイヤーの当たり判定</param>
        /// <param name="dir">チェックする方向</param>
        void CheckAndShow(BoxCollider boxCollider, Direction.Type dir)
        {
            Vector3 center = boxCollider.bounds.center + Direction.Vector[(int)dir];
            int count = Physics.OverlapSphereNonAlloc(
                center, CheckRadius, results, blockLayer);
            for (int i = 0; i < count; i++)
            {
                BlockRouteData block = results[i].GetComponent<BlockRouteData>();
                if ((block != null) && block.CanPush(dir))
                {
                    arrows[(int)dir].SetBool("Show", true);
                    arrows[(int)dir].transform.position = block.CenterTop;
                    break;
                }
            }
        }

        /// <summary>
        /// 矢印を非表示にする
        /// </summary>
        public void Hide()
        {
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].SetBool("Show", false);
            }
        }
    }
}