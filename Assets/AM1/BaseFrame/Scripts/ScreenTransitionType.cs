using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame.Assets
{
    /// <summary>
    /// 画面切り替えの演出の種類
    /// </summary>
    public enum ScreenTransitionType
    {
        None = -1,
        /// <summary>
        /// 円形に塗りつぶす
        /// </summary>
        FilledRadial,
        /// <summary>
        /// フェード
        /// </summary>
        Fade,
    }
}