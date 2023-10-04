using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ゲーム全体で利用するインスタンスやデータを管理するstaticクラス。
    /// </summary>
    public static class VBirdHiyokoManager
    {
        static InstanceDictionary instanceDictionary;

        /// <summary>
        /// 現在プレイ中のステージ
        /// </summary>
        public static Stage CurrentStage { get;private set; } = new Stage();

        /// <summary>
        /// クリア済みステージ
        /// </summary>
        public static Stage ClearedStage { get; private set; } = new Stage();

        /// <summary>
        /// 初期化
        /// </summary>
        public static void Init()
        {
            instanceDictionary = new InstanceDictionary();
            var historyMemory = new HistoryMemoryStorage();
            instanceDictionary.Register<IHistoryLoader>(historyMemory);
            instanceDictionary.Register<IHistorySaver>(historyMemory);
            instanceDictionary.Register<IGameDataStorage>(new VBirdHiyokoPlayerPrefs());
        }

        /// <summary>
        /// 指定の型のインスタンスを返す。
        /// </summary>
        /// <typeparam name="T">取り出したいクラス</typeparam>
        /// <returns>取り出したインスタンス</returns>
        public static T GetInstance<T>()
        {
            return instanceDictionary.Get<T>();
        }

        [System.Diagnostics.Conditional("DEBUG_LOG")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }
    }
}