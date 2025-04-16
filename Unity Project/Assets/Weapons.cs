using UnityEngine;

public class Weapons : MonoBehaviour
{
    public string weaponName;
    public int damage;
    public int range;
    public float fireRate;
    public float reloadTime;
    public int ammoCapacity; // Maximum ammo per magazine;
    public int currentAmmo;  // Current ammo in magazine;
    public int totalAmmo;    // Total reserve ammo;
    public int maxAmmo;      // Maximum reserve ammo;

    public Weapons(string name, int dmg, int rng, float rate, float reload, int capacity, int maxReserve)
    {
        weaponName = name;
        damage = dmg;
        range = rng;
        fireRate = rate;
        reloadTime = reload;
        ammoCapacity = capacity;
        currentAmmo = capacity;
        totalAmmo = maxReserve / 2;
        maxAmmo = maxReserve;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
