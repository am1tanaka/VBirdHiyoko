using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AM1.BaseFrame.Assets.Editor
{
    /// <summary>
    /// 状態システムのシステムシーンに必要な最小限の構成を現在のアクティブシーンに追加するスクリプト
    /// </summary>
    public class SetStateSystemToActiveSceneEditor : EditorWindow
    {
        public static string PrefabPath => "Assets/AM1/BaseFrame/Prefabs/";

        static GameObject booterObject;

        /// <summary>
        /// 実行結果のテキスト
        /// </summary>
        static string report;

        /// <summary>
        /// 既存のシステムオブジェクトのリスト
        /// </summary>
        static List<GameObject> existsSystemObjects = new List<GameObject>();

        [MenuItem("Tools/AM1/Set StateSystem to Active Scene", false, 5)]
        static void SetStateSystemToActiveScene()
        {
            // アクティブシーンにすでにシステムに必要なスクリプトが揃っているかを確認
            string mes = "";

            if (AreSystemComponents())
            {
                mes = report + "\n\n";
                Selection.objects = existsSystemObjects.ToArray();
            }

            mes += $"シーン'{SceneManager.GetActiveScene().name}'にシステムオブジェクトを追加しますか？";

            if (EditorUtility.DisplayDialog("Systemシーン用のオブジェクトの生成", mes, "追加", "いいえ"))
            {
                SetSystemObjects();
                Debug.Log($"Booterオブジェクトに、Import BaseFrame Assetsで作成したBooterスクリプトをアタッチしてください。");
                Selection.activeObject = booterObject;
            }
        }

        /// <summary>
        /// システムに必要なコンポーネントが揃っているかを確認
        /// </summary>
        /// <returns>いくつか存在している時true</returns>
        static bool AreSystemComponents()
        {
            report = "";
            bool res = false;
            existsSystemObjects.Clear();

            var stateChanger = FindObjectsOfType<SceneStateChanger>();
            res |= ExistsActiveScene(stateChanger, "状態切り替え管理スクリプト StateChanger");

            var bgm = FindObjectsOfType<BGMSourceAndClips>();
            res |= ExistsActiveScene(bgm, "BGM再生スクリプト BGMSourceAndClips");

            var se = FindObjectsOfType<SESourceAndClips>();
            res |= ExistsActiveScene(se, "効果音再生スクリプト SESourceAndClips");

            var transitions = FindObjectsOfType<StandardTransition>();
            res |= ExistsActiveScene(transitions, "画面切り替え StandardScreenTransition");

            return res;
        }

        static bool ExistsActiveScene(MonoBehaviour[] objs, string nm)
        {
            bool res = false;

            foreach (var obj in objs)
            {
                if (obj.gameObject.scene == SceneManager.GetActiveScene()) {
                    report += $"{nm}がすでにシーンにあります。\n\n";
                    existsSystemObjects.Add(obj.gameObject);
                    res = true;
                }
            }

            return res;
        }

        /// <summary>
        /// システムオブジェクトをシーンに配置
        /// </summary>
        static void SetSystemObjects()
        {
            var target = SceneManager.GetActiveScene();

            CreateBooter();
            AddPrefab("SceneStateChanger");
            AddPrefab("AudioPlayer");
            AddPrefab("FadeCanvas");
        }

        static void CreateBooter()
        {
            booterObject = new GameObject();
            booterObject.name = "Booter";
            Undo.RegisterCreatedObjectUndo(booterObject, "Created Booter Object");
        }

        /// <summary>
        /// 指定のプレハブをシーンに追加
        /// </summary>
        /// <param name="prefab">アタッチするプレハブの名前</param>
        static void AddPrefab(string prefab)
        {
            var prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabPath}{prefab}.prefab");
            var go = PrefabUtility.InstantiatePrefab(prefabObject);
            Undo.RegisterCreatedObjectUndo(go, $"Instantiated {prefab} prefab");
        }
    }
}