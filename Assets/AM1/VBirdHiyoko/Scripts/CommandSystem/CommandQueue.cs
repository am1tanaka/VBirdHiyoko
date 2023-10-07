//#define DEBUG_LOG

using UnityEngine;
using UnityEngine.Events;

namespace AM1.CommandSystem
{
    /// <summary>
    /// 操作コマンドを扱う static クラス
    /// 操作できるものをマスクで指定したり、操作可能な設定が変化したら
    /// 変更処理を呼び出す。
    /// マスクや優先度から最適なコマンドを nextState に記録して、
    /// 次のフレームで実行する。
    /// 実効は1フレームで完了させる。複数にまたがる場合はPlayerStateScenarioを使う。
    /// </summary>
    public static class CommandQueue
    {
        /// <summary>
        /// 入力の可否を切り替えた時に呼び出したい処理を登録
        /// </summary>
        static readonly UnityEvent<bool>[] onChange;

        /// <summary>
        /// 有効にする予定のマスクの状態
        /// </summary>
        static CommandInputType nextInputMask;

        /// <summary>
        /// 登録可能な入力の種類。
        /// 登録済みのコマンドには影響しない。
        /// </summary>
        public static CommandInputType CurrentInputMask { get; private set; }

        /// <summary>
        /// 次に実行予定のコマンド
        /// </summary>
        static ICommandQueueData nextCommand;

        /// <summary>
        /// 次のコマンドが設定済みのとき、true
        /// </summary>
        public static bool IsSetNextCommand => nextCommand != null;

        /// <summary>
        /// CommandInputTypeをインデックスに変換
        /// </summary>
        /// <param name="type">変換したいコマンドの種類フラグ</param>
        /// <returns>インデックス。onChangeのインデックスに利用</returns>
        static int CommandTypeTypeToIndex(CommandInputType type)
        {
            for (int i = 0; i < onChange.Length; i++)
            {
                if (type.HasFlag((CommandInputType)(1 << i)))
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// クラス初期化
        /// </summary>
        static CommandQueue()
        {
            int count = System.Enum.GetValues(typeof(CommandInputType)).Length - 1;
            onChange = new UnityEvent<bool>[count];
            for (int i = 0; i < count; i++)
            {
                onChange[i] = new UnityEvent<bool>();
            }
            nextCommand = null;
        }

        /// <summary>
        /// 起動時などのシステムの開始時に呼び出す。テストなどで利用。
        /// </summary>
        public static void Init()
        {
            CurrentInputMask = CommandInputType.None;
            RemoveAllChangeListener();
        }

        /// <summary>
        /// 更新をシステムから呼び出す
        /// </summary>
        public static void Update()
        {
            UpdateMask();
            InvokeCommand();
        }

        /// <summary>
        /// マスクの更新処理
        /// </summary>
        static void UpdateMask()
        {
            // 差分
            var diff = CurrentInputMask ^ nextInputMask;

            for (int i = 0; i < onChange.Length; i++)
            {
                if (diff.HasFlag((CommandInputType)(1 << i)))
                {
                    onChange[i].Invoke((nextInputMask.HasFlag((CommandInputType)(1 << i))));
                }
            }
            CurrentInputMask = nextInputMask;
        }

        static void InvokeCommand()
        {
            if (nextCommand == null) return;

            Log($"InvokeCommand {nextCommand}");

            // 実行
            nextCommand.Invoke();
            nextCommand = null;
        }

        /// <summary>
        /// 指定の入力を有効にする。
        /// </summary>
        /// <param name="flag">有効にする時、true</param>
        public static void ChangeInputMask(CommandInputType type)
        {
            nextInputMask = type;

            if ((nextCommand == null)
                || !type.HasFlag(nextCommand.Type))
                return;

            // 操作が無効化されていたら登録データを削除
            nextCommand = null;
        }

        /// <summary>
        /// 変更時のイベントを登録
        /// </summary>
        /// <param name="type">登録先の種類</param>
        /// <param name="action">登録する処理</param>
        public static void AddChangeListener(CommandInputType type, UnityAction<bool> action)
        {
            onChange[CommandTypeTypeToIndex(type)].AddListener(action);
        }

        /// <summary>
        /// 変更時の処理の登録を解除
        /// </summary>
        /// <param name="type">解除する種類</param>
        /// <param name="action">解除するアクション</param>
        public static void RemoveChangeListener(CommandInputType type, UnityAction<bool> action)
        {
            onChange[CommandTypeTypeToIndex(type)].RemoveListener(action);
        }

        /// <summary>
        /// 登録されている全ての変更時の処理を解除。
        /// </summary>
        public static void RemoveAllChangeListener()
        {
            for (int i = 0; i < onChange.Length; i++)
            {
                onChange[i].RemoveAllListeners();
            }
        }

        /// <summary>
        /// 指定のデータの登録を要求する。
        /// </summary>
        /// <param name="data">登録したいデータのインスタンス</param>
        /// <returns>登録できたら true</returns>
        public static bool EntryCommand(ICommandQueueData data)
        {
            Log($"EntryCommand CurrentInputMask={CurrentInputMask} data.Type={data.Type} nextCommand={nextCommand}");
            if (nextCommand != null)
            {
                Log($"  nextPri={nextCommand.Priority} data.pri={data.Priority}");
            }


            // マスクされていたら登録不可
            if (!CurrentInputMask.HasFlag(data.Type))
            {
                Log($"  HasFlagチェックがfalse");
                return false;
            }

            // 登録済みのコマンドより優先度が低ければ登録不可
            if ((nextCommand != null) && (nextCommand.Priority >= data.Priority))
            {
                Log($"  優先順位でキャンセル");
                return false;
            }

            Log($"登録 nextCommand={data}");
            nextCommand = data;
            return true;
        }

        [System.Diagnostics.Conditional("DEBUG_LOG")]
        static void Log(object mes)
        {
            Debug.Log(mes);
        }
    }
}