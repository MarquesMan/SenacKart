
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerKart : NetworkBehaviour
{
    // Start is called before the first frame update
    [SyncVar(hook =nameof(SetTextMeshValue))]
    private string playerName = "Missing";
    private KartGame.KartSystems.IInput[] oldInput;

    public string GetPlayerName() => playerName;

    private Cinemachine.CinemachineVirtualCamera localCamera;
    private TMPro.TextMeshPro playerNameText;

    private void SetTextMeshValue(string oldName, string newName)
    {
        playerNameText = GetComponentInChildren<TMPro.TextMeshPro>();
        playerNameText?.SetText(newName);
    }

    private void rotatePlayerName(float angle)
    {
        playerNameText?.rectTransform.Rotate(0, angle, 0);
    }

    [Command]
    private void setPlayerName(string newName)
    {
        playerName = newName;
    }

    void Start()
    {

        oldInput = GetComponent<KartGame.KartSystems.ArcadeKart>().m_Inputs;
        DisableInput(!isLocalPlayer);

        if (isLocalPlayer)
        {
            localCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            SetCameraProperties(transform, transform);
            setPlayerName(PlayerPrefs.GetString("playerName", "Missing"));
        }
        else
        {

            foreach (CapsuleCollider capsuleCollider in GetComponentsInChildren<CapsuleCollider>())
            {
                capsuleCollider.gameObject.layer = LayerMask.NameToLayer("OtherKart");
            }

        }
    }

    void SetCameraProperties(Transform lookAt = null, Transform follow = null , Transform positionTransform = null)
    {
        localCamera.LookAt = lookAt;
        localCamera.Follow = follow;
        if(positionTransform != null) localCamera.transform.position = positionTransform.position;
    }

    private void DisableInput(bool shouldDisable = true)
    {
        var kartComponent = GetComponent<KartGame.KartSystems.ArcadeKart>();

        if (kartComponent is null) return;

        if (shouldDisable)
            kartComponent.m_Inputs = new KartGame.KartSystems.IInput[] { };
        else if (kartComponent.m_Inputs.Length == 0)
            kartComponent.m_Inputs = oldInput;
    }

    [ClientRpc]
    public void SetWinState(Vector3 winPosition, Quaternion rotation) {

        DisableInput();

        if (isLocalPlayer)
        {
            var lapCounter = FindObjectOfType<LapCounter>();
            SetCameraProperties(lookAt: lapCounter.cameraWinLookAt, positionTransform: lapCounter.cameraWinPosition);
        }

        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = Vector3.zero;

        rotatePlayerName(180);
        transform.position = winPosition;
        transform.rotation = rotation;
    }
   
}
