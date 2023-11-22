using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueContainer
{
    [SerializeField]private GameObject root;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    public GameObject getRoot() {  return root; }
    public TMP_Text getNameText(){ return nameText; }
    public TMP_Text getDialogueText(){ return dialogueText; }

}
