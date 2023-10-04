using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴の記録を統括するクラス。
    /// TODO: Unity2021.3から使えるObjectPoolで代替可能な機能がある。
    /// </summary>
    public static class HistoryRecorder
    {
        /// <summary>
        /// 履歴保存用クラス
        /// </summary>
        public static IHistorySaver historySaver;

        /// <summary>
        /// 履歴読み込み用クラス
        /// </summary>
        public static IHistoryLoader historyLoader;

        /// <summary>
        /// 履歴データの配列。
        /// </summary>
        public static HistoryData[] HistoryArray { get; private set; }

        /// <summary>
        /// 1回の移動分を表すカウンター。
        /// </summary>
        public static int Counter { get; private set; }

        /// <summary>
        /// 最新状態へのリスト
        /// </summary>
        static readonly LinkedList<HistoryData> toLatestList = new LinkedList<HistoryData>();

        /// <summary>
        /// 最新状態へのデータの配列
        /// </summary>
        public static HistoryData[] ToLatestArray => toLatestList.ToArray<HistoryData>();

        /// <summary>
        /// 環境を初期化する。
        /// </summary>
        public static void Init()
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 移動が完了したら呼び出す。
        /// </summary>
        public static void MoveDone()
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 最新状態への情報を更新して、配列化して返す。
        /// </summary>
        /// <returns>開始してから現在の状態への変化情報の配列</returns>
        public static HistoryData[] UpdateToLatestArray()
        {
            Debug.Log("未実装");
            return null;
        }

        /// <summary>
        /// 指定のステップ数に該当する最初の配列へのインデックスを返す。
        /// </summary>
        /// <param name="step">取り出したいステップ数</param>
        /// <returns>該当するインデックス。対応するものがなければ-1</returns>
        public static int GetStepIndex(int step)
        {
            Debug.Log("未実装");
            return -1;
        }

        /// <summary>
        /// 履歴を読み込む。
        /// </summary>
        /// <returns>保存データを読んだら true。ないかLoaderが未設定なら false</returns>
        public static bool Load(int stage, out ushort innerStep)
        {
            Debug.Log("未実装");

            innerStep = 0;
            return true;
        }

        /// <summary>
        /// 履歴を保存する。
        /// </summary>
        /// <param name="stage">保存するステージ数</param>
        /// <param name="innerStep">現在の内部ステップ数</param>
        public static void Save(int stage, ushort innerStep)
        {
            Debug.Log("未実装");
        }

    }
}