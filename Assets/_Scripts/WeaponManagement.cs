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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            activeWeapon.Shoot(firstPersonCamera, character);
        }
        
            rotateBasedOnMovement();
    }

    private void rotateBasedOnMovement()
    {
        var characterVelocity = character.GetComponent<Rigidbody>().velocity;
        var characterRight = character.transform.right;

        var sidewaysVelocity = Vector3.Dot(characterRight, characterVelocity);
        gameObject.transform.Rotate(new Vector3(0, -0.6f*sidewaysVelocity, 0));
    }
}
