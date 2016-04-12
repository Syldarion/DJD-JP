using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Selection Group")]
public class SelectionGroup : MonoBehaviour
{
    public bool AllowMultipleSelection;         //Does the selection group allow you to select multiple objects?
    public Sprite BorderImage;                  //Image to put around object when selected
    public Sprite DefaultImage;                 //Image to put around object when not selected
    public List<GameObject> SelectedObjects;    //List of selected objects

	void Start()
	{

	}
	
	void Update()
	{

	}

    /// <summary>
    /// Add object to selection list
    /// </summary>
    /// <param name="selection">Object to add</param>
    public void AddSelection(GameObject selection)
    {
        if (!selection.transform.IsChildOf(transform))
            return;

        if (!AllowMultipleSelection && SelectedObjects.Count != 0)
            RemoveSelection(SelectedObjects[0]);

        if (SelectedObjects.Contains(selection))
            RemoveSelection(selection);
        else
        {
            selection.GetComponent<Image>().sprite = BorderImage;
            SelectedObjects.Add(selection);
        }
    }

    /// <summary>
    /// Remove object from selection list, if it is present
    /// </summary>
    /// <param name="selection">Object to remove</param>
    public void RemoveSelection(GameObject selection)
    {
        if (!selection.transform.IsChildOf(transform))
            return;

        if (SelectedObjects.Contains(selection))
        {
            selection.GetComponent<Image>().sprite = DefaultImage;
            SelectedObjects.Remove(selection);
        }
    }

    /// <summary>
    /// Remove all objects from selection list
    /// </summary>
    public void ClearSelection()
    {
        foreach (GameObject go in SelectedObjects)
            go.GetComponent<Image>().sprite = DefaultImage;

        SelectedObjects.Clear();
    }
}
