using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.MessageSystem;
using AM1.VBirdHiyoko;

public class Stage01Tests
{
    [UnityTest]
    public IEnumerator Stage01ClearTests()
    {
        StaticInitializer.Init();

        yield return AM1TestUtil.StartStage(1);

        Time.timeScale = 4;

        // 最初のメッセージを確認
        yield return AM1TestUtil.WaitShowMessage("最初のメッセージ");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.Stage01Start)));

        // 行けない
        yield return AM1TestUtil.WaitCanAction();
        var target = new Vector3(7,0.5f,-5);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitChangeMessage("行けない");
        Assert.That(MessageWindow.Instance.MessageText,
            Is.EqualTo(Messages.GetMessage(Messages.Type.CantGo)));

        // 移動
        target.Set(2,1.5f,-4);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkDone("移動");
        yield return AM1TestUtil.IsPlayerPosition(new Vector3(2,1,-3));

        // 押す
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkOrPushDone("押す");
        yield return AM1TestUtil.IsPlayerPosition(target);

        // 重い
        target.Set(2,1.5f,-5);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitShowMessage("重い");
        Assert.That(MessageWindow.Instance.MessageText,
            Is.EqualTo(Messages.GetMessage(Messages.Type.TooHeavy)));

        // 回り込む
        target.Set(1, 0.5f, -5);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkDone("押す");
        yield return AM1TestUtil.IsPlayerPosition(target);

        // 押す
        target.Set(2, 1.5f, -5);
        yield return AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkOrPushDone("押す2");

        // ブロック前へ移動
        target.Set(4, 1.5f, -5);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkOrPushDone("ブロック前へ移動");

        // 重い2
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitShowMessage("重い2");
        Assert.That(MessageWindow.Instance.MessageText,
            Is.EqualTo(Messages.GetMessage(Messages.Type.TooHeavy)));

        // 回り込む
        target.Set(4,0.5f,-6);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkOrPushDone("回り込む2");

        // 押す3
        target.Set(4, 1.5f, -5);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkOrPushDone("押す3");

        // 押す4
        target.Set(5, 1.5f, -5);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitWalkOrPushDone("押す4");

        Time.timeScale = 1;

        // クリア確認
        target.Set(8, 0.5f, -5);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitPlayerPosition(new Vector3(7,1,-5));
        yield return AM1TestUtil.WaitShowMessage("クリア確認");
        Assert.That(MessageWindow.Instance.MessageText,
                       Is.EqualTo(Messages.GetMessage(Messages.Type.Stage01Clear)));
        yield return AM1TestUtil.WaitPlayerPosition(new Vector3(9, 0.5f, -5));
    }
}
