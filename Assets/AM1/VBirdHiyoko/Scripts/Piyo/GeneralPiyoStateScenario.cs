using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 受け取ったシナリオテキストを再生するためのクラス。
    /// </summary>
    public class GeneralPiyoStateScenario : IPiyoStateScenario
    {
        public string ScenarioText { get; private set; }

        public GeneralPiyoStateScenario(string scenarioText)
        {
            ScenarioText = scenarioText;
        }

        public void Done() { }
    }
}