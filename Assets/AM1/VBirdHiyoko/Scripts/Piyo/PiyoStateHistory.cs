using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴再生のプレイヤーの状態。
    /// </summary>
    public class PiyoStateHistory : AM1StateQueueBase
    {
        public enum State
        {
            None = -1,
            Standby,
            Moving,
        }

        public State CurrentState { get; private set; } = State.None;

        public override void Init()
        {
            base.Init();

            InputActionDetector.Instance.OnAction.AddListener(OnAction);
            InputActionDetector.Instance.UpdatePointer();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public override void Update()
        {
            CommandQueue.Update();
            base.Update();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            HistoryPlayer.Update();
        }

        /// <summary>
        /// クリックを受け取る
        /// </summary>
        /// <param name="hit">クリックしたオブジェクトの当たり判定</param>
        public void OnAction(RaycastHit hit)
        {
            OnAction(hit.collider.GetComponent<BlockRouteData>());
        }

        /// <summary>
        /// 指定のブロックのクリックを受け取る。
        /// </summary>
        /// <param name="blockRouteData">クリックしたオブジェクト</param>
        public void OnAction(BlockRouteData blockRouteData)
        {
            if (blockRouteData == null) return;

            VBirdHiyokoManager.Log($"OnAction({blockRouteData.name}) {blockRouteData.CenterTop}");

            var playerActionCommand = PiyoBehaviour.Instance.GetInstance<PiyoActionCommand>();
            playerActionCommand.clickedBlockRouteData = blockRouteData;
            CommandQueue.EntryCommand(PiyoBehaviour.Instance.GetInstance<PiyoAcceptCommand>());
        }

        public override void Terminate()
        {
            base.Terminate();
            CommandQueue.Update();
            InputActionDetector.Instance.OnAction.RemoveListener(OnAction);
        }
    }
}