using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DL_DIALOGUE_DATA;

public class ConversationManager
{
    private Coroutine process = null;
    public bool IsRunning() { return process != null; }

    private TextArchitect archit = null;
    private bool userPrompt = false;
    public ConversationManager(TextArchitect textArchitect)
    {
        this.archit = textArchitect;
        DialogueSystem.Instance().AddPrompt_Next(OnUserPrompt_Next);
    }

    private void OnUserPrompt_Next()
    { 
        userPrompt = true; 
    }

    public void StartConversation(List<string> conversation)
    {

        StopConversation();
        process = DialogueSystem.Instance().StartCoroutine(RunningConversation(conversation));
    }
    public void StopConversation()
    {
        if (!IsRunning())
        { return; }

        else { DialogueSystem.Instance().StopCoroutine(process); process = null; }
    }

    IEnumerator RunningConversation(List<string> conversation)
    {

        for (int i = 0; i < conversation.Count; ++i)
        {

            if (string.IsNullOrWhiteSpace(conversation[i])) { continue; }

            DIALOGUE_LINE line = DialogueParser.Parse(conversation[i]);

            if (line.hasDialogue())
            {
                yield return Line_RunDialogue(line);
            }

            if (line.hasCommands())
            {
                yield return Line_RunCommands(line);
            }

        }

    }

    IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
    {

        if (line.hasSpeaker()) 
        { 
            DialogueSystem.Instance().ShowName(line.GetSpeaker().displayname()); 
        }
        yield return BuildLineSegments(line.GetDialogue());
        yield return WaitForUserInput();
    }
    IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
    {
        for (int i = 0; i < line.GetDialogue().Count; ++i)
        {
            DL_DIALOGUE_DATA.DIALOGUE_SEGMENT sement = line.GetDialogue()[i];
            yield return WaitForDialogueSegmentMarkToBeTriggered(sement);
            yield return BuildDialogue(sement.getDialogue(), sement.appendText());
            if (line.getMAxim())
            {
                yield return WaitForUserInput();
                if(i == line.GetDialogue().Count-2)
                {
                    line.setMAxim(false);
                }
                
            }

        }

    }
    IEnumerator WaitForDialogueSegmentMarkToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT sement)
    {

        switch (sement.getDialogueMark())
        {
            case DIALOGUE_SEGMENT.DialogueSignals.C:
            case DIALOGUE_SEGMENT.DialogueSignals.A:
                yield return WaitForUserInput(); 
                break; 
            case DIALOGUE_SEGMENT.DialogueSignals.WC:
            case DIALOGUE_SEGMENT.DialogueSignals.WA:
                yield return new WaitForSeconds(sement.GetSignalWaitDelay());
                break;
            default: 
                break;
        }

    }

    
    IEnumerator Line_RunCommands(DIALOGUE_LINE line)
    {
        List<DL_COMMAND_DATA> commands = line.GetCommands().getCommands();

        yield return null;
    }

    IEnumerator BuildDialogue(string dialogue, bool append = false)
    {
        if (!append)
        { archit.Build(dialogue); }
        else
        { archit.Append(dialogue); }
        while (archit.isBuilding())
        {
            if (userPrompt)
            {
                if (!archit.getHurryUp())
                {
                    archit.setHurryUp(true);
                }
                else
                {
                    archit.Forcecompleted();
                }
                userPrompt = false;
            }
            yield return null;
        }
    }

    
    IEnumerator WaitForUserInput()
    {

        while (!userPrompt)
        { 
            yield return null; 
        }
        userPrompt = false;
    }

}