using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ステージ開始時のシナリオ処理を実行する
    /// </summary>
    public class StageScenarioBehaviour : MonoBehaviour, IPiyoStateScenario
    {
        [Tooltip("ステージ開始時のシナリオ"), SerializeField]
        TextAsset startScenario = default;
        [Tooltip("ステージ開始時の自動発話データ"), SerializeField]
        AutoTalkerData defaultAutoTalkerData = default;

        public string ScenarioText => startScenario.text;

        PiyoStateScenario playerStateScenario = new PiyoStateScenario();
        AutoTalker autoTalker;

        private void OnDestroy()
        {
            if ((defaultAutoTalkerData != null) && autoTalker)
            {
                CommandQueue.RemoveChangeListener(CommandInputType.Game, autoTalker.SetTimerActive);
                autoTalker.SetTimerActive(false);
                autoTalker = null;
            }
        }

        /// <summary>
        /// シーンの読み込みが完了した後に呼び出して、開始シナリオの設定処理を実行する。
        /// </summary>
        public static void Init()
        {
            var instance = FindObjectOfType<StageScenarioBehaviour>();
            if (instance)
            {
                instance.SetStartScenario();
            }
        }

        void SetStartScenario()
        {
            if (startScenario != null)
            {
                playerStateScenario.SetSource(this);
            }

            if (startScenario != null)
            {
                PiyoBehaviour.Instance.AddScenario(playerStateScenario);
            }

            if (defaultAutoTalkerData == null) { return; }
            if (autoTalker == null)
            {
                autoTalker = GameObject.FindObjectOfType<AutoTalker>();
            }
            if (autoTalker != null)
            {
                autoTalker.SetTalkData(defaultAutoTalkerData);
                CommandQueue.AddChangeListener(CommandInputType.Game, autoTalker.SetTimerActive);
            }
        }

        public void Done() { }
    }
}