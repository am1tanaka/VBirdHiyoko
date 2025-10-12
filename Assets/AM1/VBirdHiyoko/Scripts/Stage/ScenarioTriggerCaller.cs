using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 親のScenarioTriggerに、OnTriggerを知らせる。
    /// </summary>
    public class ScenarioTriggerCaller : MonoBehaviour
    {
        ScenarioTrigger scenarioTrigger;

        private void Awake()
        {
            scenarioTrigger = GetComponentInParent<ScenarioTrigger>();
        }

        private void OnTriggerEnter(Collider other)
        {
            scenarioTrigger?.OnTriggerEnter(other);
        }
    }
}
