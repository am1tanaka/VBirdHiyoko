using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 静止するシナリオ
    /// </summary>
    public class PiyoScenarioStand : IScenarioTextCommand
    {
        static string CommandText => "@stand";
        static int CommandCount => 1;

        public bool IsCommand(string[] words)
        {
            return ScenarioTextValidator.Validate(words, CommandText, CommandCount);
        }

        public IEnumerator Invoke()
        {
            PiyoBehaviour.Instance.SetAnimState(PiyoAnimState.Stand);
            yield return null;
        }
    }
}