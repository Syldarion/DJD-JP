using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueBox : MonoBehaviour
{
    public static bool DialogueOpen;                    //Is a dialogue box open?
    public static DialogueBox CurrentDialogue;          //Reference to currently open dialogue box

    public Text DialogueMessage;                        //Reference to the message of the dialogue box
    public RectTransform OptionPanelPrefab;             //Reference to the prefab for instantiating option panels
    public Button OptionButtonPrefab;                   //Reference to the prefab for instantiating option buttons

    List<Button> options;                               //List of options in the dialogue box
    RectTransform current_option_panel;                 //Reference to the current option panel

	void Start()
    {
        DialogueOpen = false;
        options = new List<Button>();
	}
	
	void Update()
    {
        	
	}

    /// <summary>
    /// Open dialogue box
    /// </summary>
    public void OpenDialogue()
    {
        DialogueOpen = true;

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
    }

    /// <summary>
    /// Close dialogue box
    /// </summary>
    public void CloseDialogue()
    {
        DialogueOpen = false;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }

    /// <summary>
    /// Create a new dialogue box
    /// </summary>
    /// <param name="message">Message for the dialogue box</param>
    /// <param name="duration">Duration of the dialog box [0 = infinite]</param>
    public void NewDialogue(string message, float duration = 0.0f)
    {
        if(DialogueOpen)
            CurrentDialogue.CloseDialogue();

        OpenDialogue();
        DialogueMessage.text = message;

        if (duration > 0.0f)
            Destroy(this.gameObject, duration);
    }

    /// <summary>
    /// Add an option to the dialogue box
    /// </summary>
    /// <param name="option_text">Text to display for the option</param>
    /// <param name="option_action">Function to run when this option is selected</param>
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

        options.Add(new_button);
    }
}