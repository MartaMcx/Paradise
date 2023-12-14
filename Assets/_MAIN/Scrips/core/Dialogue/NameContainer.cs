using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class NameContainer 
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI nameText;
    public void Show(string maneToShow =" ")
    {
        root.SetActive(true);
        if (maneToShow != string.Empty ) 
        {
            nameText.text = maneToShow;
        }
    }
    public void Hide() 
    {
        root.SetActive(false);
    }
}
