using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 移動中のブロックを監視するクラス
    /// </summary>
    public static class BlockMoveObserver
    {
        static List<IMovableBlock> moveBlocks = new List<IMovableBlock>();

        /// <summary>
        /// すべてのブロックが動作停止指定れば true
        /// </summary>
        public static bool IsAllBlocksMoveDone => moveBlocks.Count == 0;

        /// <summary>
        /// 移動するブロックをリストに追加する
        /// </summary>
        /// <param name="block">登録するブロック</param>
        public static void Add(IMovableBlock block)
        {
            moveBlocks.Add(block);
        }

        /// <summary>
        /// 指定のブロックをリストから削除する。
        /// </summary>
        /// <param name="block">削除するブロック</param>
        /// <returns>移動ブロックがなくなったら、true</returns>
        public static bool Remove(IMovableBlock block)
        {
            moveBlocks.Remove(block);
            return moveBlocks.Count == 0;
        }
    }
}