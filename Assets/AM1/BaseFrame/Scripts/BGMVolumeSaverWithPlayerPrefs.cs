using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame.Assets
{
    /// <summary>
    /// BGMのボリュームをPlayerPrefsで読み書きする
    /// </summary>
    public class BGMVolumeSaverWithPlayerPrefs : VolumeSaverWithPlayerPrefsBase
    {
        protected override string KeyName => $"{prefix}BGMVol";
    }
}