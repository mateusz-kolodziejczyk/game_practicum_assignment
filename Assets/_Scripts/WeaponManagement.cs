using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        

        rotateBasedOnMovement();
    }

    private void rotateBasedOnMovement()
    {
        var characterVelocity = character.GetComponent<Rigidbody>().velocity;
        var characterRight = character.transform.right;

        var sidewaysVelocity = Vector3.Dot(characterRight, characterVelocity);
        gameObject.transform.Rotate(new Vector3(0, -0.6f * sidewaysVelocity, 0));
    }
}
