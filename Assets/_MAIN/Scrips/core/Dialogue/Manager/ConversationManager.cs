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
    private LogiclLineManager logiclLineManager;
    public TextArchitect GetTextArchitect() { return archit; }
    private bool watingAuto = false;
    public bool getWatingAuto() {  return watingAuto; }
    private ConversatioQueue conversatioQueue;
    private Convesation convesation=null;
    private TagManager tagManager;
    public Convesation getConvesation()
    {
        if (conversatioQueue.IsEmpty())
        {  return null; }
        else
        { return conversatioQueue.top(); }
    }
    public int getProgress() { return (conversatioQueue.IsEmpty() ? -1 : conversatioQueue.top().getProgress()); }
    public ConversationManager(TextArchitect textArchitect)
    {
        this.archit = textArchitect;
        DialogueSystem.Instance().AddPrompt_Next(OnUserPrompt_Next);
        tagManager = new TagManager();
        logiclLineManager = new LogiclLineManager();
        conversatioQueue = new ConversatioQueue();
    }
    public void Enqueue(Convesation convesation ){ conversatioQueue.Enqueue( convesation ); }
    public void EnqueuePriority(Convesation convesation ){ conversatioQueue.EnqueuePriority( convesation ); }
    private void OnUserPrompt_Next()
    { 
        userPrompt = true; 
    }

    public Coroutine StartConversation(Convesation conversation)
    {

        StopConversation();
        Enqueue( conversation );
        process = DialogueSystem.Instance().StartCoroutine(RunningConversation());
        return process;
    }
    public void StopConversation()
    {
        if (!IsRunning())
        { return; }

        else { DialogueSystem.Instance().StopCoroutine(process); process = null; }
    }

    IEnumerator RunningConversation()
    {

        while(!conversatioQueue.IsEmpty())
        {
            Convesation currentConversation = getConvesation();
            
            if(currentConversation.HasReacheEnd())
            {
                conversatioQueue.Dequeue();
                continue;
            }

            string rawLine = currentConversation.CurrentLine();
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                TryAdvanceConversation(currentConversation);
                continue; 
            }

            DIALOGUE_LINE line = DialogueParser.Parse(rawLine);
            if (logiclLineManager.TryGetLogic(line, out Coroutine logic))
            {
                yield return logic;
            }
            else 
            {
                if (line.hasDialogue())
                {
                    yield return Line_RunDialogue(line);
                }

                if (line.hasCommands())
                {
                    yield return Line_RunCommands(line);
                }
                if (line.hasDialogue())
                {
                    yield return WaitForUserInput();
                }
            }
            TryAdvanceConversation(currentConversation);
        }
        process = null;

    }
    private void TryAdvanceConversation(Convesation convesation)
    {
        convesation.IncremaentProgres();
        if(convesation != conversatioQueue.top())
        {
            return;
        }
        if(convesation.HasReacheEnd())
        {
            conversatioQueue.Dequeue();
        }

    }

    IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
    {

        if (line.hasSpeaker()) 
        { 
            DialogueSystem.Instance().ShowName(TagManager.Inject(line.GetSpeaker().displayname()));
                
            BacklogPanel.Instance().putInTest(@$"{line.GetSpeaker().displayname()} '{line.GetDialogue().getRawData()}'");

        }
        else
        {
            BacklogPanel.Instance().putInTest(@$"'{line.GetDialogue().getRawData()}'");
        }
        yield return BuildLineSegments(line.GetDialogue());
    }
    IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
    {
        for (int i = 0; i < line.GetsegmeDialogue().Count; ++i)
        {
            DIALOGUE_SEGMENT sement = line.GetsegmeDialogue()[i];
            yield return WaitForDialogueSegmentMarkToBeTriggered(sement);
            yield return BuildDialogue(sement.getDialogue(), sement.appendText());

            if (line.getMAxim())
            {
                yield return WaitForUserInput();
                if (i == line.GetsegmeDialogue().Count - 2)
                {
                    line.setMAxim(false);
                }
            }

        }

    }
    IEnumerator WaitForDialogueSegmentMarkToBeTriggered(DIALOGUE_SEGMENT sement)
    {

        switch (sement.getDialogueMark())
        {
            case DIALOGUE_SEGMENT.DialogueSignals.C:
            case DIALOGUE_SEGMENT.DialogueSignals.A:
                yield return WaitForUserInput(); 
                break; 
            case DIALOGUE_SEGMENT.DialogueSignals.WC:
            case DIALOGUE_SEGMENT.DialogueSignals.WA:
                watingAuto = true;
                yield return new WaitForSeconds(sement.GetSignalWaitDelay());
                watingAuto=false;
                break;
            default: 
                break;
        }

    }

    
    IEnumerator Line_RunCommands(DIALOGUE_LINE line)
    {
        List<DL_COMMAND_DATA.Command> commands = line.GetCommands().getCommands();
        foreach(DL_COMMAND_DATA.Command command in commands)
        {
            if(command.getWait())
            { 
                yield return CommandManager.Instance().Execute(command.getName(), command.getArguments());
            }
            else
            {
                CommandManager.Instance().Execute(command.getName(), command.getArguments());
            }
        }
        yield return null;
    }

    IEnumerator BuildDialogue(string dialogue, bool append = false)
    {
        dialogue= TagManager.Inject(dialogue);
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