using AM1.MessageSystem;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ぴよの状態で発話させるクラス
    /// </summary>
    public sealed class PiyoTalker
    {
        public enum Type
        {
            CantGo,
            TooHeavy,
            CantMove,
        }

        static readonly MessageData[] messageData =
        {
            new MessageData(Type.CantGo.ToString(), 2),
            new MessageData(Type.TooHeavy.ToString(), 2),
            new MessageData(Type.CantMove.ToString(), 2),
        };

        /// <summary>
        /// 内容を指定して発話する。
        /// </summary>
        /// <param name="type">セリフの種類</param>
        public void Talk(Type type)
        {
            MessageWindow.Instance.Show(messageData[(int)type]);
        }
    }
}