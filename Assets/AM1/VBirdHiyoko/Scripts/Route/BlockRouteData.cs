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
        /// 指定の方向に押せるかを確認。押せるならtrue。必要に応じて調査を実行。
        /// </summary>
        /// <param name="dir">押せるかを確認する方向</param>
        /// <returns>押せる時、true</returns>
        public bool CanPush(Direction.Type dir)
        {
            Debug.Log("未実装");
            return false;
        }

        /// <summary>
        /// 歩けるとき、trueを返す
        /// </summary>
        public bool CanWalk => Checked && StepCount > 0;

    }
}