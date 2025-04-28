using UnityEngine;

//Allows you to access this script via a right-click
[CreateAssetMenu]

public class Weapons : ScriptableObject
{
    [Header("***WEAPON-STATS***")]

    
    public GameObject gunMODEL;

    public ParticleSystem hitEFFECT;
    public AudioClip[] shootSOUND;

    public string weaponName;
    [Range(1, 100)] public int damage;
    [Range(5, 5000)] public int range;
    [Range(1, 100)] public float fireRate;
    [Range(0.1f, 60)] public float reloadTime;

    /// <summary>
    /// is this weapon a shotgun?
    /// </summary>
    public bool isSHOTGUN;
    /// <summary>
    /// is this a melee weapon?
    /// </summary>
    public bool isMELEE;

    [Range(1, 70)] int meleeDamage;
    [Range(0.8f, 8)] float meleeRate;
    [Range(0.1f, 3)] float meleeDist;

    public int mag_Capacity; // Maximum ammo per magazine;
    public int mag_curAmmo;  // Current ammo in magazine;

    public int currentAmmo;    // Current reserve ammo;
    public int maxAmmo;      // Maximum reserve ammo;
}



    /*
    public Weapons(string name, int dmg, int rng, float rate, float reload, int capacity, int maxReserve)
    {
        weaponName = name;
        damage = dmg;
        range = rng;
        fireRate = rate;
        reloadTime = reload;
        mag_Capacity = capacity;
        mag_curAmmo = capacity;
        totalAmmo = maxReserve / 2;
        maxAmmo = maxReserve;
    }
}*/
