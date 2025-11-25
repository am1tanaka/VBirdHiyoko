//#define TAP_DEV

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// PlayerActionsから情報を取得して、ゲーム内の指示に変換して呼び出す。
    /// </summary>
    public class InputActionDetector : MonoBehaviour
    {
        public static InputActionDetector Instance { get; private set; }

        PlayerControls playerControls;

        /// <summary>
        /// マウスカーソルの座標
        /// </summary>
        Vector2 currentPoint;

        /// <summary>
        /// 現在の座標を記録した時、true
        /// </summary>
        bool isCurrentPoint;

        /// <summary>
        /// ポインター移動時に座標を渡して呼び出すメソッドを登録する。
        /// </summary>
        /// <typeparam name="Vector2">スクリーン座標</typeparam>
        public UnityEvent<Vector2> OnMovePoint { get; private set; } = new UnityEvent<Vector2>();

        /// <summary>
        /// クリックしたゲームオブジェクトを渡す。
        /// カーソルがUI上の時は呼び出さない。
        /// </summary>
        /// <typeparam name="RaycastHit">対象の接触情報</typeparam>
        public UnityEvent<RaycastHit> OnAction { get; private set; } = new UnityEvent<RaycastHit>();

        /// <summary>
        /// ポインターが移動した時に、ゲームオブジェクトを指していたら上面の座標を渡して呼び出す。
        /// </summary>
        /// <typeparam name="Vector3">指しているワールド座標の上面座標</typeparam>
        public UnityEvent<RaycastHit> OnWorldPoint { get; private set; } = new UnityEvent<RaycastHit>();

        /// <summary>
        /// ワールドカーソルが何も指していなかったら呼び出す。
        /// </summary>
        public UnityEvent OnWorldPointExit { get; private set; } = new UnityEvent();

        /// <summary>
        /// RaycastHitの受け取り
        /// </summary>
        RaycastHit[] raycastHits = new RaycastHit[8];

        /// <summary>
        /// レイキャスト結果の受け取り
        /// </summary>
        List<RaycastResult> results = new List<RaycastResult>(8);

        int blockLayer;
        int uiLayer;
        PointerEventData pointerEventData;

        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                Instance = this;
                playerControls = new();
                playerControls.Player.Point.performed += OnPointPerformed;
                playerControls.Player.Click.performed += OnClickPerformed;
                playerControls.Player.Tap.performed += OnTapPerformed;
                blockLayer = LayerMask.GetMask("Block");
                uiLayer = LayerMask.NameToLayer("UI");
            }
        }

        private void OnEnable()
        {
            playerControls?.Enable();
        }

        private void OnDisable()
        {
            playerControls?.Disable();
            isCurrentPoint = false;
        }

        private void OnDestroy()
        {
            playerControls = null;
        }

        /// <summary>
        /// ポインターの情報を更新する。
        /// </summary>
        public void UpdatePointer()
        {
            // マウスの移動が記録されていなければ何もしない
            if (!isCurrentPoint) return;

            OnMovePoint.Invoke(currentPoint);

            if (IsOnUI(currentPoint))
            {
                // UI上ならワールドカーソル無し
                OnWorldPointExit.Invoke();
                return;
            }

            // ワールド座標を確認する
            var hit = GetBlock(currentPoint);
            if (hit == null)
            {
                OnWorldPointExit.Invoke();
            }
            else
            {
                OnWorldPoint.Invoke(hit.Value);
            }
        }

        /// <summary>
        /// 指定の座標が指しているワールドオブジェクトがあれば返す。
        /// </summary>
        /// <param name="currentPoint">スクリーン座標</param>
        /// <returns>Blockオブジェクトがあればインスタンスを返す。ない場合はnullを返す</returns>
        RaycastHit? GetBlock(Vector2 currentPoint)
        {
            var ray = Camera.main.ScreenPointToRay(currentPoint);
            int count = Physics.RaycastNonAlloc(ray, raycastHits, float.PositiveInfinity, blockLayer);
            if (count == 0)
            {
                return null;
            }

            // 最短のものを選ぶ
            float minDistance = raycastHits[0].distance;
            var hit = raycastHits[0];
            for (int i = 1; i < count; i++)
            {
                if (raycastHits[i].distance < minDistance)
                {
                    minDistance = raycastHits[i].distance;
                    hit = raycastHits[i];
                }
            }

            return hit;
        }

        /// <summary>
        /// マウスカーソル移動
        /// </summary>
        /// <param name="context"></param>
        void OnPointPerformed(InputAction.CallbackContext context)
        {
            currentPoint = context.ReadValue<Vector2>();
            isCurrentPoint = true;
            UpdatePointer();

#if TAP_DEV
            if (isTapped)
            {
                isTapped = false;
                tappedPoint = $"{currentPoint}";
            }

            lastPointUpdate = Time.frameCount;
#endif
        }

#if TAP_DEV
        int lastPointUpdate;
        bool isTapped = false;
        string tappedPoint = "";
#endif

        /// <summary>
        /// 指定の座標がUIに被っているかを返す
        /// </summary>
        /// <param name="pos">スクリーン座標</param>
        bool IsOnUI(Vector2 pos)
        {
            if (pointerEventData == null)
            {
                pointerEventData = new PointerEventData(EventSystem.current);
            }

            pointerEventData.position = currentPoint;
            EventSystem.current.RaycastAll(pointerEventData, results);
            for (int i = 0; i < results.Count; i++)
            {
                if (results[0].gameObject.layer == uiLayer)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// クリックかボタンを離した時
        /// </summary>
        /// <param name="context"></param>
        void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
#if TAP_DEV
                isTapped = true;
                tappedPointTrue = currentPoint;
#endif
                if (context.control.device is Pointer)
                {
                    currentPoint = Pointer.current.position.ReadValue();
                }
                // クリック
                Action(currentPoint);
            }
#if TAP_DEV
            clickMessage = $"{context.ReadValueAsButton()}, {tappedPointTrue} {Time.frameCount}:{lastPointUpdate}\ncontext:{context.valueType} {context.control.ToString()}";
            if (context.control.device is Mouse)
            {
                clickMessage += $"\nMouse";
            }
#endif
        }

#if TAP_DEV
        Vector2 tappedPointTrue = Vector2.zero;
#endif

        /// <summary>
        /// タップした時
        /// </summary>
        /// <param name="context"></param>
        void OnTapPerformed(InputAction.CallbackContext context)
        {
#if TAP_DEV
            tapMessage = $"{context.ReadValue<Vector2>()}";
#endif
            Action(context.ReadValue<Vector2>());
        }

#if TAP_DEV
        string tapMessage = "";
        string clickMessage = "";
        GUIStyle style = new GUIStyle();
        GUIStyleState styleState = new GUIStyleState();
        private void OnGUI()
        {
            style.fontSize = 48;
            styleState.textColor = Color.red;
            style.normal = styleState;
            GUI.Label(new Rect(30,30,800,30), $"tap:{tapMessage} click:{clickMessage} / tapped={tappedPoint}", style);
        }
#endif

        /// <summary>
        /// クリックやタップした座標を受け取って、処理を呼び出す。
        /// </summary>
        /// <param name="pos">スクリーン座標</param>
        void Action(Vector2 pos)
        {
            if (IsOnUI(pos))
            {
                // UI上は処理なし
                return;
            }

            // ワールド座標を確認する
            var hit = GetBlock(pos);
            if (hit != null)
            {
                OnAction.Invoke(hit.Value);
            }
        }
    }
}