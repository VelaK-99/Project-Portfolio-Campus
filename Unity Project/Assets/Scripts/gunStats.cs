using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public GameObject model;
    public Sprite Image;

    [Range(1, 15)] public int shootDmg;
    [Range(5, 1000)] public int shootDist;
    [Range(0.1f, 3)] public float shootRate;
    [Range(0.5f, 5)] public float reloadSpeed;
    [Range(5, 30)] public int ammoCapacity; //Gun's mag capacity
    public int currentAmmo; //Ammo in mag
    public int totalAmmo; //Mags/Ammo left
    public int maxAmmo; //Max Mags/Ammo you can have
    public string GunName;
    public bool isShotgun;
    public bool isSciFi;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSounds;
    [Range(0,1)]public float shootSoundVol;
}
