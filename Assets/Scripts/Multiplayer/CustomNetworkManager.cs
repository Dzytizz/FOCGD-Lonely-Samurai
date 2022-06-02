using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private Transform player1Spawn;
    [SerializeField] private Transform player2Spawn;
    [SerializeField] private MultiplayerGameManager gameManager;
    public List<MultiplayerPlayer> playerList = new List<MultiplayerPlayer>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform start = numPlayers == 0 ? player1Spawn : player2Spawn;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
        playerList.Add(player.GetComponent<MultiplayerPlayer>());
        if (numPlayers == 2)
        {
            gameManager.StartGame();
        }
    }
}
