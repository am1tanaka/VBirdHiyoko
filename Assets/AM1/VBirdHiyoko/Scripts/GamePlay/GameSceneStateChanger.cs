using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;
using AM1.BaseFrame.Assets;
using AM1.CommandSystem;
using AM1.VBirdHiyoko;
using AM1.MessageSystem;

/// <summary>
/// Game状態への切り替え
/// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDoneの順にStateChanger.csから呼ばれます。
/// </summary>
public class GameSceneStateChanger : SceneStateChangerBase<GameSceneStateChanger>, ISceneStateChanger
{
    /// <summary>
    /// 読み込んだステージの名前
    /// </summary>
    public string LoadedStageName { get; private set; }

    /// <summary>
    /// TODO 現在のステージ番号から該当するステージ名を返す
    /// </summary>
    static string StageName = "Stage01";

    /// <summary>
    /// タイトルからスタートの時、true
    /// </summary>
    bool isStartTitle;

    /// <summary>
    /// 画面を覆う演出の開始や、この状態に必要なシーンの非同期読み込みの開始など。
    /// </summary>
    public override void Init()
    {
        // 操作を無効にする
        CommandQueue.ChangeInputMask(CommandInputType.None);

        // プレイヤーを待機状態へ
        PiyoBehaviour.Instance.Enqueue(PiyoBehaviour.Instance.GetInstance<PiyoStateStandby>());

        // ウィンドウを閉じる
        MessageWindow.Instance.CloseAll();

        // 画面を覆う
        ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.Fade, Color.black, 0.5f);
        BGMSourceAndClips.Instance.Stop(0.5f);

        // シーンの非同期読み込み開始
        isStartTitle = (SceneStateChanger.CurrentState == TitleSceneStateChanger.Instance);
        if (isStartTitle)
        {
            SceneStateChanger.LoadSceneAsync("Game", true);
            SceneStateChanger.LoadSceneAsync(StageName, true);
            LoadedStageName = StageName;
        }
    }

    /// <summary>
    /// 画面が覆われて、不要なシーンの解放が完了した時に行いたい処理を実装します。シーンの読み直しなど。
    /// </summary>
    public override void OnHideScreen()
    {
        SceneStateChanger.LoadSceneAsync("Stage", false);
        if (!isStartTitle)
        {
            SceneStateChanger.LoadSceneAsync("Game", false);
            SceneStateChanger.LoadSceneAsync(StageName, false);
            LoadedStageName = StageName;
        }
    }

    /// <summary>
    /// 画面が覆われていて、全てのシーンが読み込まれてAwakeが実行された後に呼ばれます。<br></br>
    /// フェードインなどの状態を始めるための処理を実装します。
    /// </summary>
    public override IEnumerator OnAwakeDone() {
        // 初期化
        StageBehaviour.Instance.Init();
        CommandQueue.AddChangeListener(CommandInputType.UI, OnChangeCommandUI);
        CommandQueue.AddChangeListener(CommandInputType.Game, OnChangeCommandGame);
        StageScenarioBehaviour.Init();
        GamePlayStateQueue.Instance.Init();

        HistoryRecorder.Init();
        HistoryPlayer.Init();
        HistoryObjectRegistrant.RegisterObjects();
        ushort innerStep;
        if (HistoryRecorder.Load(VBirdHiyokoManager.CurrentStage.Current, out innerStep))
        {
            PiyoBehaviour.Instance.StepCounterInstance.SetInnerStep(innerStep);
        }

        // 画面の覆いを解除
        ScreenTransitionRegistry.StartUncover(0.5f);
        yield return ScreenTransitionRegistry.WaitAll();

        // BGM
        BGMPlayer.Play(BGMPlayer.BGM.Game);
        // ゲームを開始
        GamePlayStateQueue.Instance.Enqueue(GamePlayStateQueue.Instance.GetInstance<GameStatePlay>());
    }

    /// <summary>
    /// 次の状態への切り替えにおいて、画面が隠れた時に呼び出すシーンの終了処理。不要になったシーンの解放などを実装します。
    /// </summary>
    public override void Terminate() {
        // 登録解除
        CommandQueue.RemoveChangeListener(CommandInputType.UI, OnChangeCommandUI);
        CommandQueue.RemoveChangeListener(CommandInputType.Game, OnChangeCommandGame);

        // シーンの解放
        SceneStateChanger.UnloadSceneAsync("Game");
        SceneStateChanger.UnloadSceneAsync("Clear");
        SceneStateChanger.UnloadSceneAsync(LoadedStageName);
        LoadedStageName = "";
    }

    /// <summary>
    /// UIコマンドの有効、無効の切り替え時に呼び出される処理。
    /// </summary>
    /// <param name="flag">操作を有効にする時、true</param>
    void OnChangeCommandUI(bool flag)
    {
        Debug.Log($"OnChangeCommandUI({flag})");
    }

    /// <summary>
    /// ゲーム操作の有効無効設定
    /// </summary>
    /// <param name="flag">操作を有効にする時、true</param>
    void OnChangeCommandGame(bool flag)
    {
        Debug.Log($"OnChangeCommandGame({flag})");
    }

}
