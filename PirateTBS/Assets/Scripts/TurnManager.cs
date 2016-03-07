using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TurnManager : NetworkBehaviour
{
    [HideInInspector]
    public static TurnManager Instance;

    public Text CurrentTurnText;
    public Text ActionButtonText;

    [SyncVar]
    public int CurrentTurn;

    public override void OnStartClient()
    {
        base.OnStartClient();

        Instance = this;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 270.0f);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        CurrentTurn = 1;
    }

    void Update()
    {
        	
	}

    [Command]
    public void CmdCheckReadyForNextTurn()
    {
        foreach(GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            PlayerScript player = go.GetComponent<PlayerScript>();
            if (player && !player.ReadyForNextTurn)
                return;
        }

        foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            PlayerScript player = go.GetComponent<PlayerScript>();
            if (player)
            {
                foreach (Fleet f in player.Fleets)
                    f.MoveActionTaken = f.CombatActionTaken = false;
            }
        }

        CurrentTurn++;
        RpcNextTurn(CurrentTurn);
    }

    [ClientRpc]
    void RpcNextTurn(int current_turn)
    {
        CurrentTurnText.text = string.Format("Turn {0}", current_turn);
        PlayerScript.MyPlayer.CmdNotReadyForNextTurn();
    }

    [Client]
    public void EndTurn()
    {
        foreach(Fleet f in PlayerScript.MyPlayer.Fleets)
        {
            if(!f.MoveActionTaken)
            {
                Camera.main.GetComponent<PanCamera>().CenterOnTarget(f.transform);
                StartCoroutine(UnitWaitingForCommand());
                return;
            }
        }

        StopAllCoroutines();
        ActionButtonText.text = "WAITING...";
        PlayerScript.MyPlayer.CmdReadyForNextTurn();
    }

    [Client]
    IEnumerator UnitWaitingForCommand()
    {
        ActionButtonText.text = "UNIT NEEDS ORDERS";
        yield return new WaitForSeconds(1.0f);
        ActionButtonText.text = "END TURN";
    }
}