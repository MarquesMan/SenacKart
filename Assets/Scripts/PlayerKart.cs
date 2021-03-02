
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerKart : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().LookAt = transform;
            FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = this.transform;
        }
        else
        {
            GetComponent<KartGame.KartSystems.ArcadeKart>().m_Inputs = new KartGame.KartSystems.IInput[] { };

            foreach(CapsuleCollider capsuleCollider in GetComponentsInChildren<CapsuleCollider>())
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
