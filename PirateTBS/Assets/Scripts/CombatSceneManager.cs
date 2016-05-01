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
    public GameObject HealthBarPrefab;

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
    public void CmdSpawnShip()
    {
        List<WaterHex> AllHex = CombatHexGrid.Instance.WaterTiles;
        foreach (Ship s in CombatManager.Instance.PlayerFleet.Ships)
        {
            HexTile SpawnHex = AllHex[Random.Range(0, AllHex.Count)];
            Ship clone_ship = Instantiate(ShipPrefab).GetComponent<Ship>();
            clone_ship.Name = s.Name;
            clone_ship.SetClass(s.Class);
            clone_ship.Cargo = s.Cargo;

            clone_ship.transform.SetParent(SpawnHex.transform, false);
            clone_ship.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
            clone_ship.CurrentPosition = SpawnHex;
            GameObject Health = Instantiate(HealthBarPrefab);
            Health.transform.SetParent(clone_ship.transform, false);
            Health.GetComponent<HealthBars>().SetRefship(clone_ship);
            NetworkServer.SpawnWithClientAuthority(clone_ship.gameObject, PlayerScript.MyPlayer.gameObject);
        }
        foreach (Ship s in CombatManager.Instance.EnemyFleet.Ships)
        {
            HexTile SpawnHex = AllHex[Random.Range(0, AllHex.Count)];
            Ship clone_ship = Instantiate(ShipPrefab).GetComponent<Ship>();
            clone_ship.Name = s.Name;
            clone_ship.SetClass(s.Class);
            clone_ship.Cargo = s.Cargo;

            clone_ship.transform.SetParent(SpawnHex.transform, false);
            clone_ship.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
            clone_ship.CurrentPosition = SpawnHex;
            GameObject Health = Instantiate(HealthBarPrefab);
            Health.transform.SetParent(clone_ship.transform, false);
            Health.GetComponent<HealthBars>().SetRefship(clone_ship);
            //NetworkServer.SpawnWithClientAuthority(clone_ship.gameObject, PlayerScript.MyPlayer.gameObject);
        }
    }
}
