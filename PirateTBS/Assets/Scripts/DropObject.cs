using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DropObject : MonoBehaviour, IDropHandler
{
    public Transform DropParent;

	void Start()
	{
		
	}

	void Update()
	{
		
	}

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(transform.name);
        if (DropParent != null)
            eventData.pointerDrag.transform.SetParent(DropParent);
    }
}
