using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// メッセージ管理クラス
    /// </summary>
    public static class Messages
    {
        public enum Type
        {
            None = -1,
            RetryConfirm,
            RetryButton,
            NoButton,
            ToTitleConfirm,
            ToTitleButton,
            QuitConfirm,
            QuitButton,
            // 操作への反応
            CantGo,
            CanPush,
            TooHeavy,
            CantMove,
            Tsumi,
            GoNext,
            // チュートリアル
            TutorialClick,
            TutorialWind,
            TutorialWhere,
            TutorialLetsGo,
            // Stage01
            Stage01Start,
            Stage01Clear,
            // Stage02
            Stage02Start,
            Stage02Clear,
            // Stage03
            SandLetsGo,
            Stage03Start,
            Sand01,
            Sand02,
            // Stage04
            SandInteresting,
            // Stage05
            IceLetsGo,
            Stage05Start,
            Ice01,
            Ice02,
            IceMiss,
            // Stage06
            IceGround,
            // Stage07
            LastLetsGo,
            // Stage08
            LastClear,
        }

        static string[] messages = new string[]
        {
            "リトライしますか？",
            "リトライ",
            "いいえ",
            "タイトルヘ戻ります？",
            "タイトルヘ",
            "ゲームを終了しますか？",
            "終了する",
            // 操作への反応
            "そこへは行けないな。",
            "押して動かせるかも？",
            "重い。押せなかった。",
            "動かない。",
            "あれ？詰んだ？？\nUndoしてみよう。",
            "次行ってみよう！",
            // チュートリアル
            "クリックした場所へ歩くよ。",
            "強風で飛ばされちゃった。",
            "みんなどこ～...",
            "ここに入ってみよう！",
            // Stage01
            "あっちに穴があるね。\nあそこを目指そう。",
            "着いた！みんな向こうにいるかな？",
            // Stage02
            "誰もいないね。\nあっちに行ってみよう。",
            "次行ってみよう！",
            // Stage03
            "乾いた風だ。",
            "足場が頼りない感じ。",
            "崩れた！",
            "一度しか通れないな。",
            // Stage04
            "これは面白い!",
            // Stage05
            "ひんやりしてるね。",
            "寒いところだ。",
            "冷たくてツルツルしてる。",
            "すべるね。",
            "何かにぶつけて止めよう。",
            // Stage06
            "地面が凍ってる！",
            // Stage07
            "この先にこそ...",
            // Stage08
            "ああっ！！",
        };

        /// <summary>
        /// 指定のメッセージを取得する。
        /// </summary>
        /// <param name="type">メッセージの種類</param>
        /// <returns>指定したメッセージ文字列</returns>
        public static string GetMessage(Type type)
        {
            return messages[(int)type];
        }

        /// <summary>
        /// 指定の文字列指定に対応する文字列を返す。該当するものがない場合は空文字を返す。
        /// </summary>
        /// <param name="type">文字列での指定</param>
        /// <returns>該当文字列。なければ空文字</returns>
        public static string GetMessage(string type)
        {
            Type detectedType;
            if (System.Enum.TryParse<Type>(type, false, out detectedType))
            {
                return messages[(int)detectedType];
            }

            return type;
        }
    }
}