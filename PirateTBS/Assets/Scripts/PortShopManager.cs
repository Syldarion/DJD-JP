﻿using UnityEngine;
using System.Collections;

public class PortShopManager : MonoBehaviour
{
    public Port CurrentPort;

	void Start()
	{

	}
	
	void Update()
	{

	}

    public void OpenShop()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        PopulateShop(CurrentPort);
    }

    public void PopulateShop(Port port)
    {
        
    }

    public void CloseShop()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}