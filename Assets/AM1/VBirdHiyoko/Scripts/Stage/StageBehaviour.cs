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

        [Tooltip("ステージ管理関連のインスタンス"), SerializeField]
        private StageInstances stageInstances = default;

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
            PiyoBehaviour.Instance.SetStageInstances(stageInstances);
            stageInstances.targetCursor.Init();
            stageInstances.targetFlag.Hide();
            stageInstances.pushArrows.Hide();
            stageInstances.worldPointer.Init();
        }
    }
}