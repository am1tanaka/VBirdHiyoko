//#define DEBUG_LOG

using AM1.MessageSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class PiyoScenarioTalk : IScenarioTextCommand
    {
        List<MessageData> talkDataList = new();
        int index;

        public bool IsCommand(string[] words)
        {
            if (words[0][0] == '@') return false;

            // セリフ
            float sec = 0;

#if DEBUG_LOG
            for (int i=0;i<words.Length;i++)
            {
                Debug.Log($"words[{i}]={words[i]},");
            }
#endif

            string[] serif = ScenarioTextValidator.GetParamArray(words, 0);
            VBirdHiyokoManager.Log($"serif = {serif.Length} {serif[0]}");
            if (serif.Length > 1)
            {
                float.TryParse(serif[1], out sec);
            }

            var talk = Messages.GetMessage(serif[0].Trim());
            for (int i = 0; i < talkDataList.Count; i++)
            {
                VBirdHiyokoManager.Log($"talkDataList[{i}].Message={talkDataList[i].Message} talk={talk}");
                if (talkDataList[i].Message == talk)
                {
                    VBirdHiyokoManager.Log($"  一致 {i}");
                    index = i;
                    return true;
                }
            }

            talkDataList.Add(new MessageData(talk, sec));
            index = talkDataList.Count - 1;
            VBirdHiyokoManager.Log($"追加 index={index}");
            return true;
        }

        public IEnumerator Invoke()
        {
            VBirdHiyokoManager.Log($"talk [{index}] {talkDataList[index].Message}");
            MessageWindow.Instance.Show(talkDataList[index]);

            // セリフ待ちの時はここで一旦停止
            if (talkDataList[index].Seconds == 0)
            {
                yield return new WaitWhile(() => MessageWindow.Instance.IsShowing);
            }
        }
    }
}