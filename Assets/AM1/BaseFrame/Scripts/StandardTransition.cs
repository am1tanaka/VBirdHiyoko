using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AM1.BaseFrame.Assets
{
    /// <summary>
    /// 標準的な画面切り替え処理を管理
    /// </summary>
    public class StandardTransition : MonoBehaviour, IScreenTransition
    {
        [Tooltip("画面切り替えの種類"), SerializeField]
        ScreenTransitionType type = default;

        Animator anim;
        public bool IsTransitioning { get; private set; }

        public bool IsCover
        {
            get
            {
                return anim.GetBool("Cover");
            }
        }

        void Awake()
        {
            anim = GetComponent<Animator>();
            anim.SetBool("IsImmediate", true);
            anim.SetBool("Cover", false);
            ScreenTransitionRegistry.Register((int)type, this);
        }

        /// <summary>
        /// 色変更つき処理
        /// </summary>
        /// <param name="cover">覆いたい時、true</param>
        /// <param name="col">色</param>
        /// <param name="sec">秒数</param>
        /// <returns>成功した時true</returns>
        public bool StartCover(bool cover, Color col, float sec = 0)
        {
            var image = GetComponent<Image>();
            if (image != null)
            {
                image.color = col;
            }
            return StartCover(cover, sec);
        }

        public bool StartCover(bool cover, float sec = 0)
        {
            // 処理中の時、同じ方向の処理が開始していたら失敗
            if (IsTransitioning && (cover == IsCover))
            {
                return false;
            }

            // 即時か、遷移済みですでに閉じていたら何もしない
            if ((sec <= 1f / 60f) || (!IsTransitioning && (anim.GetBool("Cover") == cover)))
            {
                IsTransitioning = false;
            }
            else
            {
                // アニメ
                IsTransitioning = true;
                anim.SetFloat("Speed", 1f / sec);
            }
            anim.SetBool("IsImmediate", !IsTransitioning);
            anim.SetBool("Cover", cover);
            return true;
        }

        public void AnimDone()
        {
            IsTransitioning = false;
        }

        public IEnumerator Wait()
        {
            yield return new WaitUntil(() => !IsTransitioning);
        }
    }
}