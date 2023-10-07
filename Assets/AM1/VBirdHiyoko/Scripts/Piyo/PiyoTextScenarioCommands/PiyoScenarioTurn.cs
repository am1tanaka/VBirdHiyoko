using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class PiyoScenarioTurn : IScenarioTextCommand
    {
        static string CommandText => "@turn";
        static int CommandCount => 2;

        Vector3 forward;

        public bool IsCommand(string[] words)
        {
            if (!ScenarioTextValidator.Validate(words, CommandText, CommandCount)) return false;

            switch (words[1].Trim())
            {
                case "forward":
                    forward = Vector3.forward;
                    break;
                case "back":
                    forward = Vector3.back;
                    break;
                case "right":
                    forward = Vector3.right;
                    break;
                case "left":
                    forward = Vector3.left;
                    break;
                default:
                    var f = Camera.main.transform.position - PiyoBehaviour.Instance.transform.position;
                    f.y = 0;
                    forward = f.normalized;
                    break;
            }

            return true;
        }

        public IEnumerator Invoke()
        {
            yield return PiyoBehaviour.Instance.Mover.TurnTo(forward);
        }
    }
}