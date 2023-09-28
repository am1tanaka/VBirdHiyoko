using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame.Assets
{
    /// <summary>
    /// 効果音のインデックスをenumで定義して、
    /// enum指定で効果音再生をするためのstaticクラス。
    /// </summary>
    public static class SEPlayer
    {
        public enum SE
        {
            Click,
            Start,
            Jump,
            Miss,
            Clear
        }

        /// <summary>
        /// 同時再生停止秒数
        /// </summary>
        public static float DelaySeconds => 0.08f;

        /// <summary>
        /// 遅延再生登録
        /// </summary>
        public static int DelayMax => 16;

        /// <summary>
        /// 引数で指定した効果音を再生。
        /// </summary>
        /// <param name="se">enumで定義した効果音を指定</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void Play(SE se)
        {
            SESourceAndClips.Instance.Play((int)se);
        }
    }
}
