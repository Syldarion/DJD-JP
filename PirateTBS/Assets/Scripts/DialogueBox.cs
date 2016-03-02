using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueBox : MonoBehaviour
{
    public static bool DialogueOpen;
    public static DialogueBox CurrentDialogue;

    public Text DialogueMessage;
    public RectTransform OptionPanelPrefab;
    public Button OptionButtonPrefab;

    List<Button> options;
    RectTransform current_option_panel;

	void Start()
    {
        DialogueOpen = false;
	}
	
	void Update()
    {
        	
	}

    public void OpenDialogue()
    {
        DialogueOpen = true;

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
    }

    public void CloseDialogue()
    {
        DialogueOpen = false;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }

    public void NewDialogue(string message, float duration = 0.0f)
    {
        if(DialogueOpen)
            CurrentDialogue.CloseDialogue();

        OpenDialogue();
        DialogueMessage.text = message;

        if (duration > 0.0f)
            Destroy(this.gameObject, duration);
    }

    public void AddOption(string option_text, UnityAction option_action)
    {
        if (options.Count % 3 == 0)
        {
            current_option_panel = Instantiate(OptionPanelPrefab).GetComponent<RectTransform>();
            current_option_panel.SetParent(transform, false);
        }

        Button new_button = Instantiate(OptionButtonPrefab).GetComponent<Button>();
        new_button.transform.SetParent(current_option_panel.transform, false);

        new_button.GetComponentInChildren<Text>().text = option_text;
        new_button.onClick.AddListener(option_action);
    }
}