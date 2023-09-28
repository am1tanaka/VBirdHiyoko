using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.BaseFrame.Assets
{
    /// <summary>
    /// ボリュームをPlayerPrefsで読み書きするベースクラス
    /// </summary>
    public abstract class VolumeSaverWithPlayerPrefsBase : IVolumeSaver
    {
        /// <summary>
        /// キーにプレフィックスを付けるときに設定。主にテスト用
        /// </summary>
        public static string prefix;

        /// <summary>
        /// キー名
        /// </summary>
        protected abstract string KeyName { get; }

        public void ClearSaveData()
        {
            PlayerPrefs.DeleteKey(KeyName);
        }

        public int Load(int df)
        {
            return PlayerPrefs.GetInt(KeyName, df);
        }

        public void Save(int v)
        {
            PlayerPrefs.SetInt(KeyName, v);
        }
    }
}