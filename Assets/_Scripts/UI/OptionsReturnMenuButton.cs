using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsReturnMenuButton : MonoBehaviour
{
    public void ReturnToMenu()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
