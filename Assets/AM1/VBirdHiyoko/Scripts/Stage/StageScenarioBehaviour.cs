using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        /// <summary>
        /// シーンの読み込みが完了した後に呼び出して、開始シナリオの設定処理を実行する。
        /// </summary>
        public static void Init()
        {
            Debug.Log("未実装");
        }

        public void Done() { }
    }
}