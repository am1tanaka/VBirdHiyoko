using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 標準的な履歴再生用のBehaviour
    /// </summary>
    public class StandardHistoryPlayer : HistoryPlayerBase, IHistoryPlayer
    {
        public void SetMove(HistoryData data, IHistoryPlayer.Mode mode, float sec, bool isStart)
        {
            isPlaying = true;

            if (isStart)
            {
                // 開始位置とゴールを設定してアニメ
                SetStartMove(data, mode, sec);
                VBirdHiyokoManager.Log($"startPos={startPosition} target={targetPosition} rel={data.RelativePosition} state={data.State}");
            }
            else
            {
                // 現在の状態で経過秒数だけ変更
                VBirdHiyokoManager.Log($"現在の状態で経過秒数だけ変更");
            }
        }
    }
}