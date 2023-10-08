using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 砂ブロックの履歴再生Behaviour
    /// </summary>
    public class SandHistoryPlayer : HistoryPlayerBase, IHistoryPlayer
    {
        SandBlock sandBlock;

        protected override void Start()
        {
            base.Start();
            sandBlock = GetComponent<SandBlock>();
            HistoryBehaviourInstance.DataLoaded.AddListener(DataLoaded);
        }

        void OnDestroy()
        {
            HistoryBehaviourInstance.DataLoaded.RemoveListener(DataLoaded);
        }

        /// <summary>
        /// 状態変更時の砂壊れを設定する。
        /// </summary>
        /// <param name="data">設定データ</param>
        void DataLoaded(HistoryData data)
        {
            if (data.State == 1)
            {
                sandBlock.BreakWhenLoad();
            }
        }

        public void SetMove(HistoryData data, IHistoryPlayer.Mode mode, float sec, bool isStart)
        {
            isPlaying = true;

            if (isStart)
            {
                // 開始位置とゴールを設定してアニメ
                SetStartMove(data, mode, sec);

                // 状態変更あり
                if (data.State == 1)
                {
                    if (mode == IHistoryPlayer.Mode.Undo)
                    {
                        // Undoなら復帰
                        sandBlock.Restore();
                    }
                    else
                    {
                        // Redoなら再破壊
                        sandBlock.HistoryBreak();
                    }
                }
            }
            else
            {
#if UNITY_EDITOR
                // 現在の状態で経過秒数だけ変更
                VBirdHiyokoManager.Log($"現在の状態で経過秒数だけ変更");
#endif
            }
        }
    }
}