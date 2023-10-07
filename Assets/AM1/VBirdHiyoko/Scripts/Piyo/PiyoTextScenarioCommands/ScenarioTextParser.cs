using AM1.MessageSystem;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 簡易スクリプトパーサー
    /// </summary>
    public class ScenarioTextParser
    {
        /// <summary>
        /// 現在のコマンド
        /// </summary>
        public IScenarioTextCommand CurrentCommand { get; private set; }

        /// <summary>
        /// シナリオテキストのコマンド配列
        /// </summary>
        IScenarioTextCommand[] commands;
        string[] scenarioLines;
        int lineIndex;

        /// <summary>
        /// セリフのデータ
        /// </summary>
        public readonly MessageData TalkMessageData = new MessageData("");

        /// <summary>
        /// 旋回する時の方向
        /// </summary>
        public Vector3 TurnForward { get; private set; }

        /// <summary>
        /// 待ち秒数
        /// </summary>
        public float WaitSeconds { get; private set; }

        /// <summary>
        /// コマンド配列を受け取って記録する。
        /// </summary>
        /// <param name="commands"></param>
        public ScenarioTextParser(IScenarioTextCommand[] commands)
        {
            this.commands = commands;
        }

        /// <summary>
        /// シナリオテキストを設定する。
        /// </summary>
        /// <param name="scenario">シナリオテキスト</param>
        public void SetScenario(string scenario)
        {
            scenarioLines = scenario.Split("\n");
            lineIndex = 0;
        }

        /// <summary>
        /// 次のコマンドを解析する。
        /// </summary>
        /// <returns>次のコマンドのインスタンス</returns>
        public IScenarioTextCommand Next()
        {
            for (int i = lineIndex; i < scenarioLines.Length; i++)
            {
                string line = scenarioLines[i].Trim();
                lineIndex++;

                if (line.Length == 0)
                {
                    continue;
                }

                if (line[0] == '#')
                {
                    // コメント行
                    continue;
                }

                // コマンド判定
                string[] words = line.Split(' ');
                for (int j = 0; j < commands.Length; j++)
                {
                    if (commands[j].IsCommand(words))
                    {
                        CurrentCommand = commands[j];
                        return CurrentCommand;
                    }
                }

#if UNITY_EDITOR
                // コマンドが見つからなかった
                Debug.Log($"未実装コマンド:{words[0]} {(words.Length >= 2 ? words[1] : "")}");
#endif
                break;
            }

            // スクリプト終了
            CurrentCommand = null;
            return CurrentCommand;
        }
    }
}