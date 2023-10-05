using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.MessageSystem;
using AM1.VBirdHiyoko;

public class IceBlockOverTests
{
    [UnityTest]
    public IEnumerator IceBlockOverTestsWithEnumeratorPasses()
    {
        Time.timeScale = 5;

        yield return AM1TestUtil.StartStage("TestStage01");

        // 最初のメッセージを確認
        //yield return AM1TestUtil.WaitShowMessage("最初のメッセージ");
        //Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.Stage05Start)));

        // 移動
        var target = new Vector3(3, 0.5f, -6);
        yield return AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitCanAction("移動", 10);

        // 氷押す
        target.Set(3, 1.5f, -5);
        yield return AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitCanAction("氷押す");

        // 奥に移動
        target.Set(4, 1.5f, -5);
        yield return AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitCanAction("氷押す");

        // 奥に移動2
        target.Set(4, 0.5f, -4);
        yield return AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitCanAction("氷押す");

        // 氷押す2
        target.Set(3, 1.5f, -4);
        yield return AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitCanAction("氷押す2");

        // 氷があることを確認
        var ice = AM1TestUtil.GetObject(new Vector3(2, 1.5f, -4), LayerMask.GetMask("Block"));
        Assert.That(ice, Is.Not.Null, "氷がある");
    }
}
