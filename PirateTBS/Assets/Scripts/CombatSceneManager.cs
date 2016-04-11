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

    public GameObject ShipPrefab;

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
    //[Command]
    //public void CmdSpawnFleet(string fleet_name, int x, int y)
    //{
    //    Fleet new_fleet = Instantiate(FleetPrefab).GetComponent<Fleet>();
    //    new_fleet.Name = fleet_name;

    //    HexTile new_tile = GameObject.Find(string.Format("Grid/{0},{1}", x, y)).GetComponent<HexTile>();

    //    Fleets.Add(new_fleet);

    //    NetworkServer.SpawnWithClientAuthority(new_fleet.gameObject, gameObject);

    //    new_fleet.CmdSpawnOnTile(new_tile.HexCoord.Q, new_tile.HexCoord.R);
    //}
    //public void CmdSpawnShip()
    //{
    //    string ship_name = NameGenerator.Instance.GetShipName();

    //    Ship new_ship = Instantiate(ShipPrefab).GetComponent<Ship>();
    //    new_ship.Name = ship_name;
    //    new_ship.SetClass((ShipClass)Random.Range(0, 8));
    //    new_ship.Cargo = new Cargo(50, 500);

    //    NetworkServer.SpawnWithClientAuthority(new_ship.gameObject, PlayerScript.MyPlayer.gameObject);

    //    CmdAddShip(new_ship.name);
    //}
}
