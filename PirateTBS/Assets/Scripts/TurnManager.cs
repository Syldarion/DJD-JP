using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TurnManager : NetworkBehaviour
{
    [HideInInspector]
    public static TurnManager Instance;

    public Text CurrentTurnText;            //Text reference to display current turn number
    public Text ActionButtonText;           //Text reference to show message on turn button

    [SyncVar]
    public int CurrentTurn;                 //Turn game is on

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

    /// <summary>
    /// Checks all players and continues to next turn if they are all ready
    /// </summary>
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

    /// <summary>
    /// Client-side function to move on to next turn
    /// </summary>
    /// <param name="current_turn"></param>
    [ClientRpc]
    void RpcNextTurn(int current_turn)
    {
        CurrentTurnText.text = string.Format("Turn {0}", current_turn);
        PlayerScript.MyPlayer.CmdNotReadyForNextTurn();
    }

    /// <summary>
    /// Client-side function to end turn
    /// </summary>
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

    /// <summary>
    /// Client-side callback if unit needs a command
    /// </summary>
    /// <returns></returns>
    [Client]
    IEnumerator UnitWaitingForCommand()
    {
        ActionButtonText.text = "UNIT NEEDS ORDERS";
        yield return new WaitForSeconds(1.0f);
        ActionButtonText.text = "END TURN";
    }
}