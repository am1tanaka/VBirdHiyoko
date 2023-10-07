using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 指定秒数でBGMをフェードアウトさせる。
    /// </summary>
    public class PiyoScenarioStopBGM : IScenarioTextCommand
    {
        static string CommandText => "@stop_bgm";
        static int CommandCount => 2;

        float fadeOutSeconds;

        public bool IsCommand(string[] words)
        {
            if (!ScenarioTextValidator.Validate(words, CommandText, CommandCount)) return false;

            if (!float.TryParse(words[1], out fadeOutSeconds))
            {
                VBirdHiyokoManager.Log($"[{CommandText}] {words[1]} is not float.");
                return false;
            }

            return true;
        }

        public IEnumerator Invoke()
        {
            BGMSourceAndClips.Instance.Stop(fadeOutSeconds);
            yield break;
        }
    }
}