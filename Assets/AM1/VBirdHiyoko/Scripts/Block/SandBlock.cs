using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 砂ブロックの制御スクリプト
    /// </summary>
    public class SandBlock : MoveBlockBase, IPlayerExit
    {
        [Tooltip("崩れ効果音"), SerializeField]
        AudioClip breakSE = default;
        [Tooltip("戻り効果音"), SerializeField]
        AudioClip restoreSE = default;
        [Tooltip("破片パーティクル"), SerializeField]
        ParticleSystem debrisParticle = default;
        [Tooltip("煙パーティクル"), SerializeField]
        ParticleSystem smokeParticle = default;

        /// <summary>
        /// 履歴再生時のアニメ倍速
        /// </summary>
        static float HistoryAnimationSpeed => 3;

        HistoryBehaviour HistoryBehaviourInstance => historyBehaviour ? historyBehaviour : historyBehaviour = GetComponent<HistoryBehaviour>();

        Animator animator;
        Animator AnimatorInstance => animator ? animator : animator = GetComponent<Animator>();

        /// <summary>
        /// 足元が氷かを確認する。
        /// </summary>
        public override bool CanContinue => IsOnIce();

        public override bool StartPush(Vector3 direction)
        {
            bool result = base.StartPush(direction);
            if (result)
            {
                // 履歴状態を通常に設定してから履歴記録
                HistoryBehaviourInstance.State = 0;
            }
            return result;
        }

        /// <summary>
        /// プレイヤーがどいたら呼び出す
        /// </summary>
        public void OnExit()
        {
            // 動作に登録
            BlockMoveObserver.Add(this);
            HistoryStartMove();

            // 壊れ開始
            audioSource.PlayOneShot(breakSE);
            AnimatorInstance.SetFloat("Speed", 1);
            AnimatorInstance.SetBool("Break", true);
            debrisParticle.Play();
            smokeParticle.Play();
        }

        /// <summary>
        /// 壊れきったら、アニメから呼ばれる。
        /// </summary>
        public void BreakDone()
        {
            // 崩れ状態設定
            HistoryBehaviourInstance.State = 1;

            // 動作を解除
            BlockMoveObserver.Remove(this);
        }

        /// <summary>
        /// 履歴時の壊れアニメ。
        /// </summary>
        public void HistoryBreak()
        {
            if (AntiSameAudioPlayer.CanPlay(breakSE))
            {
                audioSource.PlayOneShot(breakSE);
            }
            BreakWhenLoad();
        }

        /// <summary>
        /// 読み込み時の壊れ設定。
        /// </summary>
        public void BreakWhenLoad()
        {
            AnimatorInstance.SetFloat("Speed", HistoryAnimationSpeed);
            AnimatorInstance.SetBool("Break", true);
            HistoryBehaviourInstance.State = 1;
        }

        /// <summary>
        /// 復活
        /// </summary>
        public void Restore()
        {
            if (AntiSameAudioPlayer.CanPlay(restoreSE))
            {
                audioSource.PlayOneShot(restoreSE);
            }
            AnimatorInstance.SetFloat("Speed", HistoryAnimationSpeed);
            AnimatorInstance.SetBool("Break", false);
            HistoryBehaviourInstance.State = 0;
            smokeParticle.Play();
        }
    }
}