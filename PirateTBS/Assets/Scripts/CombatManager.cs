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

    public RectTransform PlayerShipPanel;
    public RectTransform EnemyShipPanel;

    public Ship PlayerShip;
    public Ship EnemyShip;

    public ShotType SelectedShotType;
    public int ProjectedHullDamage;
    public int ProjectedSailDamage;

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
                ProjectedHullDamage = 2;
                ProjectedSailDamage = 2;
                break;
            case 1:
                SelectedShotType = ShotType.Cluster;
                ProjectedHullDamage = 3;
                ProjectedSailDamage = 1;
                break;
            case 2:
                SelectedShotType = ShotType.Chain;
                ProjectedHullDamage = 1;
                ProjectedSailDamage = 3;
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
            string.Format("Projected Hull Damage - {0}", PlayerShip.Cannons * ProjectedHullDamage * ((float)EnemyShip.DodgeChance / 100.0f));
        PlayerShipPanel.FindChild("ShipInfo/ProjectedSailDamage").GetComponent<Text>().text =
            string.Format("Projected Sail Damage - {0}", PlayerShip.Cannons * ProjectedSailDamage * ((float)EnemyShip.DodgeChance / 100.0f));
    }

    public void ConfirmCombat()
    {
        DialogueBox.CurrentDialogue.NewDialogue(string.Format("Do you want to enter combat with {0}?", EnemyShip.Name));
        DialogueBox.CurrentDialogue.AddOption("Yes", () => { ResolveCombat(); DialogueBox.CurrentDialogue.CloseDialogue(); });
        DialogueBox.CurrentDialogue.AddOption("No", () => { CloseCombatPanel(); DialogueBox.CurrentDialogue.CloseDialogue(); });
    }

    public void ResolveCombat()
    {
        
    }
}
