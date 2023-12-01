using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextingFile : MonoBehaviour
{
    [SerializeField] TextAsset fileToRead;
    //[SerializeField] TextArchitect.BuildMethod ls = TextArchitect.BuildMethod.typewriter;
    void Start()
    {
        StartConversation();
    }

    
    void StartConversation()
    { 
        List<string> lines = FileManager.ReadTextAsset(fileToRead);

        DialogueSystem.Instance().Say(lines);

    }
}
//Testing the Dialogue Marks and Dialogue Segmentation
/*
foreach (string line in lines)
{
    if (string.IsNullOrEmpty(line)) 
        return;


    Debug.Log($"Segmenting Line '{line}'");
    DIALOGUE_LINE dialogueLine = DialogueParser.ParseMethod(line);
    int i = 0;
    foreach (DIALOGUE_SEGMENT segment in dialogueLine.GetDialogue().GetDialogueSegments()) {

        Debug.Log($"Segment [{++i}] = '{segment.GetSegmentDialogue()}' [Mark={segment.GetDialogueMark()} {(segment.GetSignalWaitDelay() > 0 ? $"{segment.GetSignalWaitDelay()}" : $"")}]");

    }



}
*/