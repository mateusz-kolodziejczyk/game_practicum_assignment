using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    [SerializeField]
    Camera firstPersonCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var firstPersonEulerAngles = firstPersonCamera.transform.rotation.eulerAngles;
        // Set the rotation to be same but tilted slightly up
        gameObject.transform.rotation = Quaternion.Euler(firstPersonEulerAngles.x - 2, firstPersonEulerAngles.y, firstPersonEulerAngles.z);
    }
}
