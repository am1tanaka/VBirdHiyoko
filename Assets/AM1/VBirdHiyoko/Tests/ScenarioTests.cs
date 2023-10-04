using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.ScenarioSystem;
using AM1.BaseFrame;
using UnityEngine.SceneManagement;
using AM1.VBirdHiyoko;
using AM1.MessageSystem;
using AM1.CommandSystem;

public class ScenarioTests
{
    [UnityTest]
    public IEnumerator StartScenarioTests()
    {
        AM1TestUtil.SetGameDataTestAndClear();
        yield return AM1TestUtil.StartTitle();
        yield return new WaitUntil(() => CommandQueue.CurrentInputMask.HasFlag(CommandInputType.Game));

        // 隣へ
        AM1TestUtil.Click(new Vector3(1, 0.5f, 0));
        yield return AM1TestUtil.WaitWalkDone("隣へ歩き");

        // セリフ確認
        yield return AM1TestUtil.WaitShowMessage("押せそうセリフ待ち");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.CanPush)));

        // 押す
        AM1TestUtil.Click(new Vector3(2, 1.5f, 0));
        yield return AM1TestUtil.WaitWalkOrPushDone("押す");

        // メッセージをクリックで閉じる
        yield return new WaitUntil(() => MessageWindow.Instance.CurrentState == MessageWindow.State.Show);
        MessageWindow.Instance.Close();

        // ここに入ってみよう
        AM1TestUtil.Click(new Vector3(2, 0.5f, 1));
        yield return AM1TestUtil.WaitChangeMessage();
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.TutorialLetsGo)));

        // メッセージ閉じ待ち
        yield return AM1TestUtil.WaitCloseMessage("メッセージ閉じ待ち", 5);
    }

    [UnityTest]
    public IEnumerator AutoTalkTests()
    {
        SceneStateChanger.ResetStatics();
        yield return AM1TestUtil.StartStage();
        yield return null;

        // 準備
        var scenarioPlayer = GameObject.FindObjectOfType<ScenarioPlayer>();
        Assert.That(scenarioPlayer, Is.Not.Null);
        var autoTalker = GameObject.FindObjectOfType<AutoTalker>();
        Assert.That(autoTalker, Is.Not.Null);
        autoTalker.SetAutoTalkPlayer(new AutoTalkScenario(new ShowMessage()));
        autoTalker.SetTimerActive(true);

        // データ未設定なので何も起きない
        yield return new WaitForSeconds(2);
        Assert.That(scenarioPlayer.IsQueuingOrPlaying, Is.False);

        //　データ作成
        var tutorialTalkData = ScriptableObject.CreateInstance<AutoTalkerData>();
        tutorialTalkData.Set(false, 0.5f, 1f, new TalkData[]
        {
            new TalkData("TutorialClick", 0.5f),
            new TalkData("TutorialWind", 0.5f),
            new TalkData("TutorialWhere", 0.5f)
        });
        autoTalker.SetTalkData(tutorialTalkData);

        // 最初の発話チェック
        yield return new WaitForSeconds(1);
        Assert.That(scenarioPlayer.IsQueuingOrPlaying, Is.True);
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.TutorialClick)));
        yield return AM1TestUtil.WaitChangeMessage();
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.TutorialWind)));
        yield return AM1TestUtil.WaitChangeMessage();
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.TutorialWhere)));
        yield return new WaitForSeconds(2);

        // 閉じチェック
        Assert.That(MessageWindow.Instance.IsShowing, Is.False);
        yield return new WaitForSeconds(1);
    }

    [UnityTest]
    public IEnumerator ScenarioFundamentalTests()
    {
        var scenarioPlayer = new GameObject("ScenarioPlayer").AddComponent<ScenarioPlayer>();

        yield return null;
        Assert.That(ScenarioPlayer.Instance, Is.Not.Null);

        // ない時のみ登録
        var testScenarioPlayer = new TestScenarioPlayer();
        var tests = new TestScenarioPlayer[]
        {
            new TestScenarioPlayer(),
            new TestScenarioPlayer(),
        };
        ScenarioPlayer.Instance.EnqueueNotExists(testScenarioPlayer);
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(1));
        Assert.That(testScenarioPlayer.isDenied, Is.False);

        // 登録できない
        ScenarioPlayer.Instance.EnqueueNotExists(testScenarioPlayer);
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(1));
        Assert.That(testScenarioPlayer.isDenied, Is.True);
        ScenarioPlayer.Instance.EnqueueNotExists(tests);
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(1));
        Assert.That(tests[0].isDenied, Is.True);
        Assert.That(tests[1].isDenied, Is.True);

        // 実行
        yield return null;
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(0));
        Assert.That(ScenarioPlayer.Instance.IsPlaying, Is.True);
        yield return null;
        Assert.That(ScenarioPlayer.Instance.IsPlaying, Is.True);

        // 終了
        testScenarioPlayer.isDone = true;
        yield return null;
        Assert.That(ScenarioPlayer.Instance.IsQueuingOrPlaying, Is.False);

        // 複数シナリオ登録
        ScenarioPlayer.Instance.EnqueueNotExists(tests);
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(2));
        // シナリオ実行
        yield return null;
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(1));
        tests[0].isDone = true;
        yield return null;
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(0));
        yield return null;
        Assert.That(ScenarioPlayer.Instance.IsQueuingOrPlaying, Is.True);
        tests[1].isDone = true;
        yield return null;
        Assert.That(ScenarioPlayer.Instance.IsQueuingOrPlaying, Is.False);

        // 通常登録
        ScenarioPlayer.Instance.Enqueue(testScenarioPlayer);
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(1));

        // 同じシナリオは登録できない
        ScenarioPlayer.Instance.Enqueue(testScenarioPlayer);
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(1));

        // 連続登録
        ScenarioPlayer.Instance.Enqueue(tests);
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(3));

        // クリア
        ScenarioPlayer.Instance.ClearQueue();
        Assert.That(ScenarioPlayer.Instance.QueueCount, Is.EqualTo(0));

        yield return new WaitForSeconds(3);
    }


    class TestScenarioPlayer : IScenarioPlayer
    {
        public bool isDone;
        public bool isDenied;

        public IEnumerator Play()
        {
            isDenied = isDone = false;
            while (!isDone)
            {
                yield return null;
            }
        }

        public void DeniedEnqueue()
        {
            isDenied = true;
        }
    }
}
