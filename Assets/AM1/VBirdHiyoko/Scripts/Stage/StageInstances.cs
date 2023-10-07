using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ステージのオブジェクトのインスタンスを管理するクラス
    /// </summary>
    [System.Serializable]
    public class StageInstances
    {
        [Tooltip("目的地カーソル")]
        public TargetCursor targetCursor = default;

        [Tooltip("目的地の旗")]
        public TargetFlag targetFlag = default;

        [Tooltip("押せる方向矢印管理クラス")]
        public PushArrows pushArrows = default;

        [Tooltip("ワールドを指すカーソル")]
        public WorldPointer worldPointer = default;
    }
}