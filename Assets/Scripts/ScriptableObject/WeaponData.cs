using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public E_WeaponFireType WeaponFireType;

    [Space]
    public GameObject ProjectilePrefab;
    public GameObject MuzzelFlashPrefab;
    
    [Space]
    public int damage;
    public float RateOfFire;
    public int MagazineSize;
    public float ReloadTime;
}
