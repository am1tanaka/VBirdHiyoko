using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ゲームプレイの状態キューの管理クラス
    /// </summary>
    public class GamePlayStateQueue : AM1StateQueue
    {
        public static GamePlayStateQueue Instance { get; private set; }

        [Tooltip("歩数"), SerializeField]
        StepText stepText = default;

        static InstanceDictionary instanceDictionary = new InstanceDictionary();

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// ゲームシーンを初期化。
        /// </summary>
        public void Init()
        {
            stepText.Init();
        }

        /// <summary>
        /// 指定の方のインスタンスを返す。
        /// 未登録の時は生成した上でそのインスタンスを返す。
        /// </summary>
        /// <typeparam name="T">返す型。</typeparam>
        /// <returns>インスタンス</returns>
        public void Enqueue<T>() where T : AM1StateQueueBase, new()
        {
            Enqueue(instanceDictionary.GetOrNew<T>());
        }
    }
}