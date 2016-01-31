using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DragObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject drag_object;
    public Transform initial_parent;

    void Start()
    {

	}

	void Update()
    {
        	
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
        drag_object = gameObject;
        initial_parent = transform.parent;
        transform.SetParent(GameObject.Find("Canvas").transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        drag_object = null;
        if (transform.parent == initial_parent)
            transform.SetParent(initial_parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}