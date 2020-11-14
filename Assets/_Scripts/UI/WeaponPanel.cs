using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanel : MonoBehaviour
{
    private Dictionary<int, WeaponInventoryItem> weaponPanels;
    
    int amountOfWeapons = 3;
    int currentWeaponID = 1;
    void Awake()
    {
        weaponPanels = new Dictionary<int, WeaponInventoryItem>();
        
        for(int i = 0; i < transform.childCount; i++)
        {
            var weaponPanelObject = gameObject.transform.GetChild(i).gameObject;
            var weaponInventoryItem = weaponPanelObject.GetComponent<WeaponInventoryItem>();

            weaponPanels.Add(weaponInventoryItem.weaponID, weaponInventoryItem);
        }
    }

    public void switchWeaponHighlight(int weaponID)
    {
        // Disable outline
        weaponPanels[currentWeaponID].transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = false;
        currentWeaponID = weaponID;
        // Get the image child and set its outline to true
        weaponPanels[weaponID].transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = true;
        
    }
}
