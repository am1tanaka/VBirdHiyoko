using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴データ。
    /// データの記録と、バイト配列にして返す機能を提供。
    /// </summary>
    public class HistoryData
    {
        public enum Type
        {
            Step,
            ObjectId,
            DX,
            DY,
            DZ,
            EulerYAndState,
        }

        /// <summary>
        /// データのバイト配列長
        /// </summary>
        public static int DataSize => 6;

        /// <summary>
        /// ステップ数。保存時は0-127に丸め込む
        /// </summary>
        public int step;

        /// <summary>
        /// オブジェクトID
        /// </summary>
        public int ObjectId { get; private set; }

        /// <summary>
        /// 前回座標からの相対移動量
        /// </summary>
        Vector3Int relativePosition;
        public Vector3Int RelativePosition => relativePosition;

        /// <summary>
        /// 前回からの回転角度。0=なし、1=90度、2=180度、3=-90度
        /// </summary>
        public int EulerY { get; private set; }

        /// <summary>
        /// 状態
        /// </summary>
        public int State { get; private set; }

        readonly sbyte[] bytes = new sbyte[6];

        /// <summary>
        /// データを sbyte 配列にパックして返す。
        /// </summary>
        /// <returns>パックしたデータ配列</returns>
        public sbyte[] GetSBytes()
        {
            bytes[(int)Type.Step] = (sbyte)(step & 0x7f);
            bytes[(int)Type.ObjectId] = (sbyte)ObjectId;
            bytes[(int)Type.DX] = (sbyte)RelativePosition.x;
            bytes[(int)Type.DY] = (sbyte)RelativePosition.y;
            bytes[(int)Type.DZ] = (sbyte)RelativePosition.z;
            bytes[(int)Type.EulerYAndState] = (sbyte)(EulerY | (State << 2));
            return bytes;
        }

        /// <summary>
        /// 指定の sbyte 配列からデータを取得する。
        /// </summary>
        /// <param name="data">配列インスタンス</param>
        /// <param name="index">読み込み開始インデックス</param>
        public void SetSByte(sbyte[] data, int index)
        {
            step = data[index + (int)Type.Step] & 0x7f;
            ObjectId = data[index + (int)Type.ObjectId];
            relativePosition.x = data[index + (int)Type.DX];
            relativePosition.y = data[index + (int)Type.DY];
            relativePosition.z = data[index + (int)Type.DZ];
            EulerY = data[index + (int)Type.EulerYAndState] & 0x03;
            State = data[index + (int)Type.EulerYAndState] >> 2;
        }

        /// <summary>
        /// オブジェクトID, 相対移動量, ワールドでの向き, 状態を指定してデータを設定する。
        /// </summary>
        /// <param name="objId">オブジェクトID</param>
        /// <param name="relPos">相対移動量</param>
        /// <param name="eY">0～3でオイラーYの回転方向</param>
        /// <param name="st">状態</param>
        public void Set(int objId, Vector3Int relPos, int eY, int st = 0)
        {
            ObjectId = objId;
            relativePosition = relPos;
            EulerY = eY;
            State = st;
        }
    }
}