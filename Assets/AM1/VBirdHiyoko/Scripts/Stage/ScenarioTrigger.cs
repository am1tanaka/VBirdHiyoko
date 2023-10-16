using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ステージに指定のタグのオブジェクトが通りかかったらシナリオをプレイヤーに登録する。
    /// </summary>
    public class ScenarioTrigger : MonoBehaviour, IPiyoStateScenario
    {
        [Tooltip("対象のタグ"), SerializeField]
        Type targetTag = Type.Player;
        [Tooltip("処理後に無効化する時、チェック"), SerializeField]
        bool isDoneOff = true;
        [Tooltip("シナリオテキスト"), SerializeField]
        TextAsset scenarioText = default;

        [System.Serializable]
        public enum Type
        {
            Player,
            Block,
        }

        /// <summary>
        /// シナリオの文字列
        /// </summary>
        public string ScenarioText => scenarioText.text;

        PiyoStateScenario piyoStateScenario;
        bool isOn;

        private void Awake()
        {
            if (scenarioText == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"シナリオテキスト未設定 {transform.position} {gameObject.scene.name}");
                gameObject.SetActive(false);
#endif
            }
            else
            {
                piyoStateScenario = new ();
                piyoStateScenario.SetSource(this);
                isOn = true;
            }
        }

        void OnDestroy()
        {
            Done();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag.ToString()))
            {
                PiyoBehaviour.Instance.AddScenario(piyoStateScenario);
            }
        }

        public void Done()
        {
            if (isOn && isDoneOff)
            {
                isOn = false;
                gameObject?.SetActive(false);
            }
        }
    }
}