using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DefaultArsenal
{
    AK74,
    M4_8,
    M1911,
    Uzi
}

public enum DefaultWeapons
{
    AK74_2,
    M4_8,
    M1911,
    Uzi
}

[System.Serializable]
public class Arsenal
{
    public DefaultArsenal name;
    public GameObject rightGun;
    public GameObject leftGun;
    public RuntimeAnimatorController controller;
}