using System.Collections;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 指定したステージ番号のステージを開始するシナリオ
    /// </summary>
    public class PiyoScenarioStartStage : IScenarioTextCommand
    {
        static string CommandText => "@stage";
        static int CommandCount => 2;

        int stage;

        public bool IsCommand(string[] words)
        {
            if (!ScenarioTextValidator.Validate(words, CommandText, CommandCount))
            {
                return false;
            }

            if (!int.TryParse(words[1], out stage))
            {
                return false;
            }

            return true;
        }

        public IEnumerator Invoke()
        {
            yield return null;

            // 指定のステージへ切り替え
            if (GameSceneStateChanger.Instance.Request())
            {
                VBirdHiyokoManager.CurrentStage.Set(stage);
            }
        }
    }
}