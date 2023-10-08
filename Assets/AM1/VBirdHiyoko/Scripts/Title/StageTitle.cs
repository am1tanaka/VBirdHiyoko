using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame;

namespace AM1.VBirdHiyoko
{
    [System.Serializable]
    class RemoveObjectData
    {
        public GameObject[] objects;
    }

    /// <summary>
    /// クリア状態に応じて、ブロックを削除する。
    /// </summary>
    public class StageTitle : MonoBehaviour
    {
        [Tooltip("削除対象のブロックをステージ順にアタッチする。"), SerializeField]
        RemoveObjectData[] removeObjects = default;

        [Tooltip("ステージのクリア状態や最後に入ったステージに合わせた開始位置"), SerializeField]
        Transform[] playerStandPositions = default;

        void Start()
        {
            if (!SceneStateChanger.IsReady) return;
            if (removeObjects == null) return;

            DeleteUntil(VBirdHiyokoManager.ClearedStage.Current);
            if (TitleSceneStateChanger.Instance.From != TitleSceneStateChanger.FromState.Ending)
            {
                SetStartPosition(VBirdHiyokoManager.CurrentStage.Current);
            }
            else
            {
                SetStartPosition(0);
            }

            // 起動が済んだらこのスクリプトは不要
            enabled = false;
        }

        /// <summary>
        /// 指定のインデックス未満のオブジェクトを削除する
        /// </summary>
        /// <param name="index">この値未満のオブジェクトを削除</param>
        void DeleteUntil(int index)
        {
            for (int i = 0; i < index; i++)
            {
                if (i >= removeObjects.Length) break;
                if (removeObjects[i] == null) continue;

                for (int j = 0; j < removeObjects[i].objects.Length; j++)
                {
                    if (removeObjects[i].objects[j])
                    {
                        Destroy(removeObjects[i].objects[j]);
                    }
                }
            }
        }

        /// <summary>
        ///開始ステージの位置を示して呼び出す。
        /// </summary>
        void SetStartPosition(int index)
        {
            int clampedIndex = Mathf.Clamp(index, 0, playerStandPositions.Length - 1);
            Vector3 pos = playerStandPositions[clampedIndex].position;

            // ブロックの有無を確認
            Vector3 center = pos + 0.5f * Vector3.up;
            var result = Physics.OverlapSphere(center, 0.1f, LayerMask.GetMask("Block"));
            for (int i = 0; i < result.Length; i++)
            {
                var rb = result[i].GetComponent<Rigidbody>();
                rb.position += Vector3.right;
            }

            // プレイヤーの座標を設定            
            PiyoBehaviour.Instance.SetPosition(pos);
        }
    }
}