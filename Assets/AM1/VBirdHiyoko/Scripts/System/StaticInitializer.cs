using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// Static変数を初期化する。
    /// テストなどで起動したまま static のリセットが必要な場合のため。
    /// </summary>
    public class StaticInitializer
    {
        public static void Init()
        {
            VBirdHiyokoManager.Init();
        }
    }
}