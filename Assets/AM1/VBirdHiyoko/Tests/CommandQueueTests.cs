using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.VBirdHiyoko;
using AM1.CommandSystem;

public class CommandQueueTests
{
    bool isPlayerEnabled;
    bool isUIEnabled;
    bool isPlayerDisabled;
    bool isUIDisabled;

    [TearDown]
    public void TearDown()
    {
        StaticInitializer.Init();
    }

    [Test]
    public void CommandQueueTest()
    {
        // 処理を登録
        CommandQueue.AddChangeListener(CommandInputType.Game, OnPlayerMaskChange);
        CommandQueue.AddChangeListener(CommandInputType.UI, OnUIMaskChange);

        // 処理を登録して、登録されないことを確認
        var uiTest = new CommandQueueUITestData();
        CommandQueue.EntryCommand(uiTest);
        Assert.That(CommandQueue.IsSetNextCommand, Is.False, "UIマスク未設定なので登録なし");
        var gameTest = new CommandQueueGameTestData();
        CommandQueue.EntryCommand(gameTest);
        Assert.That(CommandQueue.IsSetNextCommand, Is.False, "Gameマスク未設定なので登録なし");

        // UIを有効化。UIの有効化が呼ばれ、無効化が呼ばれず。ゲームが呼ばれない
        CommandQueue.ChangeInputMask(CommandInputType.UI);
        Assert.That(isPlayerEnabled, Is.False, "UIを有効化。Update前");
        Assert.That(isPlayerDisabled, Is.False, "UIを有効化。Update前");
        Assert.That(isUIEnabled, Is.False, "UIを有効化。Update前");
        Assert.That(isUIDisabled, Is.False, "UIを有効化。Update前");

        CommandQueue.Update();
        Assert.That(isPlayerEnabled, Is.False, "UIを有効化。Update後");
        Assert.That(isPlayerDisabled, Is.False, "UIを有効化。Update後");
        Assert.That(isUIEnabled, Is.True, "UIを有効化。Update後");
        Assert.That(isUIDisabled, Is.False, "UIを有効化。Update後");

        // UIの操作登録と実行確認
        CommandQueue.EntryCommand(uiTest);
        Assert.That(CommandQueue.IsSetNextCommand, Is.True, "UIマスク設定済みなので登録あり");
        Assert.That(uiTest.Count, Is.EqualTo(0), "UIマスク設定済み。実行前");
        CommandQueue.Update();
        Assert.That(CommandQueue.IsSetNextCommand, Is.False, "UIマスク設定済み。実行後なのでコマンドなし");
        Assert.That(uiTest.Count, Is.EqualTo(1), "UIマスク設定済み。実行を確認");
        Assert.That(gameTest.Count, Is.EqualTo(0), "UIマスク設定済み。gameは登録していないので実行なし");

        // ゲームの操作登録がされないのを確認
        CommandQueue.EntryCommand(gameTest);
        CommandQueue.Update();
        Assert.That(CommandQueue.IsSetNextCommand, Is.False, "Gameマスクは未設定なので登録なし");

        // ゲームを有効化。ゲームの有効化のみ呼ばれる
        ClearFlags();
        CommandQueue.ChangeInputMask(CommandInputType.Game);
        CommandQueue.EntryCommand(gameTest);
        Assert.That(CommandQueue.IsSetNextCommand, Is.False, "更新前なのでGameマスクはまだ反映されていないので登録されない");

        // 更新してGame有効
        CommandQueue.Update();
        Assert.That(isPlayerEnabled, Is.True, "Playerに切り替え");
        Assert.That(isPlayerDisabled, Is.False, "Playerに切り替え");
        Assert.That(isUIEnabled, Is.False, "Playerに切り替え");
        Assert.That(isUIDisabled, Is.True, "Playerに切り替え");

        //登録を確認
        CommandQueue.EntryCommand(gameTest);
        Assert.That(CommandQueue.IsSetNextCommand, Is.True, "Gameマスク設定済みなので登録あり");
        Assert.That(gameTest.Count, Is.EqualTo(0), "Gameマスク設定済み。実行前");
        CommandQueue.Update();
        Assert.That(CommandQueue.IsSetNextCommand, Is.False, "Gameマスク設定済み。実行後なのでコマンドなし");
        Assert.That(uiTest.Count, Is.EqualTo(1), "Gameマスク設定済み。UIは前回実行のまま");
        Assert.That(gameTest.Count, Is.EqualTo(1), "Gameマスク設定済み。Gameを実行");

        // UIは無効
        CommandQueue.EntryCommand(uiTest);
        CommandQueue.EntryCommand(gameTest);
        CommandQueue.Update();
        Assert.That(uiTest.Count, Is.EqualTo(1), "UI無効");
        Assert.That(gameTest.Count, Is.EqualTo(2), "UI無効2");

        // UIとPlayerを両方有効化
        ClearFlags();
        CommandQueue.ChangeInputMask(CommandInputType.Everything);
        CommandQueue.Update();
        Assert.That(isPlayerEnabled, Is.False, "UIとPlayerを有効化。Playerはそのまま");
        Assert.That(isPlayerDisabled, Is.False, "UIとPlayerを有効化。Playerはそのまま");
        Assert.That(isUIEnabled, Is.True, "UIとPlayerを有効化。UIは有効化");
        Assert.That(isUIDisabled, Is.False, "UIとPlayerを有効化。UIは有効化");

        // UIの登録後にゲームを登録。ゲームよりUIが優先されることを確認
        CommandQueue.EntryCommand(uiTest);
        CommandQueue.EntryCommand(gameTest);
        CommandQueue.Update();
        Assert.That(uiTest.Count, Is.EqualTo(2), "UI優先");
        Assert.That(gameTest.Count, Is.EqualTo(2), "UI優先2");

        // ゲームの登録後にUIを登録。ゲームよりUIが優先されることを確認
        CommandQueue.EntryCommand(gameTest);
        CommandQueue.EntryCommand(uiTest);
        CommandQueue.Update();
        Assert.That(uiTest.Count, Is.EqualTo(3), "UI優先");
        Assert.That(gameTest.Count, Is.EqualTo(2), "UI優先2");

        // UIマスクを無効化。UIの無効化のみ呼ばれる
        ClearFlags();
        CommandQueue.ChangeInputMask(CommandInputType.Game);
        Assert.That(isUIEnabled, Is.False, "IUマスク切り替え実行前");
        Assert.That(isUIDisabled, Is.False, "IUマスク切り替え実行前");
        CommandQueue.Update();
        Assert.That(isPlayerEnabled, Is.False, "UIを無効化。Update後");
        Assert.That(isPlayerDisabled, Is.False, "UIを無効化。Update後");
        Assert.That(isUIEnabled, Is.False, "UIを無効化。Update後");
        Assert.That(isUIDisabled, Is.True, "UIを無効化。Update後");

        // UIが登録できない
        CommandQueue.EntryCommand(gameTest);
        CommandQueue.EntryCommand(uiTest);
        CommandQueue.Update();
        Assert.That(uiTest.Count, Is.EqualTo(3), "UIがマスクで登録できなかった");
        Assert.That(gameTest.Count, Is.EqualTo(3), "UIがマスクで登録できなかった2");

        // ゲームを無効化。ゲームの無効化のみ呼ばれる
        ClearFlags();
        CommandQueue.ChangeInputMask(CommandInputType.None);
        Assert.That(isPlayerEnabled, Is.False, "Gameマスク切り替え実行前");
        Assert.That(isPlayerDisabled, Is.False, "Gameマスク切り替え実行前");
        CommandQueue.Update();
        Assert.That(isPlayerEnabled, Is.False, "UIを無効化。Update後");
        Assert.That(isPlayerDisabled, Is.True, "UIを無効化。Update後");
        Assert.That(isUIEnabled, Is.False, "UIを無効化。Update後");
        Assert.That(isUIDisabled, Is.False, "UIを無効化。Update後");

        // ゲームもUIも登録できない
        CommandQueue.EntryCommand(gameTest);
        CommandQueue.EntryCommand(uiTest);
        CommandQueue.Update();
        Assert.That(uiTest.Count, Is.EqualTo(3), "どちらもマスクで登録できなかった");
        Assert.That(gameTest.Count, Is.EqualTo(3), "どちらもマスクで登録できなかった2");

        // 有効化してすぐ無効化。無効化後、登録した有効化要求がリセットされていることと、有効、無効処理が呼ばれないことを確認
        ClearFlags();
        CommandQueue.ChangeInputMask(CommandInputType.UI);
        CommandQueue.ChangeInputMask(CommandInputType.None);
        Assert.That(CommandQueue.IsSetNextCommand, Is.False, "無効化");
        Assert.That(isUIEnabled, Is.False);
        Assert.That(isUIDisabled, Is.False);

        // 無効化してすぐ有効化。次のフレームまでは有効化は機能しない
        CommandQueue.ChangeInputMask(CommandInputType.UI);
        CommandQueue.Update();

        // 一瞬、無効化しても、前と同じなので状態変更は実行されない
        ClearFlags();
        CommandQueue.ChangeInputMask(CommandInputType.None);
        CommandQueue.ChangeInputMask(CommandInputType.UI);
        Assert.That(isUIEnabled, Is.False);
        Assert.That(isUIDisabled, Is.False);

        // 次のフレームで有効が呼ばれて、登録できて、処理されることを確認
        CommandQueue.EntryCommand(uiTest);
        CommandQueue.Update();
        Assert.That(uiTest.Count, Is.EqualTo(4));

        // 操作を登録後に無効化された場合は実行する   
        CommandQueue.EntryCommand(uiTest);
        CommandQueue.ChangeInputMask(CommandInputType.None);
        CommandQueue.Update();
        Assert.That(uiTest.Count, Is.EqualTo(5));
    }

    void ClearFlags()
    {
        isPlayerEnabled = false;
        isUIEnabled = false;
        isPlayerDisabled = false;
        isUIDisabled = false;
    }

    [Test]
    public void BitCheck()
    {
        int old = 0b0011;
        int current = 0b0101;
        int diff = old ^ current;
        Assert.That(diff, Is.EqualTo(0b0110));
        string ws = "";
        for (int i = 0; i < 4; i++)
        {
            if ((diff & (1 << i)) != 0)
            {
                if ((current & (1 << i)) == 0)
                {
                    Assert.That(i, Is.EqualTo(1));
                }
                else
                {
                    Assert.That(i, Is.EqualTo(2));
                }
            }
            else
            {
                ws += $"{i}";
            }
        }
        Assert.That(ws, Is.EqualTo("03"));
    }

    void OnPlayerMaskChange(bool flag)
    {
        if (flag)
        {
            isPlayerEnabled = true;
        }
        else
        {
            isPlayerDisabled = true;
        }
    }

    void OnUIMaskChange(bool flag)
    {
        if (flag)
        {
            isUIEnabled = true;
        }
        else
        {
            isUIDisabled = true;
        }
    }


    [UnityTest]
    public IEnumerator CommandQueueTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}


class CommandQueueUITestData : ICommandQueueData
{
    public CommandInputType Type => CommandInputType.UI;
    public int Priority => 10;

    public int Count { get; private set; }

    public void Invoke()
    {
        Count++;
    }
}

class CommandQueueGameTestData : ICommandQueueData
{
    public CommandInputType Type => CommandInputType.Game;
    public int Priority => 0;

    public int Count { get; private set; }
    public void Invoke()
    {
        Count++;
    }
}