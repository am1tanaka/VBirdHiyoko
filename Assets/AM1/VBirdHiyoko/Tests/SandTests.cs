using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.VBirdHiyoko;

public class SandTests
{
    [UnityTest]
    public IEnumerator SandTestsWithEnumeratorPasses()
    {
        yield return AM1TestUtil.StartStage("Stage00");

        // 手前まで移動
        Vector3 target = new Vector3(0, 0, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "手前まで移動");

        var col = AM1TestUtil.GetObject(new Vector3(0, 0.5f, -2), LayerMask.GetMask("Block"));
        Assert.That(col, Is.Null, "ブロックが沈んだ確認");

        // 押す
        target.Set(1, 1.5f, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "押す");
        col = AM1TestUtil.GetObject(new Vector3(0, 0.5f, -3), LayerMask.GetMask("Block"));
        Assert.That(col, Is.Null, "ブロックが沈んだ確認");

        // 草の手前まで
        target.Set(2, 0.5f, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "草の手前");

        // 草を押す
        target.Set(3, 1.5f, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "草を押す");
        col = AM1TestUtil.GetObject(new Vector3(2, 0.5f, -3), LayerMask.GetMask("Block"));
        Assert.That(col, Is.Null, "落としたブロックが沈んだ確認");

        // ブロックに乗って降りる
        target.Set(4, 0.5f, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "草に乗る");
        target.Set(4, 0.5f, -4);
        yield return AM1TestUtil.ClickAndAssert(target, target, "草から降りる");
        col = AM1TestUtil.GetObject(new Vector3(4, 0.5f, -3), LayerMask.GetMask("Block"));
        Assert.That(col, Is.Not.Null, "草はそのまま");

    }
}
