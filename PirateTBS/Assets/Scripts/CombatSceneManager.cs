using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CombatSceneManager : MonoBehaviour {

    [HideInInspector]
    public static CombatSceneManager Instance;

    Scene combatscene;
    Scene mainscene;

    void Start()
    {
        Instance = this;

        SceneSetup();
    }

    void SceneSetup()
    {
        combatscene = SceneManager.GetSceneByName("combat");
        mainscene = SceneManager.GetSceneByName("main");

        if (combatscene.IsValid())
        {
            SceneManager.SetActiveScene(combatscene);
        }
    }

    public void ExitScene()
    {
        if (mainscene.IsValid())
        {
            SceneManager.SetActiveScene(mainscene);
            if (combatscene.IsValid())
            {
                SceneManager.UnloadScene(combatscene.buildIndex);
            }
        } 
    }
}
