using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 次のステージへ移動するシナリオ
    /// </summary>
    public class PiyoScenarioNextStage : IScenarioTextCommand
    {
        static string CommandText => "@next_stage";
        static int CommandCount => 1;

        public bool IsCommand(string[] words)
        {
            return ScenarioTextValidator.Validate(words, CommandText, CommandCount);
        }

        public IEnumerator Invoke()
        {
            // 次のステージへ切り替え
            if (VBirdHiyokoManager.NextStage())
            {
                // 次のステージへ
                GameSceneStateChanger.Instance.Request(true);
            }
            else
            {
                // エンディングへ
                EndingSceneStateChanger.Instance.Request(true);
            }

            yield return null;
        }
    }
}