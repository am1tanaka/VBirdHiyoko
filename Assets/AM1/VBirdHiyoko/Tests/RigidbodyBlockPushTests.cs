using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RigidbodyBlockPushTests
{
    [UnityTest]
    public IEnumerator NormalBlockPushTestsWithEnumeratorPasses()
    {
        yield return AM1TestUtil.StartStage("TestStageNormalBlock");
        yield return AM1TestUtil.WaitCanAction();
        Time.timeScale = 2;

        // 草を押す
        var target = new Vector3(0, 1.5f, -5);
        yield return AM1TestUtil.ClickAndAssert(target, target, "草");

        // 氷を押す
        target.x++;
        yield return AM1TestUtil.ClickAndAssert(target, target, "氷");

        // 砂を押す
        target.z--;
        yield return AM1TestUtil.ClickAndAssert(target, target, "砂");

        // 氷2を押す
        target.x++;
        yield return AM1TestUtil.ClickAndAssert(target, target, "氷2");

        // 氷3を押す
        target.z+=2;
        yield return AM1TestUtil.ClickAndAssert(target, target, "氷3");

        // 終了待ち
        yield return new WaitForSeconds(1);
    }
}
