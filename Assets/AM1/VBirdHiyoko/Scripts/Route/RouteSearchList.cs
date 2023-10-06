//#define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ルート探索の検索データ
    /// </summary>
    public class RouteSearchList
    {
        static int ListMax => 256;

        /// <summary>
        /// 登録されているブロックの数
        /// </summary>
        public int ListCount => blocks.Count;

        List<BlockRouteData> blocks = new List<BlockRouteData>(ListMax);

        /// <summary>
        /// 指定のブロックを探索予定に追加する。
        /// </summary>
        /// <param name="fromBlock">探索元のブロック</param>
        /// <param name="entryBlock">登録するブロック</param>
        /// <param name="from">探索元への方向</param>
        public void Add(BlockRouteData fromBlock, BlockRouteData entryBlock, Direction.Type from)
        {
            // 接続情報を書き込む
            entryBlock.SetLinkedBlock(from, fromBlock);

            VBirdHiyokoManager.Log($"Add Search List {entryBlock.transform.position} Checked={entryBlock.Checked}");

            // すでにリストに登録済み
            if (entryBlock.Checked)
            {
                // 今回の方が歩数が短ければ歩数と登録元を更新して終了
                if (fromBlock.StepCount + 1 < entryBlock.StepCount)
                {
                    VBirdHiyokoManager.Log($"Add {entryBlock.transform.position} 近いので更新 StepCount={fromBlock.StepCount + 1}");
                    entryBlock.SetStepCountAndChecked(fromBlock.StepCount + 1);
                }
                return;
            }

            // リストへ登録
            entryBlock.SetStepCountAndChecked(fromBlock.StepCount + 1);
            VBirdHiyokoManager.Log($"Add {entryBlock.transform.position} リストへ登録 StepCount={fromBlock.StepCount + 1}");
            blocks.Add(entryBlock);
        }

        /// <summary>
        /// 最初の1つ目のブロックを登録する。
        /// </summary>
        /// <param name="entryBlock">登録するブロックのインスタンス</param>
        public void AddFirstBlock(BlockRouteData entryBlock)
        {
            entryBlock.SetStepCountAndChecked(0);
            blocks.Add(entryBlock);
        }

        /// <summary>
        /// リストからブロックを１つ取り出す。
        /// </summary>
        /// <returns>取り出したブロックデータ。取り出すブロックがなければnull</returns>
        public BlockRouteData Get()
        {
            if (blocks.Count == 0)
            {
                return null;
            }

            var block = blocks[0];
            blocks.RemoveAt(0);
            return block;
        }
    }
}