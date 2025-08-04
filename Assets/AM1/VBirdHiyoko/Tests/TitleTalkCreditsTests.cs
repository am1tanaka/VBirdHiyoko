using AM1.CommandSystem;
using AM1.MessageSystem;
using AM1.VBirdHiyoko;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class TitleTalkCreditsTests
{
    [SetUp]
    public void SetUp()
    {
        StaticInitializer.Init();
        Time.timeScale = 2;
    }

    [UnityTest]
    public IEnumerator TitleTalkAfterCreditsTest()
    {
        yield return AM1TestUtil.StartTitle();

        Time.timeScale = 2f;

        yield return new WaitForSeconds(2.5f);

        TitleBehaviour.Instance.OnClickCredits();
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => CommandQueue.CurrentInputMask.HasFlag(CommandInputType.UI));

        var autoTalker = GameObject.FindFirstObjectByType<AutoTalker>();
        Assert.That(autoTalker.CurrentState, Is.EqualTo(AutoTalker.State.Stop), "メッセージ停止");
        yield return new WaitForSeconds(1);
    }

    [UnityTest]
    public IEnumerator TitleCreditsAfterTalkTest()
    {
        yield return AM1TestUtil.StartTitle();

        Time.timeScale = 2f;

        yield return new WaitForSeconds(1f);
        TitleBehaviour.Instance.OnClickCredits();
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => CommandQueue.CurrentInputMask.HasFlag(CommandInputType.UI));

        yield return new WaitForSeconds(2f);

        var autoTalker = GameObject.FindFirstObjectByType<AutoTalker>();
        Assert.That(MessageWindow.Instance.IsShowing, Is.False, "メッセージが表示されない");
        yield return new WaitForSeconds(1);

    }

}
