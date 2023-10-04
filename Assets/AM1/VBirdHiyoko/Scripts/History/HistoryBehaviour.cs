using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴に対応させるオブジェクトにアタッチするクラス。
    /// </summary>
    public class HistoryBehaviour : MonoBehaviour
    {
        /// <summary>
        /// オブジェクトの識別子
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// eulerAnglesのYを0～3の方向に変換。
        /// </summary>
        public static int EulerYToDir(float y) => Mathf.RoundToInt(Mathf.Repeat(y / 90f, 4f));

        /// <summary>
        /// 現在の状態
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 移動開始前に呼び出して、状態を登録。
        /// </summary>
        public void StartMove()
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 移動完了時に呼び出す。移動結果を登録する。
        /// </summary>
        /// <param name="data">差分を記録する先のインスタンス</param>
        /// <returns>変化があって data を設定した時、true</returns>
        public bool MoveDone(ref HistoryData data)
        {
            Debug.Log("未実装");
            return false;
        }

        /// <summary>
        /// 開始状態から最新状態までの差分を取得する。
        /// Stateの更新が必要なのでMoveDone()を呼んだあとに呼ぶ。
        /// </summary>
        /// <param name="data">記録先のインスタンス</param>
        /// <returns>変化があって data を設定した時、true</returns>
        public bool ToLatestData(ref HistoryData data)
        {
            Debug.Log("未実装");
            return false;
        }
    }
}