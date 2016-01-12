using UnityEngine;
using UnityEngine.UI;
using BeardedManStudios.Network;

public class CustomLobbyPlayer : NetworkedMonoBehavior
{
    
    public string PlayerName = "";
    public int NationalityIndex = 0;
    public bool IsReady = false;

    ColorBlock NotReadyColorBlock;
    ColorBlock ReadyColorBlock;

    void Awake()
    {
    }

    public void UpdateName(string name)
    {
        RPC("OnNameChange", NetworkReceivers.AllBuffered, name);
    }

    [BRPC]
    void OnNameChange(string name)
    {
        Debug.Log(name);
        PlayerName = name;

        OwningNetWorker.Me.SetName(name);
    }
}
