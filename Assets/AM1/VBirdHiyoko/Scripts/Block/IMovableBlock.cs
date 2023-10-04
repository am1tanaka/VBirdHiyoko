using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 移動ブロックを制御するインターフェース
    /// </summary>
    public interface IMovableBlock
    {
        /// <summary>
        /// 押す時に呼び出す。押せない時は false を返す。
        /// </summary>
        /// <param name="direction">押す方向</param>
        /// <returns>押せたとき、true。押せなかったら、false</returns>
        bool StartPush(Vector3 direction);

        /// <summary>
        /// 受け取った移動量を動かす。段差があれば乗り上げさせる。
        /// </summary>
        /// <param name="move">移動量</param>
        void Push(Vector3 move);

        /// <summary>
        /// プレイヤーが押すのを終えたら呼び出す。この後は停止するまで自律して動かす。
        /// </summary>
        void PushDone();

        /// <summary>
        /// 押す音
        /// </summary>
        void PlayPushSE();

        /// <summary>
        /// 落下や接触の効果音
        /// </summary>
        void PlayCollisionSE();
    }
}