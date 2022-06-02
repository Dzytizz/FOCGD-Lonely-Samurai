using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MultiplayerScoreUIManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerScore;
    [SerializeField] private TextMeshProUGUI otherPlayerScore;
    [SerializeField] private CustomNetworkManager networkManager;

    public void UpdateScore()
    {
        GetScores();
    }

    [Command(requiresAuthority = false)]
    public void GetScores()
    {
        SetScores(new int[] { networkManager.playerList[0].points, networkManager.playerList[1].points });
    }

    [ClientRpc]
    public void SetScores(int[] scores)
    {
        if (isServer)
        {
            playerScore.text = scores[0].ToString();
            otherPlayerScore.text = scores[1].ToString();
        }
        else
        {
            playerScore.text = scores[1].ToString();
            otherPlayerScore.text = scores[0].ToString();
        }
    }

}
