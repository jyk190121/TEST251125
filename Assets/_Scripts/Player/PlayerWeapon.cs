using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerWeapon : MonoBehaviour
{
    public List<GameObject> weaponlist;
    Item weapon;

    private void OnEnable()
    {
        DataManager.OnEquipmentChanged += WeaponPrefab;
    }

    void WeaponPrefab()
    {
        weapon = _MasterManager.Instance.DataManager.GetWeapon();
        if (weapon == null) return;
        foreach (var weaponoff in weaponlist)
        {
            weaponoff.SetActive(false);
        }
        if(weapon.itemID == 2001)
        {
            weaponlist[0].SetActive(true);
            weaponlist[4].SetActive(true);
        }
        else if(weapon.itemID == 2002)
        {
            weaponlist[1].SetActive(true);
            weaponlist[5].SetActive(true);
        }
        else if(weapon.itemID == 2003)
        {
            weaponlist[6].SetActive(true);
        }
        else if(weapon.itemID == 2004)
        {
            weaponlist[7].SetActive(true);
        }
        else if(weapon.itemID == 2005)
        {
            weaponlist[2].SetActive(true);
        }
        else if(weapon.itemID == 2006)
        {
            weaponlist[3].SetActive(true);
        }


    }

}
