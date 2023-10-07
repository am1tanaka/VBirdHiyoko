using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 音の同時再生を禁止するクラス
    /// </summary>
    public static class AntiSameAudioPlayer
    {
        /// <summary>
        /// インターバルを省略した時のデフォルト値
        /// </summary>
        const float DefaultInterval = 0.1f;

        static Dictionary<int, float> lastPlayTime = new Dictionary<int, float>();

        /// <summary>
        /// 指定の音を鳴らして良いかを確認する。
        /// </summary>
        /// <param name="audioClip">鳴らしたい対象の音</param>
        /// <param name="interval">鳴らしてから禁じる秒数</param>
        /// <returns>鳴らして良い時、true</returns>
        public static bool CanPlay(AudioClip audioClip, float interval = DefaultInterval)
        {
            int instanceId = audioClip.GetInstanceID();

            // ないので再生
            if (lastPlayTime.ContainsKey(instanceId) == false)
            {
                lastPlayTime.Add(instanceId, Time.realtimeSinceStartup + interval);
                return true;
            }

            // あるので経過時間を確認
            if (lastPlayTime[instanceId] > Time.realtimeSinceStartup)
            {
                return false;
            }

            // 更新
            lastPlayTime[instanceId] = Time.realtimeSinceStartup + interval;
            return true;
        }
    }
}