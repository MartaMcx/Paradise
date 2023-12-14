using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueContainer
{
    [SerializeField]private GameObject root;
    [SerializeField] private NameContainer  nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    private CanvasGroupController cgController;
    public GameObject getRoot() {  return root; }
    public NameContainer getNameContainer(){ return nameText; }
    public TextMeshProUGUI getDialogueText(){ return dialogueText; }
    private bool initialized=false;
    public void Initialize()
    {
        if (initialized) 
        { return; }

        cgController = new CanvasGroupController(DialogueSystem.Instance(), root.GetComponent<CanvasGroup>());
    }
    public bool isVisible() { return cgController.isVisible(); }
    public Coroutine Show() { return cgController.Show(); }
    public Coroutine Hide() { return cgController.Hide(); }
}
