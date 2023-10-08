using System.Collections;
using System.Collections.Generic;
using AM1.BaseFrame;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using AM1.VBirdHiyoko;

public class ChangeSceneTests
{
    readonly string[] titleScenes = { "Title", "Stage", "StageTitle" };
    readonly string[] gameScenes = { "Game", "Stage", "Stage01" };
    readonly string[] endingScenes = { "Ending", "Credits" };

    [UnityTest]
    public IEnumerator ChangeSceneTestsWithEnumeratorPasses()
    {
        float waitSeconds = 5;

        VBirdHiyokoPlayerPrefs.prefix = "Test";
        var storage = new VBirdHiyokoPlayerPrefs();
        storage.DeleteAll();

        // シーン起動
        yield return AM1TestUtil.StartTitle();

        // タイトルシーンの起動待ち
        float time = Time.realtimeSinceStartup;
        yield return WaitSceneLoaded(waitSeconds);
        Assert.That(Time.realtimeSinceStartup - time, Is.LessThanOrEqualTo(waitSeconds), "Title起動");
        Assert.That(CheckSceneLoaded(titleScenes), Is.True, "Title状態のシーン確認");

        // ゲーム開始
        VBirdHiyokoManager.CurrentStage.Set(1);
        GameSceneStateChanger.Instance.Request();
        time = Time.realtimeSinceStartup;
        yield return WaitSceneLoaded(waitSeconds);
        Assert.That(Time.realtimeSinceStartup - time, Is.LessThanOrEqualTo(waitSeconds), "Game起動");
        Assert.That(CheckSceneLoaded(gameScenes), Is.True, "Game状態のシーン確認");

        // ゲームからタイトルヘ戻る
        TitleSceneStateChanger.Instance.RequestFrom(TitleSceneStateChanger.FromState.Game, false);
        time = Time.realtimeSinceStartup;
        yield return WaitSceneLoaded(waitSeconds);
        Assert.That(Time.realtimeSinceStartup - time, Is.LessThanOrEqualTo(waitSeconds), "GameからTitleへ");
        Assert.That(CheckSceneLoaded(titleScenes), Is.True, "Title状態のシーン確認");

        // エンディング
        EndingSceneStateChanger.Instance.Request();
        time = Time.realtimeSinceStartup;
        yield return WaitSceneLoaded(10);
        Assert.That(Time.realtimeSinceStartup - time, Is.LessThanOrEqualTo(10), "Ending起動");
        Assert.That(CheckSceneLoaded(endingScenes), Is.True, "Ending状態のシーン確認");

        // エンディングからタイトルヘ戻る
        TitleSceneStateChanger.Instance.RequestFrom(TitleSceneStateChanger.FromState.Ending, false);
        time = Time.realtimeSinceStartup;
        yield return WaitSceneLoaded(waitSeconds);
        Assert.That(Time.realtimeSinceStartup - time, Is.LessThanOrEqualTo(waitSeconds), "EndingからTitleへ戻る");
        Assert.That(CheckSceneLoaded(titleScenes), Is.True, "Title状態のシーン確認");

        // ゲーム開始
        GameSceneStateChanger.Instance.Request();
        time = Time.realtimeSinceStartup;
        yield return WaitSceneLoaded(waitSeconds);
        Assert.That(Time.realtimeSinceStartup - time, Is.LessThanOrEqualTo(waitSeconds), "Game再起動");
        Assert.That(CheckSceneLoaded(gameScenes), Is.True, "Game状態のシーン確認");
    }

    IEnumerator WaitSceneLoaded(float wait=5)
    {
        float time = 0;

        yield return null;

        while ((time < wait)
            && SceneStateChanger.IsRequestOrChanging)
        {
            time += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Systemは常駐している前提で、それ以外のシーンが揃っていて、余計なシーンがないことを確認する。
    /// </summary>
    /// <param name="sceneList">シーン名リスト</param>
    /// <returns>揃っている時、true</returns>
    bool CheckSceneLoaded(string[] sceneList)
    {
        // リストの数にSystem1つを加えた数と一致しているか確認
        if (SceneManager.loadedSceneCount != sceneList.Length + 1)
        {
            Debug.Log($"シーン数不一致 {SceneManager.loadedSceneCount} / {sceneList.Length+1}");
            return false;
        }

        // シーンの読み込み状態確認
        foreach (string sceneName in sceneList)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if ((scene == null) || !scene.isLoaded) {
                Debug.Log($"{sceneName}がない");
                return false;
            }
        }
        return true;        
    }
}
