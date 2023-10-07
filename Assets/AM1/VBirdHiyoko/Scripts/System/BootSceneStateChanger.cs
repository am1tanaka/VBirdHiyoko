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
            // staticを初期化
            StaticInitializer.Init();

            // 履歴のストレージを登録
            HistoryRecorder.historyLoader = VBirdHiyokoManager.GetInstance<IHistoryLoader>();
            HistoryRecorder.historySaver = VBirdHiyokoManager.GetInstance<IHistorySaver>();

            // 画面を覆う
            ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.FilledRadial);
        }

        public override void OnHideScreen()
        {
            // クリア済みステージの読み込みと初期設定
            VBirdHiyokoManager.GetInstance<IGameDataStorage>().InitAndLoad();
            VBirdHiyokoManager.CurrentStage.Set(VBirdHiyokoManager.ClearedStage.Current - 1);

            // ボリューム初期化
            new VolumeSetting((int)VolumeType.BGM, new BGMVolumeSaverWithPlayerPrefs());
            BGMSourceAndClips.Instance.SetVolumeSetting(VolumeSetting.volumeSettings[(int)VolumeType.BGM]);
            var seSetting = new VolumeSetting((int)VolumeType.SE, new SEVolumeSaverWithPlayerPrefs());
            SESourceAndClips.Instance.SetVolumeSetting(VolumeSetting.volumeSettings[(int)VolumeType.SE]);
            seSetting.ChangeVolumeEvent.AddListener(() => SEPlayer.Play(SEPlayer.SE.Click));
            VolumeSlider.initEvents.Invoke();

            // 遅延再生初期化
            SESourceAndClips.Instance.InitDelaySEPlayer(System.Enum.GetValues(typeof(SEPlayer.SE)).Length, SEPlayer.DelaySeconds, SEPlayer.DelayMax);

            // タイトルシーンを起動
            TitleSceneStateChanger.Instance.RequestFrom(TitleSceneStateChanger.FromState.Boot, true);
        }
    }
}