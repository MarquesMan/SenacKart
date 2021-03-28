using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CheckPoint : MonoBehaviour
{
    private LapCounter lapCounter;
    private void Start()
    {
        lapCounter = FindObjectOfType<LapCounter>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player")) return;
        
        // Avisar o LapCounter que passamos um checkpoint
        lapCounter.PlayerPassedCheckPoint(
            other.GetComponentInParent<NetworkIdentity>(),
            this
        );
    }

}
