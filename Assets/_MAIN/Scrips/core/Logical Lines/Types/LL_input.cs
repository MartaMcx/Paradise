using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LL_input : ILogicalLine
{
    public string keyword()
    {
        return "input";
    }
    public IEnumerator Execute(DIALOGUE_LINE line)
    {
        string title=line.GetDialogue().getRawData();
        InputPanel panel = InputPanel.Instance();
        panel.Show(title);
        while(panel.getIsWaitingOnUserInput())
        {
            yield return null;
        }
    }
    public bool Maches(DIALOGUE_LINE line)
    {
        return (line.hasSpeaker() && line.GetSpeaker().getName().ToLower() == keyword());
    }
}
