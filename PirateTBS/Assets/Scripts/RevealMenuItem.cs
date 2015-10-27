using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class RevealMenuItem : MonoBehaviour
{
    CanvasGroup cGroup;

	void Start()
    {
        cGroup = GameObject.Find(string.Format("{0}SubPanel", this.name.Remove(this.name.Length - "SubPanel".Length + 2))).GetComponent<CanvasGroup>();
	}
	
	void Update()
    {

	}

    public void RevealChildren(bool reveal)
    {
        cGroup.interactable = reveal;
        StopCoroutine("FadeChildren");
        StartCoroutine("FadeChildren", reveal);
    }

    IEnumerator FadeChildren(bool fade_in)
    {
        if (fade_in)
            while (cGroup.alpha != 1)
            {
                cGroup.alpha += 0.05f;
                yield return new WaitForSeconds(0.025f);
            }
        else
            while (cGroup.alpha != 0)
            {
                cGroup.alpha -= 0.05f;
                yield return new WaitForSeconds(0.025f);
            }
    }
}
