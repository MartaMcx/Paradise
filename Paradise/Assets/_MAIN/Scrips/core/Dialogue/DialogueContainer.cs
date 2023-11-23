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
    public GameObject getRoot() {  return root; }
    public NameContainer getNameText(){ return nameText; }
    public TextMeshProUGUI getDialogueText(){ return dialogueText; }

}
