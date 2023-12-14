using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupController
{
    private float DEFAULT_FADE_SPEED = 3f;
    private MonoBehaviour owner;
    private CanvasGroup rootCG;
    private Coroutine co_showing = null;
    private Coroutine co_hiding = null;
    public bool isShowing() {  return co_showing != null; }
    public bool isHiding() {  return co_hiding != null; }
    public bool isFading() { return isShowing() || isHiding(); }
    public bool isVisible() { return (co_showing != null || rootCG.alpha > 0); }
    public float getAlpha() { return rootCG.alpha; }
    public void setAlpha(float value) { rootCG.alpha= value; }
    public CanvasGroupController(MonoBehaviour owner, CanvasGroup rootCG)
    {
        this.owner = owner;
        this.rootCG = rootCG;
    }
    public Coroutine Show() 
    {
        if(isShowing())
        {
            return co_showing;
        }
        else if (isHiding())
        {
            DialogueSystem.Instance().StopCoroutine(co_hiding);
            co_hiding = null;
        }
        co_showing = DialogueSystem.Instance().StartCoroutine(Fading(1));
        return co_showing;
    }
    public Coroutine Hide()
    {
        if (isHiding())
        {
            return co_hiding;
        }
        else if (isShowing())
        {
            DialogueSystem.Instance().StopCoroutine(co_showing);
            co_showing = null;
        }
        co_hiding = DialogueSystem.Instance().StartCoroutine(Fading(0));
        return co_showing;
    }
    private IEnumerator Fading(float alpha)
    {
        CanvasGroup cg=rootCG;
        while(cg.alpha !=alpha)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, alpha, Time.deltaTime * DEFAULT_FADE_SPEED);
            yield return null;
        }
        co_showing =null;
        co_hiding =null;
    }
    public void SetInteractiveState(bool active)
    {
        rootCG.interactable = active;
        rootCG.blocksRaycasts = active;
    }
}
