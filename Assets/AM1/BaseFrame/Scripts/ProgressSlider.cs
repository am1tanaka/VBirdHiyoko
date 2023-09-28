using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System.Resources;
using AM1.BaseFrame;

namespace AM1.VoxelorerBird
{
    /// <summary>
    /// シーン切り替えなどの進捗表示
    /// </summary>
    public class ProgressSlider : MonoBehaviour
    {
        public static ProgressSlider Instance { get; private set; } = null;

        const float BrinkFrame = 0.25f;
        const int BrinkMax = 3;

        UnityEngine.UI.Slider slider = default;
        TextMeshProUGUI loadingText = default;
        CanvasGroup canvasGroup = default;

        private void Awake()
        {
            Instance = this;
            slider = GetComponent<UnityEngine.UI.Slider>();
            loadingText = GetComponentInChildren<TextMeshProUGUI>();
            canvasGroup = GetComponent<CanvasGroup>();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 進捗表示を開始します
        /// </summary>
        public void StartProgress()
        {
            slider.value = 0f;
            canvasGroup.alpha = 1;
            gameObject.SetActive(true);
        }

        public void EndProgress()
        {
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// スライダーの進捗を設定します
        /// </summary>
        /// <param name="val"></param>
        public void SetValue(float val)
        {
            slider.value = Mathf.Clamp01(val);
        }

        private void Update()
        {
            SetValue(Mathf.Clamp01((float)SceneStateChanger.AsyncProgress/100f));
            var dots = (int)(Mathf.Repeat(Mathf.RoundToInt(Time.realtimeSinceStartup / BrinkFrame), BrinkMax + 1));
            loadingText.text = "Now Loading" + "...".Substring(0, dots);
        }
    }
}