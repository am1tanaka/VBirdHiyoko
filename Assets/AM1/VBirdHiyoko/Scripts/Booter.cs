using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AM1.BaseFrame;
using AM1.BaseFrame.Assets;

/// <summary>
/// 状態システム起動クラス
/// </summary>
public class Booter : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(BootSequence());
    }

    IEnumerator BootSequence()
    {
#if UNITY_EDITOR
        // システムシーン以外を削除
        Scene[] scenes = new Scene[SceneManager.sceneCount];
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            scenes[i] = SceneManager.GetSceneAt(i);
        }
        for (int i=0;i<scenes.Length;i++)
        {
            var sc = scenes[i];
            if (sc.name != gameObject.scene.name)
            {
                yield return SceneManager.UnloadSceneAsync(sc);
            }
        }
#endif

        BootSceneStateChanger.Instance.Request();
        yield return new WaitWhile(() => SceneStateChanger.IsRequestOrChanging);

        var gos = gameObject.GetComponents<MonoBehaviour>();
        if (gos.Length > 1)
        {
            // 2つ以上コンポーネントがアタッチされていたらこのスクリプトだけ削除
            Destroy(this);
        }
        else
        {
            // 1つの時はこれだけなのでオブジェクトごと削除
            Destroy(gameObject);
        }
    }
}
