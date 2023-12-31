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

        [Tooltip("詰んだ時のスクリプトテキスト"), SerializeField]
        TextAsset tsumiScenarioText = default;

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
        /// アニメから呼ばれるイベントを登録するインスタンス
        /// </summary>
        public AnimEvent AnimEventInstance { get; private set; }

        /// <summary>
        /// 歩数を管理するインスタンス
        /// </summary>
        public StepCounter StepCounterInstance { get; private set; } = new();

        /// <summary>
        /// Rigidbodyの座標を返す.
        /// </summary>
        public Vector3 RigidbodyPosition => rb.position;

        /// <summary>
        /// プレイヤーの当たり判定
        /// </summary>
        public BoxCollider BoxColliderInstance { get; private set; }

        /// <summary>
        /// 登録されたシナリオイベントがある時、trueを返す。
        /// </summary>
        public bool IsAddedScenario => addedScenarioQueue.Count > 0;

        /// <summary>
        /// シナリオ実行後の状態を設定する。
        /// </summary>
        public AM1StateQueueBase afterScenarioState;

        /// <summary>
        /// ルートを確認するインスタンス
        /// </summary>
        public Route RouteInstance { get; private set; }

        /// <summary>
        /// ステージシーンのインスタンス群
        /// </summary>
        StageInstances stageInstances;

        InstanceDictionary instanceDictionary = new InstanceDictionary();
        Animator anim;
        Transform pivotTransform;
        Rigidbody rb;

        /// <summary>
        /// 歩行中に受け取ったマップ上のシナリオイベントのキュー
        /// </summary>
        Queue<AM1StateQueueBase> addedScenarioQueue = new Queue<AM1StateQueueBase>();

        /// <summary>
        /// 詰み状態の時、true
        /// </summary>
        public bool IsTsumi { get; private set; }
        PiyoStateScenario stateTsumi;
        GeneralPiyoStateScenario tsumiScenario;

        /// <summary>
        /// 状態の予約がないことと指定の状態かを確認する。
        /// </summary>
        /// <returns>予約がなく状態が指定のものなら、true</returns>
        public bool IsState<T>() where T : AM1StateQueueBase
        {
            return (stateQueue.Count == 0) && (CurrentState == instanceDictionary.Get<T>());
        }

        void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
                anim = GetComponentInChildren<Animator>();
                SetAnimState(PiyoAnimState.Stand);
                rb = GetComponent<Rigidbody>();
                AnimEventInstance = GetComponentInChildren<AnimEvent>();
                pivotTransform = transform.Find("Pivot");
                Mover = new PiyoMover(this, rb, pivotTransform);
                BoxColliderInstance = pivotTransform.GetComponent<BoxCollider>();
                RouteInstance = new();
                tsumiScenario = new(tsumiScenarioText.text);
                stateTsumi = new();
                stateTsumi.SetSource(tsumiScenario);
                IsTsumi = false;
                StepCounterInstance.Clear();
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
        /// アニメ状態を設定する。
        /// </summary>
        /// <param name="state">設定するアニメ状態</param>
        public void SetAnimState(PiyoAnimState state)
        {
            anim.SetInteger("State", (int)state);
        }

        /// <summary>
        /// 表示、および物理座標を設定する。
        /// </summary>
        /// <param name="pos">設定する座標</param>
        public void SetPosition(Vector3 pos)
        {
            rb.position = pos;
            transform.position = pos;
        }

        /// <summary>
        /// ルートを現在の状態で更新する。
        /// </summary>
        public void UpdateRoute()
        {
            // ルートを削除
            RouteInstance.ClearRoute();

            // ルートチェック
            var footBlock = Mover.FootBlock;
            if (footBlock != null)
            {
                var block = footBlock.Value.collider.GetComponent<BlockRouteData>();
#if UNITY_EDITOR
                if (block == null)
                {
                    Debug.Log($"UpdateWaitInput ルートチェック 足元ブロックにBlockRouteDataが未設定 {footBlock.Value.transform.position}");
                }
#endif
                RouteInstance.Search(
                    block,
                    BoxColliderInstance,
                    PiyoMover.StepHeight);
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log($"足場がnull");
            }
#endif        
        }

        /// <summary>
        /// 行動中に発動したイベントを受け取るキュー
        /// </summary>
        /// <param name="scenario">プレイヤーの状態として使えるもの</param>
        public void AddScenario(AM1StateQueueBase scenario)
        {
            addedScenarioQueue.Enqueue(scenario);
        }

        /// <summary>
        /// 追加されたシナリオイベントを実行する。
        /// </summary>
        public void InvokeAddedScenarioState()
        {
            // シナリオイベントを実行する。
            if (addedScenarioQueue.TryDequeue(out var scenarioState))
            {
                Enqueue(scenarioState);
            }
            else
            {
                if (afterScenarioState != null)
                {
                    Enqueue(afterScenarioState);
                }
                else
                {
                    Enqueue(GetInstance<PiyoStateWaitInput>());
                }
            }
        }

        /// <summary>
        /// シナリオによる中断が終了。処理を復帰させる。
        /// </summary>
        public void ScenarioInterceptDone()
        {
            if (afterScenarioState != null)
            {
                Enqueue(afterScenarioState);
            }
            else
            {
                EnqueueState<PiyoStateWaitInput>();
            }
        }

        /// <summary>
        /// 目的地フラグのインスタンスを設定する。
        /// </summary>
        /// <param name="flag">目的地旗のインスタンス</param>
        public void SetStageInstances(StageInstances instances)
        {
            stageInstances = instances;
        }

        /// <summary>
        /// 押す矢印のインスタンスを受け取る。
        /// </summary>
        /// <param name="arrows">押す矢印インスタンス</param>
        public void ShowPushArrows()
        {
            stageInstances.pushArrows?.Show(BoxColliderInstance);
        }

        /// <summary>
        /// 押せる矢印を非表示にする。
        /// </summary>
        public void HidePushArrows()
        {
            stageInstances?.pushArrows?.Hide();
        }

        /// <summary>
        /// フラグを指定の座標へ表示する。
        /// </summary>
        /// <param name="pos">旗を立てる座標</param>
        public void ShowTargetFlag(Vector3 pos)
        {
            stageInstances.targetFlag?.Show(pos);
        }

        /// <summary>
        /// 目的地の旗を降ろす。
        /// </summary>
        public void HideTargetFlag()
        {
            stageInstances.targetFlag?.Hide();
        }

        /// <summary>
        /// 詰み状態を発動させるか確認する。
        /// </summary>
        /// <returns>詰み状態へ変更する時 true。すでに詰み状態だったり継続可能ならfalse</returns>
        public bool ChangeTsumi()
        {
            // 状態に変化がなければ何もしない
            bool nowTsumi = !RouteInstance.CanPlay;

            if (IsTsumi == nowTsumi)
            {
                return false;
            }

            // 前回、詰み状態でここに来たなら、今回詰みではない
            if (IsTsumi)
            {
                IsTsumi = false;
                return false;
            }

            // ここに来たら前回は詰みではなく、今回詰んだので、詰んだ処理を発動
            IsTsumi = true;
            afterScenarioState = GetInstance<PiyoStateWaitInput>();
            AddScenario(stateTsumi);
            InvokeAddedScenarioState();
            return true;
        }
    }
}