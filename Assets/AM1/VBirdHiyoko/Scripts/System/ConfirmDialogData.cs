using UnityEngine.Events;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 確認ダイアログ用データ
    /// </summary>
    public class ConfirmDialogData
    {
        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// 左のボタンのテキスト
        /// </summary>
        public string LeftButtonText { get; private set; }
        /// <summary>
        /// 右のボタンのテキスト
        /// </summary>
        public string RightButtonText { get; private set; }
        /// <summary>
        /// 左ボタン押下時の処理
        /// </summary>
        public UnityAction LeftAction { get; private set; }
        /// <summary>
        /// 右ボタン押下時の処理
        /// </summary>
        public UnityAction RightAction { get; private set; }

        public ConfirmDialogData(string message, string leftButtonText, string rightButtonText, UnityAction leftAction, UnityAction rightAction)
        {
            Message = message;
            LeftButtonText = leftButtonText;
            RightButtonText = rightButtonText;
            LeftAction = leftAction;
            RightAction = rightAction;
        }
    }
}