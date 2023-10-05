using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.VBirdHiyoko;
using AM1.CommandSystem;
using UnityEditor;

public class HistoryPlayTests
{
    /// <summary>
    /// 基本動作の確認
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator HistoryPlayFundamental()
    {
        Time.timeScale = 4;
        yield return AM1TestUtil.StartStage("TestStage01");
        yield return AM1TestUtil.WaitCanAction();

        Vector3[] stands =
        {
            new (0,1,0), // スタート地点
            new (0,1,-3),
            new (1, 1, -3),
            new (3, 1, -6),
            new (3, 1, -5),
            new (4, 1, -5),
            new (4, 0.5f, -4),
            new (3, 1.5f, -4),
            new (2, 0.5f, -5),
            new (2, 1.5f, -4),
            new (2, 0.5f, -3),
            new (2, 0.5f, -4),
    };

        // 操作
        // 1
        var target = new Vector3(1, 1.5f, -3);
        int index = 1;
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step1");
        // 2
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step2");
        // 3
        target.Set(3, 0.5f, -6);
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step3");
        // 4
        target.Set(3, 1.5f, -5);
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step4");
        // 5
        target.Set(4, 1.5f, -5);
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step5");
        // 6
        target.Set(4, 0.5f, -4);
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step6");
        // 7
        target.Set(3, 1.5f, -4);
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step7");
        // 8
        target.Set(2, 0.5f, -5);
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step8");
        // 9
        target.Set(2, 1.5f, -4);
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step9");
        // 10
        target.Set(2, 0.5f, -3);
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step10");
        // 11
        target.Set(2, 0.5f, -4);
        yield return AM1TestUtil.ClickAndAssert(target, stands[index++], "Step11");

        // Undo
        yield return AM1TestUtil.WaitCanAction();
        Time.timeScale = 1;
        var undo = GameObject.FindObjectOfType<UndoButton>();
        Assert.That(undo, Is.Not.Null);
        undo.OnClick();

        var stateHistory = PiyoBehaviour.Instance.GetInstance<PiyoStateHistory>();
        yield return WaitUndoStartAndDone("一手戻し");
        index--;
        AM1TestUtil.AssertPosition(stands[--index], "1手戻し");

        // もう一手戻し
        undo.OnClick();
        yield return WaitUndoStartAndDone("もう一手戻し");
        AM1TestUtil.AssertPosition(stands[--index], "もう一手戻し");

        // 2手連続押し。1回のみ実行
        undo.OnClick();
        undo.OnClick();
        yield return WaitUndoStartAndDone("2手連続、1回のみ");
        AM1TestUtil.AssertPosition(stands[--index], "2手連続、1回のみ");

        // 1フレームごとに2手戻し
        undo.OnClick();
        yield return null;
        undo.OnClick();
        yield return WaitUndoStartAndDone("1フレームごとに2手戻し");
        index -= 2;
        Assert.That(HistoryPlayer.IsCurrentState(IHistoryPlayer.Mode.Standby), Is.True);
        Assert.That(CommandQueue.IsSetNextCommand, Is.False);
        AM1TestUtil.AssertPosition(stands[index], "1フレームごとに2手戻し");

        // Redo
        var redo = GameObject.FindObjectOfType<RedoButton>();
        redo.OnClick();
        yield return WaitRedoStartAndDone("Redo");
        index++;
        AM1TestUtil.AssertPosition(stands[index], "Redo");

        // UndoしてからすぐRedo
        undo.OnClick();
        yield return null;
        redo.OnClick();
        yield return WaitRedoStartAndDone("UndoしてからすぐRedo");
        AM1TestUtil.AssertPosition(stands[index], "UndoしてからすぐRedo");

        // 先頭までUndo
        for (int i = 0; i < stands.Length; i++)
        {
            yield return new WaitUntil(() => CommandQueue.CurrentInputMask.HasFlag(CommandInputType.Game));
            undo.OnClick();
        }
        yield return WaitUndoStartAndDone("先頭までUndo");
        AM1TestUtil.AssertPosition(stands[0], "先頭までUndo");

        // 3連続Redo
        yield return new WaitUntil(() => redo.OnClick());
        yield return new WaitUntil(() => redo.OnClick());
        yield return new WaitUntil(() => redo.OnClick());
        yield return WaitRedoStartAndDone("3連続Redo");
        AM1TestUtil.AssertPosition(stands[3], "3連続Redo");

        // RedoしてからすぐUndo
        redo.OnClick();
        yield return new WaitUntil(() => CommandQueue.CurrentInputMask.HasFlag(CommandInputType.Game));
        undo.OnClick();
        yield return WaitUndoStartAndDone("RedoしてすぐUndo");
        AM1TestUtil.AssertPosition(stands[3], "RedoしてすぐUndo");

        // Undoしてから3回Redo
        yield return new WaitUntil(() => undo.OnClick());
        yield return new WaitUntil(() => redo.OnClick());
        yield return new WaitUntil(() => redo.OnClick());
        yield return new WaitUntil(() => redo.OnClick());
        yield return WaitRedoStartAndDone("Undoしてから3回Redo");
        AM1TestUtil.AssertPosition(stands[5], "Undoしてから3回Redo");

        // Redoしてから3回Undo
        yield return new WaitUntil(() => redo.OnClick());
        yield return new WaitUntil(() => undo.OnClick());
        yield return new WaitUntil(() => undo.OnClick());
        yield return new WaitUntil(() => undo.OnClick());
        yield return WaitUndoStartAndDone("Redoして3回Undo");
        AM1TestUtil.AssertPosition(stands[3], "Redoして3回Undo");

        // Accept
        var obj = AM1TestUtil.GetObject(new Vector3(0, 0.5f, -6), LayerMask.GetMask("Block"));
        stateHistory.OnAction(obj.GetComponent<BlockRouteData>());
        yield return null;
        yield return AM1TestUtil.WaitCanAction();
        AM1TestUtil.AssertPosition(new Vector3(0, 1, -6), "Accept+移動");
        Assert.That(HistoryRecorder.HistoryArray[HistoryRecorder.HistoryArray.Length-1].step, Is.EqualTo(4), "Accept+移動");

        yield return new WaitForSeconds(1);
    }

    IEnumerator WaitUndoStartAndDone(string mes="", float sec=4)
    {
        yield return WaitHistoryStart(IHistoryPlayer.Mode.Undo, mes, sec);
        yield return WaitHistoryPlayDone(mes, sec);
    }
    IEnumerator WaitRedoStartAndDone(string mes = "", float sec = 4)
    {
        yield return WaitHistoryStart(IHistoryPlayer.Mode.Redo, mes, sec);
        yield return WaitHistoryPlayDone(mes, sec);
    }

    IEnumerator WaitHistoryStart(IHistoryPlayer.Mode state, string mes="", float sec=4)
    {
        float t = 0;
        while ((t < sec) && (HistoryPlayer.CurrentState != state))
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (t >= sec)
        {
            Assert.Fail($"{state} Start Timeout: {mes}");
        }
    }

    IEnumerator WaitHistoryPlayDone(string mes="", float sec=4)
    {
        float t = 0;
        while ((t < sec) && (HistoryPlayer.CurrentState != IHistoryPlayer.Mode.Standby))
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (t >= sec)
        {
            Assert.Fail($"History Done Timeout: {mes}");
        }
    }

    /// <summary>
    /// マーカーの表示、非表示確認
    /// </summary>
    [UnityTest]
    public IEnumerator MarkerTests()
    {
        Time.timeScale = 2;
        yield return AM1TestUtil.StartStage("TestStage01");
        yield return AM1TestUtil.WaitCanAction();

        Vector3[] stands =
        {
            new (0,1,0), // スタート地点
            new (0,1,-3),
            new (1, 1, -3),
            new (3, 1, -6),
            new (3, 1, -5),
            new (4, 1, -5),
            new (4, 0.5f, -4),
    };

        // 操作
        // 1
        var target = new Vector3(1, 1.5f, -3);
        yield return AM1TestUtil.ClickAndAssert(target, stands[1], "Step1");
        // 2
        yield return AM1TestUtil.ClickAndAssert(target, stands[2], "Step2");
        // 3
        target.Set(3, 0.5f, -6);
        yield return AM1TestUtil.ClickAndAssert(target, stands[3], "Step3");
        // 4
        target.Set(3, 1.5f, -5);
        yield return AM1TestUtil.ClickAndAssert(target, stands[4], "Step4");
        /*
        // 5
        target.Set(4, 1.5f, -5);
        yield return AM1TestUtil.ClickAndAssert(target, stands[5], "Step5");
        // 6
        target.Set(4, 0.5f, -4);
        yield return AM1TestUtil.ClickAndAssert(target, stands[6], "Step6");
        */

        // Undo前の矢印確認
        Assert.That(GetArrowFlag(), Is.EqualTo(0b0010), "右矢印");

        // Undo
        Time.timeScale = 1;
        yield return AM1TestUtil.WaitCanAction();
        var undo = GameObject.FindObjectOfType<UndoButton>();
        Assert.That(undo, Is.Not.Null);
        undo.OnClick();
        yield return null;
        yield return null;

        // 矢印消え
        Assert.That(GetArrowFlag(), Is.EqualTo(0), "Undo後の矢印消え");

        // Undo終了待ち
        yield return WaitUndoStartAndDone("一手戻し");
        AM1TestUtil.AssertPosition(stands[3], "1手戻し");

        // 奥矢印表示
        Assert.That(GetArrowFlag(), Is.EqualTo(1), "奥矢印表示");

        yield return new WaitForSeconds(1);
    }

    int GetArrowFlag()
    {
        int flag = 0;
        var arrows = GameObject.FindObjectOfType<PushArrows>();
        for (int i = 0; i < 4; i++)
        {
            var child = arrows.transform.GetChild(i);
            if (!child.GetComponent<Animator>().GetBool("Show")) continue;
            
            if (child.name.EndsWith("Forward"))
            {
                flag |= 1 << (int)Direction.Type.Forward;
            }
            else if (child.name.EndsWith("Back"))
            {
                flag |= 1 << (int)Direction.Type.Back;
            }
            else if (child.name.EndsWith("Right"))
            {
                flag |= 1 << (int)Direction.Type.Right;
            }
            else
            {
                flag |= 1 << (int)Direction.Type.Left;
            }
        }
        return flag;
    }

    /// <summary>
    /// 砂ブロックの復活、壊れ確認
    /// </summary>
    [UnityTest]
    public IEnumerator SandBlockTests()
    {
        Time.timeScale = 1;
        yield return AM1TestUtil.StartStage("TestStage01");
        yield return AM1TestUtil.WaitCanAction();

        Vector3[] stands =
        {
            new (0,1,0), // スタート地点
            new (0,1,-3),
            new (1, 1, -3),
            new (3, 1, -6),
            new (3, 1, -5),
            new (4, 1, -5),
            new (4, 0.5f, -4),
    };

        // 操作
        // 1
        var target = new Vector3(1, 1.5f, -3);
        yield return AM1TestUtil.ClickAndAssert(target, stands[1], "Step1");
        // 2
        yield return AM1TestUtil.ClickAndAssert(target, stands[2], "Step2");
        // 3
        target.Set(3, 0.5f, -6);
        yield return AM1TestUtil.ClickAndAssert(target, stands[3], "Step3");

        // Undo
        Time.timeScale = 1;
        yield return AM1TestUtil.WaitCanAction();
        var undo = GameObject.FindObjectOfType<UndoButton>();
        Assert.That(undo, Is.Not.Null);
        undo.OnClick();
        yield return null;
        undo.OnClick();
        yield return WaitUndoStartAndDone("2手戻し");

        // 2手進め
        var redo = GameObject.FindObjectOfType<RedoButton>();
        redo.OnClick();
        yield return null;
        redo.OnClick();
        yield return WaitRedoStartAndDone("2手進め");

        // 1手戻し
        undo.OnClick();
        yield return WaitUndoStartAndDone("1手戻し");

        // Accept
        var obj = AM1TestUtil.GetObject(target, LayerMask.GetMask("Block"));
        var stateHistory = PiyoBehaviour.Instance.GetInstance<PiyoStateHistory>();
        stateHistory.OnAction(obj.GetComponent<BlockRouteData>());
        yield return AM1TestUtil.WaitCanAction();
        AM1TestUtil.AssertPosition(target, "Accept移動");

        yield return new WaitForSeconds(1);
    }

    [Test]
    public void SByteToIntTests()
    {
        int eulerY = 3;
        int state = -2;
        sbyte data = (sbyte)(eulerY | (state << 2));

        state = data >> 2;
        Assert.That(state, Is.EqualTo(-2), $"data={data}");
    }
}
