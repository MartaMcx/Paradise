using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIALOGUE_LINE
{
    string speaker;
    //string dialogue; --> OLD, replaced by data tpye DL_DIALOGUE_DATA
    DL_DIALOGUE_DATA dialogue;
    string commands;


    //bool hasDialogue => dialogue != string.Empty;
    bool hasDialogue => dialogue.GetHasDialogue();
    bool hasCommands => commands != string.Empty;
    bool hasSpeaker => speaker != string.Empty;

    public bool GetHasDialogue() { return hasDialogue; }
    public bool GetHasCommands() { return hasCommands; }
    public bool GetHasSpeaker() { return hasSpeaker; }

    public DL_DIALOGUE_DATA GetDialogue() { return dialogue; }
    public string GetCommands() { return commands; }
    public string GetSpeaker() { return speaker; }



    //constructor for DIALOGUE_LINE that will take the three fields and delimit them in a dialog line
    public DIALOGUE_LINE(string speaker, string dialogue, string commands)
    {

        this.speaker = speaker;
        this.dialogue = new DL_DIALOGUE_DATA(dialogue);
        this.commands = commands;

    }

}
