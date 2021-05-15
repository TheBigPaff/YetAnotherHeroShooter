using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;
using MLAPI.Prototyping;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller = null;
    public Transform rightGunBone;
    public Transform leftGunBone;
    public RectTransform healthBar;


    PlayerHealth playerHealth;

    Joystick movementJoystick;
    Joystick aimJoystick;
    //Button fireButton;
    WeaponsManager weaponsManager;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;

    private SceneTransitionHandler.SceneStates m_CurrentSceneState;
    private bool m_HasGameStarted;
    public bool IsAlive = true;

    GunRaycast rightGun;
    GunRaycast leftGun;

    Gun equippedGun;

    private ClientRpcParams m_OwnerRPCParams;


    [SerializeField] private TrailRenderer bulletTrail;

    private Transform muzzle;
    [SerializeField] private float shootingDistance = 200f;
    [SerializeField] private int damage = 10;


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

    public void SetArsenal(Arsenal arsenal)
    {
        if (rightGunBone.childCount > 0)
            Destroy(rightGunBone.GetChild(0).gameObject);
        if (leftGunBone.childCount > 0)
            Destroy(leftGunBone.GetChild(0).gameObject);
        if (arsenal.rightGun != null)
        {
            GameObject newRightGun = (GameObject)Instantiate(arsenal.rightGun);
            newRightGun.transform.parent = rightGunBone;
            newRightGun.transform.localPosition = Vector3.zero;
            newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);

            rightGun = newRightGun.GetComponent<GunRaycast>();
        }
        else rightGun = null;
        if (arsenal.leftGun != null)
        {
            GameObject newLeftGun = (GameObject)Instantiate(arsenal.leftGun);
            newLeftGun.transform.parent = leftGunBone;
            newLeftGun.transform.localPosition = Vector3.zero;
            newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);

            leftGun = newLeftGun.GetComponent<GunRaycast>();
        }
        else leftGun = null;

        // TODO MAKE ANIMATIONS WORK!!!!
        //animator.runtimeAnimatorController = arsenal.controller; 

        return;
    }

    private void SceneTransitionHandler_clientLoadedScene(ulong clientId)
    {
        SceneStateChangedClientRpc(m_CurrentSceneState);
    }

    [ClientRpc]
    private void SceneStateChangedClientRpc(SceneTransitionHandler.SceneStates state)
    {
        if (!IsServer) SceneTransitionHandler.sceneTransitionHandler.SetSceneState(state);


        // GET REFERENCES 
        movementJoystick = GameObject.Find("MovementJoystick").GetComponent<Joystick>();
        aimJoystick = GameObject.Find("AimJoystick").GetComponent<Joystick>();
        //fireButton = GameObject.Find("FireButton").GetComponent<Button>();
        //fireButton.onClick.AddListener(Shoot);

        healthBar = GameObject.Find("Bar").GetComponent<RectTransform>();
        playerHealth = gameObject.GetComponent<PlayerHealth>();

        // camera shit
        if (IsLocalPlayer)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>().enabled = true;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>().player = gameObject;
        }

        //// equip AK74 - default
        //Arsenal gunToEquip = weaponsManager.GetWeapon(DefaultArsenal.AK74);
        //SetArsenal(gunToEquip);

        weaponsManager = GameObject.FindGameObjectWithTag("WeaponsManager").GetComponent<WeaponsManager>();
        equippedGun = weaponsManager.guns[0]; // default
        EquipGun();

        ////get muzzle from gun
        //muzzle = rightGunBone.GetChild(0).Find("Muzzle");
    }

    private void EquipGun()
    {
        // always right handed

        if (rightGunBone.childCount > 0)
            Destroy(rightGunBone.GetChild(0).gameObject);
        if (equippedGun != null)
        {
            GameObject newGun = Instantiate(equippedGun.gameObject);
            newGun.transform.parent = rightGunBone;
            newGun.transform.localPosition = Vector3.zero;
            newGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
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
        if (!IsAlive) return;

        Vector3 movementInput = new Vector3(movementJoystick.Horizontal, 0, movementJoystick.Vertical);
        controller.Move(movementInput * movementSpeed * Time.deltaTime);
        UpdateAnimator();


        if (Mathf.Abs(aimJoystick.Horizontal) > 0.01 || Mathf.Abs(aimJoystick.Vertical) > 0.01)
        {
            Vector3 aimInput = new Vector3(aimJoystick.Horizontal, 0, aimJoystick.Vertical);
            transform.rotation = Quaternion.LookRotation(aimInput);
        }

        if (Mathf.Abs(aimJoystick.Horizontal) > 0.7 || Mathf.Abs(aimJoystick.Vertical) > 0.7)
        {
            //Shoot();
            equippedGun.OnTriggerHold();
        }
        else
        {
            equippedGun.OnTriggerRelease();
        }

        // update UI
        if (IsLocalPlayer)
        {
            float healthPercent;
            healthPercent = (float)playerHealth.health.Value / playerHealth.startingHealth;
            healthBar.localScale = new Vector3(healthPercent, 1, 1);
        }
    }

    void Shoot()
    {
        if (!IsAlive) return;

        if (rightGunBone != null)
        {
            muzzle = rightGunBone.GetChild(0).Find("Muzzle");
            ShootServerRpc(muzzle.position, muzzle.forward);
        }
    }

    // these run on the server called by a client; client => server
    [ServerRpc]
    public void ShootServerRpc(Vector3 muzzlePosition, Vector3 muzzleForward)
    {
        // do raycast on the server to see if we hit an enemy and take damage
        if (Physics.Raycast(muzzlePosition, muzzleForward, out RaycastHit hit, shootingDistance))
        {
            // we hit something - is it a player?
            var enemyHealth = hit.transform.GetComponent<PlayerHealth>();
            if (enemyHealth)
            {
                // it was a player, then damage them
                enemyHealth.TakeDamage(damage);
            }
        }
        ShootClientRpc(muzzlePosition, muzzleForward);
    }

    // server => client
    [ClientRpc]
    void ShootClientRpc(Vector3 muzzlePosition, Vector3 muzzleForward)
    {
        var bullet = Instantiate(bulletTrail, muzzlePosition, Quaternion.identity);
        bullet.AddPosition(muzzlePosition);

        if (Physics.Raycast(muzzlePosition, muzzleForward, out RaycastHit hit, shootingDistance))
        {
            bullet.transform.position = hit.point;
        }
        else
        {
            bullet.transform.position = muzzlePosition + (muzzleForward * shootingDistance);
        }
    }
    void UpdateAnimator()
    {
        Vector3 velocity = controller.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;

        GetComponent<NetworkAnimator>().Animator.SetFloat("forwardSpeed", speed);
    }

    [ClientRpc]
    public void NotifyDeathClientRpc(ClientRpcParams clientParams)
    {
        m_HasGameStarted = false;
        PVPGame.Singleton.DisplayGameOverText("You Are Dead!");
    }
}
