using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CombatSceneManager : MonoBehaviour
{

    [HideInInspector]
    public static CombatSceneManager Instance;

    Scene combat_scene;
    Scene main_scene;

    void Start()
    {
        Instance = this;

        SceneSetup();
    }

    void SceneSetup()
    {
        combat_scene = SceneManager.GetSceneByName("combat");
        main_scene = SceneManager.GetSceneByName("main");

        if (combat_scene.IsValid())
            SceneManager.SetActiveScene(combat_scene);
    }

    public void ExitScene()
    {
        if (main_scene.IsValid())
        {
            SceneManager.SetActiveScene(main_scene);
            CanvasGroup main_scene_canvas = GameObject.Find("MainSceneCanvas").GetComponent<CanvasGroup>();
            if (main_scene_canvas)
            {
                main_scene_canvas.alpha = 1;
                main_scene_canvas.interactable = true;
                main_scene_canvas.blocksRaycasts = true;
            }
            else
                Debug.LogWarning("Could not find MainSceneCanvas");

            if (combat_scene.IsValid())
                SceneManager.UnloadScene(combat_scene.buildIndex);
        }
    }
}
