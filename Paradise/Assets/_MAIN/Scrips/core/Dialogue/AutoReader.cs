using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AutoReader : MonoBehaviour
{
    private int DEFAUTL_CHARACTER = 18;
    private float READER_TIME_PADING = 0.5f;
    private float MAX_READ_TIME = 99f;
    private float MIN_READ_TIME = 1f;
    private string STATUS_TEXT_AUTO = "AUTO";
    private string STATUS_TEXT_SKIP = "Skiping";


    private ConversationManager conversationManager;
    private TextArchitect architect;
    private bool skip=false;
    private float speed = 1f;
    public bool getSkip() { return skip; }
    public float getSpeed() { return speed; }
    public bool isOn() { return co_runing != null; }
    private Coroutine co_runing = null;
    [SerializeField]private TextMeshProUGUI statusText;
    public void Initialize(ConversationManager conversation)
    {
        this.conversationManager = conversation;
        architect = conversationManager.GetTextArchitect();
        statusText.text = string.Empty;
    }
    public void Enable()
    {
        if (isOn()) { return; }
        co_runing = StartCoroutine(AutoRead());
    }
    public void Disable()
    {
        if (!isOn()) { return; }
        StopCoroutine(co_runing);
        skip = false;
        co_runing = null;
        statusText.text = string.Empty;
    }
    private IEnumerator AutoRead()
    {
        if(!conversationManager.IsRunning())
        {
            Disable();
            yield break;
        }
        if(!architect.isBuilding() && architect.currentText() != string.Empty)
        {
            DialogueSystem.Instance().onAutoPressed();
        }
        while(conversationManager.IsRunning())
        {
            if(!skip)
            {
                while(!architect.isBuilding() && !conversationManager.getWatingAuto())
                { yield return null; }

                float timeStarted=Time.time;

                while (!architect.isBuilding() || conversationManager.getWatingAuto())
                { yield return null; }

                float timeRead = Mathf.Clamp(((float) architect.tmpro().textInfo.characterCount/DEFAUTL_CHARACTER), MIN_READ_TIME, MAX_READ_TIME);
                timeRead = Mathf.Clamp((timeRead-(Time.time-timeStarted)),MIN_READ_TIME,MAX_READ_TIME);
                timeRead = (timeRead / speed) + READER_TIME_PADING;
                yield return new WaitForSeconds(timeRead);
            }
            else
            {
                architect.Forcecompleted();
                yield return new WaitForSeconds(0.05f);
            }
            DialogueSystem.Instance().onAutoPressed();
        }
    }
    public void Toggle_Auto()
    {
        if(skip)
        { Enable(); }
        else
        {
            if(!isOn())
            { Enable(); }
            else
                Disable();
        }
        statusText.text = STATUS_TEXT_AUTO;
        skip = false;
    }
    public void Toggle_Skip()
    {
        if (!skip)
        { Enable(); }
        else
        {
            if (!isOn())
            { Enable(); }
            else
                Disable();
        }
        statusText.text=STATUS_TEXT_SKIP;
        skip = true;
    }
}
