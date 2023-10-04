using AM1.MessageSystem;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class MessageWindowTests
{
    [UnityTest]
    public IEnumerator MessageWindowFundamentalTests()
    {
        yield return AM1TestUtil.StartStage("Stage00");

        // メッセージウィンドウを取得
        Assert.That(MessageWindow.Instance, Is.Not.Null);

        // メッセージを登録
        MessageWindow.Instance.Show(new MessageData("Test Message"));
        Assert.That(MessageWindow.Instance.IsShowing, Is.True);

        // すぐには閉じない
        MessageWindow.Instance.Close();
        Assert.That(MessageWindow.Instance.IsShowing, Is.True, "すぐには閉じない");
        yield return null;
        MessageWindow.Instance.Close();
        Assert.That(MessageWindow.Instance.IsShowing, Is.True, "閉じる");

        // 表示チェック
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo("Test Message"));

        // 一定時間待っても閉じない確認
        yield return new WaitForSeconds(2f);
        Assert.That(MessageWindow.Instance.IsShowing, Is.True, "自動閉じではない");

        // 閉じたことを確認
        MessageWindow.Instance.Close();
        yield return new WaitForSeconds(1f);
        Assert.That(MessageWindow.Instance.IsShowing, Is.False);

        // 自動で閉じるメッセージを2つ登録
        MessageWindow.Instance.Show(new MessageData("自動閉じ1", 1));
        MessageWindow.Instance.Show(new MessageData("自動閉じ2", 1));

        // ウケつけ秒数を過ぎたら閉じられることを確認
        yield return new WaitForSeconds(0.1f);
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo("自動閉じ1"));
        yield return new WaitUntil(() => MessageWindow.Instance.CurrentState == MessageWindow.State.Show);
        yield return new WaitForSeconds(MessageWindow.IgnoreCloseSeconds + 0.1f);
        MessageWindow.Instance.Close();
        yield return new WaitForSeconds(0.1f);
        Assert.That(MessageWindow.Instance.MessageText, Is.EqualTo("自動閉じ2"));

        // 一定時間待って閉じることを確認
        yield return new WaitForSeconds(2);
        Assert.That(MessageWindow.Instance.IsShowing, Is.False);

        // メッセージを複数登録
        MessageWindow.Instance.Show(new MessageData("一斉閉じ1", 1));
        MessageWindow.Instance.Show(new MessageData("一斉閉じ2", 1));

        // 全て同時に閉じることを確認
        MessageWindow.Instance.CloseAll();
        yield return new WaitForSeconds(1f + 0.1f);
        Assert.That(MessageWindow.Instance.IsShowing, Is.False, $"全て同時閉じ。{MessageWindow.Instance.CurrentState}");
    }
}
