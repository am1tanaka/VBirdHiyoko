using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ブロックにアタッチするルートの情報
    /// </summary>
    public class BlockRouteData : MonoBehaviour
    {
        [Tooltip("押せるブロックの時、チェック"), SerializeField]
        bool isPushBlock = false;

        /// <summary>
        /// 押す方向フラグ
        /// </summary>
        [System.Flags]
        public enum CanPushFlag
        {
            None = 0,
            Forward = 1 << 0,
            Right = 1 << 1,
            Back = 1 << 2,
            Left = 1 << 3,
        }

        // Forward=-3, Left=-2, Back=-1, Right=0
        static readonly int[] dirPoint = { -3, 0, -1, -2 };

        /// <summary>
        /// フラグを設定したブロックのインスタンス。クリア時、このスタックのブロックをクリアする。
        /// </summary>
        static int StackMax => 100;
        public static Stack<BlockRouteData> checkedInstanceStack = new(StackMax);

        CanPushFlag canPush;

        /// <summary>
        /// 探索リストに登録済みフラグ
        /// </summary>
        public bool Checked { get; private set; }

        /// <summary>
        /// プレイヤーからの歩数
        /// </summary>
        public int StepCount { get; private set; }

        /// <summary>
        /// 接続先のブロック。Direction.Typeに対応する
        /// </summary>
        public BlockRouteData[] LinkedBlock { get; private set; } = new BlockRouteData[4];

        /// <summary>
        /// 指定の方向に押せるかを確認。押せるならtrue。必要に応じて調査を実行。
        /// </summary>
        /// <param name="dir">押せるかを確認する方向</param>
        /// <returns>押せる時、true</returns>
        public bool CanPush(Direction.Type dir)
        {
            if (!isPushBlock) return false;

            CheckPushDirectionAndNextToThePlayer();
            return canPush.HasFlag((CanPushFlag)(1 << (int)dir));
        }

        /// <summary>
        /// 歩けるとき、trueを返す
        /// </summary>
        public bool CanWalk => Checked && StepCount > 0;

        BoxCollider boxCollider;

        /// <summary>
        /// 上面座標
        /// </summary>
        public float TopY => boxCollider.bounds.max.y;

        /// <summary>
        /// 上面の中心座標
        /// </summary>
        public Vector3 CenterTop
        {
            get
            {
                centerTop = boxCollider.bounds.center;
                centerTop.y = TopY;
                return centerTop;
            }
        }
        Vector3 centerTop;

        /// <summary>
        /// ブロックを押そうとする座標。上面から該当するブロックの高さ0.5の座標。
        /// </summary>
        /// <value>ブロックのX,Z中心、かつ、Yは上のブロックが属する</value>
        Vector3 PushPosition
        {
            get
            {
                Vector3 top = boxCollider.bounds.center;
                top.y = Mathf.Floor(boxCollider.bounds.max.y - PiyoMover.StepHeight) + 0.5f;
                return top;
            }
        }

        /// <summary>
        /// プレイヤーの隣のブロックの時,true。必要に応じて調査を実行。
        /// </summary>
        public bool IsNextToThePlayer
        {
            get
            {
                CheckPushDirectionAndNextToThePlayer();
                return isNextToThePlayer;
            }
        }
        bool isNextToThePlayer;

        /// <summary>
        /// チェックする半径
        /// </summary>
        static float CheckRadius => 0.2f;
        static Collider[] results = new Collider[8];
        static RaycastHit[] raycastHits = new RaycastHit[8];

        /// <summary>
        /// 越えられる段差の高さ
        /// </summary>
        static float StepHeight => PiyoMover.StepHeight;

        /// <summary>
        /// 確認するレイヤー。プレイヤーとブロック
        /// </summary>
        static int checkLayer;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            checkLayer = LayerMask.GetMask("Player", "Block");
        }

        /// <summary>
        /// 指定の座標から押せるか。Yは無視
        /// </summary>
        /// <param name="fromPos">押す元の位置。基本的にはプレイヤー座標</param>
        /// <returns>押せる時、true</returns>
        public bool CanPushFrom(Vector3 fromPos)
        {
            if (!isPushBlock) return false;

            Vector3 to = boxCollider.bounds.center - fromPos;
            to.y = 0;
            var dir = Direction.DetectType(to);

            // 角度が不鮮明なら押せない
            if (dir == Direction.Type.Turning) return false;

            return CanPush(dir);
        }

        /// <summary>
        /// 隣の歩ける床のインスタンス。なければ null
        /// </summary>
        public BlockRouteData GetNearWalkFloorBlock()
        {
            VBirdHiyokoManager.Log($"GetNearWalkFloorBlock");
            if (!isPushBlock) { return null; }

            CheckPushDirectionAndNextToThePlayer();
            BlockRouteData floor = null;
            int point = int.MaxValue;
            for (int i = 0; i < LinkedBlock.Length; i++)
            {
                VBirdHiyokoManager.Log($"GetNearWalkFloorBlock LinkedBlock[{i}]={LinkedBlock[i]}");
                // 記録がなかったりチェックしていないブロックなら対象外
                if ((LinkedBlock[i] == null) || (!LinkedBlock[i].CanWalk)) continue;

                int t = LinkedBlock[i].StepCount * 8;
                if (CanPush((Direction.Type)i))
                {
                    t -= 4;
                }
                t += dirPoint[i];

                VBirdHiyokoManager.Log($"  near {i} t={t} step={LinkedBlock[i].StepCount} CanPush={CanPush((Direction.Type)i)} dir={dirPoint[i]} / point={point}");

                if (t < point)
                {
                    point = t;
                    floor = LinkedBlock[i];
                }
            }

            return floor;
        }

        /// <summary>
        /// 押せる方向とプレイヤーの隣かを調べてデータを反映。
        /// チェックしたらリストに登録して、Checkedを true にする
        /// </summary>
        /// <param name="step">段差</param>
        public void CheckPushDirectionAndNextToThePlayer()
        {
            // チェック済みなら何もしない
            if (Checked) return;

            Checked = true;
            checkedInstanceStack.Push(this);

            // 隣を確認
            CheckNextTo(Direction.Type.Forward);
            CheckNextTo(Direction.Type.Right);
            CheckNextTo(Direction.Type.Back);
            CheckNextTo(Direction.Type.Left);
        }

        /// <summary>
        /// 指定の方向について、横方向の接触、下方向の床の情報を調べる。
        /// /// </summary>
        /// <param name="dir">確認する方向</param>
        void CheckNextTo(Direction.Type dir)
        {
            Vector3 pushPos = PushPosition + Direction.Vector[(int)dir];

            VBirdHiyokoManager.Log($"CheckNextTo({dir}) pushPos={pushPos} isPushBlock={isPushBlock}");

            // 押せるブロックで、接触チェック
            if (isPushBlock)
            {
                if (CheckNextCollision(pushPos, dir) > 0)
                {
                    return;
                }
            }

            // 足元チェック
            Ray ray = new Ray(CenterTop + Direction.Vector[(int)dir], Vector3.down);
            int count = Physics.RaycastNonAlloc(
                ray, raycastHits, float.PositiveInfinity, checkLayer);

            VBirdHiyokoManager.Log($"ray={ray.origin} count={count}");

            // 何もなければ接続先無し
            if (count == 0) return;

            // 一番高いものを接続
            float top = float.NegativeInfinity;
            int index = -1;
            for (int i = 0; i < count; i++)
            {
                float y = raycastHits[i].collider.bounds.max.y;
                if (y > top)
                {
                    top = y;
                    index = i;
                }
            }

            LinkedBlock[(int)dir] = raycastHits[index].collider.GetComponent<BlockRouteData>();
            VBirdHiyokoManager.Log($"  Linked[{dir}] = {LinkedBlock[(int)dir]}");
        }

        /// <summary>
        /// 隣の接触するオブジェクトについて調べる。
        /// </summary>
        /// <param name="center">チェックする中心座標</param>
        /// <param name="dir">方向</param>
        /// <returns>接触したオブジェクトの数</returns>
        int CheckNextCollision(Vector3 center, Direction.Type dir)
        {
            int count = Physics.OverlapSphereNonAlloc(
                center, CheckRadius, results, checkLayer);

            // 接触するものがなければ押せる。
            if (count == 0)
            {
                canPush |= (CanPushFlag)(1 << (int)dir);
                return 0;
            }

            // 一番高いものとプレイヤーを探す
            float top = float.NegativeInfinity;
            int index = -1;
            for (int i = 0; i < count; i++)
            {
                // 隣がプレイヤー
                if (results[i].CompareTag("Player"))
                {
                    isNextToThePlayer = true;
                    continue;
                }

                float y = results[i].bounds.max.y;
                if (y > top)
                {
                    top = Mathf.Max(top, y);
                    index = i;
                }
            }

            if (index > -1)
            {
                LinkedBlock[(int)dir] = results[index].GetComponent<BlockRouteData>();
            }

            // 下面と隣の再上面の高さを比較
            float dy = top - boxCollider.bounds.min.y;
            if (dy <= StepHeight)
            {
                // 既定の高さ以下なら押せる
                canPush |= (CanPushFlag)(1 << (int)dir);
            }

            return count;
        }

    }
}