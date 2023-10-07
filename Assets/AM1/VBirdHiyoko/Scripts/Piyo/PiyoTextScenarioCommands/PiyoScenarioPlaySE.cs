using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 指定の効果音を鳴らす
    /// </summary>
    public class PiyoScenarioPlaySE : IScenarioTextCommand
    {
        static string CommandText => "@se";
        static int CommandCount => 2;

        string se;

        public bool IsCommand(string[] words)
        {
            if (!ScenarioTextValidator.Validate(words, CommandText, CommandCount)) return false;

            se = words[1].Trim();

            return true;
        }

        public IEnumerator Invoke()
        {
            var seNames = System.Enum.GetNames(typeof(SEPlayer.SE));
            for (int i = 0; i < seNames.Length; i++)
            {
                if (seNames[i] == se)
                {
                    SEPlayer.Play((SEPlayer.SE)i);
                    break;
                }
            }
            yield break;
        }
    }
}