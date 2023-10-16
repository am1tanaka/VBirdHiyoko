using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 移動ブロックの基底クラス
    /// </summary>
    public abstract class MoveBlockBase : AM1StateQueue, IMovableBlock
    {
        /// <summary>
        /// 接触判定の半径
        /// </summary>
        static float CheckRadius => 0.1f;

        [Tooltip("押された時の音"), SerializeField]
        AudioClip pushSE = default;
        [Tooltip("接触音"), SerializeField]
        AudioClip collisionSE = default;

        public Rigidbody rb { get; private set; }
        public BoxCollider BoxColliderInstance { get; private set; }

        public static int BlockLayer { get; private set; }

        /// <summary>
        /// 継続で移動するかどうかのフラグ
        /// </summary>
        public abstract bool CanContinue { get; }

        public static int BlockAndWallLayer { get; private set; }

        public Direction.Type MoveDirection { get; protected set; }

        public static Collider[] Results { get; private set; } = new Collider[4];

        /// <summary>
        /// 越えられる段差の高さ
        /// </summary>
        public static float StepHeight => PiyoMover.StepHeight;

        /// <summary>
        /// 押された時の速度
        /// </summary>
        public Vector3 PushVelocity => pushedVector / Mathf.Max(pushedSeconds, 0.01f);

        /// <summary>
        /// 再生に使うAudioSource
        /// </summary>
        protected AudioSource audioSource;
        protected BlockRouteData routeData { get; private set; }
        /// <summary>
        /// 押された距離
        /// </summary>
        protected Vector3 pushedVector;
        /// <summary>
        /// 押された秒数
        /// </summary>
        protected float pushedSeconds;

        protected BlockStateAfterPushMove stateAfterPushMove;
        protected BlockStateFall stateFall;

        /// <summary>
        /// 履歴クラスのインスタンス
        /// </summary>
        protected HistoryBehaviour historyBehaviour;

        protected virtual void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                rb = GetComponent<Rigidbody>();
                routeData = GetComponent<BlockRouteData>();
                BoxColliderInstance = GetComponent<BoxCollider>();
                BlockLayer = LayerMask.GetMask("Block");
                BlockAndWallLayer = LayerMask.GetMask("Block", "Wall");
                audioSource = GetComponent<AudioSource>();
            }
        }

        /// <summary>
        /// 押せるかの確認と、押せる時は押し始めの処理を実行。
        /// </summary>
        /// <param name="direction">押す方向</param>
        /// <returns>押せる時 true</returns>
        protected bool TryPush(Vector3 direction)
        {
            // 押せないなら何もしない
            if (!routeData.CanPush(Direction.DetectType(direction)))
            {
#if UNITY_EDITOR
                Debug.Log($"TODO 押せない時の演出");
#endif
                return false;
            }

            // 押す処理開始
            MoveDirection = Direction.DetectType(direction);
            BlockMoveObserver.Add(this);
            pushedVector = Vector3.zero;
            pushedSeconds = 0;
            PlayPushSE();
            return true;
        }

        public virtual bool StartPush(Vector3 direction)
        {
            if (!TryPush(direction))
            {
                return false;
            }

            // 押し終わったあとの自律動作用の状態
            if (stateAfterPushMove == null)
            {
                stateFall = new BlockStateFall(this);
                stateAfterPushMove = new BlockStateAfterPushMove(this, stateFall);
            }

            HistoryStartMove();
            return true;
        }

        /// <summary>
        /// プレイヤーから押される処理
        /// </summary>
        /// <param name="move">移動させるベクトル。Yは無視</param>
        public virtual void Push(Vector3 move)
        {
            move.y = 0;

            Vector3 center = BoxColliderInstance.bounds.center + move;
            int count = Physics.OverlapBoxNonAlloc(center, BoxColliderInstance.bounds.extents, Results, Quaternion.identity, BlockLayer);

            float upY = 0;
            for (int i = 0; i < count; i++)
            {
                // 自分は対象外
                if (Results[i] == BoxColliderInstance) continue;

                float dy = Results[i].bounds.max.y - BoxColliderInstance.bounds.min.y;
                upY = Mathf.Max(upY, dy);
            }

            pushedVector += move;
            pushedSeconds += Time.fixedDeltaTime;

            rb.position += move + upY * Vector3.up;
        }

        /// <summary>
        /// 押し終えたらプレイヤーから呼び出す。
        /// ここからきりがよいところまで移動して落下まで自律して実行。
        /// </summary>
        public void PushDone()
        {
            AdjustXZ();
            Enqueue(stateAfterPushMove);
        }

        /// <summary>
        /// 指定の座標を中心にした球体の衝突判定を取得する。
        /// 結果はMoveBlockBase.Resultsに格納される。
        /// </summary>
        /// <param name="pos">中心座標</param>
        /// <returns>接触数</returns>
        public static int OverlapSphereBlockAndWall(Vector3 pos)
        {
            int count = Physics.OverlapSphereNonAlloc(
                pos, CheckRadius,
                Results,
                BlockAndWallLayer);
            return count;
        }

        /// <summary>
        /// 指定の座標を中心にした球体の衝突判定を取得する。
        /// 結果はMoveBlockBase.Resultsに格納される。
        /// </summary>
        /// <param name="pos">中心座標</param>
        /// <returns>接触数</returns>
        public static int OverlapSphereBlock(Vector3 pos)
        {
            int count = Physics.OverlapSphereNonAlloc(
                pos, CheckRadius,
                Results,
                BlockLayer);
            return count;
        }

        public void PlayCollisionSE()
        {
            if (collisionSE != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(collisionSE);
            }
        }

        public void PlayPushSE()
        {
            if (pushSE != null)
            {
                audioSource.PlayOneShot(pushSE);
            }
        }

        /// <summary>
        /// 氷の上かどうかを判定する。
        /// </summary>
        /// <returns>氷の上の時、true</returns>
        protected bool IsOnIce()
        {
            int count = OverlapSphereBlock(BoxColliderInstance.bounds.center + Vector3.down);
            IBlockInfo blockInfo = null;

            for (int i = 0; i < count; i++)
            {
                if ((blockInfo = Results[i].GetComponent<IBlockInfo>()) != null)
                {
                    // 滑るか
                    return blockInfo.IsSlippery;
                }
            }

            return false;
        }

        /// <summary>
        /// 履歴の移動開始を通知する。
        /// </summary>
        protected void HistoryStartMove()
        {
            // 履歴登録
            if (historyBehaviour == null)
            {
                historyBehaviour = GetComponent<HistoryBehaviour>();
            }
            if (historyBehaviour != null)
            {
                HistoryRecorder.StartMove(historyBehaviour);
            }
        }

        /// <summary>
        /// XZ座標を四捨五入で丸め込む。
        /// </summary>
        protected void AdjustXZ()
        {
            var pos = rb.position;
            pos.x = Mathf.Round(pos.x);
            pos.z = Mathf.Round(pos.z);
            rb.position = pos;
        }
    }
}