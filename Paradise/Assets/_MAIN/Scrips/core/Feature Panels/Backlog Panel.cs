using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BacklogPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button autoButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private CanvasGroup canvasGroup;
    private List<string> text;
    private TagManager tagManager;
    public void setTest(List<string> text) { this.text = text; }
    public void putInTest(string line) 
    { 
        text.Add(ReplaceText(line)); 
    }
    private string ReplaceText(string text)
    {
        text=TagManager.Inject(text);
        if(text.Contains("{a}"))
        {
            text = text.Replace("{a}", "");
        }
        if (text.Contains("{c}"))
        {
            text = text.Replace("{c}", "\n");
        }
        if (text.Contains("{wa}"))
        {
            text = text.Replace("{wa}", "");
        }
        if (text.Contains("{wc}"))
        {
            text = text.Replace("{wc}", "\n");
        }
        return text;
    }
    private CanvasGroupController cg;
    static BacklogPanel instance = null;
    public static BacklogPanel Instance() { return instance; }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        tagManager = new TagManager();
        text = new List<string>();
        Text.text=string.Empty;
        cg = new CanvasGroupController(this, canvasGroup);
        cg.setAlpha(0);
        cg.SetInteractiveState(active: false);
        exitButton.gameObject.SetActive(false);
        exitButton.onClick.AddListener(Hide);
    }
    public void Show()
    {
        Text.text = string.Empty;
        for (int i=0; i< text.Count;++i )
        {
            Text.text += "\n \n" + text[i];
        }
        cg.Show();
        exitButton.gameObject.SetActive(true);
        autoButton.interactable = false;
        skipButton.interactable = false;
        cg.SetInteractiveState(active: true);
    }
    public void Hide()
    {
        autoButton.interactable = true;
        skipButton.interactable = true;
        cg.Hide();
        cg.SetInteractiveState(active: false);
    }
}
