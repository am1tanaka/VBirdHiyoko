using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AM1.VBirdHiyoko;
using AM1.MessageSystem;
using AM1.CommandSystem;

public class PlayerTalkTests
{
    [UnityTest]
    public IEnumerator PlayerTalkTestsWithEnumeratorPasses()
    {
        // 記録を無効化
        AM1TestUtil.SetGameDataTestAndClear();

        yield return AM1TestUtil.StartTitle();
        yield return new WaitUntil(() => CommandQueue.CurrentInputMask.HasFlag(CommandInputType.Game));

        // 行けないメッセージ
        AM1TestUtil.Click(new Vector3(6, 0.5f, 0));
        yield return AM1TestUtil.WaitShowMessage();
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.CantGo)));
        MessageWindow.Instance.CloseAll();
        yield return AM1TestUtil.WaitCloseMessage("行けないメッセージ");

        // 行けないメッセージ連打
        AM1TestUtil.Click(new Vector3(6, 0.5f, 0));
        yield return AM1TestUtil.WaitShowMessage();
        AM1TestUtil.Click(new Vector3(6, 0.5f, 0));
        yield return AM1TestUtil.WaitCloseMessage("行けないメッセージ連打", 4);

        // 隣へ
        AM1TestUtil.Click(new Vector3(1, 0.5f, 0));
        yield return AM1TestUtil.WaitWalkDone("隣へ歩き");

        // セリフ確認
        yield return AM1TestUtil.WaitShowMessage("押せそうセリフ待ち");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.CanPush)));

        // 行けないメッセージ連続表示
        AM1TestUtil.Click(new Vector3(6, 0.5f, 0));
        yield return AM1TestUtil.WaitCloseMessage("押せそう後の行けないメッセージ連打", 5);

        // 押す
        AM1TestUtil.Click(new Vector3(2, 1.5f, 0));
        yield return AM1TestUtil.WaitWalkOrPushDone("押す");

        // 重くて押せない
        AM1TestUtil.Click(new Vector3(3, 1.5f, 0));
        yield return AM1TestUtil.WaitShowMessage("重い");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.TooHeavy)));

        // 遠くの岩は行けない
        AM1TestUtil.Click(new Vector3(18, 1.5f, 0));
        yield return AM1TestUtil.WaitChangeMessage("遠くの岩");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.CantGo)));

        // メッセージをクリックで閉じる
        yield return new WaitUntil(() => MessageWindow.Instance.CurrentState == MessageWindow.State.Show);
        MessageWindow.Instance.Close();

        // ここに入ってみよう
        AM1TestUtil.Click(new Vector3(2, 0.5f, 1));
        yield return AM1TestUtil.WaitPlayerPosition(new Vector3(2, 1, 1));
        yield return AM1TestUtil.WaitChangeMessage("ここに入ってみよう");
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo(Messages.GetMessage(Messages.Type.TutorialLetsGo)));

        yield return AM1TestUtil.WaitPlayerPosition(new Vector3(2, 1, 2));
    }
}
