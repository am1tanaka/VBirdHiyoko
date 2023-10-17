using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 氷の床。上のオブジェクトが滑る。
    /// </summary>
    public class IceGround : MonoBehaviour, IBlockInfo
    {
        public bool IsSlippery => true;
    }
}