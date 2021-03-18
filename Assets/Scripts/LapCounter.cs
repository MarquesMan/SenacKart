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

    int playerCount = 0,
        finishedPlayers = 0;

    Dictionary<uint, bool> finishedById = new Dictionary<uint, bool>();

    public override void OnStartClient()
    {
        lapDict.Callback += UpdateDict;
    }

    private class ScoreComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            var result = y - x;

            if (result == 0) // Quando os dois sao iguais
                return -1;
            else
                return result;
        }
    }


    private void UpdateDict(SyncIDictionary<uint, PlayerNameAndLap>.Operation op, uint key, PlayerNameAndLap item)
    {
        Debug.Log($"Player :{item.playerName} Volta:{item.score}");

        System.Text.StringBuilder scoreTextString = new System.Text.StringBuilder("");

        var scoreList = new SortedList<int, string>(new ScoreComparer());

        foreach (PlayerNameAndLap values in lapDict.Values)
        {
            scoreList.Add( values.score, values.playerName );
        }

        for(int i = 0; i < scoreList.Count; ++i)
        {
            scoreTextString.AppendLine($"{scoreList.Values[i]} : {i+1}º");
        }

        scoreText.text = scoreTextString.ToString();
    }

    public struct PlayerNameAndLap
    {
        public string playerName;
        public int score;
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
        tempPlayerNameAndLap.score += 1; //currentCheckPoint == 0? 1 : 0;
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
            score = 0
        });
        checkPointDict[id.netId] = 0;
        finishedById[id.netId] = false;
        playerCount += 1;

    }
}
