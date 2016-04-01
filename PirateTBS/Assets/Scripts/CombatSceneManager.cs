using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CombatSceneManager : MonoBehaviour {

    [HideInInspector]
    public static CombatSceneManager Instance;

    public bool InCombat;

    Scene combatscene;
    Scene mainscene;

    void Start()
    {
        Instance = this;
        InCombat = false;

        SceneSetup();
    }

    void SceneSetup()
    {
        combatscene = SceneManager.GetSceneByName("combat");
        mainscene = SceneManager.GetSceneByName("main");

        if (combatscene.IsValid())
        {
            InCombat = true;

            foreach (Ship s in CombatManager.Instance.PlayerFleet.Ships)
                s.GetComponent<NetworkTransform>().enabled = true;
            foreach (Ship s in CombatManager.Instance.EnemyFleet.Ships)
                s.GetComponent<NetworkTransform>().enabled = true;

            SceneManager.SetActiveScene(combatscene);
        }
    }

    public void ExitScene()
    {
        if (mainscene.IsValid())
        {
            InCombat = false;

            foreach (Ship s in CombatManager.Instance.PlayerFleet.Ships)
                s.GetComponent<NetworkTransform>().enabled = false;
            foreach (Ship s in CombatManager.Instance.EnemyFleet.Ships)
                s.GetComponent<NetworkTransform>().enabled = false;

            SceneManager.SetActiveScene(mainscene);
            if (combatscene.IsValid())
            {
                SceneManager.UnloadScene(combatscene.buildIndex);
            }
        } 
    }
}
