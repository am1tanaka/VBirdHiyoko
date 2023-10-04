using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    public class PiyoBehaviour : AM1StateQueue
    {
        public static PiyoBehaviour Instance { get; private set; }

        /// <summary>
        /// プレイヤーの移動管理
        /// </summary>
        public PiyoMover Mover { get; private set; }

        /// <summary>
        /// 向いている方向をDirection.Typeで返す。
        /// </summary>
        public Direction.Type CurrentDirectionType => Direction.DetectType(pivotTransform.forward);

        /// <summary>
        /// 指定の方向を向いている時、trueを返す。
        /// </summary>
        /// <param name="dir">確認したい方向</param>
        /// <returns></returns>
        public bool IsFaceTo(Direction.Type dir) => CurrentDirectionType == dir;

        /// <summary>
        /// 歩数を管理するインスタンス
        /// </summary>
        public StepCounter StepCounterInstance { get; private set; } = new();

        /// <summary>
        /// Rigidbodyの座標を返す.
        /// </summary>
        public Vector3 RigidbodyPosition => rb.position;

        InstanceDictionary instanceDictionary = new InstanceDictionary();
        Transform pivotTransform;
        Rigidbody rb;

        /// <summary>
        /// 状態の予約がないことと指定の状態かを確認する。
        /// </summary>
        /// <returns>予約がなく状態が指定のものなら、true</returns>
        public bool StateIs<T>() where T : AM1StateQueueBase
        {
            return (stateQueue.Count == 0) && (CurrentState == instanceDictionary.Get<T>());
        }

        void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
                rb = GetComponent<Rigidbody>();
                Mover = new PiyoMover(rb, pivotTransform);
            }
        }

        /// <summary>
        /// 指定の方のインスタンスを返す。
        /// 未登録のインスタンスは生成して登録してから返す。
        /// </summary>
        /// <typeparam name="T">取り出したい型</typeparam>
        /// <returns>インスタンス</returns>
        public T GetInstance<T>() where T : new()
        {
            return instanceDictionary.GetOrNew<T>();
        }

        /// <summary>
        /// ジェネリックで指定したクラスの状態をキューに積む。
        /// </summary>
        /// <typeparam name="T">キューに積みたいクラスの型</typeparam>
        public void EnqueueState<T>() where T : AM1StateQueueBase, new()
        {
            Enqueue(GetInstance<T>());
        }

        /// <summary>
        /// 表示、および物理座標を設定する。
        /// </summary>
        /// <param name="pos">設定する座標</param>
        public void SetPosition(Vector3 pos)
        {
            Debug.Log($"未実装");
        }

        /// <summary>
        /// 行動中に発動したイベントを受け取るキュー
        /// </summary>
        /// <param name="scenario">プレイヤーの状態として使えるもの</param>
        public void AddScenario(AM1StateQueueBase scenario)
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 押す矢印のインスタンスを受け取る。
        /// </summary>
        /// <param name="arrows">押す矢印インスタンス</param>
        public void ShowPushArrows()
        {
            Debug.Log("未実装");
        }

        /// <summary>
        /// 押せる矢印を非表示にする。
        /// </summary>
        public void HidePushArrows()
        {
            Debug.Log("未実装");
        }
    }
}