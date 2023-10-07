using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko {
    public class VBirdHiyokoPlayerPrefs : IGameDataStorage
    {
        /// <summary>
        /// デフォルトのクリアステージ数
        /// </summary>
        public static int DefaultClearedStage => 0;

        public static string prefix = "";
        static string DefaultClearedStageKey => "ClearedStage";
        static string ClearedStageKey => $"{prefix}{DefaultClearedStageKey}";

        public void InitAndLoad()
        {
            VBirdHiyokoManager.ClearedStage.Set(PlayerPrefs.GetInt(ClearedStageKey, DefaultClearedStage));
            VBirdHiyokoManager.Log($"PlayerPrefs Init And Load Stage = {VBirdHiyokoManager.ClearedStage.Current}");
        }

        public void SaveClearedStage(int newStage)
        {
            VBirdHiyokoManager.Log($"PlayerPrefs Save({newStage})");
            PlayerPrefs.SetInt(ClearedStageKey, newStage);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 現在のprefixのデータを全て削除
        /// </summary>
        public void DeleteAll()
        {
            PlayerPrefs.DeleteKey(ClearedStageKey);
        }
    }
}