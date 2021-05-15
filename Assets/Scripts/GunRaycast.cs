using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class GunRaycast : NetworkBehaviour
{
    [SerializeField] private TrailRenderer bulletTrail;

    [SerializeField] private Transform muzzle;

    [SerializeField] private float shootingDistance = 200f;
    [SerializeField] private int damage = 200;

    // these run on the server called by a client; client => server
    [ServerRpc]
    public void ShootServerRpc()
    {
        Debug.Log("yes");
        // do raycast on the server to see if we hit an enemy and take damage
        if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, shootingDistance))
        {
            // we hit something - is it a player?
            var enemyHealth = hit.transform.GetComponent<PlayerHealth>();
            if (enemyHealth)
            {
                // it was a player, then damage them
                enemyHealth.TakeDamage(damage);
            }
        }
        ShootClientRpc();
    }

    // server => client
    [ClientRpc]
    void ShootClientRpc()
    {
        var bullet = Instantiate(bulletTrail, muzzle.position, Quaternion.identity);
        bullet.AddPosition(muzzle.position);

        if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, shootingDistance))
        {
            bullet.transform.position = hit.point;
        }
        else
        {
            bullet.transform.position = muzzle.position + (muzzle.forward * shootingDistance);
        }
    }
}
