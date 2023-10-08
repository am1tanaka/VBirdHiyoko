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
        /// カウンターをループさせるためのビット積値。
        /// </summary>
        public static int CounterAnd => 127;

        /// <summary>
        /// 履歴の記録上限。超過検出の都合でシーケンス番号より小さい値にする。
        /// </summary>
        static int HistoryDataMax => 100;

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
        /// 最新状態へのデータの配列
        /// </summary>
        public static HistoryData[] ToLatestArray => toLatestList.ToArray<HistoryData>();

        /// <summary>
        /// 移動中のオブジェクトのリスト
        /// </summary>
        static readonly List<HistoryBehaviour> movingObjects = new List<HistoryBehaviour>(HistoryObjectList.RegisterMax);

        /// <summary>
        /// 最新状態へのリスト
        /// </summary>
        static readonly LinkedList<HistoryData> toLatestList = new LinkedList<HistoryData>();

        /// <summary>
        /// 履歴データを使い回す時に貯めておくためのプール。
        /// </summary>
        static readonly LinkedList<HistoryData> historyDataPool = new LinkedList<HistoryData>();

        /// <summary>
        /// 履歴データを記録していくリスト。
        /// </summary>
        static readonly LinkedList<HistoryData> historyDataList = new LinkedList<HistoryData>();

        /// <summary>
        /// 環境を初期化する。
        /// </summary>
        public static void Init()
        {
            HistoryObjectList.Init();
            movingObjects.Clear();
            Counter = 0;
            AllListToPool();
        }

        /// <summary>
        /// 記録と最新データのリストをプールに戻す。
        /// </summary>
        static void AllListToPool()
        {
            while (historyDataList.Count > 0)
            {
                historyDataPool.AddLast(historyDataList.First.Value);
                historyDataList.RemoveFirst();
            }
            ReleaseToLatestListToPool();
        }

        /// <summary>
        /// 最新状態へのリストを全てプールに戻す。
        /// </summary>
        static void ReleaseToLatestListToPool()
        {
            while (toLatestList.Count > 0)
            {
                historyDataPool.AddLast(toLatestList.First.Value);
                toLatestList.RemoveFirst();
            }
        }

        /// <summary>
        /// 移動開始したオブジェクトを受け取って記録しておく。
        /// </summary>
        /// <param name="behaviour">移動を開始したオブジェクト</param>
        public static void StartMove(HistoryBehaviour behaviour)
        {
            if (!movingObjects.Contains(behaviour))
            {
                movingObjects.Add(behaviour);
                behaviour.StartMove();
            }
        }

        /// <summary>
        /// 移動が完了したら呼び出す。
        /// </summary>
        public static void MoveDone()
        {
            if (movingObjects.Count == 0) return;

            Counter = (Counter + 1) & CounterAnd;
            toLatestList.Clear();

            HistoryData result = null;
            for (int i = 0; i < movingObjects.Count; i++)
            {
                if (result == null)
                {
                    result = GetBlankHistoryData();
                    result.step = Counter;
                }

                if (movingObjects[i].MoveDone(ref result))
                {
                    // 履歴を記録
                    historyDataList.AddLast(result);
                    result = null;
                }
            }

            // 余ったらプールに戻す
            if (result != null)
            {
                ReleaseHistoryDataToPool(result);
            }

            // 移動オブジェクトをクリア
            movingObjects.Clear();

            // 登録数オーバーの処理
            RemoveOverData();

            // 配列化
            HistoryArray = null;
            HistoryArray = historyDataList.ToArray<HistoryData>();
        }

        /// <summary>
        /// データ登録数が規定数をオーバーしていたら、古いデータを削除する。
        /// </summary>
        static void RemoveOverData()
        {
            int d = historyDataList.Last.Value.step - historyDataList.First.Value.step + 1;
            if (d < 0)
            {
                d += (CounterAnd + 1);
            }
            if (d <= HistoryDataMax) return;

            // 対象のシーケンスIDまで削除する
            // 最後が127の時、28-127を残す。よって、28が出たらループを停止したい
            int targetStep = historyDataList.Last.Value.step - HistoryDataMax + 1;
            if (targetStep < 0)
            {
                targetStep += (CounterAnd + 1);
            }
            while (historyDataList.First.Value.step != targetStep)
            {
                ReleaseHistoryDataToPool(historyDataList.First.Value);
                historyDataList.RemoveFirst();
            }
        }

        /// <summary>
        /// 利用可能な履歴データを返す。
        /// プールに余裕があればプールから取り出し、なければ新規作成する。
        /// </summary>
        /// <returns>プールから取り出したか、新規作成したHistoryDataのインスタンス</returns>
        static HistoryData GetBlankHistoryData()
        {
            if (historyDataPool.Count > 0)
            {
                var data = historyDataPool.First.Value;
                historyDataPool.RemoveFirst();
                return data;
            }
            else
            {
                return new HistoryData();
            }
        }

        /// <summary>
        /// 指定の履歴データをプールに戻す。
        /// </summary>
        /// <param name="data">戻したい履歴データのインスタンス</param>
        static void ReleaseHistoryDataToPool(HistoryData data)
        {
            historyDataPool.AddLast(data);
        }

        /// <summary>
        /// 最新状態への情報を更新して、配列化して返す。
        /// </summary>
        /// <returns>開始してから現在の状態への変化情報の配列</returns>
        public static HistoryData[] UpdateToLatestArray()
        {
            ReleaseToLatestListToPool();

            HistoryData result = null;
            for (int i = 0; i < HistoryObjectList.Count; i++)
            {
                if (result == null)
                {
                    result = GetBlankHistoryData();
                }
                if (HistoryObjectList.objectList[i].ToLatestData(ref result))
                {
                    // 変更があったので記録
                    toLatestList.AddLast(result);
                    result = null;
                }
            }

            // 余ったらプールに戻す
            if (result != null)
            {
                ReleaseHistoryDataToPool(result);
            }

            return ToLatestArray;
        }

        /// <summary>
        /// 指定のステップ数に該当する最初の配列へのインデックスを返す。
        /// </summary>
        /// <param name="step">取り出したいステップ数</param>
        /// <returns>該当するインデックス。対応するものがなければ-1</returns>
        public static int GetStepIndex(int step)
        {
            int target = step & CounterAnd;
            for (int i = 0; i < HistoryArray.Length; i++)
            {
                if (HistoryArray[i].step == target)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Undoを確定した時に呼び出す。
        /// 引数のインデックス以降の履歴を削除する。
        /// </summary>
        /// <param name="index">このインデックスから後ろの履歴を削除する</param>
        public static void UndoAccept(int index)
        {
            if (index < 0 || index >= HistoryArray.Length) return;

            // プールに戻す
            while (historyDataList.Count > index)
            {
                historyDataPool.AddLast(historyDataList.Last.Value);
                historyDataList.RemoveLast();
            }

            // 配列化
            HistoryArray = null;
            HistoryArray = historyDataList.ToArray<HistoryData>();
            if (HistoryArray.Length > 0)
            {
                Counter = HistoryArray[HistoryArray.Length - 1].step;
            }
            else
            {
                Counter = 0;
            }

            // 最新情報を更新
            UpdateToLatestArray();
        }

        /// <summary>
        /// 履歴を読み込む。
        /// </summary>
        /// <returns>保存データを読んだら true。ないかLoaderが未設定なら false</returns>
        public static bool Load(int stage, out ushort innerStep)
        {
            AllListToPool();
            innerStep = 0;

            if (historyLoader == null) return false;

            sbyte[] history;
            sbyte[] toLatest;
            if (!historyLoader.Load(stage, out innerStep, out history, out toLatest))
            {
                return false;
            }
            if (history.Length == 0)
            {
                return false;
            }

            // 履歴へ反映
            for (int i = 0; i < history.Length; i += HistoryData.DataSize)
            {
                var data = GetBlankHistoryData();
                data.SetSByte(history, i);
                historyDataList.AddLast(data);
            }
            Counter = history[history.Length - HistoryData.DataSize];

            // 最新状態へ反映
            for (int i = 0; i < toLatest.Length; i += HistoryData.DataSize)
            {
                var data = GetBlankHistoryData();
                data.SetSByte(toLatest, i);
                toLatestList.AddLast(data);
                HistoryObjectList.LatestTransform(data);
            }

            return true;
        }

        /// <summary>
        /// 履歴を保存する。
        /// </summary>
        /// <param name="stage">保存するステージ数</param>
        /// <param name="innerStep">現在の内部ステップ数</param>
        public static void Save(int stage, ushort innerStep)
        {
            VBirdHiyokoManager.Log($"Save({stage}, {innerStep}) {historySaver} / {historyDataList.Count}");
            if (historySaver == null) return;
            if (historyDataList.Count == 0) return;

            List<sbyte> historySBytes = new List<sbyte>();
            for (int i = 0; i < HistoryArray.Length; i++)
            {
                var data = HistoryArray[i].GetSBytes();
                for (int j = 0; j < data.Length; j++)
                {
                    historySBytes.Add(data[j]);
                }
            }

            var toLatestArray = UpdateToLatestArray();
            List<sbyte> toLatestSBytes = new List<sbyte>();
            for (int i = 0; i < toLatestArray.Length; i++)
            {
                var data = toLatestArray[i].GetSBytes();
                for (int j = 0; j < data.Length; j++)
                {
                    toLatestSBytes.Add(data[j]);
                }
            }

            historySaver.Save(
                stage,
                innerStep,
                historySBytes.ToArray(),
                toLatestSBytes.ToArray());

            historySBytes.Clear();
            toLatestSBytes.Clear();
        }
    }
}