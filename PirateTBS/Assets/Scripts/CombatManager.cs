using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;


public class CombatManager : MonoBehaviour
{
    [HideInInspector]
    public static CombatManager Instance;

    public RectTransform PlayerShipPanel;
    public RectTransform EnemyShipPanel;

    public Ship PlayerShip;
    public Ship EnemyShip;

    public ShotType SelectedShotType;
    public int ShotHullDamage;
    public int ShotSailDamage;

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

    public void StartCombat(Ship player, Ship enemy)
    {
        PlayerShip = player;
        EnemyShip = enemy;

        UpdateShotType(0);

        UpdatePlayerPanel();
        UpdateEnemyPanel();
        ConfirmCombat();
    }

    public void UpdateShotType(int new_type)
    {
        //0 - normal, 1 - cluster, 2 - chain

        switch (new_type)
        {
            case 0:
            default:
                SelectedShotType = ShotType.Normal;
                ShotHullDamage = 2;
                ShotSailDamage = 2;
                break;
            case 1:
                SelectedShotType = ShotType.Cluster;
                ShotHullDamage = 3;
                ShotSailDamage = 1;
                break;
            case 2:
                SelectedShotType = ShotType.Chain;
                ShotHullDamage = 1;
                ShotSailDamage = 3;
                break;
        }

        UpdatePlayerDamage();
    }

    public void UpdatePlayerPanel()
    {
        PlayerShipPanel.FindChild("ShipName/NameText").GetComponent<Text>().text = PlayerShip.Name;

        PlayerShipPanel.FindChild("ShipHullHealth/HealthForeground").GetComponent<RectTransform>().localScale =
            new Vector3((float)PlayerShip.HullHealth / (float)PlayerShip.MaxHullHealth, 1.0f);
        PlayerShipPanel.FindChild("ShipHullHealth/HealthText").GetComponent<Text>().text = string.Format("{0} / {1}",
            PlayerShip.HullHealth, PlayerShip.MaxHullHealth);
        PlayerShipPanel.FindChild("ShipSailHealth/HealthForeground").GetComponent<RectTransform>().localScale =
            new Vector3((float)PlayerShip.SailHealth / (float)PlayerShip.MaxSailHealth, 1.0f);
        PlayerShipPanel.FindChild("ShipSailHealth/HealthText").GetComponent<Text>().text = string.Format("{0} / {1}",
            PlayerShip.SailHealth, PlayerShip.MaxSailHealth);

        UpdatePlayerDamage();
    }

    public void UpdateEnemyPanel()
    {
        EnemyShipPanel.FindChild("ShipName/NameText").GetComponent<Text>().text = EnemyShip.Name;

        EnemyShipPanel.FindChild("ShipHullHealth/HealthForeground").GetComponent<RectTransform>().localScale =
            new Vector3((float)EnemyShip.HullHealth / (float)EnemyShip.MaxHullHealth, 1.0f);
        EnemyShipPanel.FindChild("ShipHullHealth/HealthText").GetComponent<Text>().text = string.Format("{0} / {1}",
            EnemyShip.HullHealth, EnemyShip.MaxHullHealth);
        EnemyShipPanel.FindChild("ShipSailHealth/HealthForeground").GetComponent<RectTransform>().localScale =
            new Vector3((float)EnemyShip.SailHealth / (float)EnemyShip.MaxSailHealth, 1.0f);
        EnemyShipPanel.FindChild("ShipSailHealth/HealthText").GetComponent<Text>().text = string.Format("{0} / {1}",
            EnemyShip.SailHealth, EnemyShip.MaxSailHealth);

        EnemyShipPanel.FindChild("ShipInfo/ProjectedHullDamage").GetComponent<Text>().text =
            string.Format("{0} - Projected Hull Damage", EnemyShip.Cannons * 2 * ((float)PlayerShip.DodgeChance / 100.0f));
        EnemyShipPanel.FindChild("ShipInfo/ProjectedSailDamage").GetComponent<Text>().text =
            string.Format("{0} - Projected Sail Damage", EnemyShip.Cannons * 2 * ((float)PlayerShip.DodgeChance / 100.0f));
    }

    public void UpdatePlayerDamage()
    {
        PlayerShipPanel.FindChild("ShipInfo/ProjectedHullDamage").GetComponent<Text>().text =
            string.Format("Projected Hull Damage - {0}", PlayerShip.Cannons * ShotHullDamage * ((float)EnemyShip.DodgeChance / 100.0f));
        PlayerShipPanel.FindChild("ShipInfo/ProjectedSailDamage").GetComponent<Text>().text =
            string.Format("Projected Sail Damage - {0}", PlayerShip.Cannons * ShotSailDamage * ((float)EnemyShip.DodgeChance / 100.0f));
    }

    public void ConfirmCombat()
    {
        DialogueBox.CurrentDialogue.NewDialogue(string.Format("Do you want to enter combat with {0}?", EnemyShip.Name));
        DialogueBox.CurrentDialogue.AddOption("Yes", () => { ResolveCombat(); DialogueBox.CurrentDialogue.CloseDialogue(); });
        DialogueBox.CurrentDialogue.AddOption("No", () => { CloseCombatPanel(); DialogueBox.CurrentDialogue.CloseDialogue(); });
    }

    public void ResolveCombat()
    {
        CmdResolveCombat();
    }

    [Command]
    public void CmdResolveCombat()
    {
        int player_shots_remaining = PlayerShip.Cannons;
        int enemy_shots_remaining = EnemyShip.Cannons;
        int enemy_shot_sail_damage;
        int enemy_shot_hull_damage;

        int player_total_sail_damage = 0;
        int player_total_hull_damage = 0;
        int enemy_total_sail_damage = 0;
        int enemy_total_hull_damage = 0;

        ShotType enemy_shot_type = (ShotType)Random.Range(0, 3);
        switch(enemy_shot_type)
        {
            case ShotType.Chain:
                enemy_shot_sail_damage = 3;
                enemy_shot_hull_damage = 1;
                break;
            case ShotType.Cluster:
                enemy_shot_sail_damage = 1;
                enemy_shot_hull_damage = 3;
                break;
            case ShotType.Normal:
            default:
                enemy_shot_sail_damage = 2;
                enemy_shot_hull_damage = 2;
                break;
        }

        while(player_shots_remaining > 0)
        {
            if(Random.Range(0, 100) > EnemyShip.DodgeChance)
            {
                player_total_sail_damage += ShotSailDamage;
                player_total_hull_damage += ShotHullDamage;
            }
            player_shots_remaining--;
        }

        while(enemy_shots_remaining > 0)
        {
            if(Random.Range(0, 100) > PlayerShip.DodgeChance)
            {
                enemy_total_sail_damage += enemy_shot_sail_damage;
                enemy_total_hull_damage += enemy_shot_hull_damage;
            }
            enemy_shots_remaining--;
        }

        PlayerShip.HullHealth -= enemy_total_hull_damage;
        PlayerShip.SailHealth -= enemy_total_sail_damage;

        EnemyShip.HullHealth -= player_total_hull_damage;
        EnemyShip.SailHealth -= player_total_sail_damage;

        if (PlayerShip.HullHealth <= 0) PlayerShip.RpcSinkShip();
        else PlayerShip.Speed = (int)(PlayerShip.FullSpeed * (PlayerShip.SailHealth / 100.0f));
        if (EnemyShip.HullHealth <= 0) PlayerShip.RpcSinkShip();
        else EnemyShip.Speed = (int)(EnemyShip.FullSpeed * (EnemyShip.SailHealth / 100.0f));

        RpcResolveCombat();
    }

    [ClientRpc]
    public void RpcResolveCombat()
    {
        
    }
}
