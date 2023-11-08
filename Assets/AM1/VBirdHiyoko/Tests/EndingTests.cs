using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using AM1.BaseFrame;
using AM1.VBirdHiyoko;

public class EndingTests
{
    [UnityTest]
    public IEnumerator EndingTestsWithEnumeratorPasses()
    {
        yield return SceneManager.LoadSceneAsync("System");
        // タイトルの起動を待つ
        yield return new WaitUntil(() => SceneStateChanger.IsStateStarted(TitleSceneStateChanger.Instance));

        // エンディング
        EndingSceneStateChanger.Instance.Request();
        yield return new WaitUntil(() => SceneStateChanger.IsStateStarted(EndingSceneStateChanger.Instance));
        var ending = EndingBehaviour.Instance;
        yield return new WaitUntil(() => CreditsBehaviour.CurrentState == CreditsBehaviour.State.EndRoll);
        Time.timeScale = 4;
        yield return new WaitUntil(() => CreditsBehaviour.CurrentState == CreditsBehaviour.State.WaitToTitle);

        // 少し待つ
        Time.timeScale = 1;
        yield return new WaitForSeconds(3);
    }
}
