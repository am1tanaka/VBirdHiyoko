#define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;
using AM1.BaseFrame.Assets;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// Test用のGame状態への切り替え
    /// Request > Init > (前のシーンのTerminate) > (シーンの解放待ち) > OnHideScreen > OnAwakeDoneの順にStateChanger.csから呼ばれます。
    /// </summary>
    public class TestSceneStateChanger : SceneStateChangerBase<TestSceneStateChanger>, ISceneStateChanger
    {
        /// <summary>
        /// 読み込んだステージの名前
        /// </summary>
        public string LoadedStageName { get; private set; }

        /// <summary>
        /// リトライの時、true
        /// </summary>
        bool isRetry;

        /// <summary>
        /// テスト用に任意にステージ名を設定する。
        /// </summary>
        public static string StageName;

        /// <summary>
        /// 画面を覆う演出の開始や、この状態に必要なシーンの非同期読み込みの開始など。
        /// </summary>
        public override void Init()
        {
            VBirdHiyokoManager.Log($"Test.Init()");

            // 操作を無効にする
            CommandQueue.ChangeInputMask(CommandInputType.None);

            // プレイヤーを待機状態へ
            PiyoBehaviour.Instance.EnqueueState<PiyoStateStandby>();

            // 画面を覆う
            ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.Fade, Color.black, 0.5f);
            BGMSourceAndClips.Instance.Stop(0.5f);

            // シーンの非同期読み込み開始
            isRetry = LoadedStageName == StageName;
            if (!isRetry)
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
            VBirdHiyokoManager.Log($"Test.OnHideScreen()");

            SceneStateChanger.LoadSceneAsync("Stage", false);
            if (isRetry)
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
        public override IEnumerator OnAwakeDone()
        {
            VBirdHiyokoManager.Log($"Test.OnAwakeDone()");

            // 初期化
            StageBehaviour.Instance.Init();
            CommandQueue.AddChangeListener(CommandInputType.UI, OnChangeCommandUI);
            CommandQueue.AddChangeListener(CommandInputType.Game, OnChangeCommandGame);

            HistoryRecorder.Init();
            HistoryPlayer.Init();
            HistoryObjectRegistrant.RegisterObjects();

            // 画面の覆いを解除
            ScreenTransitionRegistry.StartUncover(0.5f);
            yield return ScreenTransitionRegistry.WaitAll();

            // BGM
            BGMPlayer.Play(BGMPlayer.BGM.Game);
            // ゲームを開始
            GamePlayStateQueue.Instance.Enqueue<GameStatePlay>();
        }

        /// <summary>
        /// 次の状態への切り替えにおいて、画面が隠れた時に呼び出すシーンの終了処理。不要になったシーンの解放などを実装します。
        /// </summary>
        public override void Terminate()
        {
            VBirdHiyokoManager.Log($"Test.Terminate()");

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
            Button3DAnimator.SetEnabledAll(flag);
        }

        /// <summary>
        /// プレイヤー操作の有効無効設定
        /// </summary>
        /// <param name="flag">操作を有効にする時、true</param>
        void OnChangeCommandGame(bool flag)
        {
            if (flag)
            {
                PiyoBehaviour.Instance.ShowPushArrows();
            }
            else
            {
                PiyoBehaviour.Instance.HidePushArrows();
            }
        }
    }
}