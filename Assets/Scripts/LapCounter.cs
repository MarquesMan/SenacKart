using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class LapCounter : NetworkBehaviour
{


    SyncDictionary<uint, PlayerNameAndLap> lapDict = new SyncDictionary<uint, PlayerNameAndLap>();

    Dictionary<uint, int> checkPointDict = new Dictionary<uint, int>();

    [SerializeField]
    List<CheckPoint> checkPoints;

    [SerializeField]
    UnityEngine.UI.Text scoreText;

    public override void OnStartClient()
    {
        lapDict.Callback += UpdateDict;
    }

    private void UpdateDict(SyncIDictionary<uint, PlayerNameAndLap>.Operation op, uint key, PlayerNameAndLap item)
    {
        // Debug.Log($"Player :{item.playerName} Volta:{item.currentLap}");

        System.Text.StringBuilder scoreTextString = new System.Text.StringBuilder("");

        //foreach (uint dictKey in lapDict.Keys) Debug.Log(dictKey);

        foreach(PlayerNameAndLap values in lapDict.Values)
        {
            // "" + "Player: X Volta: N\n"
            scoreTextString.AppendLine($"{values.playerName} : {values.currentLap}");
        }

        scoreText.text = scoreTextString.ToString();
    }

    public struct PlayerNameAndLap
    {
        public string playerName;
        public int currentLap;
    }

    [Server]
    public void PlayerPassedCheckPoint(NetworkIdentity id, CheckPoint check)
    {
        // Recuperar checkpoint atual do player
        int currentCheckPoint;
        uint networkID = id.netId;

        try
        {
            currentCheckPoint = checkPointDict[networkID];
        }
        catch (KeyNotFoundException )
        {
            // Insira o player
            InsertPlayer(id);
            currentCheckPoint = checkPointDict[networkID];
        }

        // Verifica se o checkpoint eh o atual
        if (!checkPoints[currentCheckPoint].Equals(check)) return;

        // Verificar se o checkpoint eh o checkpoint 0
  
        var tempPlayerNameAndLap = lapDict[networkID];
        tempPlayerNameAndLap.currentLap += 1;//currentCheckPoint == 0? 1 : 0;
        lapDict[networkID] = tempPlayerNameAndLap;
        

        // Se nao, passar para o proximo checkpoint
        var nextCheckPoint = (currentCheckPoint + 1) % checkPoints.Count;
        checkPointDict[networkID] = nextCheckPoint;
        // Debug.LogWarning($"{nextCheckPoint}:{lapDict[id].currentLap}");
    }

    private void InsertPlayer(NetworkIdentity id)
    {

        lapDict.Add(id.netId, new PlayerNameAndLap
        {
            playerName = id.gameObject.GetComponent<PlayerKart>().GetPlayerName(),
            currentLap = -1
        });
        checkPointDict[id.netId] = 0;

    }
}
