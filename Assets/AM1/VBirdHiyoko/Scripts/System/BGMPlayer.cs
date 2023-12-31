using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// BGMを定義したenumで再生するためのクラス
    /// </summary>
    public static class BGMPlayer
    {
        public enum BGM
        {
            /// <summary>
            /// 未指定
            /// </summary>
            None = -1,
            /// <summary>
            /// タイトル曲
            /// </summary>
            Title,
            /// <summary>
            /// BGM
            /// </summary>
            Game,
            /// <summary>
            /// Ending
            /// </summary>
            Ending,
        }

        /// <summary>
        /// 指定のBGMを再生。
        /// </summary>
        /// <param name="bgm">鳴らしたいBGM</param>
        /// <param name="time">フェードイン秒数。省略するか0で即時再生</param>
        public static void Play(BGM bgm, float time = 0)
        {
            BGMSourceAndClips.Instance.Play((int)bgm, time);
        }
    }
}