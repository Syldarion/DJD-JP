using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    [HideInInspector]
    public static CombatManager Instance;

    public ShipStatBlock StatBlockPrefab;

    public Ship PlayerShip;
    public Ship EnemyShip;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        
    }

    public void OpenCombatPanel()
    {
        PlayerScript.MyPlayer.OpenUI = GetComponent<CanvasGroup>();

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
    }

    public void CloseCombatPanel()
    {
        PlayerScript.MyPlayer.OpenUI = null;

        PanelUtilities.DeactivatePanel(GetComponent<CanvasGroup>());
    }
}
