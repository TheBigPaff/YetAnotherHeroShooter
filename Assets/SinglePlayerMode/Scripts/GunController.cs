using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinglePlayerMode
{
    public class GunController : MonoBehaviour
    {
        public Gun[] allGuns;
        public Transform weaponHolder;
        public Transform weaponHolderInside;
        public Gun[] weirdWeapons;
        private Gun equippedGun;

        float howMuchHasMovedForward;
        private void Start()
        {

        }
        Gun oldEquippedGun;
        public void EquipGun(Gun gunToEquip)
        {
            if (equippedGun != null)
            {
                Destroy(equippedGun.gameObject);

                weaponHolder.localPosition += new Vector3(0, 0, -howMuchHasMovedForward);
            }
            equippedGun = Instantiate(gunToEquip, weaponHolder.position, weaponHolder.rotation) as Gun;
            equippedGun.transform.parent = weaponHolder;

            //weird bug, move forward to fix it
            weaponHolder.localPosition += new Vector3(0, 0, gunToEquip.moveForwardIfWeird);
            howMuchHasMovedForward = gunToEquip.moveForwardIfWeird;
        }

        public void EquipGun(int gunToEquipIndex)
        {
            if (gunToEquipIndex < allGuns.Length)
                EquipGun(allGuns[gunToEquipIndex]);
            else
                EquipGun(allGuns.Length - 1);
        }
        public void OnTriggerHold()
        {
            if (equippedGun != null)
            {
                equippedGun.OnTriggerHold();
            }
        }
        public void OnTriggerRelease()
        {
            if (equippedGun != null)
            {
                equippedGun.OnTriggerRelease();
            }
        }

        public void Reload()
        {
            if (equippedGun != null)
            {
                equippedGun.Reload();
            }
        }

        public void Aim(Vector3 aimpoint)
        {
            if (equippedGun != null)
            {
                equippedGun.Aim(aimpoint);
            }
        }

        public float GunHeight
        {
            get
            {
                return weaponHolder.position.y;
            }
        }
    }
}
