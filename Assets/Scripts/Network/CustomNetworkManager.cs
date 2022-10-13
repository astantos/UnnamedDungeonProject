using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        Debug.Log("[ SERVER ] Server has been started");
    }
    
    public override void OnStopServer()
    {
        Debug.Log("[ SERVER ] Server has been stopped");
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"[ SERVER ] Client {conn.connectionId} has connected!");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log($"[ SERVER ] Client {conn.connectionId} has disconnected!");
    }
}
