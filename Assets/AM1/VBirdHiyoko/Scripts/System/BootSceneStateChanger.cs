using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;
using AM1.BaseFrame.Assets;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 各種初期化や起動処理を行う。
    /// </summary>
    public class BootSceneStateChanger : SceneStateChangerBase<BootSceneStateChanger>, ISceneStateChanger
    {
        public override void Init()
        {
            // 画面を覆う
            ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.FilledRadial);
        }

        public override void OnHideScreen()
        {
            // ボリューム初期化
            new VolumeSetting((int)VolumeType.BGM, new BGMVolumeSaverWithPlayerPrefs());
            BGMSourceAndClips.Instance.SetVolumeSetting(VolumeSetting.volumeSettings[(int)VolumeType.BGM]);
            new VolumeSetting((int)VolumeType.SE, new SEVolumeSaverWithPlayerPrefs());
            SESourceAndClips.Instance.SetVolumeSetting(VolumeSetting.volumeSettings[(int)VolumeType.SE]);
            VolumeSlider.initEvents.Invoke();

            // 遅延再生初期化
            SESourceAndClips.Instance.InitDelaySEPlayer(System.Enum.GetValues(typeof(SEPlayer.SE)).Length, SEPlayer.DelaySeconds, SEPlayer.DelayMax);

            // タイトルシーンを起動
            TitleSceneStateChanger.Instance.RequestFrom(TitleSceneStateChanger.FromState.Boot, true);
        }
    }
}