using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfoManager : MonoBehaviour
{
    [HideInInspector]
    public static PlayerInfoManager Instance;

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
        Instance = this;
        StartCoroutine(WaitForPlayer());
	}
	
	void Update()
    {

	}

    public void Initialize()
    {
        UpdateCaptainName(OwningPlayer.Name);
        UpdateShipCount();
        UpdateGoldCount();
        UpdateAverageMoraleLevel();
    }

    public void SetOwningPlayer(PlayerScript player)
    {
        OwningPlayer = player;

        Initialize();
    }

    /// <summary>
    /// Change the displayed captain name in player info
    /// </summary>
    /// <param name="new_name">New name to display</param>
    public void UpdateCaptainName(string new_name)
    {
        CaptainNameText.text = new_name;
    }

    public void UpdateShipCount()
    {
        OwningPlayer.TotalShips = 0;

        foreach (Fleet f in OwningPlayer.Fleets)
            OwningPlayer.TotalShips += f.Ships.Count;

        ShipCountText.text = OwningPlayer.TotalShips.ToString();
    }

    /// <summary>
    /// Change displayed gold count in player info
    /// </summary>
    public void UpdateGoldCount()
    {
        OwningPlayer.TotalGold = 0;

        foreach (Fleet f in OwningPlayer.Fleets)
            foreach (Ship s in f.Ships)
                OwningPlayer.TotalGold += s.Cargo.Gold;

        GoldCountText.text = OwningPlayer.TotalGold.ToString();
    }

    public void UpdateAverageMoraleLevel()
    {
        float average_morale = 0.0f;

        foreach(Fleet f in OwningPlayer.Fleets)
        {
            float fleet_morale = 0.0f;
            foreach (Ship s in f.Ships)
                fleet_morale += s.CrewMorale;
            average_morale += fleet_morale / f.Ships.Count;
        }

        AverageMoraleText.text = (average_morale / OwningPlayer.Fleets.Count).ToString();
    }

    /// <summary>
    /// Adds a new ship to the player info ship list
    /// </summary>
    /// <param name="ship">Ship to be added</param>
    public void AddShipToList(Ship ship)
    {
        ShipStatBlock new_block = Instantiate(StatBlockPrefab);
        new_block.PopulateStatBlock(ship);
        new_block.transform.SetParent(PlayerInfoShipList, false);

        ShipCountText.text = (++OwningPlayer.TotalShips).ToString();
    }

    /// <summary>
    /// Removes ship block from player info ship list
    /// </summary>
    /// <param name="ship">Ship to be removed</param>
    public void RemoveShipFromList(Ship ship)
    {
        for (int i = 0; i < PlayerInfoShipList.childCount; i++)
        {
            ShipStatBlock stat_block = PlayerInfoShipList.GetChild(i).GetComponent<ShipStatBlock>();
            if (stat_block && stat_block.ReferenceShip == ship)
            {
                Destroy(stat_block.gameObject);
                break;
            }
        }

        ShipCountText.text = (--OwningPlayer.TotalShips).ToString();
    }

    public void PopulateIncomeList()
    {

    }

    public void PopulateMoraleList()
    {

    }

    IEnumerator WaitForPlayer()
    {
        while (!PlayerScript.MyPlayer)
            yield return null;
        SetOwningPlayer(PlayerScript.MyPlayer);
    }
}
