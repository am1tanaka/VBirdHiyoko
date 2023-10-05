using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 歩行コースを作成するクラス
    /// </summary>
    public static class WalkCourse
    {
        /// <summary>
        /// リストの初期最大数
        /// </summary>
        static float ListMax => 256;

        /// <summary>
        /// プレイヤーの座標から歩く方向を並べたスタック
        /// </summary>
        public static readonly Stack<Direction.Type> walkForwards = new Stack<Direction.Type>((int)ListMax);

    }
}