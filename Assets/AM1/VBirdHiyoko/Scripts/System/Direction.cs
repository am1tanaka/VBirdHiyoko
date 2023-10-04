using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 方向に関連する機能を提供
    /// </summary>
    public static class Direction
    {
        /// <summary>
        /// Typeに該当する向き
        /// </summary>
        public static Vector3[] Vector =
        {
            Vector3.forward,
            Vector3.right,
            Vector3.back,
            Vector3.left,
        };

        /// <summary>
        /// 方向
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 方向無効
            /// </summary>
            None = -1,
            /// <summary>
            /// 奥向き
            /// </summary>
            Forward,
            /// <summary>
            /// 右向き
            /// </summary>
            Right,
            /// <summary>
            /// 手前向き
            /// </summary>
            Back,
            /// <summary>
            /// 左向き
            /// </summary>
            Left,
            /// <summary>
            /// 旋回中
            /// </summary>
            Turning
        }

        /// <summary>
        /// 1%以内なら一致
        /// </summary>
        static float WithIn => 0.01f;

        /// <summary>
        /// 向きベクトルがTypeでどの方向かを返す。
        /// </summary>
        /// <param name="face">向いている方向ベクトル</param>
        /// <returns>Typeの値。中途半端な角度の時はTurningを返す</returns>
        public static Type DetectType(Vector3 face)
        {
            for (int i = 0; i < Vector.Length; i++)
            {
                float dot = Vector3.Dot(face, Vector[i]);
                if (dot >= 1f - WithIn) return (Type)i;
            }

            return Type.Turning;
        }

        /// <summary>
        /// 指定した方向の逆方向を返す
        /// </summary>
        /// <param name="type">対象の方向</param>
        /// <returns>逆の方向</returns>
        public static Type Reverse(Type type)
        {
            switch (type)
            {
                case Type.Forward: return Type.Back;
                case Type.Right: return Type.Left;
                case Type.Back: return Type.Forward;
                case Type.Left: return Type.Right;
            }

            return Type.Turning;
        }
    }
}