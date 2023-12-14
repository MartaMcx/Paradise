using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using System;

public class ChoicePanel : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private static ChoicePanel instance;
    public static ChoicePanel Instance() { return instance; }

    [SerializeField] private float BUTTON_MIN_WIDTH = 50;
    [SerializeField] private float BUTTON_MAX_WIDTH = 1000;
    [SerializeField] private float BUTTON_WIDTH_PADDING = 25;

    private float BUTTON_HEIGHT_PERLINE = 50f;
    private float BUTTON_HEIGHT_PADDING = 20;


    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleTex;
    [SerializeField] private GameObject choiceButtonpref;
    [SerializeField] private VerticalLayoutGroup buttonLayoutGroup;
    private bool isWaitingOnUserChoice;
    public bool getIsWaitingOnUserChoice() { return isWaitingOnUserChoice; }

    private CanvasGroupController cg = null;
    private List<ChoiceButton> buttons = new List<ChoiceButton>();
    private ChoicePanelDecision lastDecision;
    public ChoicePanelDecision getLastDecision() { return lastDecision; }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        cg = new CanvasGroupController(this, canvasGroup);
        cg.setAlpha(0);
        cg.SetInteractiveState(false);
    }
    public void Show(string question, string[] choices)
    {
        backButton.interactable = false;
        lastDecision = new ChoicePanelDecision(question, choices);
        isWaitingOnUserChoice = true;
        cg.Show();
        cg.SetInteractiveState(active: true);
        titleTex.text = question;
        StartCoroutine(GenerateChoice(choices));
    }
    private IEnumerator GenerateChoice(string[] choices)
    {
        float maxWidth = 0;
        for (int i = 0; i < choices.Length; ++i) 
        {
            ChoiceButton choiceButton;
            if (i < buttons.Count)
            {
                choiceButton = buttons[i];
            } 
            else 
            {
                GameObject newButtonObject = Instantiate(choiceButtonpref, buttonLayoutGroup.transform);
                newButtonObject.SetActive(true);

                Button newButton = newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newTitle = newButtonObject.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayoout = newButtonObject.GetComponent<LayoutElement>();

                choiceButton = new ChoiceButton();
                choiceButton.Iniciate(newButton, newLayoout, newTitle);
                buttons.Add(choiceButton);
            }
            choiceButton.getButton().onClick.RemoveAllListeners();
            int buttonIndex = i;
            choiceButton.getButton().onClick.AddListener(()=>AcceptedAnswer(buttonIndex));
            choiceButton.getTitle().text=choices[i];

            float buttonWhidth = Mathf.Clamp(BUTTON_WIDTH_PADDING + choiceButton.getTitle().preferredWidth, BUTTON_MIN_WIDTH, BUTTON_MAX_WIDTH);
            maxWidth = Mathf.Max(maxWidth, buttonWhidth);
        }
        foreach(var button in buttons)
        {
            button.getLayout().preferredWidth = maxWidth;
        }
        for(int i=0; i<choices.Length;++i)
        {
            bool Show =(i<choices.Length);
            if (i == 0) 
            {
                buttons[i].getButton().Select();
            }
            buttons[i].getButton().gameObject.SetActive(Show);
        }
        yield return new WaitForEndOfFrame();
        foreach(var button in buttons)
        {
            button.getTitle().ForceMeshUpdate();
            int lines = button.getTitle().textInfo.lineCount;
            button.getLayout().preferredWidth = BUTTON_HEIGHT_PADDING + (BUTTON_HEIGHT_PERLINE * lines);

        }
    }
    public void Hide()
    {
        backButton.interactable = true;
        cg.SetInteractiveState(false);
        cg.Hide();
    }
    private void AcceptedAnswer(int index)
    {
        if (index < 0 || index > lastDecision.getChoices().Length - 1)
        { return; }
        lastDecision.setAnswerIndex(index);
        isWaitingOnUserChoice = false;
        Hide();
    }
    public class ChoicePanelDecision
    {
        private string question = string.Empty;
        private int answerIndex = -1;
        private string[] choces = new string[0];
        public string[] getChoices() { return choces; }
        public void setAnswerIndex(int value) { answerIndex = value; }
        public int getAnswerIndex() { return answerIndex ; }

        public ChoicePanelDecision(string question, string[] choces)
        {
            this.question = question;
            this.answerIndex = -1;
            this.choces = choces;
        }
    }
    class ChoiceButton
    {
        private Button button;
        private TextMeshProUGUI title;
        private LayoutElement layout;
        public Button getButton() {  return button; }
        public TextMeshProUGUI getTitle() {  return title; }
        public LayoutElement getLayout() { return layout;}
        public void Iniciate(Button button, LayoutElement layout, TextMeshProUGUI title)
        {
            this.button = button;
            this.layout = layout;
            this.title = title;
        }

    }
}
