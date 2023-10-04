using AM1.BaseFrame;
using AM1.MessageSystem;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AM1.VBirdHiyoko;

public class AM1TestUtil
{
    public static IEnumerator StartTitle()
    {
        SceneManager.LoadScene("System");
        yield return new WaitUntil(
            () => SceneStateChanger.IsStateStarted(TitleSceneStateChanger.Instance));
    }

    public static IEnumerator StartStage(string stageName)
    {
        TestSceneStateChanger.StageName = stageName;
        yield return StartTitle();

        // ゲームスタート
        TestSceneStateChanger.Instance.Request(true);
        yield return new WaitUntil(
            () => SceneStateChanger.IsStateStarted(TestSceneStateChanger.Instance));
        yield return new WaitWhile(() => SceneStateChanger.IsChanging);
        yield return WaitCanAction();
    }

    public static IEnumerator WaitCanAction(string mes = "", float sec = 3)
    {
        float time = 0;
        for (time = 0; time < sec; time += Time.deltaTime)
        {
            if (PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput)
            {
                yield break;
            }

            yield return null;
        }

        Assert.Fail($"入力待ちタイムアウト:{mes}");
    }

    /// <summary>
    /// 座標チェック
    /// </summary>
    /// <param name="target">確認したい座標</param>
    /// <param name="mes">失敗時に追加する文字</param>
    public static void AssertPosition(Vector3 target, string mes = "")
    {
        var pos = target - PiyoBehaviour.Instance.RigidbodyPosition;
        pos.y = 0;
        Assert.That(pos.magnitude, Is.LessThan(0.02f), $"{mes} target={target.x}, {target.y}, {target.z} player={PiyoBehaviour.Instance.RigidbodyPosition.x}, {PiyoBehaviour.Instance.RigidbodyPosition.y}, {PiyoBehaviour.Instance.RigidbodyPosition.z}");
    }

}
