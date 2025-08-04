using AM1.VBirdHiyoko;
using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ConfirmDialogTests
{
    bool leftActionCalled = false;
    bool rightActionCalled = false;

    [UnityTest]
    public IEnumerator ConfirmDialogTestsWithEnumeratorPasses()
    {
        SceneManager.LoadScene("System");

        // シーン開始のために1フレーム待つ
        yield return null;

        // 状態確認
        Assert.That(ConfirmDialog.IsCurrentState(ConfirmDialog.State.Hide), Is.True);

        // 表示
        ClearFlags();
        var data = new ConfirmDialogData("テストメッセージ", "左", "右", LeftAction, RightAction);
        Assert.That(leftActionCalled, Is.False);
        Assert.That(rightActionCalled, Is.False);
        
        bool res = ConfirmDialog.Instance.Show(data);
        Assert.That(res, Is.True);
        res = ConfirmDialog.Instance.Show(data);
        Assert.That(res, Is.False);
        Assert.That(ConfirmDialog.CurrentState, Is.EqualTo(ConfirmDialog.State.ToShow));

        // 表示完了を待つ
        yield return new WaitUntil(() => ConfirmDialog.CurrentState == ConfirmDialog.State.Show);

        // 表示内容を確認
        var dialog = GameObject.FindFirstObjectByType<ConfirmDialog>();
        Assert.That(dialog, Is.Not.Null);
        var message = dialog.transform.Find("Obi").transform.Find("Message");
        Assert.That(message, Is.Not.Null);
        var messageText = message.GetComponent<TextMeshProUGUI>();
        Assert.That(messageText, Is.Not.Null);
        Assert.That(messageText.text, Is.EqualTo("テストメッセージ"));

        var left = dialog.transform.Find("Obi").transform.Find("LeftButton");
        Assert.That(left, Is.Not.Null);
        var leftText = left.GetComponentInChildren<TextMeshProUGUI>();
        Assert.That(leftText, Is.Not.Null);
        Assert.That(leftText.text, Is.EqualTo("左"));

        var right = dialog.transform.Find("Obi").transform.Find("RightButton");
        Assert.That(right, Is.Not.Null);
        var rightText = right.GetComponentInChildren<TextMeshProUGUI>();
        Assert.That(rightText, Is.Not.Null);
        Assert.That(rightText.text, Is.EqualTo("右"));

        // 左クリック
        var leftButton = left.GetComponent<UnityEngine.UI.Button>();
        Assert.That(leftButton, Is.Not.Null);
        leftButton.onClick.Invoke();
        Assert.That(leftActionCalled, Is.True);
        Assert.That(rightActionCalled, Is.False);

        // 右クリック
        ClearFlags();
        var rightButton = right.GetComponent<UnityEngine.UI.Button>();
        Assert.That(rightButton, Is.Not.Null);
        rightButton.onClick.Invoke();
        Assert.That(leftActionCalled, Is.False);
        Assert.That(rightActionCalled, Is.True);
        Assert.That(ConfirmDialog.CurrentState, Is.EqualTo(ConfirmDialog.State.ToHide));

        // 非表示完了を待つ
        yield return new WaitUntil(() => ConfirmDialog.CurrentState == ConfirmDialog.State.Hide);
        Assert.That(ConfirmDialog.CurrentState, Is.EqualTo(ConfirmDialog.State.Hide));
    
        yield return new WaitForSeconds(0.5f);
    }

    void LeftAction()
    {
        leftActionCalled = true;
    }

    void RightAction()
    {
        rightActionCalled = true;
        ConfirmDialog.Instance.Hide();
    }

    void ClearFlags()
    {
        leftActionCalled = false;
        rightActionCalled = false;
    }
}
