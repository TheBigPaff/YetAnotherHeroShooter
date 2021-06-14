using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode;



    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 350f;
    public float muzzleVelocity = 35f;
    public int shotsInABurst;
    public int ammoPerMag;
    public float reloadTime = 1f;

    [Header("Effects")]
    public MuzzleFlash muzzleFlash;
    public Transform shell;
    public Transform shellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    public float moveForwardIfWeird = 0;

    private float timeForNextShot = 0f;

    private bool triggerReleasedSinceLastShot = true;
    private int shotsRemainingToShootInThisCurrentBurst_OMGIloveTheseVariablesNames;
    int ammoRemainingInMag;
    bool isReloading;

    [Header("Recoil")]
    public Vector2 kickBackMinMax = new Vector2(.1f, .5f);
    public Vector2 recoilAngleMinMax = new Vector2(10, 20);
    public float recoilSettleTime = .1f;
    public float recoilAngleSettleTime = .1f;

    float recoilAngle;
    //things for refs
    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;

    private void Start()
    {
        shotsRemainingToShootInThisCurrentBurst_OMGIloveTheseVariablesNames = shotsInABurst;
        ammoRemainingInMag = ammoPerMag;
    }

    private void LateUpdate()
    {
        //animate recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilSettleTime);
        if (!isReloading)
        {
            recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilAngleSettleTime);
            transform.localEulerAngles = Vector3.forward * recoilAngle;
        }

        if (ammoRemainingInMag < 1 && !isReloading)
        {
            Reload();
        }
    }
    void Shoot()
    {
        if (!isReloading && Time.time > timeForNextShot && ammoRemainingInMag > 0)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingToShootInThisCurrentBurst_OMGIloveTheseVariablesNames == 0)
                    return;

                shotsRemainingToShootInThisCurrentBurst_OMGIloveTheseVariablesNames--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                    return;
            }
            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                if (ammoRemainingInMag == 0)
                {
                    break;
                }
                ammoRemainingInMag--;
                timeForNextShot = Time.time + (msBetweenShots / 1000);
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            //AudioManager.instance.PlaySound(shootAudio, transform.position);

            transform.localPosition -= Vector3.right * Random.Range(kickBackMinMax.x, kickBackMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
        }
    }

    public void Reload()
    {
        if (!isReloading && ammoRemainingInMag != ammoPerMag)
        {
            StartCoroutine(AnimateReload());
            //AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }
    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / reloadTime;
        float percentage = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percentage < 1)
        {
            percentage += Time.deltaTime * reloadSpeed;
            float interpolation = ((-percentage * percentage) + percentage) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.forward * reloadAngle;

            yield return null;
        }

        isReloading = false;
        ammoRemainingInMag = ammoPerMag;
    }

    public void Aim(Vector3 aimpoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimpoint);
            transform.Rotate(0, -90, 0);
        }
    }
    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }
    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingToShootInThisCurrentBurst_OMGIloveTheseVariablesNames = shotsInABurst;
    }
}
