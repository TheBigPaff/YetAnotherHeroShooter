using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    public Arsenal[] arsenal;
    public Gun[] guns;

    public Arsenal GetWeapon(DefaultArsenal name)
    {
        foreach (Arsenal weapon in arsenal)
        {
            if (weapon.name == name) return weapon;
        }
        return null;
    }
}
