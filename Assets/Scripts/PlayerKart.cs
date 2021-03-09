
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerKart : NetworkBehaviour
{
    // Start is called before the first frame update
    [SyncVar(hook =nameof(SetTextMeshValue))]
    private string playerName = "Missing";

    public string GetPlayerName() => playerName;

    private void SetTextMeshValue(string oldName, string newName)
    {
        GetComponentInChildren<TMPro.TextMeshPro>().SetText(newName);
    }

    [Command]
    private void setPlayerName(string newName)
    {
        playerName = newName;
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().LookAt = transform;
            FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = this.transform;
            setPlayerName(PlayerPrefs.GetString("playerName", "Missing"));
        }
        else
        {
            try
            {
                GetComponent<KartGame.KartSystems.ArcadeKart>().m_Inputs = new KartGame.KartSystems.IInput[] { };
            }catch (System.NullReferenceException _)
            {
                //pass
            }

            foreach (CapsuleCollider capsuleCollider in GetComponentsInChildren<CapsuleCollider>())
            {
                capsuleCollider.gameObject.layer = LayerMask.NameToLayer("OtherKart");
            }

        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
