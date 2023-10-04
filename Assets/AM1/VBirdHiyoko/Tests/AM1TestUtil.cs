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

    /// <summary>
    /// 保存データをTestにして削除
    /// </summary>
    internal static void SetGameDataTestAndClear()
    {
        VBirdHiyokoPlayerPrefs.prefix = "Test";
        VBirdHiyokoManager.ResetStatics();
        VBirdHiyokoManager.Init();
        VBirdHiyokoManager.GetInstance<IGameDataStorage>().DeleteAll();
    }

    static Collider[] results = new Collider[1];
    public static BlockRouteData Click(Vector3 target)
    {
        Debug.Log($"Click({target})");
        int count = Physics.OverlapSphereNonAlloc(target, 0.1f, results, LayerMask.GetMask("Block"));
        Assert.That(count, Is.GreaterThan(0), $"ブロックあり {target}");
        var block = results[0].GetComponent<BlockRouteData>();
        Assert.That(block, Is.Not.Null, $"BlockRouteDataあり {target}");
        PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().OnAction(block);
        return block;
    }

    /// <summary>
    /// 歩き終わるのを指定秒数のタイムアウト付きで待つ。
    /// </summary>
    public static IEnumerator WaitWalkDone(string mes = "", float limit = 5f)
    {
        // 歩き始めるのを待つ
        float t = 0;
        while ((t < limit) && !PiyoBehaviour.Instance.StateIs<PiyoStateWalk>())
        {
            yield return null;
            t += Time.deltaTime;
        }
        Assert.That(t, Is.LessThan(limit), $"歩き待ち 制限時間オーバー:{mes}");

        // 入力待ちまで待つ
        t = 0;
        while ((t < limit) && !PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput)
        {
            yield return null;
            t += Time.deltaTime;
        }
        Assert.That(t, Is.LessThan(limit), $"入力待機 制限時間オーバー:{mes}");
    }

    /// <summary>
    /// 押し終わるのを指定秒数のタイムアウト付きで待つ。
    /// </summary>
    public static IEnumerator WaitWalkOrPushDone(string mes = "", float limit = 5f)
    {
        // 押し始めるのを待つ
        float t = 0;
        while ((t < limit)
            && !PiyoBehaviour.Instance.StateIs<PiyoStatePush>()
            && !PiyoBehaviour.Instance.StateIs<PiyoStateWalk>())
        {
            yield return null;
            t += Time.deltaTime;
        }
        Assert.That(t, Is.LessThan(limit), $"歩くか押し待ち 制限時間オーバー:{mes}");

        // 入力待ちまで待つ
        t = 0;
        while ((t < limit) && !PiyoBehaviour.Instance.GetInstance<PiyoStateWaitInput>().IsWaitInput)
        {
            yield return null;
            t += Time.deltaTime;
        }
        Assert.That(t, Is.LessThan(limit), $"入力待機 制限時間オーバー:{mes}");
    }

    /// <summary>
    /// 指定の座標につくまで待つ。
    /// </summary>
    /// <param name="pos">確認する座標</param>
    public static IEnumerator WaitPlayerPosition(Vector3 pos, float time = 5f)
    {
        float t = 0;

        for (; t < time; t += Time.fixedDeltaTime)
        {
            if (IsPlayerPosition(pos))
            {
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        Assert.That(t, Is.LessThan(time), $"座標につくまで待つ 制限時間オーバー {pos}");
    }

    /// <summary>
    /// プレイヤーの座標が指定の座標と一致するかどうかを返す。
    /// </summary>
    /// <param name="pos">確認する座標</param>
    /// <param name="withIn">許容誤差</param>
    /// <returns>一致していると見做せる時、true</returns>
    public static bool IsPlayerPosition(Vector3 pos, float withIn = 0.01f)
    {
        var p = PiyoBehaviour.Instance.RigidbodyPosition - pos;
        p.y = 0;
        return p.magnitude < withIn;
    }

    /// <summary>
    /// メッセージが表示状態になるのを待つ。
    /// </summary>
    /// <param name="timeout">タイムアウトの秒数</param>
    public static IEnumerator WaitShowMessage(string mes = "", float timeout = 3)
    {
        float time = 0;
        yield return null;
        while ((time < timeout) && (MessageWindow.Instance.CurrentState != MessageWindow.State.Show))
        {
            yield return null;
            time += Time.deltaTime;
        }

        if (time >= timeout)
        {
            Assert.Fail("Timeout" + (mes.Length > 0 ? $":{mes}" : ""));
        }
    }

    /// <summary>
    /// メッセージが変わるまで待つ
    /// </summary>
    /// <param name="timeout">タイムアウトの秒数</param>
    public static IEnumerator WaitChangeMessage(string mes = "", float timeout = 3)
    {
        float time = 0;
        string last = MessageWindow.Instance.MessageText;
        while ((time < timeout) && (MessageWindow.Instance.MessageText == last))
        {
            yield return null;
            time += Time.deltaTime;
        }

        if (time >= timeout)
        {
            Assert.Fail("Timeout" + (mes.Length > 0 ? $":{mes}" : ""));
        }
    }

    /// <summary>
    /// メッセージが閉じるのを待つ。
    /// </summary>
    /// <param name="timeout">タイムアウトの秒数</param>
    internal static IEnumerator WaitCloseMessage(string mes = "", float timeout = 3)
    {
        float time = 0;
        yield return null;
        while ((time < timeout) && (MessageWindow.Instance.CurrentState != MessageWindow.State.Hide))
        {
            yield return null;
            time += Time.deltaTime;
        }

        if (time >= timeout)
        {
            Assert.Fail("Timeout" + (mes.Length > 0 ? $":{mes}" : ""));
        }
    }
}
