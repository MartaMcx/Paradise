using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPanel : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;
    static InputPanel instance=null;
    public static InputPanel Instance() { return instance; }
    private string lastInput;
    public string getLastInput() { return lastInput; }
    private bool isWaitingOnUserInput;
    public bool getIsWaitingOnUserInput() {return isWaitingOnUserInput; }
    private CanvasGroupController cg;
    void Start()
    {
        cg= new CanvasGroupController(this, canvasGroup);
        cg.setAlpha(0);
        cg.SetInteractiveState(active: false);
        acceptButton.gameObject.SetActive(false);
        inputField.onValueChanged.AddListener(OnInputCharged); 
        acceptButton.onClick.AddListener(OnAcceptInput);
    }
    private void Awake()
    {
       instance = this;
    }
    public void Show(string title)
    {
        backButton.interactable = false;
        titleText.text = title;
        Debug.Log(title);
        inputField.text = string.Empty;
        cg.Show();
        isWaitingOnUserInput= true;
        cg.SetInteractiveState(active: true);
    }
    public void Hide()
    {
        backButton.interactable = true;
        cg.Hide();
        isWaitingOnUserInput= false;
        cg.SetInteractiveState(active: false);
    }
    public void OnAcceptInput()
    {
        if(inputField.text ==string.Empty)
        {
            return;
        }
        lastInput= inputField.text;
        Hide();
    }
    public void OnInputCharged(string value)
    {
        acceptButton.gameObject.SetActive(HasValirText());
    }
    private bool HasValirText()
    {
        return inputField.text != string.Empty ;
    }
}
