using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace AM1.BaseFrame.Assets
{
    /// <summary>
    /// ボリューム用のスライダーにアタッチする。
    /// 変更時の登録はInspectorは利用せずスクリプトで設定する。
    /// VolumeSettingに依存。
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        [Tooltip("このスライダーが担当するボリュームの種類"), SerializeField]
        VolumeType volumeType = default;

        /// <summary>
        /// ボリュームスライダーのリスト
        /// </summary>
        public static readonly UnityEvent initEvents = new UnityEvent();

        Slider slider;
        VolumeSetting currentVolumeSetting;

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Init();
            }
            else
            {
                initEvents.AddListener(Init);
            }
        }

        void Init()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnChangeValue);

            // VolumeSettingの初期化は起動処理で実行済みで、
            // この処理が呼ばれるのはタイトルシーンを読み込んでからなので、
            // タイミング上は必ずインスタンスが存在する。
#if UNITY_EDITOR
            if (VolumeSetting.volumeSettings.Count < (int)volumeType + 1)
            {
                Debug.LogWarning($"{volumeType}のVolumeSettingが初期化されていません。");
                return;
            }
#endif
            currentVolumeSetting = VolumeSetting.volumeSettings[(int)volumeType];
            slider.value = currentVolumeSetting.Volume;

            initEvents.RemoveListener(Init);
        }

        private void OnDestroy()
        {
            if (slider != null)
            {
                slider.onValueChanged.RemoveListener(OnChangeValue);
            }
        }

        /// <summary>
        /// スライダーの変更を反映
        /// </summary>
        /// <param name="newval">新しい値</param>
        void OnChangeValue(float newval)
        {
            int newValInt = Mathf.RoundToInt(newval);
            if (newValInt != currentVolumeSetting.Volume)
            {
                currentVolumeSetting.ChangeVolume(newValInt);
            }
        }
    }
}