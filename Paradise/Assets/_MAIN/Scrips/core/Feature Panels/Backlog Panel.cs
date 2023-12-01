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
    [SerializeField] private CanvasGroup canvasGroup;
    private List<string> text;
    public void setTest(List<string> text) { this.text = text; }
    public void putInTest(string line) { text.Add(line); }

    private CanvasGroupController cg;
    static BacklogPanel instance = null;
    public static BacklogPanel Instance() { return instance; }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
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
        cg.SetInteractiveState(active: true);
    }
    public void Hide()
    {
        cg.Hide();
        cg.SetInteractiveState(active: false);
    }
}
