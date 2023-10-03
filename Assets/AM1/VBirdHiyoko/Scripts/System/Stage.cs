using System.Collections;
using System.Collections.Generic;
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
        public int CurrentStage { get; private set; } = 0;

        /// <summary>
        /// 最大ステージ
        /// </summary>
        public static int MaxStage => 8;

        /// <summary>
        /// ステージ数を設定する
        /// </summary>
        /// <param name="stage">設定したいステージ数。0=StageTitle, 1=Stage01</param>
        public void SetStage(int stage)
        {
            CurrentStage = Mathf.Clamp(stage, 0, MaxStage);
        }

        /// <summary>
        /// 次のステージに更新。最終ステージだった場合、CurrentStageはそのままでfalseを返す
        /// </summary>
        /// <returns>次のステージがあったらtrue。最終ステージだったらfalse</returns>
        public bool NextStage()
        {
            if (CurrentStage >= MaxStage)
            {
                return false;
            }

            CurrentStage++;
            return true;
        }
    }
}
