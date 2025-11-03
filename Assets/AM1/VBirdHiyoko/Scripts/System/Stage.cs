using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ステージ数を管理するクラス
    /// </summary>
    public class Stage
    {
        /// <summary>
        /// 現在のステージ数。0=Title, 1=Stage01
        /// </summary>
        public int Current { get; private set; } = 0;

        /// <summary>
        /// 最大ステージ
        /// </summary>
        public static int MaxStage => 4;

        /// <summary>
        /// ステージ数を設定する
        /// </summary>
        /// <param name="stage">設定したいステージ数。0=StageTitle, 1=Stage01</param>
        public void Set(int stage)
        {
            Current = Mathf.Clamp(stage, 0, MaxStage);
        }

        /// <summary>
        /// 次のステージに更新。最終ステージだった場合、Currentはそのままでfalseを返す
        /// </summary>
        /// <returns>次のステージがあったらtrue。最終ステージだったらfalse</returns>
        public bool Next()
        {
            if (Current >= MaxStage)
            {
                return false;
            }

            Current++;
            return true;
        }
    }
}
