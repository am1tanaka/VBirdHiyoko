using AM1.BaseFrame;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 状態システム起動クラス
    /// </summary>
    public class BookBooter : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(BootSequence());
        }

        IEnumerator BootSequence()
        {
            yield return null;

            Button3DAnimator.SetEnabledAll(true);
        }
    }
}