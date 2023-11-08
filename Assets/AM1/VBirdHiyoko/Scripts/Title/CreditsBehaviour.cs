using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace AM1.VBirdHiyoko
{
    public class CreditsBehaviour : MonoBehaviour
    {
        public static CreditsBehaviour Instance { get; private set; }

        [Tooltip("Timeline Asset"), SerializeField]
        PlayableAsset[] framePlayableAssets = new PlayableAsset[2];
        [Tooltip("Title用UI"), SerializeField]
        GameObject[] titleUIs = default;
        [Tooltip("ScrollRect"), SerializeField]
        ScrollRect scrollRect = default;
        [Tooltip("Content"), SerializeField]
        RectTransform contentRectTransform = default;
        [Tooltip("クレジットのRectTransform"), SerializeField]
        RectTransform[] creditsRectTransforms = default;

        [Header("End Credits")]
        [Tooltip("エンドロール後のクリックボタン"), SerializeField]
        GameObject toTitleButton = default;

        /// <summary>
        /// デフォルトスクロール速度
        /// </summary>
        static float DefaultScrollSpeed => 120f;
        /// <summary>
        /// クリック時の倍率
        /// </summary>
        static float ScrollSpeedUp => 3f;

        /// <summary>
        /// 実効モード
        /// </summary>
        public enum Mode
        {
            TitleCredits,
            EndCredits
        }

        /// <summary>
        /// 状態
        /// </summary>
        public enum State
        {
            None = -1,
            LoadScene,    // シーン読み込み中
            Hide,
            ToShow,
            Show,
            ToHide,
            EndRoll,
            WaitToTitle,
            ToTitle,
        }

        /// <summary>
        /// PlayableAssetのインデックス
        /// </summary>
        enum PlayableType
        {
            Show,
            Hide
        }

        float[] contentHeights =
        {
            1200,
            2860
        };

        ScrollRect.MovementType[] movementType =
{
             ScrollRect.MovementType.Elastic,
             ScrollRect.MovementType.Clamped
        };

        /// <summary>
        /// 現在の状態
        /// </summary>
        public static State CurrentState { get; private set; } = State.None;

        static Mode currentMode;

        /// <summary>
        /// フレームの高さ
        /// </summary>
        float frameHeight;

        PlayableDirector _playableDirector;
        PlayableDirector PlayableDirectorInstance
        {
            get
            {
                if (_playableDirector == null)
                {
                    _playableDirector = GetComponent<PlayableDirector>();
                }
                return _playableDirector;
            }
        }

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
                toTitleButton.SetActive(false);
            }
        }

        private void Start()
        {
            if (CurrentState == State.LoadScene)
            {
                ShowCredits();
            }
        }

        private void OnDestroy()
        {
            Instance = null;
            CurrentState = State.None;
        }

        private void Update()
        {
            switch (CurrentState)
            {
                case State.EndRoll:
                    UpdateEndRoll();
                    break;
            }
        }

        /// <summary>
        /// 開始。シーンがなければ読み込んで実行
        /// </summary>
        /// <param name="mode">開始するモード</param>
        public static void Show(Mode mode)
        {
            currentMode = mode;

            if (CurrentState == State.None)
            {
                SceneStateChanger.LoadSceneAsync("Credits", false);
                CurrentState = State.LoadScene;
                return;
            }

            // 読み込み済みで非表示だった時
            Instance.ShowCredits();
        }

        /// <summary>
        /// クレジットシーンの表示演出を開始
        /// </summary>
        void ShowCredits()
        {
            for (int i = 0; i < titleUIs.Length; i++)
            {
                titleUIs[i].SetActive(currentMode == Mode.TitleCredits);
            }

            if (currentMode == Mode.EndCredits)
            {
                // エンドクレジットの初期化
                var canvasGroup = titleUIs[0].GetComponentInParent<CanvasGroup>();
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                frameHeight = canvasGroup.GetComponent<RectTransform>().sizeDelta.y;
            }

            // 表示開始
            PlayableDirectorInstance.playableAsset = framePlayableAssets[(int)PlayableType.Show];
            PlayableDirectorInstance.Play();
            CurrentState = State.ToShow;

            // 状態設定
            scrollRect.movementType = movementType[(int)currentMode];
            scrollRect.vertical = currentMode == Mode.TitleCredits;
            creditsRectTransforms[(int)currentMode].gameObject.SetActive(true);
            creditsRectTransforms[1 - (int)currentMode].gameObject.SetActive(false);
            contentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeights[(int)currentMode]);
            contentRectTransform.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// 閉じるボタンが押された時にスイッチから呼び出します
        /// </summary>
        public void OnClickClose()
        {
            if (CurrentState != State.Show) return;

            SEPlayer.Play(SEPlayer.SE.Cancel);
            CurrentState = State.ToHide;
            PlayableDirectorInstance.Play(framePlayableAssets[(int)PlayableType.Hide], DirectorWrapMode.None);
        }

        /// <summary>
        /// エンドロール表示後のクリック
        /// </summary>
        public void OnClickEndToTitle()
        {
            if (CurrentState != State.WaitToTitle) return;

            SEPlayer.Play(SEPlayer.SE.Click);
            CurrentState = State.ToTitle;
            toTitleButton.SetActive(false);
            // エンディングクレジット
            TitleSceneStateChanger.Instance.RequestFrom(TitleSceneStateChanger.FromState.Ending, false);
        }

        public void OnShowed()
        {
            if (currentMode == Mode.TitleCredits)
            {
                CurrentState = State.Show;
            }
            else
            {
                CurrentState = State.EndRoll;
            }
        }

        public void OnHided()
        {
            TitleBehaviour.Instance.ChangeState(TitleBehaviour.State.Play);
            CurrentState = State.Hide;
        }

        /// <summary>
        /// エンドロールの処理
        /// </summary>
        void UpdateEndRoll()
        {
            // スクロール
            float delta = (Mouse.current.leftButton.isPressed ? ScrollSpeedUp : 1) * DefaultScrollSpeed;
            contentRectTransform.anchoredPosition += (Time.deltaTime * delta * Vector2.up);

            // 終了チェック
            if (contentRectTransform.anchoredPosition.y >= contentHeights[(int)Mode.EndCredits] - frameHeight)
            {
                // エンドロール終了
                CurrentState = State.WaitToTitle;
                toTitleButton.SetActive(true);
            }
        }
    }
}