using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;
using AM1.BaseFrame.Assets;

/// <summary>
/// Title状態への切り替え
/// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDoneの順にStateChanger.csから呼ばれます。
/// </summary>
public class TitleSceneStateChanger : SceneStateChangerBase<TitleSceneStateChanger>, ISceneStateChanger
{
    /// <summary>
    /// 遷移元の状態
    /// </summary>
    public enum FromState
    {
        Boot,
        Game,
        Ending
    }
    public FromState From { get; private set; }

    /// <summary>
    /// 移行元の状態を受け取って、切り替えをリクエストします。
    /// </summary>
    /// <param name="from">移行元の状態</param>
    /// <param name="isQueue">積む時、true</param>
    /// <returns>リクエストに成功したら true</returns>
    public bool RequestFrom(FromState from, bool isQueue)
    {
        bool result = SceneStateChanger.ChangeRequest(this, isQueue);
        if (result)
        {
            From = from;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 画面を覆う演出の開始や、この状態に必要なシーンの非同期読み込みの開始など。
    /// </summary>
    public override void Init()
    {
        if (From != FromState.Ending)
        {
            // 通常の画面を覆う
            ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.FilledRadial, 0.5f);
            BGMSourceAndClips.Instance.Stop(0.5f);
        }
        else
        {
            //　エンディングからの復帰
            ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.Fade, 2f);
            BGMSourceAndClips.Instance.Stop(2);
        }

        // シーンの非同期読み込み開始
        SceneStateChanger.LoadSceneAsync("Title", true);
        SceneStateChanger.LoadSceneAsync("StageTitle", true);
        SceneStateChanger.LoadSceneAsync("Stage", true);
    }

    /// <summary>
    /// 画面が覆われて、不要なシーンの解放が完了した時に行いたい処理を実装します。シーンの読み直しなど。
    /// </summary>
    public override void OnHideScreen()
    {
        
    }

    /// <summary>
    /// 画面が覆われていて、全てのシーンが読み込まれてAwakeが実行された後に呼ばれます。<br></br>
    /// フェードインなどの状態を始めるための処理を実装します。
    /// </summary>
    public override IEnumerator OnAwakeDone() {
        // 画面の覆いを解除
        ScreenTransitionRegistry.StartUncover(0.5f);
        yield return ScreenTransitionRegistry.WaitAll();

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
