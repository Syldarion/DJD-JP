using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

struct Resolution
{
    public int Width;
    public int Height;

    public Resolution(int w, int h)
    {
        Width = w;
        Height = h;
    }

    public override string ToString()
    {
        return string.Format("{0} x {1}", Width, Height);
    }
}

public class GameSettingsManager : MonoBehaviour
{
    public Text ResolutionText;

    Resolution[] Resolutions = new Resolution[9]
    {
        new Resolution(1280, 720),
        new Resolution(1366, 768),
        new Resolution(1600, 900),
        new Resolution(1920, 1080),
        new Resolution(2048, 1152),
        new Resolution(2560, 1440),
        new Resolution(2880, 1620),
        new Resolution(3200, 1800),
        new Resolution(3840, 2160)
    };

    Dictionary<string, int> Settings;
    Dictionary<string, int> Changes;

	void Start()
    {
        DontDestroyOnLoad(this);

        Settings = new Dictionary<string, int>();

        LoadConfigFile();

        ModifyResolution(0);
	}
	
	void Update()
    {

	}

    public void LoadConfigFile()
    {
        //file will always be usersettings.ini

        string[] settings = File.ReadAllLines("usersettings.ini");

        foreach(string s in settings)
        {
            if (!s.Contains("//")) //Allow comments in settings file
            {
                string setting_name = s.Split(' ')[0];
                int setting_val = int.Parse(s.Split(' ')[1]);

                Settings[setting_name] = setting_val;
            }
        }
    }

    public void WriteConfigFile()
    {
        //file will always be usersettings.ini

        List<string> settings = new List<string>();

        foreach (KeyValuePair<string, int> setting in Settings)
            settings.Add(string.Format("{0} {1}", setting.Key, setting.Value.ToString()));

        File.WriteAllLines("usersettings.ini", settings.ToArray());
    }

    public void ModifyResolution(int change)
    {
        Changes["CurrentResolution"] = Mathf.Clamp(Changes["CurrentResolution"] + change, 0, Resolutions.Length);
        ResolutionText.text = Resolutions[Changes["CurrentResolution"]].ToString();
    }

    public void ApplyChanges()
    {
        foreach (KeyValuePair<string, int> change in Changes)
            Settings[change.Key] = change.Value;

        Changes.Clear();

        WriteConfigFile();
    }
}
