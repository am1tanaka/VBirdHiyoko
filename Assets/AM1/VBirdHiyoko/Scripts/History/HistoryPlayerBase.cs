using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public abstract class HistoryPlayerBase : MonoBehaviour
    {
        protected Vector3 startPosition;
        protected Vector3 startForward;
        protected Vector3 targetPosition;
        protected Vector3 targetForward;
        protected bool isPlaying;
        protected float currentTime;
        protected float targetTime;
        protected HistoryBehaviour historyBehaviour;
        protected HistoryBehaviour HistoryBehaviourInstance
        {
            get
            {
                if (historyBehaviour == null)
                {
                    historyBehaviour = GetComponent<HistoryBehaviour>();
                }
                return historyBehaviour;
            }
        }

        protected Collider colliderInstance;
        protected Collider ColliderInstance
        {
            get
            {
                if (colliderInstance == null)
                {
                    colliderInstance = GetComponent<Collider>();
                }
                return colliderInstance;
            }
        }

        protected virtual void Start()
        {
            isPlaying = false;
        }

        protected void Update()
        {
            if (!isPlaying) return;

            // 時間経過
            currentTime += Time.deltaTime;
            if (currentTime >= targetTime)
            {
                MoveDone();
            }
            else
            {
                // 移動
                float t = currentTime / targetTime;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                HistoryBehaviourInstance.ObserveTransform.forward = Vector3.Slerp(startForward, targetForward, t);
            }
        }

        /// <summary>
        /// 移動を開始するためのデータを設定する。
        /// </summary>
        /// <param name="data">移動のための情報</param>
        /// <param name="mode">UndoかRedoか</param>
        /// <param name="sec">移動秒数</param>
        protected void SetStartMove(HistoryData data, IHistoryPlayer.Mode mode, float sec)
        {
            // 開始位置とゴールを設定してアニメ
            int undoRedo = mode == IHistoryPlayer.Mode.Undo ? -1 : 1;

            startPosition = transform.position;
            targetPosition = Vector3Int.RoundToInt(startPosition) + undoRedo * data.RelativePosition;
            startForward = HistoryBehaviourInstance.ObserveTransform.forward;
            int targetDir = ((undoRedo * data.EulerY) & 3) * 90;
            targetForward = Quaternion.Euler(0, targetDir, 0) * startForward;
            currentTime = 0;
            targetTime = sec;
            VBirdHiyokoManager.Log($"startPos={startPosition} target={targetPosition} rel={data.RelativePosition}");
            SetCollider(false);
        }

        /// <summary>
        /// 移動完了
        /// </summary>
        public void MoveDone()
        {
            if (!isPlaying) return;

            transform.position = targetPosition;
            HistoryBehaviourInstance.ObserveTransform.forward = targetForward;
            isPlaying = false;
            SetCollider(true);
        }

        /// <summary>
        /// コライダーがあれば、指定のコライダーのフラグを設定。
        /// </summary>
        /// <param name="flag">enabledに設定する値</param>
        protected void SetCollider(bool flag)
        {
            if (ColliderInstance != null)
            {
                ColliderInstance.enabled = flag;
            }
        }

    }
}