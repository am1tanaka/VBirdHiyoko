using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 歩数を表示するテキスト
    /// </summary>
    public class StepText : MonoBehaviour
    {
        TextMeshProUGUI stepText;

        /// <summary>
        /// 初期化。シーン状態でAwakeが完了したら呼び出す。
        /// </summary>
        public void Init()
        {
            stepText = GetComponent<TextMeshProUGUI>();

            if ((PiyoBehaviour.Instance != null) && (PiyoBehaviour.Instance.StepCounterInstance != null))
            {
                PiyoBehaviour.Instance.StepCounterInstance.OnChangeTempStep.AddListener(OnChange);
            }
        }

        private void OnDestroy()
        {
            if ((PiyoBehaviour.Instance != null) && (PiyoBehaviour.Instance.StepCounterInstance != null))
            {
                PiyoBehaviour.Instance.StepCounterInstance.OnChangeTempStep.RemoveListener(OnChange);
            }
        }

        void OnChange(int step)
        {
            stepText.text = $"{step:000}";
        }

    }
}