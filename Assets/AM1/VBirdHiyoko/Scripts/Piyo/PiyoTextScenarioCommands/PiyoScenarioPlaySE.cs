using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class PiyoScenarioPlaySE : IScenarioTextCommand
    {
        public IEnumerator Invoke()
        {
            Debug.Log($"未実装");
            yield return null;
        }

        public bool IsCommand(string[] words)
        {
            Debug.Log($"未実装");
            return false;
        }
    }
}