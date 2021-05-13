using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;
using MLAPI.Prototyping;

public class PlayerScript : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller = null;
    Joystick joystick;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;

    private SceneTransitionHandler.SceneStates m_CurrentSceneState;
    private bool m_HasGameStarted;
    bool m_IsAlive = true;


    private ClientRpcParams m_OwnerRPCParams;


    private void Awake()
    {
        m_HasGameStarted = false;
    }
    private void Start()
    {
        //m_PlayerVisual = GetComponent<SpriteRenderer>();
        //if (m_PlayerVisual != null) m_PlayerVisual.material.color = Color.black;
    }

    private void Update()
    {
        switch (m_CurrentSceneState)
        {
            case SceneTransitionHandler.SceneStates.Ingame:
                {
                    InGameUpdate();
                    break;
                }
        }
    }
    protected void OnDestroy()
    {
        if (IsClient)
        {
            //m_Lives.OnValueChanged -= OnLivesChanged;
            //m_Lives.OnValueChanged -= OnScoreChanged;
        }

        if (PVPGame.Singleton)
        {
            PVPGame.Singleton.isGameOver.OnValueChanged -= OnGameStartedChanged;
            PVPGame.Singleton.hasGameStarted.OnValueChanged -= OnGameStartedChanged;
        }
    }

    private void SceneTransitionHandler_clientLoadedScene(ulong clientId)
    {
        SceneStateChangedClientRpc(m_CurrentSceneState);
    }

    [ClientRpc]
    private void SceneStateChangedClientRpc(SceneTransitionHandler.SceneStates state)
    {
        if (!IsServer) SceneTransitionHandler.sceneTransitionHandler.SetSceneState(state);
        joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
    }

    private void SceneTransitionHandler_sceneStateChanged(SceneTransitionHandler.SceneStates newState)
    {
        m_CurrentSceneState = newState;
        if (m_CurrentSceneState == SceneTransitionHandler.SceneStates.Ingame)
        {
            //if (m_PlayerVisual != null) m_PlayerVisual.material.color = Color.green;
        }
        else
        {
            //if (m_PlayerVisual != null) m_PlayerVisual.material.color = Color.black;
        }
    }
    public override void NetworkStart()
    {
        base.NetworkStart();

        // Bind to OnValueChanged to display in log the remaining lives of this player
        // And to update InvadersGame singleton client-side
        //m_Lives.OnValueChanged += OnLivesChanged;
        //m_Score.OnValueChanged += OnScoreChanged;

        if (IsServer) m_OwnerRPCParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { OwnerClientId } } };

        if (!PVPGame.Singleton)
            PVPGame.OnSingletonReady += SubscribeToDelegatesAndUpdateValues;
        else
            SubscribeToDelegatesAndUpdateValues();

        if (IsServer) SceneTransitionHandler.sceneTransitionHandler.OnClientLoadedScene += SceneTransitionHandler_clientLoadedScene;

        SceneTransitionHandler.sceneTransitionHandler.OnSceneStateChanged += SceneTransitionHandler_sceneStateChanged;
    }

    private void SubscribeToDelegatesAndUpdateValues()
    {
        PVPGame.Singleton.hasGameStarted.OnValueChanged += OnGameStartedChanged;
        PVPGame.Singleton.isGameOver.OnValueChanged += OnGameStartedChanged;

        if (IsClient && IsOwner)
        {
            //PVPGame.Singleton.SetScore(m_Score.Value);
            //PVPGame.Singleton.SetLives(m_Lives.Value);
        }
    }
    private void OnGameStartedChanged(bool previousValue, bool newValue)
    {
        m_HasGameStarted = newValue;
    }

    private void InGameUpdate()
    {
        
        if (!IsLocalPlayer || !IsOwner || !m_HasGameStarted) return;
        if (!m_IsAlive) return;


        //Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 input = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        controller.Move(input * movementSpeed * Time.deltaTime);


        if (controller.velocity.sqrMagnitude > 0.2f)
        {
            transform.rotation = Quaternion.LookRotation(input);
        }

        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        Vector3 velocity = controller.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;

        //GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        GetComponent<NetworkAnimator>().Animator.SetFloat("forwardSpeed", speed);
    }

    [ClientRpc]
    public void NotifyDeathClientRpc(ClientRpcParams clientParams)
    {
        m_HasGameStarted = false;
        PVPGame.Singleton.DisplayGameOverText("You Are Dead!");
    }
}
