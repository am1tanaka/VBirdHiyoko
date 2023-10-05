using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    [RequireComponent(typeof(AudioSource))]
    /// <summary>
    /// 足音とジャンプ音
    /// </summary>
    public class PiyoAudio : MonoBehaviour
    {
        [Tooltip("足音の音源"), SerializeField]
        AudioClip[] footStamps = default;
        [Tooltip("ジャンプ音"), SerializeField]
        AudioClip jump = default;

        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// 足音をランダムで鳴らす
        /// </summary>
        public void PlayFootStamp()
        {
            int rand = Random.Range(0, footStamps.Length);
            audioSource.PlayOneShot(footStamps[rand]);
        }

        /// <summary>
        /// ジャンプ音
        /// </summary>
        public void PlayJump()
        {
            audioSource.PlayOneShot(jump);
        }
    }
}