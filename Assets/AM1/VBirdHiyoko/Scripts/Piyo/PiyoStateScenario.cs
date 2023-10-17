using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// シナリオが登録されていたら実行する状態
    /// </summary>
    public sealed class PiyoStateScenario : AM1StateQueueBase
    {
        IPiyoStateScenario scenarioTextData;

        /// <summary>
        /// 他の状態へ切り替え可能になったら true にする
        /// </summary>
        public override bool CanChangeToOtherState => canChangeToOtherState;

        /// <summary>
        /// シナリオテキスト用のクラスを生成
        /// </summary>
        readonly static IScenarioTextCommand[] scenarioTextCommands = new IScenarioTextCommand[]
        {
            new PiyoScenarioTalk(),
            new PiyoScenarioTurn(),
            new PiyoScenarioWait(),
            new PiyoScenarioWaitClick(),
            new PiyoScenarioForceWalk(),
            new PiyoScenarioStartStage(),
            new PiyoScenarioBanzai(),
            new PiyoScenarioNextStage(),
            new PiyoScenarioStand(),
            new PiyoScenarioPlaySE(),
            new PiyoScenarioStopBGM(),
        };

        bool canChangeToOtherState;
        ScenarioTextParser scenarioTextParser = new ScenarioTextParser(scenarioTextCommands);

        /// <summary>
        /// シナリオを発動したシナリオデータを登録する
        /// </summary>
        /// <param name="src">登録するインスタンス</param>
        public void SetSource(IPiyoStateScenario src)
        {
            scenarioTextData = src;
            scenarioTextParser.SetScenario(scenarioTextData.ScenarioText);
        }

        public override void Init()
        {
            base.Init();

            canChangeToOtherState = false;
            PiyoBehaviour.Instance.StartCoroutine(ScriptProcess());
        }

        /// <summary>
        /// スクリプトを解析して処理を実行
        /// </summary>
        /// <returns></returns>
        IEnumerator ScriptProcess()
        {
            // 立ち止まる
            yield return PiyoBehaviour.Instance.Mover.Stand();

            // シナリオ処理開始
            bool isLoop = true;
            while (isLoop)
            {
                var currentCommand = scenarioTextParser.Next();
                if (currentCommand != null)
                {
                    VBirdHiyokoManager.Log($"invoke {currentCommand}");
                    yield return currentCommand.Invoke();
                }
                else
                {
                    VBirdHiyokoManager.Log($"見つからず");
                    isLoop = false;
                }
            }

            // シナリオ終了
            canChangeToOtherState = true;
            PiyoBehaviour.Instance.ScenarioInterceptDone();
        }

        public override void Terminate()
        {
            base.Terminate();
            scenarioTextData?.Done();
        }
    }
}