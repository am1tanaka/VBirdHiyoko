using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// クリックするか、指定秒数が経過するまで待機する
    /// </summary>
    public class PiyoScenarioWaitClick : IScenarioTextCommand
    {
        static string CommandText => "@wait_click";
        static int CommandCount => 2;

        float waitSeconds;
        bool isClicked;

        public bool IsCommand(string[] words)
        {
            if (words[0].Trim().ToLower() != CommandText)
            {
                return false;
            }

            if (words.Length < CommandCount)
            {
                waitSeconds = 0;
                return true;
            }

            waitSeconds = float.TryParse(words[1], out float click_sec) ? click_sec : 0;
            return true;
        }

        public IEnumerator Invoke()
        {
            float time = 0;
            isClicked = false;
            InputActionDetector.Instance.OnAction.AddListener(Click);
            while (Mathf.Approximately(waitSeconds, 0)
                || (time < waitSeconds))
            {
                if (isClicked)
                {
                    break;
                }
                time += Time.deltaTime;
                yield return null;
            }
        }

        void Click(RaycastHit hit)
        {
            isClicked = true;
        }
    }
}