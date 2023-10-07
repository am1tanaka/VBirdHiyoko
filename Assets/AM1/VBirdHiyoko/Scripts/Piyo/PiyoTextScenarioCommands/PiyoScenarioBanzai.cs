using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class PiyoScenarioBanzai : IScenarioTextCommand
    {
        static string CommandText => "@banzai";
        static int CommandCount => 1;

        bool isDone;

        public bool IsCommand(string[] words)
        {
            if (!ScenarioTextValidator.Validate(words, CommandText, CommandCount)) return false;

            return true;
        }

        public IEnumerator Invoke()
        {
            isDone = false;
            PiyoBehaviour.Instance.AnimEventInstance.onEvent.AddListener(OnDone);
            PiyoBehaviour.Instance.SetAnimState(PiyoAnimState.Banzai);
            yield return new WaitUntil(() => isDone);
            PiyoBehaviour.Instance.AnimEventInstance.onEvent.RemoveListener(OnDone);
        }

        void OnDone()
        {
            isDone = true;
        }
    }
}