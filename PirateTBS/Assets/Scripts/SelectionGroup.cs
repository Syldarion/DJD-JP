using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Selection Group")]
public class SelectionGroup : MonoBehaviour
{
    public bool AllowMultipleSelection;
    public Sprite BorderImage;
    public Sprite DefaultImage;
    public List<GameObject> SelectedObjects;

	void Start()
	{

	}
	
	void Update()
	{

	}

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

    public void ClearSelection()
    {
        foreach (GameObject go in SelectedObjects)
            go.GetComponent<Image>().sprite = DefaultImage;

        SelectedObjects.Clear();
    }
}
