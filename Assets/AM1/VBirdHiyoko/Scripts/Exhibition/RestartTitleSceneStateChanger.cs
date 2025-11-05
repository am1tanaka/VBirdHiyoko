using AM1.BaseFrame;
using AM1.BaseFrame.Assets;
using AM1.VBirdHiyoko;
using System.Collections;
using AM1.CommandSystem;

/// <summary>
/// クリアデータを削除して、Title状態への切り替え
/// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDoneの順にStateChanger.csから呼ばれます。
/// </summary>
public class RestartTitleSceneStateChanger : SceneStateChangerBase<RestartTitleSceneStateChanger>, ISceneStateChanger
{
    /// <summary>
    /// 画面を覆う演出の開始や、この状態に必要なシーンの非同期読み込みの開始など。
    /// </summary>
    public override void Init()
    {
        // 通常の画面を覆う
        ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.FilledRadial, 0.5f);
        BGMSourceAndClips.Instance.Stop(0.5f);
    }

    /// <summary>
    /// 画面が覆われて、不要なシーンの解放が完了した時に行いたい処理を実装します。シーンの読み直しなど。
    /// </summary>
    public override void OnHideScreen()
    {
        // シーンの非同期読み込み開始
        SceneStateChanger.LoadSceneAsync("Title", true);
        SceneStateChanger.LoadSceneAsync("StageTitle", true);
        SceneStateChanger.LoadSceneAsync("Stage", true);
    }

    /// <summary>
    /// 画面が覆われていて、全てのシーンが読み込まれてAwakeが実行された後に呼ばれます。<br></br>
    /// フェードインなどの状態を始めるための処理を実装します。
    /// </summary>
    public override IEnumerator OnAwakeDone() {
        // 初期化
        StageBehaviour.Instance.Init();
        StageScenarioBehaviour.Init();

        // 画面の覆いを解除
        ScreenTransitionRegistry.StartUncover(0.5f);
        yield return ScreenTransitionRegistry.WaitAll();

        // TitleBGM
        BGMPlayer.Play(BGMPlayer.BGM.Title);
        TitleBehaviour.Instance.ChangeState(TitleBehaviour.State.Play);

        // 入力受付開始
        CommandQueue.ChangeInputMask(CommandInputType.Everything);
    }

    /// <summary>
    /// 次の状態への切り替えにおいて、画面が隠れた時に呼び出すシーンの終了処理。不要になったシーンの解放などを実装します。
    /// </summary>
    public override void Terminate() {
        // シーンの解放
        SceneStateChanger.UnloadSceneAsync("Title");
        SceneStateChanger.UnloadSceneAsync("StageTitle");
        SceneStateChanger.UnloadSceneAsync("Stage");
        SceneStateChanger.UnloadSceneAsync("Credits");
    }
}
