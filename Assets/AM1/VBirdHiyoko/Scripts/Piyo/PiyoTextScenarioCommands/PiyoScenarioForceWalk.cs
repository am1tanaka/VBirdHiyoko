using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class PiyoScenarioForceWalk : IScenarioTextCommand
    {
        static string CommandText => "@force_walk";
        static int CommandCount => 2;

        Vector3 to;

        public bool IsCommand(string[] words)
        {
            if (!ScenarioTextValidator.Validate(words, CommandText, CommandCount)) return false;

            var toParams = ScenarioTextValidator.GetParamArray(words);
            if (toParams.Length != 3)
            {
#if UNITY_EDITOR
                Debug.LogError($"ForceWalkのパラメーターのX,Y,Zが足りないか多すぎる。:{words[1]}");
#endif
                return false;
            }

            to.x = float.TryParse(toParams[0], out var x) ? x : 0;
            to.y = float.TryParse(toParams[1], out var y) ? y : 0;
            to.z = float.TryParse(toParams[2], out var z) ? z : 0;
            return true;
        }

        public IEnumerator Invoke()
        {
            yield return PiyoBehaviour.Instance.Mover.ForceWalkTo(to);
        }
    }
}