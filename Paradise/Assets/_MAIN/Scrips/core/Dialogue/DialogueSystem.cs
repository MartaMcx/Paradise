using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DialogueSystem : MonoBehaviour
{

    [SerializeField] DialogueContainer dialogueContainer = new DialogueContainer();
    

    private ConversationManager conversatonionManager;
    private TextArchitect archi;

    //IsRunning from GetConversationManager
    public bool isRunningConversation() { return conversatonionManager.GetIsRunning(); } //not sure if i need this

    //getter setter for DialogueContainer
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
    public static DialogueSystem Instance() { return instance; }

    //initialice dialog system, called in awake, also doing GetTextArchitect on private
    bool init = false;
    private void Initialize()
    {
        if (init) return;
        archi = new TextArchitect(dialogueContainer.getDialogueText()); // we are setting here which text box are we going to use to display the dialogue
        conversatonionManager = new ConversationManager(archi); // will indirectly enable user input feature (ConversationManager.OnUserPrompt_Next() on event)

        //GetTextArchitect.SetBuildMethodTextType(2); // select which kind of Build method you want ##OLD, use ChooseBuildMethod() instead

        init = true;
    }

    public void onPressed(int UserInput)
    {
        //archi.SetBuildMethodTextType(UserInput);
    }


    public void AddUserPrompt_Next(DialogueSystemEvent function)
    {
        UserPrompt_Next += function;

    }
    public void InvokeOnUserPrompt_Next()
    {
        UserPrompt_Next(); //if EventOnUserPrompt_Next = null, does nothing

    }

    public void ShowName(string speakerName = "")
    {

        if (speakerName.ToLower() != "narrator") { dialogueContainer.getDialogueText()/*.Show(speakerName)*/; } //will not show the name narrator
        else { HideName(); }

    }
    //hide it
    public void HideName() { dialogueContainer.getDialogueText()/*.Hide()*/; }


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



