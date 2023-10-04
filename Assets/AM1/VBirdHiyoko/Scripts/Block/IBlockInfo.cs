namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 移動する、しないに関わらないブロックの情報を提供するインターフェース
    /// </summary>
    public interface IBlockInfo
    {
        /// <summary>
        /// このブロックがすべるかどうか。
        /// </summary>
        bool IsSlippery => false;
    }
}