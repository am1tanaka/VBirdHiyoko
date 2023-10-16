using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 氷ブロックの制御クラス
    /// </summary>
    public class IceBlock : MoveBlockBase, IMovableBlock, IBlockInfo
    {
        /// <summary>
        /// 氷は常に滑る
        /// </summary>
        public override bool CanContinue => true;

        public bool IsSlippery => true;
    }
}