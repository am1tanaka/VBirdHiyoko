//#define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// プレイヤーの移動処理クラス。アニメも実装
    /// </summary>
    public class PiyoMover
    {
        /// <summary>
        /// 歩行速度
        /// </summary>
        public static float WalkSpeed => 3f;

        /// <summary>
        /// 1秒の旋回角度
        /// </summary>
        public static float AngularVelocity => 360f * 2.5f;

        /// <summary>
        /// 登ったり降りたりできる段差の高さ
        /// </summary>
        public static float StepHeight => 0.25f;

        /// <summary>
        /// 移動などの誤差
        /// </summary>
        static float Margin => 0.001f;

        static readonly int blockLayer = LayerMask.GetMask("Block");

        /// <summary>
        /// 衝突判定を受け取る配列
        /// </summary>
        RaycastHit[] results = new RaycastHit[4];

        /// <summary>
        /// 動作状態
        /// </summary>
        public enum State
        {
            Stand,
            Fall,
            Walk,
            Push,
            Turn,
            Banzai,
        }

        /// <summary>
        /// 状態に応じたアニメ割り当て
        /// </summary>
        static readonly int[] PiyoAnimStates =
        {
            (int)PiyoAnimState.Stand,
            (int)PiyoAnimState.Turn,
            (int)PiyoAnimState.Walk,
            (int)PiyoAnimState.Push,
            (int)PiyoAnimState.Turn,
            (int)PiyoAnimState.Banzai,
        };

        public static State CurrentState { get; private set; } = State.Stand;

        /// <summary>
        /// 足場ブロックのインスタンス
        /// </summary>
        public RaycastHit? FootBlock { get; private set; }

        /// <summary>
        /// 回転を管理するTransform
        /// </summary>
        Transform pivotTransform;

        /// <summary>
        /// 押しているブロックのインスタンス
        /// </summary>
        IMovableBlock moveBlock;

        Rigidbody rb;
        Animator animator;
        Vector3 velocity;
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        BoxCollider boxCollider;
        PiyoBehaviour piyoBehaviour;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="rigidbody">制御するRigidbodyのインスタンス</param>
        /// <param name="pivot">軸となる座標</param>
        public PiyoMover(PiyoBehaviour piyo, Rigidbody rigidbody, Transform pivot)
        {
            piyoBehaviour = piyo;
            rb = rigidbody;
            pivotTransform = pivot;
            animator = rb.GetComponentInChildren<Animator>();
            boxCollider = rb.GetComponentInChildren<BoxCollider>();
            velocity = Vector3.zero;
            CurrentState = State.Fall;
        }

        /// <summary>
        /// 状態を指定のものに設定して、アニメを切り替える。
        /// </summary>
        /// <param name="state">切り替えたい状態</param>
        void SetState(State state)
        {
            CurrentState = state;
            animator.SetInteger("State", PiyoAnimStates[(int)state]);
        }

        /// <summary>
        /// 目的地算出
        /// </summary>
        /// <param name="dir">移動方向ベクトル</param>
        /// <returns>dirを足して端数を丸め込んだ座標</returns>
        Vector3 GetTargetPosition(Vector3 dir)
        {
            Vector3 target = rb.position + dir;
            target.x = Mathf.Round(target.x);
            target.z = Mathf.Round(target.z);
            return target;
        }

        /// <summary>
        /// その場で着地するまで落下
        /// </summary>
        public IEnumerator Fall()
        {
            bool isGrounded = false;

            while (!isGrounded)
            {
                // 重力加速
                velocity.y += Physics.gravity.y * Time.deltaTime;
                float dy = velocity.y * Time.fixedDeltaTime;

                // 落下していなければ終了
                if (dy > 0)
                {
                    yield break;
                }

                // ターンに切り替え
                SetState(State.Fall);

                int count = Physics.BoxCastNonAlloc(
                    boxCollider.bounds.center,
                    boxCollider.bounds.extents,
                    Vector3.down,
                    results,
                    Quaternion.identity, -dy, blockLayer);

                if (count == 0)
                {
                    // 落下
                    rb.MovePosition(rb.position + Vector3.up * dy);
                    FootBlock = null;
                }
                else
                {
                    // 着地
                    isGrounded = true;

                    // 一番高い位置を決める
                    float high = results[0].collider.bounds.max.y;
                    FootBlock = results[0];
                    for (int i = 1; i < count; i++)
                    {
                        if (results[i].point.y > high)
                        {
                            high = results[i].collider.bounds.max.y;
                            FootBlock = results[i];
                        }
                    }
                    var pos = rb.position;
                    pos.y = high + boxCollider.bounds.extents.y - boxCollider.center.y + Margin;
                    rb.MovePosition(pos);
                }

                yield return wait;
            }
        }

        /// <summary>
        /// 指定の方向へ歩く
        /// </summary>
        /// <param name="dir">歩く方向。Yは無効</param>
        public IEnumerator WalkTo(Vector3 dir)
        {
            yield return TurnTo(Direction.DetectType(dir));

            SetState(State.Walk);

            // 目的座標算出
            Vector3 target = GetTargetPosition(dir);
            // 目的座標まで移動
            bool isWalking = true;
            float step = Time.fixedDeltaTime * WalkSpeed;
            while (isWalking)
            {
                Vector3 to = target - rb.position;
                to.y = 0;
                float distance = step;
                if (to.magnitude <= distance)
                {
                    // 次の移動で到着
                    isWalking = false;
                    distance = to.magnitude;
                }

                SideMove(distance * to.normalized);
                yield return wait;
            }
        }

        /// <summary>
        /// 指定のベクトルを横へ移動する。
        /// 移動前に段差登りチェック、
        /// 移動後に落下をチェックする。
        /// StepHeight以上の段差では本来はここに来ないはず。
        /// フェールセーフとして、登りなら警告を表示して無理やり移動。
        /// 下りなら落下させてから移動を継続
        /// </summary>
        /// <param name="move">移動ベクトル。Yは無視</param>
        /// <returns>着地していたら true。空中になったら false</returns>
        bool SideMove(Vector3 move)
        {
            move.y = 0;

            // 下方向の確認
            int count = Physics.BoxCastNonAlloc(
                boxCollider.bounds.center,
                boxCollider.bounds.extents,
                Vector3.down,
                results,
                Quaternion.identity,
                StepHeight,
                blockLayer);
            if (count == 0)
            {
                // 床がない。本来はないが、床がなかったら何もせずに戻る
                return false;
            }

            // 下方向の地面までの距離を求める
            var adjustedMove = move;
            adjustedMove.y = GetLandingDistance(count);

            // 横方向の接触物を調べる
            count = Physics.BoxCastNonAlloc(
                boxCollider.bounds.center + adjustedMove.y * Vector3.up,
                boxCollider.bounds.extents,
                move.normalized,
                results,
                Quaternion.identity,
                move.magnitude,
                blockLayer);

            adjustedMove.y += GetStepDistance(count, adjustedMove.y);

            // 移動実行
            var nextPos = rb.position + adjustedMove;
            ChangeGroundCheck(nextPos);
            rb.MovePosition(nextPos);
            return true;
        }

        /// <summary>
        /// 乗っている足場の変化を調べる。
        /// 変更があったら前のブロックのOnExitを呼び出す。
        /// </summary>
        /// <param name="pos">チェックする座標</param>
        void ChangeGroundCheck(Vector3 pos)
        {
            pos.y = boxCollider.bounds.center.y;
            // 下方向の確認
            int count = Physics.BoxCastNonAlloc(
                pos,
                boxCollider.bounds.extents,
                Vector3.down,
                results,
                Quaternion.identity,
                StepHeight,
                blockLayer);

            // 接触していなければなし
            if (count == 0)
            {
                FootBlock = null;
                return;
            }

            // 足場がなければ現在のものを入れて終わり
            if (FootBlock == null)
            {
                FootBlock = results[0];
                return;
            }

            // 足場があった。変わったかを確認
            for (int i = 0; i < count; i++)
            {
                // 同じブロックがあれば処理なし
                if (results[i].collider == FootBlock.Value.collider)
                {
                    return;
                }
            }

            var exit = FootBlock.Value.collider.GetComponent<IPlayerExit>();
            exit?.OnExit();
            FootBlock = results[0];
        }

        /// <summary>
        /// resultsの結果から、着地に必要な距離を求めて返す。
        /// </summary>
        /// <param name="count">Raycastの数</param>
        /// <returns>着地に必要な距離。下方向なら負の値を返す</returns>
        float GetLandingDistance(int count)
        {
            if (count == 0)
            {
                // 0なら移動なし
                return 0;
            }

            // 高さ補正
            float minLength = 0;
            for (int i = 0; i < count; i++)
            {
                minLength = Mathf.Min(minLength, results[i].collider.bounds.max.y - boxCollider.bounds.min.y + Margin);
            }
            return Mathf.Min(minLength, 0);
        }

        /// <summary>
        /// 段差があったら上面が足場になるように調整
        /// </summary>
        /// <param name="count">resultsの数</param>
        /// <returns>足場までの距離。上方向で正の値</returns>
        float GetStepDistance(int count, float defaultY)
        {
            if (count == 0)
            {
                // 接触するものがなければ何もせずに返す
                return defaultY;
            }

            // 上面で一番高い値を求める
            float highest = 0;
            for (int i = 0; i < count; i++)
            {
                highest = Mathf.Max(
                    highest,
                    ContactFootDistance(results[i].collider.bounds.max.y + Margin));
            }
            return highest;
        }

        /// <summary>
        /// 指定の座標に立たせるための現在の位置からの距離を返す。
        /// </summary>
        /// <returns>移動距離</returns>
        float ContactFootDistance(float contactY)
        {
            return Mathf.Abs(contactY - boxCollider.bounds.min.y);
        }

        /// <summary>
        /// 指定の方角へ向く。
        /// </summary>
        /// <param name="dir">方向を指示</param>
        /// <param name="isImmediate">1フレームで実行する時 true。省略すると速度に応じてアニメ</param>
        public IEnumerator TurnTo(Direction.Type dir, bool isImmediate = false)
        {
            if (isImmediate || piyoBehaviour.IsFaceTo(dir))
            {
                pivotTransform.forward = Direction.Vector[(int)dir];
            }
            else
            {
                yield return TurnTo(Direction.Vector[(int)dir], isImmediate);
            }
        }

        /// <summary>
        /// 指定の方向へ向く。方向をベクトルで与えるバージョン。
        /// </summary>
        /// <param name="forward">前方ベクトル</param>
        /// <param name="isImmediate">即時の時、true。省略するとアニメ</param>
        public IEnumerator TurnTo(Vector3 forward, bool isImmediate = false)
        {
            // 即時かすでに向いている時は即時終了
            if (isImmediate || piyoBehaviour.IsFaceTo(Direction.DetectType(forward)))
            {
                pivotTransform.forward = forward;
                yield break;
            }

            // 旋回処理開始
            SetState(State.Turn);

            // 旋回処理
            Vector3 from = pivotTransform.forward;
            float angle = Vector3.SignedAngle(from, forward, Vector3.up);
            float time = Mathf.Abs(angle) / AngularVelocity;
            for (float t = 0; t < time; t += Time.fixedDeltaTime)
            {
                pivotTransform.forward = Vector3.Slerp(from, forward, t / time);
                yield return wait;
            }

            pivotTransform.forward = forward;
        }

        /// <summary>
        /// 指定方向へ押す
        /// </summary>
        /// <param name="dir">押す方向。Yは無効</param>
        /// <param name="block">押す対象のブロック</param>
        public IEnumerator PushTo(Vector3 dir, IMovableBlock block)
        {
            if (!block.StartPush(dir))
            {
                // 押せなかったので取り消し
                yield break;
            }

            // 押す処理
            moveBlock = block;
            SetState(State.Push);

            // 目的座標算出
            Vector3 target = rb.position + dir;
            target.x = Mathf.Round(target.x);
            target.z = Mathf.Round(target.z);

            // 目的座標まで移動
            bool isWalking = true;
            float step = Time.fixedDeltaTime * WalkSpeed;
            while (isWalking)
            {
                Vector3 to = target - rb.position;
                to.y = 0;
                float distance = step;
                if (to.magnitude <= distance)
                {
                    // 次の移動で到着
                    isWalking = false;
                    distance = to.magnitude;
                }

                moveBlock.Push(distance * to.normalized);
                Vector3 next = rb.position + distance * to.normalized;
                ChangeGroundCheck(next);
                rb.MovePosition(next);
                yield return wait;
            }

            moveBlock.PushDone();
        }

        /// <summary>
        /// きりのよい場所へ静止して立ちモーション。
        /// </summary>
        public IEnumerator Stand()
        {
            // 着地
            yield return Fall();

            // TODO きりのよい場所へ移動

            // 停止
            SetState(State.Stand);
        }

        /// <summary>
        /// 万歳アニメをして、終了したら次へ
        /// </summary>
        public IEnumerator Banzai()
        {
            yield return Fall();

            // 万歳
            //SetState(State.Banzai);
            //yield return PiyoBehaviour.Instance.AnimEventInstance.WaitEvent();
            Debug.Log("未実装");
        }

        /// <summary>
        /// 指定の相対位置へ当たり判定なしで移動する。
        /// </summary>
        /// <param name="dir">歩く方向。Yも有効</param>
        public IEnumerator ForceWalkTo(Vector3 dir)
        {
            yield return TurnTo(Direction.DetectType(dir));

            SetState(State.Walk);

            // 目的座標算出
            Vector3 target = rb.position + dir;

            // 当たり判定を切る
            boxCollider.enabled = false;

            // 目的座標まで移動
            bool isWalking = true;
            float step = Time.fixedDeltaTime * WalkSpeed;
            while (isWalking)
            {
                Vector3 to = target - rb.position;
                float distance = step;
                if (to.magnitude <= distance)
                {
                    // 次の移動で到着
                    isWalking = false;
                    distance = to.magnitude;
                }

                var nextPos = rb.position + distance * to.normalized;
                rb.MovePosition(nextPos);
                yield return wait;
            }

            // 当たり判定復帰
            boxCollider.enabled = true;
        }
    }
}
