using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 歩数をカウントするクラス。PlayerBehaviourで生成と管理。
    /// 最新歩数と一時歩数を管理する。
    /// 内部歩数は歩数超過を正しく扱うために、ゲーム内で利用する歩数よりも大きい値を扱う。
    /// メソッドでは内部歩数を更新して、戻り値はゲーム内歩数を返す。
    /// 値の変更を試みた時にイベントを発行する。
    /// 初期化の簡略化と、頻度が低く処理速度への影響は無視できるので、値が変更されなくてもイベントを発行する。
    /// </summary>
    public class StepCounter
    {
        /// <summary>
        /// 指定の歩数を設定する。
        /// </summary>
        /// <param name="step">設定する歩数</param>
        public void SetInnerStep(int step)
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 現在の歩数を一歩増やす。一時歩数も一致させる。
        /// </summary>
        /// <returns>更新したゲーム内歩数を返す。</returns>
        public int IncrementAll()
        {
            Debug.Log("未実装");
            return 0;
        }
    }
}