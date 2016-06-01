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
    List<RectTransform> panels;
    RectTransform current_option_panel;                 //Reference to the current option panel

	void Start()
    {
        DialogueOpen = false;
        options = new List<Button>();
        panels = new List<RectTransform>();

        CurrentDialogue = this;
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

        foreach (Button b in options) Destroy(b.gameObject);
        foreach (RectTransform rt in panels) Destroy(rt.gameObject);

        options.Clear();
        panels.Clear();
    }

    /// <summary>
    /// Create a new dialogue box
    /// </summary>
    /// <param name="message">Message for the dialogue box</param>
    /// <param name="duration">Duration of the dialog box [0 = infinite]</param>
    public void NewDialogue(string message, float duration = 0.0f)
    {
        CurrentDialogue = this;

        OpenDialogue();
        DialogueMessage.text = message;

        if (duration > 0.0f)
            StartCoroutine(TimedDialogue(duration));
    }

    IEnumerator TimedDialogue(float time)
    {
        while(time > 0.0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        CloseDialogue();
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
            current_option_panel.gameObject.SetActive(true);
            panels.Add(current_option_panel);
        }

        Button new_button = Instantiate(OptionButtonPrefab).GetComponent<Button>();
        new_button.transform.SetParent(current_option_panel.transform, false);

        new_button.GetComponentInChildren<Text>().text = option_text;
        new_button.onClick.AddListener(option_action);

        new_button.gameObject.SetActive(true);

        options.Add(new_button);
    }
}