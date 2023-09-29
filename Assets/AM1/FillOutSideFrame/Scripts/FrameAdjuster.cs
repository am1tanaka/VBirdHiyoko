using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace AM1
{
    [ExecuteInEditMode]
    /// <summary>
    /// ゲーム画面の外側を塗りつぶすシェーダーに画面範囲を知らせるスクリプト
    /// </summary>
    public class FrameAdjuster : MonoBehaviour
    {
        public static FrameAdjuster Instance { get; private set; }

        [Tooltip("UIのサイズを表すRectTransform"), SerializeField]
        RectTransform uiRectTransform = default;
        [Tooltip("URP用のシェーダーを使う時、チェック"), SerializeField]
        bool isURP = false;

        /// <summary>
        /// 動作を有効にする時trueを設定
        /// </summary>
        public static bool isActive = true;

        Image image;
        Vector4 viewRect;
        RectTransform canvasRectTransform;

        /// <summary>
        /// 画面サイズに対してゲーム画面の範囲を左下と右上の
        /// </summary>
        void LateUpdate()
        {
            if (!isActive || (uiRectTransform == null)) return;

            if (image == null)
            {
                image = GetComponent<Image>();
            }
            if (canvasRectTransform == null)
            {
                canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            }

            float scW = (float)Screen.width;
            float scH = (float)Screen.height;
            float halfW = scW * 0.5f;
            float halfH = scH * 0.5f;
            float scale = canvasRectTransform.localScale.x;
            if (isURP)
            {
                viewRect.x = (halfW + scale * uiRectTransform.rect.xMin) / scW;
                viewRect.y = (halfH + scale * uiRectTransform.rect.yMin) / scH;
                viewRect.z = (halfW + scale * uiRectTransform.rect.xMax) / scW;
                viewRect.w = (halfH + scale * uiRectTransform.rect.yMax) / scH;
            }
            else
            {
                viewRect.x = (halfW + scale * uiRectTransform.rect.xMin);
                viewRect.y = (halfH + scale * uiRectTransform.rect.yMin);
                viewRect.z = (halfW + scale * uiRectTransform.rect.xMax);
                viewRect.w = (halfH + scale * uiRectTransform.rect.yMax);
            }
            image.material.SetVector("_ViewRect", viewRect);
        }
    }
}