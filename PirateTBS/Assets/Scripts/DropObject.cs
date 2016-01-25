using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DropObject : MonoBehaviour, IDropHandler
{
	void Start()
	{
		
	}

	void Update()
	{
		
	}

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(transform.name);
        eventData.pointerDrag.transform.SetParent(transform.GetChild(0));
    }
}
