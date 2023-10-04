using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.VBirdHiyoko;

public class PlayerPrefsTests
{
    [Test]
    public void PlayerPrefsTestsSimplePasses()
    {
        VBirdHiyokoManager.ResetStatics();
        AM1TestUtil.SetGameDataTestAndClear();
        var storageClass = VBirdHiyokoManager.GetInstance<VBirdHiyokoPlayerPrefs>();

        Assert.That(VBirdHiyokoManager.ClearedStage.Current, Is.EqualTo(0), "初期値0");

        VBirdHiyokoManager.NextStage();   // tutorialクリア
        VBirdHiyokoManager.NextStage();   // stage1クリア
        Assert.That(VBirdHiyokoManager.ClearedStage.Current, Is.EqualTo(1), "記録更新");

        // インスタンスを新しくして読み直す
        VBirdHiyokoManager.RemoveInstance<IGameDataStorage>();
        VBirdHiyokoManager.Init();
        Assert.That(VBirdHiyokoManager.ClearedStage.Current, Is.EqualTo(1), "読み込んで更新確認");

        // 最終ステージまで更新
        VBirdHiyokoManager.NextStage();   // stage2クリア
        VBirdHiyokoManager.NextStage();   // stage3クリア
        Assert.That(VBirdHiyokoManager.ClearedStage.Current, Is.EqualTo(3), "stage3");

        VBirdHiyokoManager.CurrentStage.Next();   // 上限確認
        Assert.That(VBirdHiyokoManager.ClearedStage.Current, Is.EqualTo(3), "これ以上クリアしない");

        // インスタンスを新しくして読み直す
        VBirdHiyokoManager.RemoveInstance<IGameDataStorage>();
        VBirdHiyokoManager.Init();
        Assert.That(VBirdHiyokoManager.ClearedStage.Current, Is.EqualTo(3), "最終チェック");
    }
}
