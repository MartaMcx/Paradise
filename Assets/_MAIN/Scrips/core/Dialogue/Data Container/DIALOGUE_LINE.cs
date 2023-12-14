using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIALOGUE_LINE
{
    private string rawData=string.Empty;
    public string getRawData() { return rawData; }
    DL_SPEAKER_DATA speaker;
    DL_DIALOGUE_DATA dialogue;
    DL_COMMAND_DATA commands;

    public bool hasSpeaker() { return speaker != null; }//!= string.Empty; }
    public bool hasDialogue() { return dialogue != null; }
    public bool hasCommands(){ return commands != null; }

    public DL_SPEAKER_DATA GetSpeaker() { return speaker; }
    public DL_DIALOGUE_DATA GetDialogue() { return dialogue; }
    public DL_COMMAND_DATA GetCommands() { return commands; }
    public DIALOGUE_LINE(string rawLine, string speaker, string dialogue, string commands)
    {
        rawData = rawLine;
        this.speaker = (string.IsNullOrWhiteSpace(speaker) ? null: new DL_SPEAKER_DATA(speaker));
        this.dialogue = (string.IsNullOrWhiteSpace(dialogue) ? null : new DL_DIALOGUE_DATA(dialogue));
        this.commands = (string.IsNullOrWhiteSpace(commands) ? null : new DL_COMMAND_DATA(commands));

    }

}
