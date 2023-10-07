using UnityEngine;
using UnityEngine.Events;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 歩数をカウントするクラス。PlayerBehaviourで生成と管理。
    /// 最新歩数と一時歩数を管理する。
    /// 内部歩数は歩数超過を正しく扱うために、ゲーム内で利用する歩数よりも大きい値を扱う。
    /// メソッドでは内部歩数を更新して、戻り値はゲーム内歩数を返す。
    /// 値の変更を試みた時にイベントを発行する。
    /// 初期化の簡略化と、頻度が低く処理速度への影響は無視できるので、値が変更されなくてもイベントを発行する。
    /// </summary>
    public class StepCounter
    {
        /// <summary>
        /// ゲーム内に置ける最大歩数。プレイヤーへやInnerStepMaxからこの範囲に丸め込んだ値を提示する。
        /// 歩数を1つの値で管理すると、最後の歩数からUndoで戻すと歩数が戻ってしまう。
        /// それを防ぐために十分に大きな値を内部では利用しておく。
        /// 履歴に歩数を記録しておく方法に対して、この方式なら最新状態の歩数をInnerStepで管理して2byteで保存すれば
        /// 済むので、保存対象のデータを減らす観点でこの方式を採用。
        /// </summary>
        public int GameStepMax { get; private set; } = 999;

        /// <summary>
        /// 内部的な最大歩数。内部的な最終歩数はこの値を超えない。
        /// </summary>
        public int InnerStepMax { get; private set; } = 0xffff;

        /// <summary>
        /// 現在のゲーム内歩数
        /// </summary>
        public int CurrentGameStep => Mathf.Min(currentInnerStep, GameStepMax);

        /// <summary>
        /// 指定の内部歩数をゲーム内歩数に変換して返
        /// </summary>
        /// <param name="step">内部歩数</param>
        /// <returns>ゲーム内歩数</returns>
        public int GetGameStep(int step)
        {
            return Mathf.Min(step, GameStepMax);
        }

        /// <summary>
        /// 現在の内部歩数。
        /// </summary>
        int currentInnerStep;

        /// <summary>
        /// Undo操作時などの一時的な内部歩数。
        /// </summary>
        int tempInnerStep;

        /// <summary>
        /// 内部歩数を ushort で返す。
        /// </summary>
        public ushort CurrentInnerStep => (ushort)Mathf.Min(currentInnerStep, InnerStepMax);

        /// <summary>
        /// 一時歩数をゲーム歩数に変換して返す。
        /// </summary>
        public int TempGameStep => GetGameStep(tempInnerStep);

        /// <summary>
        /// 一時歩数が変更されたら呼び出す処理を登録する。
        /// </summary>
        public UnityEvent<int> OnChangeTempStep = new UnityEvent<int>();

        /// <summary>
        /// テスト用に歩数上限を変更する。
        /// </summary>
        /// <param name="max">歩数上限値</param>
        public void SetStepMax(int gameMax, int innerMax)
        {
            GameStepMax = gameMax;
            InnerStepMax = innerMax;
        }

        /// <summary>
        /// 歩数を0に戻す。
        /// </summary>
        public void Clear()
        {
            currentInnerStep = 0;
            tempInnerStep = 0;
            OnChangeTempStep.Invoke(0);
        }

        /// <summary>
        /// 現在の歩数を一歩増やす。一時歩数も一致させる。
        /// </summary>
        /// <returns>更新したゲーム内歩数を返す。</returns>
        public int IncrementAll()
        {
            return AddAll(1);
        }

        /// <summary>
        /// 現在の歩数に値を加える。一時歩数も更新する。
        /// </summary>
        /// <returns>更新したゲーム内歩数を返す。</returns>
        public int AddAll(int step = 1)
        {
            currentInnerStep = Mathf.Min(currentInnerStep + step, InnerStepMax);
            tempInnerStep = currentInnerStep;
            OnChangeTempStep.Invoke(TempGameStep);
            return TempGameStep;
        }

        /// <summary>
        /// 一時歩数を1歩加算する。
        /// </summary>
        /// <returns>加算後の一時歩数</returns>
        public int IncrementTemp()
        {
            return AddTemp(1);
        }

        /// <summary>
        /// 一時歩数を1歩減算する。
        /// </summary>
        /// <returns>減算後の一時歩数</returns>
        public int DecrementTemp()
        {
            return AddTemp(-1);
        }

        /// <summary>
        /// 一時歩数を加算する。
        /// </summary>
        /// <param name="step">加減算する値</param>
        /// <returns>更新した一時ゲーム内歩数を返す。</returns>
        public int AddTemp(int step = 1)
        {
            int next = Mathf.Min(tempInnerStep + step, InnerStepMax);
            next = Mathf.Max(0, tempInnerStep);
            if (next != tempInnerStep)
            {
                tempInnerStep = next;
            }
            OnChangeTempStep.Invoke(TempGameStep);

            return TempGameStep;
        }

        /// <summary>
        /// 一時歩数を現在の歩数に反映する。
        /// </summary>
        public void ApplyTemp()
        {
            currentInnerStep = tempInnerStep;
        }

        /// <summary>
        /// 一時歩数を削除して、現在の歩数に戻す。
        /// </summary>
        public void RevertTemp()
        {
            tempInnerStep = currentInnerStep;
        }

        /// <summary>
        /// 指定の歩数を設定する。
        /// </summary>
        /// <param name="step">設定する歩数</param>
        public void SetInnerStep(int step)
        {
            currentInnerStep = Mathf.Min(step, InnerStepMax);
            tempInnerStep = currentInnerStep;
            OnChangeTempStep.Invoke(TempGameStep);
        }
    }
}