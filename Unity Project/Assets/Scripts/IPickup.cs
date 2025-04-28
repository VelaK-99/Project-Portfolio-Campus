using UnityEngine;

public interface IPickup
{
    public void GetGunStats(gunStats gun);

    public void HealthPickup(int amount);

    public void AmmoPickup(int amount);
}
