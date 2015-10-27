using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RevealMenuPanel : MonoBehaviour
{
    float pixelStep;
    float transitionTime;
    LayoutElement lElement;
    LayoutElement childLElement;

	void Start()
    {
        pixelStep = 1;
        transitionTime = 0.1f;

        lElement = GetComponent<LayoutElement>();
        childLElement = transform.GetChild(0).GetComponent<LayoutElement>();
	}

	void Update()
    {

	}

    public void Reveal(bool reveal)
    {
        StopCoroutine("RevealChildren");
        StartCoroutine("RevealChildren", reveal);
    }

    IEnumerator RevealChildren(bool reveal)
    {
        if(reveal)
        {
            while(lElement.preferredHeight < 250.0f)
            {
                lElement.preferredHeight = Mathf.Lerp(lElement.preferredHeight, 250.0f, (1 / transitionTime) * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while(lElement.preferredHeight > 100.0f)
            {
                lElement.preferredHeight = Mathf.Lerp(lElement.preferredHeight, 100.0f, (1 / transitionTime) * Time.deltaTime);
                yield return null;
            }
        }
    }
}
