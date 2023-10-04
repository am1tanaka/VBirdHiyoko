using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.VBirdHiyoko;
using UnityEditorInternal;

public class PushTests
{
    [UnityTest]
    public IEnumerator RapidPushTests()
    {
        yield return AM1TestUtil.StartStage("Stage00");

        Vector3 target = new Vector3(1, 1.5f, -3);
        yield return AM1TestUtil.Click(target);
        yield return AM1TestUtil.WaitPlayerPosition(new Vector3(0, 0, -3));

        for (int i=0;i<60;i++)
        {
            Debug.Log($"{i}");
            yield return AM1TestUtil.Click(target);

            if (PiyoBehaviour.Instance.IsState<PiyoStatePush>())
            {
                break;
            }

            Assert.That(PiyoBehaviour.Instance.RigidbodyPosition.z, Is.GreaterThan(-3.5f) , "行き過ぎチェック");

            yield return new WaitForFixedUpdate();
        }

        yield return AM1TestUtil.WaitWalkOrPushDone();
        AM1TestUtil.AssertPosition(new Vector3(1, 0, -3), "押し完了");
    }

    [UnityTest]
    public IEnumerator PushStageTests()
    {
        Time.timeScale = 2;

        yield return AM1TestUtil.StartStage("StagePush");

        // 氷1つ
        Vector3 target = new Vector3(0, 0, -2);
        yield return AM1TestUtil.ClickAndAssert(target, target, "氷1歩き");
        target.Set(1, 1.5f, -2);
        yield return AM1TestUtil.ClickAndAssert(target, target, "氷1押す");
        var col = AM1TestUtil.GetObject(new Vector3(2, 0.5f, -2), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, "氷1移動確認");

        // 氷の上を滑らせて止める
        target.Set(0, 0, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "氷の上を滑らす 歩き");
        target.Set(1, 1.5f, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "氷の上を滑らす 押す");
        col = AM1TestUtil.GetObject(new Vector3(4, 1.5f, -3), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, "氷の上を滑らす 移動確認");

        // 滑って止める
        string title = "滑って止める";
        target.Set(0, 0, -4);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -4);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 押す");
        col = AM1TestUtil.GetObject(new Vector3(3, 1.5f, -4), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title} 移動確認");

        // 押せない
        title = "押せない";
        target.Set(0, 0, -5);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -5);
        yield return new WaitUntil(() => PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput);
        col = AM1TestUtil.GetObject(new Vector3(1, 1.5f, -5), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title}");

        // 押せない2
        title = "押せない2";
        target.Set(0, 0, -6);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -6);
        yield return new WaitUntil(() => PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput);
        col = AM1TestUtil.GetObject(new Vector3(1, 1.5f, -6), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title}");

        // 押せない3
        title = "押せない3";
        target.Set(0, 0, -7);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -7);
        yield return new WaitUntil(() => PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput);
        col = AM1TestUtil.GetObject(new Vector3(1, 1.5f, -7), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title}");

        // 押せない4
        title = "押せない4";
        target.Set(0, 0, -8);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -8);
        yield return new WaitUntil(() => PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput);
        col = AM1TestUtil.GetObject(new Vector3(1, 1.5f, -8), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title}");

        // 滑って止める
        title = "滑って止める";
        target.Set(0, 0, -9);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -9);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 押す");
        col = AM1TestUtil.GetObject(new Vector3(2, 1.5f, -9), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title} 移動と停止確認");

        // 更に押す
        title = "更に押す";
        target.Set(2, 1.5f, -9);
        yield return AM1TestUtil.Click(target);
        yield return new WaitUntil(() => PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput);
        col = AM1TestUtil.GetObject(new Vector3(2, 1.5f, -9), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title} 移動と停止確認");

        // 草での停止
        title = "草での停止";
        target.Set(0, 0f, -10);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -10);
        yield return AM1TestUtil.Click(target);
        yield return new WaitUntil(() => PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput);
        col = AM1TestUtil.GetObject(new Vector3(1, 1.5f, -10), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title} 移動と停止確認");

        // 砂岩での停止
        title = "砂岩での停止";
        target.Set(0, 0f, -11);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -11);
        yield return AM1TestUtil.Click(target);
        yield return new WaitUntil(() => PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput);
        col = AM1TestUtil.GetObject(new Vector3(1, 1.5f, -11), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title} 移動と停止確認");

        // 離れた草での停止
        title = "離れた草での停止";
        target.Set(0, 0f, -12);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -12);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 押し");
        col = AM1TestUtil.GetObject(new Vector3(2, 1.5f, -12), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title} 移動と停止確認");

        // 離れた砂岩での停止
        title = "離れた砂岩での停止";
        target.Set(0, 0f, -13);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -13);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 押し");
        col = AM1TestUtil.GetObject(new Vector3(2, 1.5f, -13), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title} 移動と停止確認");

        // 離れた氷での停止
        title = "離れた氷での停止";
        target.Set(0, 0f, -14);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 歩き");
        target.Set(1, 1.5f, -14);
        yield return AM1TestUtil.ClickAndAssert(target, target, $"{title} 押し");
        col = AM1TestUtil.GetObject(new Vector3(2, 1.5f, -14), MoveBlockBase.BlockLayer);
        Assert.That(col, Is.Not.Null, $"{title} 移動と停止確認");


        yield return new WaitForSeconds(2);
    }

    [UnityTest]
    public IEnumerator PushTestsWithEnumeratorPasses()
    {
        yield return AM1TestUtil.StartStage("Stage00");

        // 移動
        Vector3 target = new Vector3(0, 0, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "ブロック横まで移動");

        // 押す
        target = new Vector3(1, 1.5f, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "Sand押す");

        // 押したブロック上へ移動
        target = new Vector3(2, 0, -3);
        yield return AM1TestUtil.ClickAndAssert(target, target, "押した後へ移動");

        // 氷前へ
        target = new Vector3(3, 1.5f, -5);
        yield return AM1TestUtil.ClickAndAssert(target, new Vector3(3, 0, -4), "氷の隣へ");

        // 氷を押す
        target = new Vector3(3, 1.5f, -5);
        yield return AM1TestUtil.ClickAndAssert(target, new Vector3(3, 0, -5), "氷押し");

        // 氷の座標
        target.Set(3, 0.5f, -7);
        var col = AM1TestUtil.GetObject(target, LayerMask.GetMask("Block"));
        Assert.That(col, Is.Not.Null, "氷移動確認");

        yield return new WaitForSeconds(1);
    }
}
