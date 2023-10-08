using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.VBirdHiyoko;
using AM1.MessageSystem;

public class Stage03Tests
{

    [UnityTest]
    public IEnumerator Stage03testsWithEnumeratorPasses()
    {
        StaticInitializer.Init();
        Time.timeScale = 2;

        yield return AM1TestUtil.StartStage(3);

        // 最初のメッセージ
        yield return AM1TestUtil.WaitShowMessage("最初のメッセージ");
        Assert.That(MessageWindow.Instance.MessageText,
            Is.EqualTo(Messages.GetMessage(Messages.Type.Stage03Start)));

        // 最初の移動
        yield return AM1TestUtil.WaitCanAction("移動");
        Vector3 target = new Vector3(0, 0.5f, -3);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitChangeMessage("最初の移動");
        Assert.That(MessageWindow.Instance.MessageText
            , Is.EqualTo(Messages.GetMessage(Messages.Type.Sand01)));
        MessageWindow.Instance.CloseAll();

        // 移動続き
        yield return AM1TestUtil.WaitWalkDone("移動続き", 5);
        AM1TestUtil.IsPlayerPosition(target);

        // 押す
        yield return AM1TestUtil.WaitCanAction("押す");
        target.Set(1, 1.5f, -3);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitCanAction("押す");

        // 乗る
        target.Set(3, 1.5f, -3);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitShowMessage("乗る");
        Assert.That(MessageWindow.Instance.MessageText
                       , Is.EqualTo(Messages.GetMessage(Messages.Type.Sand02)));

        // 降りて詰み
        yield return AM1TestUtil.WaitCanAction();
        target.Set(1, 0.5f, -3);
        AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitChangeMessage("降りて詰み");
        Assert.That(MessageWindow.Instance.MessageText
                       , Is.EqualTo(Messages.GetMessage(Messages.Type.Tsumi)));
        yield return AM1TestUtil.WaitCanAction("降りて詰み");
    }
}
