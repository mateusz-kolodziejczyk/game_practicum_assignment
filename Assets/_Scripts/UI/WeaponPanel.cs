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
            weaponInventoryItem.gameObject.SetActive(false);
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
    // Method updates the panel to only show unlocked weapons
    public void updatePanels(List<int> ownedWeaponIDs)
    {
       foreach(int weaponID in ownedWeaponIDs)
        {
            // Only set the unlocked weapons to active
            weaponPanels[weaponID].gameObject.SetActive(true);
        }
    }
                
}
