using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DialogueSystem : MonoBehaviour
{

    [SerializeField] DialogueContainer dialogueContainer = new DialogueContainer();
    

    private ConversationManager conversatonionManager;
    private TextArchitect archi;

    public bool isRunning() { return conversatonionManager.IsRunning(); } //not sure if i need this
    public static DialogueSystem Instance() { return instance; }
    bool init = false;
    public DialogueContainer getDialogueContainer() { return dialogueContainer; }

    public delegate void DialogueSystemEvent();
    event DialogueSystemEvent UserPrompt_Next;



    //singleton, if done correctly, calls on Initialize()
    static DialogueSystem instance;
    private void Awake()
    {
        if (instance == null) 
        { 
            instance = this; 
            Initialize(); 
        }
        else 
        { 
            DestroyImmediate(gameObject); 
        }

    }
    private void Initialize()
    {
        if (init) return;
        archi = new TextArchitect(dialogueContainer.getDialogueText());
        conversatonionManager = new ConversationManager(archi); 
        
        init = true;
    }

    public void AddPrompt_Next(DialogueSystemEvent function)
    {
        UserPrompt_Next += function;

    }
    public void onPressed()
    {
        UserPrompt_Next();

    }

    public void ShowName(string speakerName = "")
    {

        if (speakerName.ToLower() != "narrator") 
        {
            dialogueContainer.getNameText(); 
        }
        else 
        {
            HideName(); 
        }

    }
    //hide it
    public void HideName() 
    {
        dialogueContainer.getNameText();
    }

    //print lines on text box, conversation being the txt file to use, 
    public void Say(string speaker, string dialogue)
    {

        List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
        Say(conversation);

    }
    //will indirectly call coroutine RunningConversation() over on ConversationManager
    public void Say(List<string> conversation)
    {

        conversatonionManager.StartConversation(conversation);
    }

}



