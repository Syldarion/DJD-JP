using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Resolution
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
    
    public static bool operator==(Resolution lhs, Resolution rhs)
    {
        return (lhs.Width == rhs.Width) && (lhs.Height == rhs.Height);
    }

    public static bool operator!=(Resolution lhs, Resolution rhs)
    {
        return (lhs.Width != rhs.Width) || (lhs.Height != rhs.Height);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Mathf.Abs(Width - Height) * Width;
    }
}

public class GameSettingsManager : MonoBehaviour
{
    //Settings

    //ResolutionWidth
    //ResolutionHeight
    //AALevel
    //Fullscreen
    //MasterVolume
    //MusicVolume
    //EffectVolume

    public Text ResolutionText;
    public Text CurrentAAText;
    public Toggle FullscreenToggle;

    public AudioMixer MasterMixer;

    Resolution[] Resolutions = new Resolution[9]
    {
        new Resolution(1280, 720),  //16:9
        new Resolution(1366, 768),  //16:9
        new Resolution(1600, 900),  //16:9
        new Resolution(1920, 1080), //16:9
        new Resolution(2048, 1152), //16:9
        new Resolution(2560, 1440), //16:9
        new Resolution(2880, 1620), //16:9
        new Resolution(3200, 1800), //16:9
        new Resolution(3840, 2160)  //16:9
    };
    int CurrentResolutionIndex = 0;

    int[] AALevels = new int[4] { 0, 2, 4, 8 };
    int CurrentAAIndex = 0;

    Dictionary<string, int> Settings;
    Dictionary<string, int> Changes;

    string[] DefaultSettings = new string[7]
    {
        "ResolutionWidth 1280",
        "ResolutionHeight 720",
        "AALevel 0",
        "Fullscreen 0",
        "MasterVolume 100",
        "MusicVolume 100",
        "EffectVolume 100"
    };

	void Start()
    {
        //DontDestroyOnLoad(this);

        Settings = new Dictionary<string, int>();
        Changes = new Dictionary<string, int>();

        LoadConfigFile();

        ModifyResolution(0);
	}
	
	void Update()
    {

	}

    public void LoadConfigFile()
    {
        //file will always be usersettings.ini

        if (!File.Exists("usersettings.ini"))
            File.WriteAllLines("usersettings.ini", DefaultSettings);
        string[] settings = File.ReadAllLines("usersettings.ini");

        foreach(string s in settings)
        {
            if (s != string.Empty && s[0] != '#') //Allow comments/blank lines in settings file
            {
                string setting_name = s.Split(' ')[0];
                int setting_val = int.Parse(s.Split(' ')[1]);

                if (!Settings.ContainsKey(setting_name))
                    Settings.Add(setting_name, setting_val);
                else
                    Settings[setting_name] = setting_val;
            }
        }

        //Update resolution
        Resolution user_res = new Resolution(Settings["ResolutionWidth"], Settings["ResolutionHeight"]);

        CurrentResolutionIndex = 0;

        for(int i = 0; i < Resolutions.Length; i++)
        {
            if (Resolutions[i] == user_res)
                CurrentResolutionIndex = i;
        }

        ResolutionText.text = user_res.ToString();

        //Update antialiasing level

        int user_aa = Settings["AALevel"];

        CurrentAAIndex = 0;

        for (int i = 0; i < 4; i++)
            if (AALevels[i] == user_aa)
                CurrentAAIndex = i;

        CurrentAAText.text = user_aa.ToString();

        int user_wfs = Mathf.Clamp(Settings["Fullscreen"], 0, 1);

        FullscreenToggle.isOn = user_wfs == 0 ? false : true;

        Screen.SetResolution(Settings["ResolutionWidth"], Settings["ResolutionHeight"], Settings["Fullscreen"] == 1);
        QualitySettings.antiAliasing = Settings["AALevel"];
    }

    public void WriteConfigFile()
    {
        //file will always be usersettings.ini

        List<string> settings = new List<string>();

        foreach (KeyValuePair<string, int> setting in Settings)
            settings.Add(string.Format("{0} {1}", setting.Key, setting.Value.ToString()));

        File.WriteAllLines("usersettings.ini", settings.ToArray());
    }

    public void ChangeSetting(string key, int value)
    {
        if (!Changes.ContainsKey(key))
            Changes.Add(key, value);
        else
            Changes[key] = value;
    }

    public void ModifyResolution(int change)
    {
        CurrentResolutionIndex = Mathf.Clamp(CurrentResolutionIndex + change, 0, Resolutions.Length);
        ResolutionText.text = Resolutions[CurrentResolutionIndex].ToString();

        ChangeSetting("ResolutionWidth", Resolutions[CurrentResolutionIndex].Width);
        ChangeSetting("ResolutionHeight", Resolutions[CurrentResolutionIndex].Height);
    }

    public void ModifyAA(int change)
    {
        CurrentAAIndex = Mathf.Clamp(CurrentAAIndex + change, 0, 3);
        CurrentAAText.text = AALevels[CurrentAAIndex].ToString();

        ChangeSetting("AALevel", AALevels[CurrentAAIndex]);
    }

    public void ToggleFullscreen(bool fullscreen)
    {
        ChangeSetting("Fullscreen", fullscreen ? 1 : 0);
    }

    public void ModifyMasterVolume(float value)
    {
        MasterMixer.SetFloat("MasterVolume", value - 80);
        ChangeSetting("MasterVolume", (int)(value - 80));
    }

    public void ModifyMusicVolume(float value)
    {
        MasterMixer.SetFloat("MusicVolume", value - 80);
        ChangeSetting("MusicVolume", (int)(value - 80));
    }

    public void ModifyEffectsVolume(float value)
    {
        MasterMixer.SetFloat("EffectsVolume", value - 80);
        ChangeSetting("EffectsVolume", (int)(value - 80));
    }

    public void ApplyChanges()
    {
        foreach (KeyValuePair<string, int> change in Changes)
            Settings[change.Key] = change.Value;

        Changes.Clear();

        WriteConfigFile();
        
        //Video settings
        Screen.SetResolution(Settings["ResolutionWidth"], Settings["ResolutionHeight"], Settings["Fullscreen"] == 1);
        QualitySettings.antiAliasing = Settings["AALevel"];
    }
}
