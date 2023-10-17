using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class StandardBlockMove : MoveBlockBase, IMovableBlock, IBlockInfo
    {
        /// <summary>
        /// 足元が氷かを確認する。
        /// </summary>
        public override bool CanContinue => IsOnIce();
    }
}