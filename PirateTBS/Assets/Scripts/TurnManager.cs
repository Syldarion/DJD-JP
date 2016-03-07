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

    [SyncVar]
    public int CurrentTurn;

    public override void OnStartClient()
    {
        base.OnStartClient();

        Instance = this;
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
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
                foreach (Fleet f in player.Fleets)
                    f.MoveActionTaken = f.CombatActionTaken = false;
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
        PlayerScript.MyPlayer.CmdReadyForNextTurn();
    }
}