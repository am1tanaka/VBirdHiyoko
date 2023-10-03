using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    public class EndingBehaviour : MonoBehaviour
    {
        public static EndingBehaviour Instance { get; private set; }
        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// エンディングシーケンス開始
        /// </summary>
        public IEnumerator EndingSequence()
        {
            BGMPlayer.Play(BGMPlayer.BGM.Ending);

            // 画面の覆いを解除
            ScreenTransitionRegistry.StartUncover(1f);
            yield return ScreenTransitionRegistry.WaitAll();

            // 少し待つ
            yield return new WaitForSeconds(4);

            // エンドクレジット
            Time.timeScale = 0.5f;
            CreditsBehaviour.Show(CreditsBehaviour.Mode.EndCredits);
            // TODO CreditsBehaviourを実装したら戻す
            //yield return new WaitUntil(() => CreditsBehaviour.CurrentState == CreditsBehaviour.State.ToShow);
            //yield return new WaitWhile(() => CreditsBehaviour.CurrentState == CreditsBehaviour.State.ToShow);

            Time.timeScale = 1;
        }
    }
}