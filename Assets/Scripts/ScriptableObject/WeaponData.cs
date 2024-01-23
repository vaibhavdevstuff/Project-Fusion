using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "WA_NewWeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    [ReadOnly][SerializeField] private WeaponData weaponData;

    [Space]
    public E_WeaponFireMode FireMode;
    //public E_WeaponFireModel FireModel;

    [Space]
    public GameObject ProjectilePrefab;
    public GameObject MuzzelFlashPrefab;
    
    [Space]
    public int Damage;
    public float Range;
    public float RateOfFire;
    public int MagazineSize;

    private float ReloadTime = 0f;


    private void OnValidate()
    {
        if(weaponData == null || weaponData != this)
            weaponData = this;
    }

}
