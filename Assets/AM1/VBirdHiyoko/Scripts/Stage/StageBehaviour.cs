using UnityEngine;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// Stageシーンを管理するクラス。
    /// </summary>
    public class StageBehaviour : MonoBehaviour
    {
        public static StageBehaviour Instance { get; private set; }

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// ステージの初期化処理
        /// </summary>
        public void Init()
        {
            Debug.Log("未実装");
        }
    }
}