using System.Collections;
using System.Collections.Generic;
using AM1.BaseFrame;
using AM1.VBirdHiyoko;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RouteTests
{
    [UnityTest]
    public IEnumerator RockTests()
    {
        yield return AM1TestUtil.StartStage("TestStage01");

        // 1, -5へ歩く
        yield return AM1TestUtil.ClickAndAssert(new Vector3(0, 0.5f, -4), new Vector3(0, 0, -4), "1, -5へ");

        // 氷を指定
        yield return AM1TestUtil.ClickAndAssert(new Vector3(3, 1.5f, -5), new Vector3(2, 0, -5), "氷");

        // 草
        yield return AM1TestUtil.ClickAndAssert(new Vector3(3, 1.5f, -3), new Vector3(3, 0, -4), "草");
        yield return new WaitForSeconds(1.0f);
    }

    [UnityTest]
    public IEnumerator RouteTestsWithEnumeratorPasses()
    {
        yield return AM1TestUtil.StartStage("TestStage01");

        // 1, -5へ歩く
        Vector3 target = new(1, 0.5f, -5);
        var block = AM1TestUtil.Click(target);

        yield return null;
        yield return null;
        yield return null;

        Assert.That(block, Is.Not.Null, "Block取得");
        Assert.That(block.Checked, Is.True, $"Block {block.transform.position} チェック済み");
        Assert.That(block.StepCount, Is.GreaterThan(0), $"Block {block.transform.position} 歩ける");
        Assert.That(block.CanWalk, Is.True, $"Block歩ける {block.StepCount}");

        yield return new WaitUntil(
            () => PiyoBehaviour.Instance.CurrentState == PiyoBehaviour.Instance.GetInstance<PiyoStateWalk>());

        var wayDirs = WalkCourse.walkForwards.ToArray();

        Assert.That(wayDirs.Length, Is.EqualTo(5), "5歩のデータ");
        Assert.That(wayDirs[0], Is.EqualTo(Direction.Type.Back), "2歩目は手前");
        Assert.That(wayDirs[1], Is.EqualTo(Direction.Type.Back), "3歩目は手前");
        Assert.That(wayDirs[2], Is.EqualTo(Direction.Type.Back), "4歩目は手前");
        Assert.That(wayDirs[3], Is.EqualTo(Direction.Type.Back), "5歩目は手前");
        Assert.That(wayDirs[4], Is.EqualTo(Direction.Type.Right), "6歩目は右");

        float time = 0;
        while ((time < 6) && (PiyoBehaviour.Instance.CurrentState == PiyoBehaviour.Instance.GetInstance<PiyoStateWalk>()))
        {
            yield return null;
            time += Time.deltaTime;
        }
        Assert.That(time, Is.LessThan(6), "到着");

        AM1TestUtil.AssertPosition(target);

        // 手前へ移動
        yield return AM1TestUtil.ClickAndAssert(new Vector3(1, 0.5f, -6), new Vector3(1, 0.5f, -6));

        // ブロックを指定
        yield return AM1TestUtil.ClickAndAssert(new Vector3(3, 1.5f, -5), new Vector3(2, 0f, -5));
        yield return new WaitForSeconds(1.0f);
    }
}
