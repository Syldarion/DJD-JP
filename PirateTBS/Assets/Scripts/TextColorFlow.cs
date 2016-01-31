using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class TextColorFlow : MonoBehaviour
{
    public string BaseColorHex;
    public string FlowColorHex;
    public float FlowTime;
    public float DelayTime;
    public string BaseText;

    float CurrentTime;
    float TotalTime;
    int CurrentStep;
    int MaxStep;
    float[] StepTimes;

	void Start()
    {
        CurrentTime = 0.0f;
        TotalTime = FlowTime + DelayTime;
        CurrentStep = 99;
        MaxStep = BaseText.Length + 1;      //+1 so that the last letter doesn't stay the flow color during the delay

        StepTimes = new float[MaxStep];
        for (int i = 0; i < MaxStep; i++)
            StepTimes[i] = DelayTime + (FlowTime / MaxStep * i);
    }

	void Update()
    {
        CurrentTime += Time.deltaTime;

        //This means that the time is within the frame for "flowing"
        if (CurrentTime > TotalTime - FlowTime)
            for (int i = 0; i < MaxStep; i++)
                if (CurrentTime > StepTimes[i])
                    CurrentStep = i;
        GetComponent<Text>().text = BuildFlowString();
        if (CurrentTime > TotalTime)
            CurrentTime = 0.0f;
	}

    string BuildFlowString()
    {
        StringBuilder builder = new StringBuilder();

        for(int i = 0; i < MaxStep - 1; i++)
        {
            if(i == CurrentStep)
                builder.Append(string.Format("<color=#{0}>{1}</color>", FlowColorHex, BaseText[i]));
            else
                builder.Append(string.Format("<color=#{0}>{1}</color>", BaseColorHex, BaseText[i]));
        }

        return builder.ToString();
    }
}
