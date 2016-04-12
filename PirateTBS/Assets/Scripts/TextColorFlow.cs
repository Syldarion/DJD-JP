using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class TextColorFlow : MonoBehaviour
{
    public string BaseColorHex;         //Hexadecimal value for default text color
    public string FlowColorHex;         //Hexadecimal value for color to change text to
    public float FlowTime;              //Time in seconds it takes to complete a run across all letters
    public float DelayTime;             //Time in seconds to delay between running across letter
    public string BaseText;             //Text to display

    float CurrentTime;                  //Current time in seconds of cycle
    float TotalTime;                    //Total time for a cycle
    int CurrentStep;                    //Current index of cycle
    int MaxStep;                        //Max index of cycle
    float[] StepTimes;                  //Times in seconds for when to step

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

    /// <summary>
    /// Builds string with appropriate letter colors
    /// </summary>
    /// <returns></returns>
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
