using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// プレイヤーの操作アクション
    /// </summary>
    public class PiyoActionCommand : ICommandQueueData
    {
        public CommandInputType Type => CommandInputType.Game;
        public int Priority => 0;

        /// <summary>
        /// クリックしたブロックのインスタンス。nullなら押されていない。
        /// </summary>
        public BlockRouteData clickedBlockRouteData { private get; set; }

        HistoryBehaviour historyBehaviour;
        HistoryBehaviour HistoryBehaviourInstance
        {
            get
            {
                if (historyBehaviour == null)
                {
                    historyBehaviour = PiyoBehaviour.Instance.GetComponent<HistoryBehaviour>();
                }
                return historyBehaviour;
            }
        }

        public void Invoke()
        {
            if (clickedBlockRouteData == null)
            {
                return;
            }

            // 足元にブロックがあって、クリックしたところにデータがあって、歩数があるとき、歩ける
            if (clickedBlockRouteData.CanWalk)
            {
                // 歩けるなら移動
                if (PiyoBehaviour.Instance.GetInstance<PiyoStateWalk>().SetWalk(clickedBlockRouteData))
                {
                    HistoryRecorder.StartMove(HistoryBehaviourInstance);
                    CommandQueue.ChangeInputMask(CommandInputType.None);
                    SEPlayer.Play(SEPlayer.SE.Click);
                }
            }
            else
            {
                // 押せるか周辺へ歩けるか何もしないか
                PushOrWalkCheck(clickedBlockRouteData);
            }

            InputActionDetector.Instance.UpdatePointer();
            clickedBlockRouteData = null;
        }

        /// <summary>
        /// 隣なら押せるかを確認。
        /// 離れているなら隣の足場への移動を確認。
        /// 反応ができなければ何もしない
        /// </summary>
        /// <param name="blockRouteData">確認するブロック</param>
        void PushOrWalkCheck(BlockRouteData blockRouteData)
        {
            if (blockRouteData.IsNextToThePlayer)
            {
                // プレイヤーの隣の時、押せるかを確認
                if (blockRouteData.CanPushFrom(PiyoBehaviour.Instance.RigidbodyPosition))
                {
                    Vector3 to = blockRouteData.transform.position - PiyoBehaviour.Instance.RigidbodyPosition;
                    var dir = Direction.DetectType(to);
                    if (dir != Direction.Type.Turning)
                    {
                        if (PiyoBehaviour.Instance.GetInstance<PiyoStatePush>().SetPush(blockRouteData, dir))
                        {
                            // 押す処理へ
                            HistoryRecorder.StartMove(HistoryBehaviourInstance);
                            CommandQueue.ChangeInputMask(CommandInputType.None);
                            SEPlayer.Play(SEPlayer.SE.Click);
                        }
                    }
                }
                else
                {
                    // 押せなかった
                    PiyoBehaviour.Instance.GetInstance<PiyoTalker>().Talk(PiyoTalker.Type.TooHeavy);
                    SEPlayer.Play(SEPlayer.SE.Cancel);
                }
            }
            else
            {
                bool toWalk = false;

                // 離れている。隣にある受ける足場があるか
                var nearBlock = blockRouteData.GetNearWalkFloorBlock();
                if (nearBlock)
                {
                    if (PiyoBehaviour.Instance.GetInstance<PiyoStateWalk>().SetWalk(nearBlock))
                    {
                        // 歩きへ
                        HistoryRecorder.StartMove(HistoryBehaviourInstance);
                        CommandQueue.ChangeInputMask(CommandInputType.None);
                        toWalk = true;
                        SEPlayer.Play(SEPlayer.SE.Click);
                    }
                }

                // 歩かない時、メッセージ
                if (!toWalk)
                {
                    PiyoBehaviour.Instance.GetInstance<PiyoTalker>().Talk(PiyoTalker.Type.CantGo);
                    SEPlayer.Play(SEPlayer.SE.Cancel);
                }
            }
        }
    }
}