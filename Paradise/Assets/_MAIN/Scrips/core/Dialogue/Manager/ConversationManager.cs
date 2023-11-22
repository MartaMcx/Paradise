using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager
{


    //defining a coroutine field for coroutine RunningConversation to function, and a bool check (iRunning) if such is currently running
    //RunningConversation() will call another two coroutines, Line_RunDialogue() & Line_RunCommands() depending on suitable conditions
    private Coroutine process = null;
    bool isRunning => process != null;
    public bool GetIsRunning() { return isRunning; }

    private TextArchitect textArchitect = null;
    private bool userPrompt = false;
    public ConversationManager(TextArchitect textArchitect)
    {
        this.textArchitect = textArchitect; //isolating the TextArchitect
        DialogueSystem.Instance().AddToEventOnUserPrompt_Next(OnUserPrompt_Next);
    } //whenever the event is called, do this.OnUserPrompt_Next()

    private void OnUserPrompt_Next() { userPrompt = true; }

    //start coroutine RunningConversation after doing a stop check
    public void StartConversation(List<string> conversation)
    {

        StopConversation();
        process = DialogueSystem.Instance().StartCoroutine(RunningConversation(conversation));
    }
    public void StopConversation()
    {
        if (!isRunning) { return; }

        else { DialogueSystem.Instance().StopCoroutine(process); process = null; }
    }

    //convert to dialogue lines format, then feed them to TextArchitect
    IEnumerator RunningConversation(List<string> conversation)
    {

        for (int i = 0; i < conversation.Count; ++i)
        {

            if (string.IsNullOrWhiteSpace(conversation[i])) { continue; } //skip blank lines

            DIALOGUE_LINE line = DialogueParser.ParseMethod(conversation[i]); //parse correctly the lines


            //AQUI VARIABLESMODIFIER.CS

            //show dialogue, if speaker, then that too
            if (line.GetHasDialogue())
            {
                yield return Line_RunDialogue(line);
            }

            //do commands
            if (line.GetHasCommands())
            {
                yield return Line_RunCommands(line);
            }

            //yield return new WaitForSeconds(1); --> messes up with the player input, not necessary

        }

    }

    IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
    {

        //show speaker name if present, if not, hide box
        if (line.GetHasSpeaker()) { GetDialogueSystemInstance.ShowSpeakerName(line.GetSpeaker()); }
        //else { GetDialogueSystemInstance.HideSpeakerName(); } -> OLD, by commenting this line, if no speaker is specified in the next line, it will use the previous speaker

        //Build Dialogue
        //yield return BuildDialogue(line.GetDialogue()); OLD, replaced by BuildLineSegments(DL_DIALOGUE_DATA)
        yield return BuildLineSegments(line.GetDialogue());

        //Wait for user input
        yield return WaitForUserInput();
    }

    //will build each segment of the dialogue line, but wait for user input if a segment has input marks
    IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
    {
        for (int i = 0; i < line.GetDialogueSegments().Count; ++i)
        {

            DIALOGUE_SEGMENT segment = line.GetDialogueSegments()[i];

            yield return WaitForDialogueSegmentMarkToBeTriggered(segment);

            yield return BuildDialogue(segment.GetSegmentDialogue(), segment.GetSegmentAppendText()); //builds the dialogue of the segment, will use append if its marked as such

        }

    }

    IEnumerator WaitForDialogueSegmentMarkToBeTriggered(DL_DIALOGUE_DATA segment)
    {

        switch (segment.GetDialogueMark())
        {

            case "none":
                break;
            case "c":
            case "a":
                yield return WaitForUserInput(); break; 
            case "wc":
            case "wa":
                yield return new WaitForSeconds(segment.GetSignalWaitDelay()); break; 



        }

    }


    
    IEnumerator Line_RunCommands(DIALOGUE_LINE line)
    {
        Debug.Log(line.GetCommands());

        yield return null;
    }

    IEnumerator BuildDialogue(string dialogue, bool append = false)
    {
        
        if (!append) { textArchitect.Build(dialogue); } 
        else { textArchitect.AppendText(dialogue); }

        
        while (textArchitect.GetisBuilding())
        {
            if (userPrompt)
            {
                if (!textArchitect.GetHurryText()) { textArchitect.SetHurryText(true); } 
                else { textArchitect.ForceComplete(); } 
                userPrompt = false;
            }
            yield return null;
        }
    }

    
    IEnumerator WaitForUserInput()
    {

        while (!userPrompt) { yield return null; }

        userPrompt = false;
    }

}