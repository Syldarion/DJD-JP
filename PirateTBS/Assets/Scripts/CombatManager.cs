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
    public ShotType SelectedShotType;

    public SelectionGroup PlayerSelectionGroup;
    public SelectionGroup EnemySelectionGroup;
    public Text PlayerFleetName;
    public Text EnemyFleetName;
    public RectTransform PlayerShipList;
    public RectTransform EnemyShipList;
    public Text HitChanceText;
    public Text SailDamageText;
    public Text HullDamageText;

    void Start()
    {

    }

    void Update()
    {

    }

    public void OpenCombatPanel()
    {
        PlayerScript.MyPlayer.UIOpen = true;

        PanelUtilities.ActivatePanel(GetComponent<CanvasGroup>());
    }

    public void CloseCombatPanel()
    {
        PlayerScript.MyPlayer.UIOpen = false;

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

        RunCalculations();
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

        RunCalculations();
    }

    void RunCalculations()
    {
        if (SelectedPlayerShip && SelectedEnemyShip)
        {
            CalculateDamage(SelectedShotType);
            CalculateHitChance();
        }
    }

    /// <summary>
    /// Calculate chance to hit enemy ship
    /// </summary>
    public void CalculateHitChance()
    {
        HitChanceText.text = (100 - SelectedEnemyShip.DodgeChance).ToString();
    }

    /// <summary>
    /// Calculate damage based on shot type
    /// </summary>
    /// <param name="type">Shot type being used</param>
    public void CalculateDamage(ShotType type)
    {
        double hitmodifier = (100 - SelectedEnemyShip.DodgeChance) / 100;
        switch (type)
        {
            case ShotType.Normal:
                HullDamageText.text = ((SelectedPlayerShip.Cannons * 2) * hitmodifier).ToString();
                SailDamageText.text = ((SelectedPlayerShip.Cannons * 2) * hitmodifier).ToString();
                break;
            case ShotType.Cluster:
                HullDamageText.text = ((SelectedPlayerShip.Cannons * 3) * hitmodifier).ToString();
                SailDamageText.text = ((SelectedPlayerShip.Cannons * 1) * hitmodifier).ToString();
                break;
            case ShotType.Chain:
                HullDamageText.text = ((SelectedPlayerShip.Cannons * 1) * hitmodifier).ToString();
                SailDamageText.text = ((SelectedPlayerShip.Cannons * 3) * hitmodifier).ToString();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Switch shot type being used in combat
    /// </summary>
    /// <param name="type">New shot type to use</param>
    public void SwitchShotType(ShotType type)
    {
        SelectedShotType = type;
        CalculateDamage(type);
    }

    /// <summary>
    /// Initiate combat animation
    /// </summary>
    public void StartCombat()
    {
        ApplyDamage();
        if (SelectedPlayerShip.HullHealth == 0)
        {
            PlayerFleet.CmdRemoveShip(SelectedPlayerShip.Name);

            ShipStatBlock to_destroy = PlayerSelectionGroup.SelectedObjects[0].GetComponent<ShipStatBlock>();

            if (to_destroy)
            {
                PlayerSelectionGroup.RemoveSelection(to_destroy.gameObject);
                Destroy(to_destroy);
            }
        }

        if (PlayerSelectionGroup.SelectedObjects.Count > 0)
            PlayerSelectionGroup.SelectedObjects[0].GetComponent<ShipStatBlock>().PopulateStatBlock(SelectedPlayerShip);
        if (EnemySelectionGroup.SelectedObjects.Count > 0)
            EnemySelectionGroup.SelectedObjects[0].GetComponent<ShipStatBlock>().PopulateStatBlock(SelectedEnemyShip);
    }

    public void ApplyDamage()
    {
        switch (SelectedShotType)
        {
            case ShotType.Normal:
                for (int i = 0; i < SelectedPlayerShip.Cannons; i++)
                {
                    int temp = Random.Range(1, 101);
                    if (temp < (100 - SelectedEnemyShip.DodgeChance))
                    {
                        SelectedEnemyShip.HullHealth = SelectedEnemyShip.HullHealth - 2;
                        SelectedEnemyShip.SailHealth = SelectedEnemyShip.SailHealth - 2;
                    }
                }
                break;
            case ShotType.Cluster:
                for (int i = 0; i < SelectedPlayerShip.Cannons; i++)
                {
                    int temp = Random.Range(1, 101);
                    if (temp < (100 - SelectedEnemyShip.DodgeChance))
                    {
                        SelectedEnemyShip.HullHealth = SelectedEnemyShip.HullHealth - 3;
                        SelectedEnemyShip.SailHealth = SelectedEnemyShip.SailHealth - 1;
                    }
                }
                break;
            case ShotType.Chain:
                for (int i = 0; i < SelectedPlayerShip.Cannons; i++)
                {
                    int temp = Random.Range(1, 101);
                    if (temp < (100 - SelectedEnemyShip.DodgeChance))
                    {
                        SelectedEnemyShip.HullHealth = SelectedEnemyShip.HullHealth - 1;
                        SelectedEnemyShip.SailHealth = SelectedEnemyShip.SailHealth - 3;
                    }
                }
                break;
            default:
                break;
        }
        if (SelectedEnemyShip.SailHealth < 0)
        {
            SelectedEnemyShip.SailHealth = 0;
        }
        if (SelectedEnemyShip.HullHealth < 0)
        {
            SelectedEnemyShip.HullHealth = 0;
        }
    }
}
