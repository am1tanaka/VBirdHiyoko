using AM1.BaseFrame.Editor;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AM1.BaseFrame.Assets.Editor
{
    public class NewStateWindowEditor : EditorWindow
    {
        static string ImportedEditorFolder => "Assets/AM1/BaseFrame/Scripts/Editor";

        TextField stateName;
        Toggle makeSceneToggle;
        TextField sceneName;
        EnumField transitionEnum;
        FloatField transitionSec;
        Button createButton;

        /// <summary>
        /// 状態切り替えのファイル名
        /// </summary>
        string StateChangerScriptFileName => $"{stateName.text}SceneStateChanger.cs";

        [MenuItem("Tools/AM1/New SceneState", false, 20)]
        public static void ShowNewSceneState()
        {
            NewStateWindowEditor wnd = GetWindow<NewStateWindowEditor>();
            wnd.titleContent = new GUIContent("New SceneState Window");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{ImportedEditorFolder}/UXML/NewStateWindowEditor.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            // 列挙子の初期設定
            stateName = rootVisualElement.Query<TextField>("StateNameText").First();
            makeSceneToggle = rootVisualElement.Query<Toggle>("MakeSceneToggle").First();
            sceneName = rootVisualElement.Query<TextField>("SceneName").First();
            transitionEnum = rootVisualElement.Query<EnumField>("ScreenTransitionTypeEnum").First();
            transitionSec = rootVisualElement.Query<FloatField>("ScreenTransitionSeconds").First();
            createButton = rootVisualElement.Query<Button>("CreateButton").First();

            transitionEnum.Init(ScreenTransitionType.FilledRadial);

            UpdateActivity();

            // ハンドラ登録
            SetupHandler();
        }

        /// <summary>
        /// 有効性を現状に合わせて更新
        /// </summary>
        void UpdateActivity()
        {
            // シーン名
            sceneName.SetEnabled(makeSceneToggle.value);

            // 画面遷移スクリプトを設定
            transitionSec.SetEnabled((ScreenTransitionType)transitionEnum.value != ScreenTransitionType.None);

            // 作成ボタン
            createButton.SetEnabled(stateName.value.Length > 0);
        }

        /// <summary>
        /// ハンドラを登録
        /// </summary>
        void SetupHandler()
        {
            stateName.RegisterCallback<InputEvent>(OnChangeStateName);
            makeSceneToggle.RegisterCallback<ClickEvent>((ClickEvent cvt) => UpdateActivity());
            transitionEnum.RegisterCallback<ChangeEvent<System.Enum>>((ChangeEvent<System.Enum> ch) => UpdateActivity());
            createButton.RegisterCallback<ClickEvent>(CreateButtonProc);
        }

        void OnChangeStateName(InputEvent iev)
        {            
            if (iev.previousData == sceneName.value)
            {
                sceneName.value = iev.newData;
            }

            UpdateActivity();
        }

        /// <summary>
        /// 指定のフォルダーを起点にして保存先フォルダーを特定してスクリプトを保存。
        /// 保存先はfolder/Scripts > folder/Script > folder直下の順。folderがない時は作成
        /// </summary>
        /// <param name="folder">起点フォルダー</param>
        /// <param name="tempText">スクリプトファイルの中身</param>
        void WriteScript(string folder, string tempText)
        {
            // スクリプトの作成先を特定
            string scriptFolder = Path.Combine(folder, "Scripts");
            if (!Directory.Exists(scriptFolder))
            {
                scriptFolder = Path.Combine(folder, "Script");
                if (!Directory.Exists(scriptFolder))
                {
                    scriptFolder = folder;
                    if (!Directory.Exists(scriptFolder))
                    {
                        Directory.CreateDirectory(scriptFolder);
                    }
                }
            }

            // 保存
            string filePath = Path.Combine(scriptFolder, StateChangerScriptFileName);
            string relPath = "Assets/" + AM1BaseFrameUtils.GetRelativePath(Application.dataPath, filePath);
            string savePath = AssetDatabase.GenerateUniqueAssetPath(relPath);
            File.WriteAllText(savePath, tempText);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        }

        /// <summary>
        /// 状態の作成を実行
        /// </summary>
        /// <param name="cvt"></param>
        void CreateButtonProc(ClickEvent cvt)
        {
            createButton.SetEnabled(false);

            // 保存先フォルダー選択
            var saveStatePath = AM1BaseFrameUtils.LocalSettings.stateFolder;
            string selectedFolder= EditorUtility.SaveFolderPanel("保存先フォルダー", saveStatePath, "");
            if (string.IsNullOrEmpty(selectedFolder))
            {
                // 指定がなければ処理止め
                Debug.Log($"保存先がキャンセルされたため新しい状態の作成を中止しました。");
                UpdateActivity();
                return;
            }

            // スクリプトを保存
            string tempText = MakeStateChangerScript();
            WriteScript(selectedFolder, tempText);

            // シーンの作成
            if (makeSceneToggle.value)
            {
                MakeNewScene(selectedFolder);
            }

            AM1BaseFrameUtils.LocalSettings.stateFolder = Path.Combine("Assets", AM1BaseFrameUtils.GetRelativePath(Application.dataPath, selectedFolder));
            AM1BaseFrameUtils.LocalSettings.Save();
        }

        /// <summary>
        /// 指定のフォルダー以下のScenes, Scene, 直下でフォルダーを検索し、
        /// 最初に見つけた場所へシーンを保存。
        /// </summary>
        /// <param name="selectedFolder">指定フォルダー</param>
        void MakeNewScene(string selectedFolder)
        {
            string savePath = Path.Combine(selectedFolder, "Scenes");
            if (!Directory.Exists(savePath))
            {
                savePath = Path.Combine(selectedFolder, "Scene");
                if (!Directory.Exists(savePath))
                {
                    savePath = selectedFolder;
                }
            }

            NewSceneEditor.NewScene(sceneName.text, savePath);
        }

        /// <summary>
        /// テンプレートから指定の状態名のスクリプトを作成して返す。
        /// </summary>
        /// <returns>更新を加えたスクリプト</returns>
        string MakeStateChangerScript()
        {
            string packagePath = AM1BaseFrameUtils.packageFullPath;
            string templatePath = $"{packagePath}/Package Resources/SceneStateChangerTemplate.cs.txt";
            string tempText = File.ReadAllText(templatePath);

            // 状態名を置き換え
            tempText = tempText.Replace(":StateName:", stateName.text);

            // 画面切り替えの設定
            if (transitionEnum.text != "None")
            {
                var lines = tempText.Split(new char[] { '\n' });
                tempText = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("//ScreenTransitionRegistry."))
                    {
                        lines[i] = lines[i].Replace("//", "");
                        lines[i] = lines[i].Replace(":ScreenTransitionType:", transitionEnum.text.Replace(" ", ""));
                        lines[i] = lines[i].Replace(":ScreenTransitionSeconds:", transitionSec.text + "f");
                    }

                    tempText += lines[i].Replace("\r", "\r\n");
                }
            }

            // シーン読み込み
            if (makeSceneToggle.value)
            {
                // シーン作成
                string loadCode = "// シーンの非同期読み込み開始\r\n";
                loadCode += $"        SceneStateChanger.LoadSceneAsync(\"{sceneName.text}\", true);\r\n";
                tempText = tempText.Replace(":LoadScene:", loadCode);
            }
            else
            {
                // シーン作成しない
                tempText = tempText.Replace(":LoadScene:", "");
            }

            // HideScreenは一先ず不要
            tempText = tempText.Replace(":OnHideScreen:", "");

            // OnAwakeDoneに画面遷移の解除
            if (transitionEnum.text != "None")
            {
                string uncover = "// 画面の覆いを解除\r\n";
                uncover += $"        ScreenTransitionRegistry.StartUncover({transitionSec.value}f);\r\n";
                uncover += $"        yield return ScreenTransitionRegistry.WaitAll();\r\n";
                tempText = tempText.Replace(":OnAwakeDone:", uncover);
            }

            // Terminateでシーンの解除
            if (makeSceneToggle.value)
            {
                string terminate = "// シーンの解放\r\n";
                terminate += $"        SceneStateChanger.UnloadSceneAsync(\"{sceneName.text}\");\r\n";
                tempText = tempText.Replace(":Terminate:", terminate);
            }
            else
            {
                tempText = tempText.Replace(":Terminate:", "");
            }

            return tempText;
        }
    }
}