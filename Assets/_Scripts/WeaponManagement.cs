using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManagement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Weapon activeWeapon;
    [SerializeField]
    Camera firstPersonCamera;
    [SerializeField]
    GameObject character;

    IEnumerator shootAutomatic;
    GameManagement gameManagement;

    public Weapon ActiveWeapon { get { return activeWeapon; } set { activeWeapon = value; } }

    private void Awake()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
    }
    void Update()
    {
        if(Time.timeScale == 0)
        {
            // If game is paused dont do anything
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(activeWeapon.CurrentAmmo > 0)
            {
                // Run the shoot multiple script if its an automatic weapon
                if (activeWeapon is AutomaticWeapon automaticWeapon)
                {
                    shootAutomatic = automaticWeapon.ShootAutomatic(firstPersonCamera, character, GetComponent<AudioSource>());
                    StartCoroutine(shootAutomatic);
                }
                else
                {
                    activeWeapon.ShootSingle(firstPersonCamera, character, GetComponent<AudioSource>());
                }
            }
        }
        // Switching weapons

        // Using scroll wheel
        if (Input.mouseScrollDelta.y > 0f) // forward
        {
            switchWeapon(true);
        }
        else if (Input.mouseScrollDelta.y < 0f) // backwards
        {
            switchWeapon(false);
        }
        // Using numerical keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            switchWeaponByIndex(1); 
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            switchWeaponByIndex(2); 
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            switchWeaponByIndex(3); 
        }

        rotateBasedOnMovement();
    }

    private void switchWeapon(bool changingUp)
    {
        gameManagement.ChangeWeaponScrolling(changingUp);
        // Deactivate old weapon
        activeWeapon.gameObject.SetActive(false);
        activeWeapon = gameManagement.WeaponsInventory[gameManagement.ActiveWeaponID];
        // Activate new weapon
        activeWeapon.gameObject.SetActive(true);
        activeWeapon.setAmmoText();
    }
    private void switchWeaponByIndex(int weaponIndex)
    {
        bool result = gameManagement.ChangeWeaponByIndex(weaponIndex);

        if(result)
        {
            activeWeapon.gameObject.SetActive(false);
            activeWeapon = gameManagement.WeaponsInventory[gameManagement.ActiveWeaponID];

            activeWeapon.gameObject.SetActive(true);
        }
 
    }

    private void rotateBasedOnMovement()
    {
        var characterVelocity = character.GetComponent<Rigidbody>().velocity;
        var characterRight = character.transform.right;

        var sidewaysVelocity = Vector3.Dot(characterRight, characterVelocity);
        gameObject.transform.Rotate(new Vector3(0, -0.6f * sidewaysVelocity, 0));
    }
}
