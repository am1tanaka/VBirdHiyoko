using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.MessageSystem;
using AM1.VBirdHiyoko;

public class Stage05Tests
{
    [SetUp]
    public void SetUp()
    {
        StaticInitializer.Init();
        Time.timeScale = 2;
    }

    [UnityTest]
    public IEnumerator Stage05ClearTests()
    {
        yield return AM1TestUtil.StartStage(5);

        // 最初のメッセージを確認
        yield return AM1TestUtil.WaitShowMessage("最初のメッセージ");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.Stage05Start)));

        // 移動
        var target = new Vector3(0, 1.5f, -3);
        yield return AM1TestUtil.Click(target);

        // つめたいメッセージ
        yield return AM1TestUtil.WaitChangeMessage("つめたいメッセージ");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.Ice01)));
        MessageWindow.Instance.CloseAll();

        // 押す
        yield return AM1TestUtil.WaitCanAction("おす");
        yield return AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkOrPushDone("押す");

        // 滑る
        yield return AM1TestUtil.WaitShowMessage("滑る");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.Ice02)));
    }

    [UnityTest]
    public IEnumerator Stage05MissTests()
    {
        yield return AM1TestUtil.StartStage(5);

        // 移動
        var target = new Vector3(0, 1.5f, -3);
        yield return AM1TestUtil.Click(target);

        // つめたいメッセージ
        yield return AM1TestUtil.WaitChangeMessage("つめたいメッセージ");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.Ice01)));
        MessageWindow.Instance.CloseAll();

        // 押す
        yield return AM1TestUtil.WaitCanAction("おす");
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkOrPushDone("押す");

        // 滑る
        yield return AM1TestUtil.WaitShowMessage("滑る");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.Ice02)));
        yield return AM1TestUtil.WaitCanAction("移動");

        // 手前
        target.Set(2, 0.5f, -5);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkDone("手前");

        // 何かにぶつけて止めよう
        target.Set(3, 1.5f, -5);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitShowMessage("何かにぶつけて止めよう");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.IceMiss)));

        // トリガーが消えていることを確認
        var res = AM1TestUtil.GetObject(new Vector3(1, 0.5f, -5), LayerMask.NameToLayer("Trigger"));
        Assert.That(res, Is.Null, "トリガーが消えていることを確認");

        // 詰み表示
        yield return AM1TestUtil.WaitChangeMessage("詰み表示");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.Tsumi)));
    }
}
