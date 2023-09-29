using AM1.BaseFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AM1TestUtil
{
    public static IEnumerator StartTitle()
    {
        SceneManager.LoadScene("System");
        yield return new WaitUntil(
            () => SceneStateChanger.IsStateStarted(TitleSceneStateChanger.Instance));
    }
}
