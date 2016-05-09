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

    public Fleet PlayerFleet;
    public Fleet EnemyFleet;
    public Ship SelectedPlayerShip;
    public Ship SelectedEnemyShip;
    
    public Text PlayerFleetName;
    public Text EnemyFleetName;
    public RectTransform PlayerShipList;
    public RectTransform EnemyShipList;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) //For testing purposes.
        {
            OpenCombatPanel();
            PopulateFleetLists(PlayerScript.MyPlayer.ActiveFleet, PlayerScript.MyPlayer.ActiveFleet);
        }
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

    /// <summary>
    /// Populate fleet list UI with player/enemy fleets
    /// </summary>
    /// <param name="player_fleet">player fleet to populate with</param>
    /// <param name="enemy_fleet">enemy fleet to populate with</param>
    public void PopulateFleetLists(Fleet player_fleet, Fleet enemy_fleet)
    {
        PlayerFleet = player_fleet;
        EnemyFleet = enemy_fleet;
        if (PlayerFleet.Ships.Count < 1)
            CloseCombatPanel();
        else
        {
            foreach (ShipStatBlock child in PlayerShipList.GetComponentsInChildren<ShipStatBlock>())
            {
                Destroy(child.gameObject);
            }
            foreach (ShipStatBlock child in EnemyShipList.GetComponentsInChildren<ShipStatBlock>())
            {
                Destroy(child.gameObject);
            }
            foreach (Ship player_ship in player_fleet.Ships)
            {
                ShipStatBlock stat_block = Instantiate(StatBlockPrefab).GetComponent<ShipStatBlock>();
                stat_block.PopulateStatBlock(player_ship);
                stat_block.transform.SetParent(PlayerShipList, false);
            }
            foreach (Ship enemy_ship in enemy_fleet.Ships)
            {
                ShipStatBlock stat_block = Instantiate(StatBlockPrefab).GetComponent<ShipStatBlock>();
                stat_block.PopulateStatBlock(enemy_ship);
                stat_block.transform.SetParent(EnemyShipList, false);
            }
        }
    }

    /// <summary>
    /// Initiate combat animation
    /// </summary>
    public void StartCombat()
    {
        CanvasGroup main_scene_canvas = GameObject.Find("MainSceneCanvas").GetComponent<CanvasGroup>();
        if (main_scene_canvas)
        {
            main_scene_canvas.alpha = 0;
            main_scene_canvas.blocksRaycasts = false;
            main_scene_canvas.interactable = false;
        }
        else
            Debug.LogWarning("Could not find MainSceneCanvas");

        LoadCombatScene();
        CloseCombatPanel();
    }

    void LoadCombatScene()
    {
        SceneManager.LoadSceneAsync("combat", LoadSceneMode.Additive);
    }
}
