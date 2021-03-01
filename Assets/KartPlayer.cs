using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPlayer : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            GetComponent<KartGame.KartSystems.KeyboardInput>().enabled = false;
            GetComponent<KartGame.KartSystems.KartBounce>().enabled = false;
        }
        else
        {
            FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = this.transform;
            FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().LookAt = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
