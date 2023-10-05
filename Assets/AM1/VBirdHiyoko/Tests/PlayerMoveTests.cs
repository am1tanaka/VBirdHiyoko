using AM1.VBirdHiyoko;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMoveTests
{
    [TearDown]
    public void TearDown()
    {
        StaticInitializer.Init();
    }

    [UnityTest]
    public IEnumerator WalkAndPushAndTurnTests()
    {
        Debug.Log($"WalkAndPushAndTurnTests");
        var stabBlock = new StabBlock();

        yield return AM1TestUtil.StartStage("TestStage01");

        PiyoBehaviour.Instance.SetPosition(PiyoBehaviour.Instance.transform.position + 0.5f * Vector3.up);

        //UnityEditor.EditorApplication.isPaused = true;

        // 落下
        yield return PiyoBehaviour.Instance.Mover.Fall();
        Assert.That(PiyoBehaviour.Instance.transform.position.y, Is.EqualTo(1).Within(0.02f), "着地");

        // 手前向き
        Assert.That(PiyoBehaviour.Instance.CurrentDirectionType, Is.EqualTo(Direction.Type.Back), "後ろ向き");

        // 5歩歩く
        var startPos = PiyoBehaviour.Instance.transform.position;
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        yield return new WaitForFixedUpdate();
        var pos1 = startPos + 5 * Vector3.back;
        AM1TestUtil.AssertPosition(pos1);

        // 右へ向く
        yield return PiyoBehaviour.Instance.Mover.TurnTo(Direction.Type.Right);
        Assert.That(PiyoBehaviour.Instance.IsFaceTo(Direction.Type.Right), Is.True, "右向き");

        // スタブを押す
        yield return PiyoBehaviour.Instance.Mover.PushTo(Vector3.right, stabBlock);
        var pos2 = pos1 + Vector3.right;
        AM1TestUtil.AssertPosition(pos2, "押した");

        // 1歩前進
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.right);
        var pos3 = pos2 + Vector3.right;
        AM1TestUtil.AssertPosition(pos3, "押した後一歩右へ");

        // 手前へ旋回込みで一歩前進
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        var pos4 = pos3 + Vector3.back;
        AM1TestUtil.AssertPosition(pos4, "手前へ旋回で移動");
        Assert.That(PiyoBehaviour.Instance.CurrentDirectionType, Is.EqualTo(Direction.Type.Back), "手前向き");

        // 終了待ち
        yield return new WaitForSeconds(1);
    }

    [UnityTest]
    public IEnumerator DansaTests()
    {
        Debug.Log($"DansaTests");

        var stabBlock = new StabBlock();
        Debug.Log($"1");
        yield return AM1TestUtil.StartStage("TestStage01");
        Debug.Log($"2");

        // 落下
        yield return PiyoBehaviour.Instance.Mover.Fall();
        Assert.That(PiyoBehaviour.Instance.transform.position.y, Is.EqualTo(1).Within(0.02f), "着地");
        Debug.Log($"3");

        // 4歩移動
        Vector3 pos = PiyoBehaviour.Instance.transform.position;
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        var pos1 = pos + 4 * Vector3.back + 0.2f * Vector3.down;
        Assert.That(Vector3.Distance(pos1, PiyoBehaviour.Instance.transform.position), Is.LessThan(0.02f), "４歩移動＋段差");

        // 2歩
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        yield return PiyoBehaviour.Instance.Mover.WalkTo(Vector3.back);
        var pos2 = pos1 + 2 * Vector3.back + 0.2f * Vector3.up;
        Assert.That(Vector3.Distance(pos2, PiyoBehaviour.Instance.transform.position), Is.LessThan(0.02f), "2歩移動＋段差");

        // その場で万歳
        yield return PiyoBehaviour.Instance.Mover.Banzai();
        yield return PiyoBehaviour.Instance.Mover.Stand();
        Assert.That(PiyoMover.CurrentState, Is.EqualTo(PiyoMover.State.Stand), "立ち");

        // 少し待つ
        yield return new WaitForSeconds(1);
    }
}
