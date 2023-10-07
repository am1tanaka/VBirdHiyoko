using AM1.BaseFrame;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 歩く先を表すカーソル
    /// </summary>
    public class TargetCursor : MonoBehaviour
    {
        Animator animator;
        AudioSource audioSource;

        static float IntervalSeconds => 0.04f;

        void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                animator = GetComponent<Animator>();
                animator.SetBool("Show", false);
                audioSource = GetComponent<AudioSource>();
            }
        }

        private void Start()
        {
            if (InputActionDetector.Instance)
            {
                InputActionDetector.Instance.OnWorldPoint.AddListener(OnWorldPoint);
                InputActionDetector.Instance.OnWorldPointExit.AddListener(OnWorldPointExit);
            }
        }

        private void OnDestroy()
        {
            if (InputActionDetector.Instance)
            {
                InputActionDetector.Instance.OnWorldPoint.RemoveListener(OnWorldPoint);
                InputActionDetector.Instance.OnWorldPointExit.RemoveListener(OnWorldPointExit);
            }
        }

        /// <summary>
        /// カーソルが移動した
        /// </summary>
        /// <param name="hit">ワールドで指しているブロックの接触情報</param>
        void OnWorldPoint(RaycastHit hit)
        {
            var blockRouteData = hit.collider.GetComponent<BlockRouteData>();
            VBirdHiyokoManager.Log($"TargetCursor {hit.collider.name} {(blockRouteData ? $"{blockRouteData.Checked} {blockRouteData.StepCount}" : "false")}");

            // ルート調査ができていないか、
            // 指しているブロックがルートデータを持っていないか、
            // プレイヤーの隣のブロックか、
            // プレイヤーの状態が待機入力状態でないならカーソルを消す
            if (!PiyoBehaviour.Instance.RouteInstance.IsSearched
                || (blockRouteData == null)
                || blockRouteData.IsNextToThePlayer
                || !PiyoBehaviour.Instance.IsState<PiyoStateWaitInput>())
            {
                animator.SetBool("Show", false);
                return;
            }

            // 歩けるならその場所にカーソル表示
            if (blockRouteData.CanWalk)
            {
                var pos = hit.transform.position;
                pos.y = hit.collider.bounds.max.y;
                SetPosition(pos);
                return;
            }

            // 隣の目的地を確認
            var near = blockRouteData.GetNearWalkFloorBlock();
            if (near != null)
            {
                SetPosition(near.CenterTop);
            }
            else
            {
                animator.SetBool("Show", false);
            }
        }

        /// <summary>
        /// 位置を指定してカーソルを表示
        /// </summary>
        /// <param name="pos">表示先座標</param>
        void SetPosition(Vector3 pos)
        {
            bool lastShow = animator.GetBool("Show");
            Vector3 sa = transform.position - pos;

            // 設定
            animator.SetBool("Show", true);
            transform.position = pos;

            // 効果音確認
            sa.y = 0;
            if (!lastShow
                || (sa.magnitude > 0.1f))
            {
                if (AntiSameAudioPlayer.CanPlay(audioSource.clip, IntervalSeconds))
                {
                    audioSource.PlayOneShot(audioSource.clip);
                }
            }
        }

        /// <summary>
        /// ワールドポイントの指定が外れた
        /// </summary>
        void OnWorldPointExit()
        {
            animator.SetBool("Show", false);
        }
    }
}
