using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using System;

public class TextArchitect
{

    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_word;
    public TMP_Text tmpro()
    {
        if (tmpro_ui != null)
        { return tmpro_ui; }
        else
        { return tmpro_word; }
    }
    public string currentText() { return tmpro().text; }
    private string targetText = "";
    public string getTargetText() { return targetText; }
    private string preText = "";
    public string getPreText() { return preText; }

    public string fullTargetText() { return preText + targetText; }
    public enum BuildMethod { instant, typewriter, fade }

    private BuildMethod buildMethod = BuildMethod.typewriter;
    public void setBuildMethod(BuildMethod buildMethod) { this.buildMethod = buildMethod; }
    public Color textColor { get { return tmpro().color; } set { tmpro().color = value; } }
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private float baseSpeed = 1;
    private float speedMultiplier = 1;

    public int characterPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;

    private bool hurryUp = false;
    public void setHurryUp(bool hurryUp) { this.hurryUp = hurryUp; }
    public bool getHurryUp() { return hurryUp; }

    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }
    public TextArchitect(TextMeshPro tmpro_word)
    {
        this.tmpro_word = tmpro_word;
    }


    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;
        Stop();
        buildProcess = tmpro().StartCoroutine(Building());

        return buildProcess;
    }
    public Coroutine Append(string text)
    {
        preText = tmpro().text;
        targetText = text;
        Stop();
        buildProcess = tmpro().StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess = null;

    public bool isBuilding() { return buildProcess != null; }
    public void Stop()
    {
        if (!isBuilding()) { return; }
        tmpro().StopCoroutine(buildProcess);
        buildProcess = null;
    }
    IEnumerator Building()
    {
        Prepare();
        switch(buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Buid_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Buid_Fade();
                break;
        }
        OnConplete();
    }
    private void OnConplete()
    {
        buildProcess = null;
        hurryUp = false;

    }
    public void Forcecompleted()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro().maxVisibleCharacters = tmpro().textInfo.characterCount;
                break;
            case BuildMethod.fade:
                break;
                
        }
        Stop();
        OnConplete();
    }

    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;
        }
        
    }
    private void Prepare_Instant()
    {
        tmpro().color = tmpro().color;
        tmpro().text = fullTargetText();
        tmpro().ForceMeshUpdate();
        tmpro().maxVisibleCharacters=tmpro().textInfo.characterCount;
    }
    private void Prepare_Typewriter()
    {
        tmpro().color = tmpro().color;
        tmpro().maxVisibleCharacters = 0;
        tmpro().text = preText;
        if(preText != "")
        { 
            tmpro().ForceMeshUpdate();
            tmpro().maxVisibleCharacters = tmpro().textInfo.characterCount;
        }
        tmpro().text += targetText;
        tmpro().ForceMeshUpdate();
        
        
    }
    private void Prepare_Fade()
    {

    }
    private IEnumerator Buid_Typewriter()
    {
        while(tmpro().maxVisibleCharacters < tmpro().textInfo.characterCount)
        {
            tmpro().maxVisibleCharacters += hurryUp ? characterPerCycle * 5 : characterPerCycle;
            yield return new WaitForSeconds(0.015f/speed);
        }
    }
    private IEnumerator Buid_Fade()
    {
        yield return null;
    }
}