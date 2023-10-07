using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AM1.VBirdHiyoko;

namespace AM1.MessageSystem
{
    public class MessageWindow : MonoBehaviour
    {
        public static MessageWindow Instance { get; private set; }

        [System.Flags]
        public enum State
        {
            None = 0,
            Hide = 1,
            ToShow = 2,
            Show = 4,
            ToHide = 8,
            CloseAll = 16,
        }

        /// <summary>
        /// 閉じられない秒数
        /// </summary>
        public static float IgnoreCloseSeconds => 0.5f;

        static int DefaultMessageMax => 8;

        /// <summary>
        /// 現在の状態
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// 次の状態
        /// </summary>
        public State NextState { get; private set; } = State.None;

        /// <summary>
        /// 表示を開始してからの秒数
        /// </summary>
        public float ShowTime { get; private set; }

        /// <summary>
        /// 表示中のメッセージ
        /// </summary>
        public string MessageText => messageText != null ? messageText.text : "";

        /// <summary>
        /// メッセージ表示中フラグ
        /// </summary>
        public bool IsShowing => (CurrentState != State.Hide) || (NextState != State.None);

        /// <summary>
        /// 閉じられるとき、trueを返す。
        /// </summary>
        bool CanClose => (ShowTime > IgnoreCloseSeconds);

        List<MessageData> messages = new List<MessageData>(DefaultMessageMax);
        Animator animator;
        TextMeshProUGUI messageText;

        /// <summary>
        /// 表示中のメッセージの表示秒数
        /// </summary>
        float showSeconds;

        private void Awake()
        {
            Instance = this;
            CurrentState = State.Hide;
            animator = GetComponent<Animator>();
            messageText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Update()
        {
            // メッセージ発動チェック
            if ((CurrentState == State.Hide) && (NextState == State.ToShow))
            {
                CurrentState = State.ToShow;
                NextState = State.None;
                StartCoroutine(MessageLoop());
            }
        }

        IEnumerator MessageLoop()
        {
            // 吹き出しの表示を開始
            ToShowMessage();

            // 表示完了待ち
            while (!NextState.HasFlag(State.Show))
            {
                yield return null;
            }
            CurrentState = State.Show;
            NextState &= ~State.Show;

            // メッセージウィンドウが閉じるまでのループ
            while (true)
            {
                // メッセージ表示
                yield return ShowMessage();

                // 次のメッセージか閉じる
                if (!NextMessageOrToHide())
                {
                    // 閉じる
                    break;
                }
            }

            // 閉じるのを待つ
            while (!NextState.HasFlag(State.Hide))
            {
                yield return null;
            }

            // ToShow以外のフラグをクリア
            if (NextState.HasFlag(State.ToShow) && (messages.Count > 0))
            {
                NextState = State.ToShow;
            }
            else
            {
                NextState = State.None;
            }
            CurrentState = State.Hide;
        }

        /// <summary>
        /// 1つのメッセージの表示。
        /// </summary>
        IEnumerator ShowMessage()
        {
            // クリック無効時間
            ShowTime = 0;
            while (ShowTime < IgnoreCloseSeconds)
            {
                // 全て閉じるフラグが設定されていたら即時終了
                if (CheckCloseAll())
                {
                    break;
                }

                ShowTime += Time.deltaTime;
                yield return null;

                NextState = State.None;
            }

            // メッセージがなければこのまま終了
            if (messages.Count == 0)
            {
                yield break;
            }

            // クリックされるか、待ち時間が経過するまで待つ
            animator.SetBool("WaitCursor", true);
            InputActionDetector.Instance.OnAction.AddListener(ToHide);
            while ((ShowTime < showSeconds)
                || Mathf.Approximately(showSeconds, 0f))
            {
                // 閉じる系のフラグが設定されたら終了
                if (CheckCloseAll() || NextState.HasFlag(State.ToHide))
                {
                    break;
                }

                // 自動閉じでメッセージが予約されたらクリック扱い
                if ((showSeconds > 0) && (messages.Count > 1))
                {
                    break;
                }

                ShowTime += Time.deltaTime;
                yield return null;
            }

            // クリックを無効化
            animator.SetBool("WaitCursor", false);
            InputActionDetector.Instance.OnAction.RemoveListener(ToHide);
        }

        /// <summary>
        /// クリックに閉じるメソッドを割り当てるためのメソッド。
        /// </summary>
        void ToHide(RaycastHit hit)
        {
            Close();
        }

        /// <summary>
        /// メッセージをクリアして閉じる要求があったら処理する。
        /// </summary>
        /// <returns>全て閉じる時、true</returns>
        bool CheckCloseAll()
        {
            if (!NextState.HasFlag(State.CloseAll))
            {
                return false;
            }

            // 閉じアニメチェック
            SetToHide();
            return true;
        }

        /// <summary>
        /// 最初に登録されているメッセージを設定してメッセージボックスを開く。
        /// </summary>
        void ToShowMessage()
        {
            messageText.text = messages[0].Message;
            showSeconds = messages[0].Seconds;
            animator.SetBool("Show", true);
            animator.SetBool("WaitCursor", false);
            SEPlayer.Play(SEPlayer.SE.Message);
        }

        /// <summary>
        /// メッセージを閉じる時の初期化
        /// </summary>
        /// <returns>次のメッセージがある時、true</returns>
        bool NextMessageOrToHide()
        {
            // 次のメッセージへ
            NextState = State.None;
            if (messages.Count > 0)
            {
                messages.RemoveAt(0);
            }

            // メッセージが続くのでそのまま表示へ
            if (messages.Count > 0)
            {
                messageText.text = messages[0].Message;
                showSeconds = messages[0].Seconds;
                CurrentState = State.Show;
                SEPlayer.Play(SEPlayer.SE.Message);
                return true;
            }

            // 最後のメッセージだったので閉じる
            SetToHide();

            return false;
        }

        /// <summary>
        /// ウィンドウが開いていたら閉じる。
        /// </summary>
        void SetToHide()
        {
            if (animator.GetBool("Show"))
            {
                animator.SetBool("Show", false);
                CurrentState = State.ToHide;
                SEPlayer.Play(SEPlayer.SE.CloseMessage);
            }
        }

        /// <summary>
        /// メッセージの表示を登録する。
        /// </summary>
        /// <param name="data">登録するメッセージのデータ</param>
        /// <returns>登録</returns>
        public bool Show(MessageData data)
        {
            VBirdHiyokoManager.Log($"MessageWindow.Show({data.Message})");
            if (messages.Contains(data))
            {
                VBirdHiyokoManager.Log($"  含まれていたのでキャンセル");
                return false;
            }

            messages.Add(data);
            if (NextState == State.None)
            {
                NextState = State.ToShow;
            }
            VBirdHiyokoManager.Log($"  登録。NextState={NextState}");
            return true;
        }

        /// <summary>
        /// 現在のメッセージを1つ閉じて次のメッセージ表示へ。
        /// </summary>
        /// <param name="isForce">時間の経過と関係なく無条件で閉じる時、trueを指定。デフォルトはfalse</param>
        public void Close(bool isForce = false)
        {
            // メッセージがなければ何もしない
            if (messages.Count == 0)
            {
                return;
            }

            if (!isForce
                && ((CurrentState != State.Show) || !CanClose))
            {
                return;
            }

            NextState |= State.ToHide;
        }

        /// <summary>
        /// 登録済みのメッセージも含めて全て閉じる。
        /// </summary>
        public void CloseAll()
        {
            messages.Clear();

            // 発動前の時は切り替えをキャンセル
            if (CurrentState == State.Hide)
            {
                NextState = State.None;
            }
            else
            {
                NextState |= State.CloseAll;
            }
        }

        /// <summary>
        /// 表示が完了したらアニメから呼び出す。
        /// </summary>
        public void OnShowDone()
        {
            NextState |= State.Show;
            ShowTime = 0;
        }

        /// <summary>
        /// 非表示が完了したらアニメから呼び出す。
        /// </summary>
        public void OnHideDone()
        {
            NextState |= State.Hide;
        }

    }
}