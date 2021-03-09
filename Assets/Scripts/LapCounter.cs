using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class LapCounter : NetworkBehaviour
{


    SyncDictionary<NetworkIdentity, PlayerNameAndLap> lapDict = new SyncDictionary<NetworkIdentity, PlayerNameAndLap>();

    Dictionary<NetworkIdentity, int> checkPointDict = new Dictionary<NetworkIdentity, int>();

    [SerializeField]
    List<CheckPoint> checkPoints;

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
        try
        {
            currentCheckPoint = checkPointDict[id];
        }
        catch (KeyNotFoundException )
        {
            // Insira o player
            InsertPlayer(id);
            currentCheckPoint = checkPointDict[id];
        }

        // Verifica se o checkpoint eh o atual
        if (!checkPoints[currentCheckPoint].Equals(check)) return;

        // Verificar se o checkpoint eh o checkpoint 0
        if(currentCheckPoint == 0) // Se sim, incrementar o contador de voltas
        {
            var tempPlayerNameAndLap = lapDict[id];
            tempPlayerNameAndLap.currentLap += 1;
            lapDict[id] = tempPlayerNameAndLap;
        }

        // Se nao, passar para o proximo checkpoint
        var nextCheckPoint = (currentCheckPoint + 1) % checkPoints.Count;
        checkPointDict[id] = nextCheckPoint;
        Debug.LogWarning(lapDict[id].currentLap);
    }

    private void InsertPlayer(NetworkIdentity id)
    {

        lapDict[id] = new PlayerNameAndLap { 
            playerName =  id.gameObject.GetComponent<PlayerKart>().GetPlayerName(), 
            currentLap = 0
        };
        checkPointDict[id] = 0;

    }
}
