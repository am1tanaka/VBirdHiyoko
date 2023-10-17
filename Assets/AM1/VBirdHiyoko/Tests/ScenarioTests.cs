using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
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
        StaticInitializer.Init();

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
        StaticInitializer.Init();
        yield return AM1TestUtil.StartStage("TestStage01");
        yield return null;

        Time.timeScale = 4;

        // 準備
        var autoTalker = GameObject.FindObjectOfType<AutoTalker>();
        Assert.That(autoTalker, Is.Not.Null);
        autoTalker.SetTimerActive(true);

        // データ未設定なので何も起きない
        yield return new WaitForSeconds(2f);
        Assert.That(MessageWindow.Instance.IsShowing, Is.False, "データ未設定のため何も起きない");

        //　データ作成
        var tutorialTalkData = ScriptableObject.CreateInstance<AutoTalkerData>();
        tutorialTalkData.Set(false, 0.5f, 1f, new MessageData[]
        {
            new MessageData("TutorialClick", 0.5f),
            new MessageData("TutorialWind", 0.5f),
            new MessageData("TutorialWhere", 0.5f)
        });
        autoTalker.SetTalkData(tutorialTalkData);

        // 最初の発話チェック
        yield return new WaitForSeconds(1);
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
}
