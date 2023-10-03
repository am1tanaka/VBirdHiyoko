using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko {
    public class VBirdHiyokoPlayerPrefs : IGameDataStorage
    {
        public static string prefix = "";
        public void InitAndLoad()
        {
            Debug.Log($"未実装");
        }

        public void SaveClearedStage(int newStage)
        {
            Debug.Log($"未実装");
        }
        public void DeleteAll()
        {
            Debug.Log($"未実装");
        }
    }
}