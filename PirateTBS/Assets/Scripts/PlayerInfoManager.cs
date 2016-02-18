using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfoManager : MonoBehaviour
{
    public PlayerScript OwningPlayer; //I'm sure this will be relevant somewhere

    public ShipStatBlock StatBlockPrefab;

    public Text CaptainNameText;
    public Text ShipCountText;
    public Text GoldCountText;
    public Text AverageMoraleText;

    public RectTransform PlayerInfoShipList;
    public RectTransform PlayerInfoIncomeList;
    public RectTransform PlayerInfoMoraleList;

	void Start()
    {

	}
	
	void Update()
    {

	}

    public void SetOwningPlayer(PlayerScript player)
    {
        OwningPlayer = player;
    }

    /// <summary>
    /// Change the displayed captain name in player info
    /// </summary>
    /// <param name="new_name">New name to display</param>
    public void UpdateCaptainName(string new_name)
    {
        //The actual name of the player won't be changed here
        //This is meant to be called whenever the player's name changes
        //Which shouldn't ever happen, but we should have it just in case
        //So just update the text of the relevant UI (CaptainNameText) here.
    }

    public void UpdateShipCount()
    {

    }

    /// <summary>
    /// Change displayed gold count in player info
    /// </summary>
    public void UpdateGoldCount()
    {
        //In here, just sum up the total gold on all of the player's ships
        //Then just change the relevant UI (GoldCountText)
    }

    public void UpdateAverageMoraleLevel()
    {
        //As with the previous function, just get the info from all of the ships
        //And then take the average of the morale
        //Update relevant UI (AverageMoraleText)
    }

    /// <summary>
    /// Adds a new ship to the player info ship list
    /// </summary>
    /// <param name="ship">Ship to be added</param>
    public void AddShipToList(Ship ship)
    {
        //Instantiate a new shipstatblock
        //Call PopulateStatBlock on it, passing the ship passed to this function
        //Set the parent of the statblock to PlayerInfoShipList, like this
        //new_stat_block.transform.SetParent(PlayerInfoShipList, false);
        //The false there is to make sure it doesn't keep its default position when you set the parent
        //If you didn't have that, the stat block would spawn in the middle of the screen and just have an offset relevant to the parent
    }

    /// <summary>
    /// Removes ship block from player info ship list
    /// </summary>
    /// <param name="ship">Ship to be removed</param>
    public void RemoveShipFromList(Ship ship)
    {
        //Search through all of the stat blocks that are currently children of PlayerInfoShipList
        //If stat_block.ReferenceShip is equal the ship parameter, delete the stat block

        for(int i = 0; i < PlayerInfoShipList.childCount; i++)
        {
            ShipStatBlock stat_block = PlayerInfoShipList.GetChild(i).GetComponent<ShipStatBlock>();
            if (stat_block && stat_block.ReferenceShip == ship)
                Destroy(stat_block.gameObject);
        }
    }

    public void PopulateIncomeList()
    {
        //Don't worry about this one, the relevant code doesn't exist yet
    }

    public void PopulateMoraleList()
    {
        //Same here
    }
}
