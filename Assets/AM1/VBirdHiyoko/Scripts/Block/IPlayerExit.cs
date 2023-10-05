using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public interface IPlayerExit
    {
        /// <summary>
        /// プレイヤーがブロックから降りた時に呼ぶメソッド。
        /// </summary>
        void OnExit();
    }
}