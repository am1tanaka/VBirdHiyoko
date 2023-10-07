using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class PiyoScenarioWait : IScenarioTextCommand
    {
        static string CommandText => "@wait";
        static int CommandCount => 2;

        float waitSeconds;

        public bool IsCommand(string[] words)
        {
            if (!ScenarioTextValidator.Validate(words, CommandText, CommandCount)) return false;

            waitSeconds = float.TryParse(words[1], out float sec) ? sec : 0;
            return true;
        }

        public IEnumerator Invoke()
        {
            yield return new WaitForSeconds(waitSeconds);
        }
    }
}