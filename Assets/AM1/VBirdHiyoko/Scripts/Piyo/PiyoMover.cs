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

        public static State CurrentState { get; private set; } = State.Stand;

        public PiyoMover(Rigidbody rigidbody, Transform pivot)
        {

        }

        /// <summary>
        /// その場で着地するまで落下
        /// </summary>
        public IEnumerator Fall()
        {
            Debug.Log("未実装");
            yield return null;
        }

        /// <summary>
        /// 指定の方向へ歩く
        /// </summary>
        /// <param name="dir">歩く方向。Yは無効</param>
        public IEnumerator WalkTo(Vector3 dir)
        {
            Debug.Log("未実装");
            yield return null;
        }

        /// <summary>
        /// 指定の方角へ向く。
        /// </summary>
        /// <param name="dir">方向を指示</param>
        /// <param name="isImmediate">1フレームで実行する時 true。省略すると速度に応じてアニメ</param>
        public IEnumerator TurnTo(Direction.Type dir, bool isImmediate = false)
        {
            Debug.Log("未実装");
            yield return null;
        }

        /// <summary>
        /// 指定方向へ押す
        /// </summary>
        /// <param name="dir">押す方向。Yは無効</param>
        /// <param name="block">押す対象のブロック</param>
        public IEnumerator PushTo(Vector3 dir, IMovableBlock block)
        {
            Debug.Log("未実装");
            yield return null;
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
            Debug.Log("未実装");
        }

        /// <summary>
        /// 万歳アニメをして、終了したら次へ
        /// </summary>
        public IEnumerator Banzai()
        {
            yield return Fall();

            // 万歳
            Debug.Log("未実装");
        }
    }
}
