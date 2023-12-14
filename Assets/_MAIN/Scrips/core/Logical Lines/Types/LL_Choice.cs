using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LogicalLineUtils.Encapsulation;
public class LL_Choice : ILogicalLine
{
    private char CHOICE_IDENTIFIER = '-';
    public string keyword()
    {
        return "choice";
    }
    public IEnumerator Execute(DIALOGUE_LINE line)
    {
        var currentConversation = DialogueSystem.Instance().getConversationManager().getConvesation();
        var progress = DialogueSystem.Instance().getConversationManager().getProgress();
        EncapsulatedData data = RipEncapsulatedData(currentConversation, progress, ripHeadEncap:true);
        List<Choice> choices = getChoicesFromData(data);
        string title = line.GetDialogue().getRawData();
        ChoicePanel panel = ChoicePanel.Instance();
        string[] choiceTitles = choices.Select(c=> c.getTitle()).ToArray();
        panel.Show(title, choiceTitles);
        while(panel.getIsWaitingOnUserChoice())
        {
            yield return null;
        }
        Choice SelectChoice = choices[panel.getLastDecision().getAnswerIndex()];
        
        Convesation newConversation =new Convesation(SelectChoice.getResultLines());
        DialogueSystem.Instance().getConversationManager().getConvesation().setProgress(data.getEndingIndex());
        DialogueSystem.Instance().getConversationManager().EnqueuePriority(newConversation);
    }
    public bool Maches(DIALOGUE_LINE lINE)
    {
        return (lINE.hasSpeaker() && lINE.GetSpeaker().getName().ToLower() == keyword());
    }
    
    private List<Choice> getChoicesFromData(EncapsulatedData data)
    {
        List<Choice> choices = new List<Choice>();
        int encpsulationDepth = 0;
        bool isFirstChoice = true;
        Choice choice = new Choice();
        choice.Iniciate();
        foreach (var line in data.getLines().Skip(1))
        {
            if(IsChoiceStart(line) && encpsulationDepth ==1)
            {
                if (!isFirstChoice) 
                {
                    choices.Add(choice);
                    choice = new Choice();
                    choice.Iniciate();
                }
                choice.setTitle(line.Trim().Substring(1));
                isFirstChoice = false;
                continue;
            }
            AddLineToResults(line,ref choice, ref encpsulationDepth);
        }
        if(!choices.Contains(choice))
        {
            choices.Add(choice);
        }
        return choices;
    }
    private void AddLineToResults(string line, ref Choice choice, ref int encpsulationDepth)
    {
        line.Trim();
        if(IsEncapsulatingStart(line))
        {
            if (encpsulationDepth > 0)
            {
                choice.getResultLines().Add(line);
            }
            ++encpsulationDepth;
            return;
        }
        if(IsEncapsulatingEnd(line))
        {
            --encpsulationDepth;
            if(encpsulationDepth>0)
            {
                choice.getResultLines().Add(line);
            }
            return;
        }
        choice.getResultLines().Add(line);
    }


    private bool IsChoiceStart(string line) { return line.Trim().StartsWith(CHOICE_IDENTIFIER); }

    
    private class Choice
    {
        private string title;
        private List<string> resultLines;
        public void Iniciate()
        {
            resultLines = new List<string>();
            title = string.Empty;
        }
        public void setTitle(string title) { this.title = title;}
        public string getTitle() {  return title; }
        public List<string> getResultLines() {  return resultLines; }
    }
}
