using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 履歴に対応させるオブジェクトにアタッチするクラス。
    /// </summary>
    public class HistoryBehaviour : MonoBehaviour
    {
        [Tooltip("対象のTransform。未設定時はこのオブジェクト"), SerializeField]
        Transform observeTransform = default;

        /// <summary>
        /// この距離以上あったら移動と見なす
        /// </summary>
        static float DetectDistance => 0.5f;

        /// <summary>
        /// 監視対象のTransform
        /// </summary>
        public Transform ObserveTransform => observeTransform == null ? transform : observeTransform;

        /// <summary>
        /// オブジェクトの識別子
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// ステージの開始時の座標
        /// </summary>
        public Vector3Int StartPosition { get; private set; }

        /// <summary>
        /// 開始時の向き。0=奥, 1=右, 2=手前, 3=左
        /// </summary>
        public int StartDir { get; private set; }

        /// <summary>
        /// 開始時の状態
        /// </summary>
        public int StartState { get; private set; }

        /// <summary>
        /// 開始時の状態
        /// </summary>
        public int BeforeMoveState { get; private set; }

        /// <summary>
        /// 移動開始前の座標
        /// </summary>
        public Vector3Int BeforeMovePosition { get; private set; }

        /// <summary>
        /// 移動開始前の旋回値。0=奥, 1=右, 2=手前, 3=左
        /// </summary>
        public int BeforeMoveDir { get; private set; }

        /// <summary>
        /// eulerAnglesのYを0～3の方向に変換。
        /// </summary>
        public static int EulerYToDir(float y) => Mathf.RoundToInt(Mathf.Repeat(y / 90f, 4f));

        /// <summary>
        /// 現在の状態
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// SetHistoryDataでデータを更新した時に実行するイベントを登録する。
        /// </summary>
        public UnityEvent<HistoryData> DataLoaded { get; set; } = new();

        /// <summary>
        /// 今回のステップで移動を登録した場合、trueにして、同じステップ中に座標を更新しないようにする。
        /// </summary>
        bool isMoving = false;

        /// <summary>
        /// 履歴の再生を行うクラス
        /// </summary>
        IHistoryPlayer historyPlayer;
        public IHistoryPlayer HistoryPlayerInstance
        {
            get
            {
                if (historyPlayer == null)
                {
                    historyPlayer = GetComponent<IHistoryPlayer>();
                }
                return historyPlayer;
            }
        }

        /// <summary>
        /// 自分を登録、IDを受け取り、現在座標をスタート座標として記録する。
        /// </summary>
        public void Register(int state = 0)
        {
            int id;
            if ((id = HistoryObjectList.Register(this)) >= 0)
            {
                Id = id;
                StartPosition = Vector3Int.RoundToInt(ObserveTransform.position);
                StartDir = EulerYToDir(ObserveTransform.eulerAngles.y);
                StartState = State;
                State = state;
                isMoving = false;
            }
        }

        /// <summary>
        /// 移動開始前に呼び出して、状態を登録。
        /// </summary>
        public void StartMove()
        {
            VBirdHiyokoManager.Log($"StartMove {isMoving}");
            if (isMoving) return;

            isMoving = true;
            BeforeMovePosition = Vector3Int.RoundToInt(transform.position);
            BeforeMoveDir = EulerYToDir(ObserveTransform.eulerAngles.y);
            BeforeMoveState = State;
            VBirdHiyokoManager.Log($"  BeforeMovePos={BeforeMovePosition}");
            HistoryRecorder.StartMove(this);
        }

        /// <summary>
        /// 移動完了時に呼び出す。移動結果を登録する。
        /// </summary>
        /// <param name="data">差分を記録する先のインスタンス</param>
        /// <returns>変化があって data を設定した時、true</returns>
        public bool MoveDone(ref HistoryData data)
        {
            VBirdHiyokoManager.Log($"MoveDone");
            isMoving = false;
            return CheckAndRelative(ref data, BeforeMovePosition, BeforeMoveDir, BeforeMoveState);
        }

        /// <summary>
        /// 開始状態から最新状態までの差分を取得する。
        /// Stateの更新が必要なのでMoveDone()を呼んだあとに呼ぶ。
        /// </summary>
        /// <param name="data">記録先のインスタンス</param>
        /// <returns>変化があって data を設定した時、true</returns>
        public bool ToLatestData(ref HistoryData data)
        {
            return CheckAndRelative(ref data, StartPosition, StartDir, StartState);
        }

        /// <summary>
        /// 指定の座標と回転、状態と現在の状態が変化しているかを確認して、
        /// 変化していたら差分を設定した data に設定する。
        /// </summary>
        /// <param name="data">データの格納先</param>
        /// <param name="pos">確認元ワールド座標</param>
        /// <param name="dir">確認元ワールド角度</param>
        /// <param name="st">確認元状態</param>
        /// <returns>変更があった時、true</returns>
        bool CheckAndRelative(ref HistoryData data, Vector3Int pos, int dir, int st)
        {
            Vector3Int nowpos = Vector3Int.RoundToInt(ObserveTransform.position);
            int nowdir = EulerYToDir(ObserveTransform.eulerAngles.y);

            VBirdHiyokoManager.Log($"nowpos={nowpos} from={pos} nowdir={nowdir} nowst={State} fromDir={dir} nowSt={State} fromSt={BeforeMoveState} fromState={st}");

            // 前回と同じなら記録なし
            if ((Vector3Int.Distance(nowpos, pos) < DetectDistance)
                && (nowdir == dir)
                && (State) == st)
            {
                return false;
            }

            // 変化したのでデータを設定して true
            data.Set(Id, nowpos - pos, (nowdir - dir) & 3, State - st);
            return true;
        }

        /// <summary>
        /// 指定の履歴データを反映させる。オブジェクトIDが異なる場合は何もしない。
        /// </summary>
        /// <param name="data">反映させるデータ</param>
        /// <returns>IDが一致して、値を設定したら true</returns>
        public bool SetHistoryData(HistoryData data)
        {
            if (Id != data.ObjectId) return false;

            transform.position += data.RelativePosition;
            ObserveTransform.eulerAngles = data.EulerY * 90 * Vector3.up;
            State = data.State;
            DataLoaded.Invoke(data);
            return true;
        }
    }
}