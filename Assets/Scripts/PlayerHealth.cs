using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class PlayerHealth : NetworkBehaviour
{
    public int startingHealth = 100;
    // make a synchronizable variable to store health
    public NetworkVariableInt health = new NetworkVariableInt( new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly });


    //PlayerSpawner playerSpawner;

    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (health.Value <= 0 & IsLocalPlayer && gameObject.GetComponent<PlayerScript>().IsAlive)
        {
            // despawn
            gameObject.GetComponent<PlayerScript>().IsAlive = false;
        }
    }

    public void TakeDamage(int damage)
    {
        health.Value -= damage;
    }
}
