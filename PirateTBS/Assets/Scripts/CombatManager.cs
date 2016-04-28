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

    public SelectionGroup PlayerSelectionGroup;
    public SelectionGroup EnemySelectionGroup;
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
        { CloseCombatPanel(); }
        else
        {
            PlayerSelectionGroup.SelectedObjects.Clear();
            EnemySelectionGroup.SelectedObjects.Clear();

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

                stat_block.StatBlockDelegate = SelectPlayerShip;
            }
            foreach (Ship enemy_ship in enemy_fleet.Ships)
            {
                ShipStatBlock stat_block = Instantiate(StatBlockPrefab).GetComponent<ShipStatBlock>();
                stat_block.PopulateStatBlock(enemy_ship);
                stat_block.transform.SetParent(EnemyShipList, false);

                stat_block.StatBlockDelegate = SelectEnemyShip;
            }
        }
    }

    /// <summary>
    /// Select player ship to use in combat
    /// </summary>
    public void SelectPlayerShip()
    {
        ShipStatBlock current_block = null;

        Debug.Log(PlayerSelectionGroup.SelectedObjects.Count);
        if (PlayerSelectionGroup.SelectedObjects.Count > 0)
            current_block = PlayerSelectionGroup.SelectedObjects[0].GetComponent<ShipStatBlock>();

        if (current_block)
            SelectedPlayerShip = current_block.ReferenceShip;
    }

    /// <summary>
    /// Select enemy ship to use in combat
    /// </summary>
    public void SelectEnemyShip()
    {
        ShipStatBlock current_block = null;

        if (EnemySelectionGroup.SelectedObjects.Count > 0)
            current_block = EnemySelectionGroup.SelectedObjects[0].GetComponent<ShipStatBlock>();

        if (current_block)
            SelectedEnemyShip = current_block.ReferenceShip;
    }

    /// <summary>
    /// Initiate combat animation
    /// </summary>
    public void StartCombat()
    {
        LoadNextScene();
        CloseCombatPanel();

        //need to close player end turn panel and mini map panel and maybe player info panel.
    }

    void LoadNextScene()
    {
        SceneManager.LoadSceneAsync("combat", LoadSceneMode.Additive);
    }
}
