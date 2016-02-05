using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    public Fleet PlayerFleet;
    public Fleet EnemyFleet;
    public Ship SelectedPlayerShip;
    public Ship SelectedEnemyShip;
    public ShotType SelectedShotType;

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
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void CloseCombatPanel()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    /// <summary>
    /// Populate fleet list UI with player/enemy fleets
    /// </summary>
    /// <param name="player_fleet">player fleet to populate with</param>
    /// <param name="enemy_fleet">enemy fleet to populate with</param>
    public void PopulateFleetLists(Fleet player_fleet, Fleet enemy_fleet)
    {
        //for each ship in fleet
        //create ship stat block
        //add listener to on click
        //SelectPlayerShip(statblock.ReferenceShip)

        //Parenting
        //for each ship in player_fleet
        //add to PlayerFleetShipListContent
        //for each ship in enemy_fleet
        //add to EnemeyFleetShipListContent

        //StatBlock.transform.SetParent(content);
    }

    /// <summary>
    /// Select player ship to use in combat
    /// </summary>
    /// <param name="player_ship">Ship to be used</param>
    public void SelectPlayerShip(Ship player_ship)
    {
        //Highlight the selected stat block or some shit
        //Set SelectedPlayerShip to the passed ship
    }

    /// <summary>
    /// Select enemy ship to use in combat
    /// </summary>
    /// <param name="enemy_ship">Ship to be used</param>
    public void SelectEnemyShip(Ship enemy_ship)
    {
        //Highlight the selected stat block
        //Set SelectedEnemyShip to the passed ship
    }

    /// <summary>
    /// Calculate chance to hit enemy ship
    /// </summary>
    public void CalculateHitChance()
    {
        //Just subtract dodge chance of enemy ship from 100
        //Set hit chance text
    }

    /// <summary>
    /// Calculate damage based on shot type
    /// </summary>
    /// <param name="type">Shot type being used</param>
    public void CalculateDamage(ShotType type)
    {
        //Calculate damage depending on shot type
        //Set damage text

        //type - sail, hull
        //normal - 2, 2
        //chain - 3, 1
        //cluster 1, 3
    }

    /// <summary>
    /// Switch shot type being used in combat
    /// </summary>
    /// <param name="type">New shot type to use</param>
    public void SwitchShotType(ShotType type)
    {
        //Modify selectedshottype
        //Call calculate damage
    }

    /// <summary>
    /// Initiate combat animation
    /// </summary>
    public void StartCombat()
    {

    }
}
