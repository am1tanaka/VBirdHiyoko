using System.Text;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 簡易シナリオテキストの共通処理を持たせた基底クラス。
    /// </summary>
    public static class ScenarioTextValidator
    {
        static readonly StringBuilder merge = new StringBuilder(256);
        static readonly string[] blankArray = new string[0];

        /// <summary>
        /// コマンドとパラメーター数をチェックする。
        /// </summary>
        /// <param name="words">コマンド行をスペース区切りした配列</param>
        /// <param name="command">コマンド</param>
        /// <param name="count">パラメーター数</param>
        /// <returns>合致する時 true。コマンド違いやパラメーター違いなら false</returns>
        public static bool Validate(string[] words, string command, int count)
        {
            if (words[0].Trim().ToLower() != command.ToLower())
            {
                return false;
            }
            if (words.Length < count)
            {
#if UNITY_EDITOR
                Debug.LogError("パラメーター不足:{words[0]} {words[1]}");
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        /// スペース区切りの文字列配列を受け取って、2つめ以降の要素をコンマ区切りの文字列にして返す。
        /// </summary>
        /// <param name="words">シナリオ行をスペース区切りした配列</param>
        /// <param name="startIndex">統合を開始する配列。省略すると1から</param>
        /// <returns>2つめ以降のデータをコンマ区切りで栗り直した配列</returns>
        public static string[] GetParamArray(string[] words, int startIndex = 1)
        {
            if (words.Length < (startIndex + 1))
            {
                return blankArray;
            }

            merge.Clear();
            for (int i = startIndex; i < words.Length; i++)
            {
                merge.Append(words[i]);
            }
            return merge.ToString().Split(',');
        }
    }
}