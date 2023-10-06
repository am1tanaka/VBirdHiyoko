#define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;
using AM1.CommandSystem;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 操作待ち状態
    /// </summary>
    public class PiyoStateWaitInput : AM1StateQueueBase
    {
        /// <summary>
        /// 入力待ちになったら true
        /// </summary>
        public bool IsWaitInput =>
            PiyoBehaviour.Instance.IsState<PiyoStateWaitInput>()
            && CommandQueue.CurrentInputMask.HasFlag(CommandInputType.Game)
            && !CommandQueue.IsSetNextCommand;

        public override void Init()
        {
            base.Init();

            CommandQueue.ChangeInputMask(CommandInputType.None);
            PiyoBehaviour.Instance.StartCoroutine(UpdateWaitInput());
        }

        IEnumerator UpdateWaitInput()
        {
            // 落下
            yield return PiyoBehaviour.Instance.Mover.Stand();

            // ブロックがすべて停止するのを待つ
            yield return new WaitUntil(() => BlockMoveObserver.IsAllBlocksMoveDone);

            // コマンドがあれば実行
            if (PiyoBehaviour.Instance.IsAddedScenario)
            {
                // 終了したらこの状態に戻る
                PiyoBehaviour.Instance.afterScenarioState = this;
                PiyoBehaviour.Instance.InvokeAddedScenarioState();
                yield break;
            }

            // 動作完了
            HistoryRecorder.MoveDone();

            // ルートを更新
            PiyoBehaviour.Instance.UpdateRoute();

            // 詰みならここまで
            if (PiyoBehaviour.Instance.ChangeTsumi())
            {
                yield break;
            }

            // プレイへ移行
            InputActionDetector.Instance.OnAction.AddListener(OnAction);
            InputActionDetector.Instance.UpdatePointer();
            CommandQueue.ChangeInputMask(CommandInputType.Everything);
        }

        public override void Update()
        {
            CommandQueue.Update();
            base.Update();
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
            CommandQueue.EntryCommand(playerActionCommand);
        }
    }
}