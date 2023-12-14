using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DialogueSystem : MonoBehaviour
{

    [SerializeField] DialogueContainer dialogueContainer = new DialogueContainer();
    

    private ConversationManager conversatonionManager;
    public ConversationManager getConversationManager()
    {
        return conversatonionManager;
    }
    private TextArchitect archi;
    private AutoReader autoReader;
    [SerializeField] private CanvasGroup mainCanvas;
    public bool isRunning() { return conversatonionManager.IsRunning(); }
    static DialogueSystem instance;
    public static DialogueSystem Instance() { return instance; }
    bool init = false;
    public DialogueContainer getDialogueContainer() { return dialogueContainer; }
    private CanvasGroupController cgController;
    public delegate void DialogueSystemEvent();
    event DialogueSystemEvent UserPrompt_Next;

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
        cgController = new CanvasGroupController(this, mainCanvas);
        dialogueContainer.Initialize();
        if(TryGetComponent(out autoReader))
        {
            autoReader.Initialize(conversatonionManager);
        }
        init = true;
    }

    public void AddPrompt_Next(DialogueSystemEvent function)
    {
        UserPrompt_Next += function;

    }
    public void onAutoPressed()
    {
        UserPrompt_Next();
    }
    public void onPressed()
    {
        UserPrompt_Next();
        if(autoReader !=null && autoReader.isOn())
        {
            autoReader.Disable();
        }

    }

    public void ShowName(string speakerName = "")
    {

        if (speakerName.ToLower() != "narrador") 
        {
            dialogueContainer.getNameContainer().Show(speakerName); 
        }
        else 
        {
            HideName(); 
        }

    }
    //hide it
    public void HideName() 
    {
        dialogueContainer.getNameContainer().Hide();
    }

    //print lines on text box, conversation being the txt file to use, 
    public Coroutine Say(string speaker, string dialogue)
    {

        List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
        return Say(conversation);

    }
    public Coroutine Say(Convesation convesation)
    {
        return conversatonionManager.StartConversation(convesation);
    }
    public Coroutine Say(List<string> Lines)
    {
        Convesation convesation =new Convesation(Lines);
        return conversatonionManager.StartConversation(convesation);
    }
    public bool isVisible() { return cgController.isVisible(); }
    public Coroutine Show() { return cgController.Show(); }
    public Coroutine Hide() { return cgController.Hide(); }

}



