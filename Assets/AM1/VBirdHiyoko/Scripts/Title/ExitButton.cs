using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 終了ボタン
    /// </summary>
    public class ExitButton : MonoBehaviour
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        ConfirmDialogData exitConfirm = new ConfirmDialogData("ゲームを終了しますか？", "終了する", "つづける", Exit, Back, Hided);

        /// <summary>
        /// 終了ボタンから呼び出す
        /// </summary>
        public void OnClick()
        {
            if (TitleBehaviour.Instance.CanChangeState
                && ConfirmDialog.Instance
                && ConfirmDialog.Instance.Show(exitConfirm))
            {
                SEPlayer.Play(SEPlayer.SE.Click);
                TitleBehaviour.Instance.ChangeState(TitleBehaviour.State.Dialog);
            }
        }

        /// <summary>
        /// 終了
        /// </summary>
        private static void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 戻る
        /// </summary>
        static void Back()
        {
            SEPlayer.Play(SEPlayer.SE.Cancel);
            ConfirmDialog.Instance.Hide();
        }

        /// <summary>
        /// 閉じ終えたら実行する処理。
        /// </summary>
        static void Hided()
        {
            TitleBehaviour.Instance.ChangeState(TitleBehaviour.State.Play);
        }
#else

        // WebGLやスマホでは終了ボタンは表示しない
        private void Awake()
        {
            Destroy(gameObject);
        }

#endif
    }
}